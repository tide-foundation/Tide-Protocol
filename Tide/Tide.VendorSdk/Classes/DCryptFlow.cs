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
        private readonly List<OrkClient> _clients;
        private readonly IdGenerator _userId;

        public BigInteger UserId { get => _userId.Id; }
        public Guid VuId { get => _userId.Guid; }

        public DCryptFlow(Guid guid, IEnumerable<Uri> uris)
        {
            _clients = uris.Select(uris => new OrkClient(uris)).ToList();
            _userId = new IdGenerator(guid);
        }

        public async Task<C25519Key> SignUp(AesKey cmkAuth, int threshold)
        {
            var cvk = new C25519Key();
            var ids = _clients.Select(cln => cln.Id).ToList();

            var cvks = cvk.Share(threshold, ids, true);
            var cvkAuths = _clients.Select(cln => cln.Guid.ToByteArray().Concat(VuId.ToByteArray()))
                .Select(buff => cmkAuth.Derive(buff.ToArray())).ToList();

            await Task.WhenAll(_clients.Select((cli, i) =>
              cli.RegisterCvk(VuId, cvks[i].X, cvkAuths[i], cvk.GetPublic())));

            return cvk;
        }

        public async Task<byte[]> Decrypt(byte[] cipher, C25519Key prv)
        {
            var keyId = IdGenerator.Seed(prv.GetPublic().ToByteArray()).Guid;

            var challenges = await Task.WhenAll(_clients.Select(cli => cli.Challenge(VuId, keyId)));

            var asymmetric = Cipher.Asymmetric(cipher);
            var sessionKeys = challenges.Select(ch => prv.DecryptKey(ch.Challenge)).ToList();
            var signs = sessionKeys.Select(key => key.Hash(asymmetric)).ToList();

            var ciphers = await Task.WhenAll(_clients.Select((cli, i) =>
                cli.Decrypt(VuId, keyId, asymmetric, challenges[i].Token, signs[i])));

            var ciph = Cipher.CipherFromAsymmetric(asymmetric);
            var partials = ciphers.Select((cph, i) => C25519Point.From(sessionKeys[i].Decrypt(cph)))
                .Select(pnt => new C25519Cipher(pnt, ciph.C2)).ToList();

            var ids = _clients.Select(cln => cln.Id);

            return C25519Cipher.DecryptShares(partials, ids);
        }
    }
}