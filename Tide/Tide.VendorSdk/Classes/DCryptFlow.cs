using System;
using System.Threading.Tasks;
using System.Numerics;
using Tide.Encryption.AesMAC;
using Tide.Encryption.Ecc;
using System.Collections.Generic;
using System.Linq;

namespace Tide.VendorSdk.Classes
{
    public class DCryptFlow
    {
        private readonly IdGenerator _userId;

        public List<CvkClient> Clients { get; }
        public BigInteger UserId { get => _userId.Id; }
        public Guid VuId { get => _userId.Guid; }

        public DCryptFlow(Guid guid, IEnumerable<Uri> uris)
        {
            Clients = uris.Select(uris => new CvkClient(uris)).ToList();
            _userId = new IdGenerator(guid);
        }

        public async Task<C25519Key> SignUp(AesKey cmkAuth, int threshold, Guid keyId, List<byte[]> signatures)
        {
            var cvk = new C25519Key();
            var ids = await Task.WhenAll(Clients.Select(cln => cln.GetId()));
            var guids = await Task.WhenAll(Clients.Select(cln => cln.GetGuid()));

            var cvks = cvk.Share(threshold, ids, true);
            var cvkAuths = guids.Select(guid => guid.ToByteArray().Concat(VuId.ToByteArray()))
                .Select(buff => cmkAuth.Derive(buff.ToArray())).ToList();

            await Task.WhenAll(Clients.Select((cli, i) =>
              cli.Add(VuId, cvks[i].X, cvkAuths[i], cvk.GetPublic(), keyId, signatures[i])));

            return cvk;
        }

        public async Task<byte[]> Decrypt(byte[] cipher, C25519Key prv)
        {
            var keyId = IdGenerator.Seed(prv.GetPublic().ToByteArray()).Guid;

            var challenges = await Task.WhenAll(Clients.Select(cli => cli.Challenge(VuId, keyId)));

            var asymmetric = Cipher.Asymmetric(cipher);
            var sessionKeys = challenges.Select(ch => prv.DecryptKey(ch.Challenge)).ToList();
            var signs = sessionKeys.Select(key => key.Hash(asymmetric)).ToList();

            var ciphers = await Task.WhenAll(Clients.Select((cli, i) =>
                cli.Decrypt(VuId, keyId, asymmetric, challenges[i].Token, signs[i])));

            var ciph = Cipher.CipherFromAsymmetric(asymmetric);
            var partials = ciphers.Select((cph, i) => C25519Point.From(sessionKeys[i].Decrypt(cph)))
                .Select(pnt => new C25519Cipher(pnt, ciph.C2)).ToList();

            var ids = await Task.WhenAll(Clients.Select(cln => cln.GetId()));

            var plain = C25519Cipher.DecryptShares(partials, ids);

            var symmetric = Cipher.Symmetric(cipher);
            if (symmetric.Length == 0)
            {
                return Cipher.UnPad32(plain);
            }

            var symmetricKey = AesSherableKey.Parse(plain);
            return symmetricKey.Decrypt(symmetric);
        }
    }
}