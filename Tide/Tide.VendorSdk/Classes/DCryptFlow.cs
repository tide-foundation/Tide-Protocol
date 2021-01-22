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

        public async Task<C25519Key> SignUp(AesKey cmkAuth, int threshold, Guid keyId, IReadOnlyList<byte[]> signatures)
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

        public async Task<byte[]> Decrypt(C25519Key prv, byte[] cipher)
            => (await this.DecryptBulk(prv, new List<byte[]>() { cipher })).First();

        public async Task<byte[][]> DecryptBulk(C25519Key prv, params byte[][] ciphers)
            => await DecryptBulk(prv, ciphers as IReadOnlyList<byte[]>);

        public async Task<byte[][]> DecryptBulk(C25519Key prv, IReadOnlyList<byte[]> ciphers)
        {
            var keyId = IdGenerator.Seed(prv.GetPublic().ToByteArray()).Guid;
            var challenges = await Task.WhenAll(Clients.Select(cli => cli.Challenge(VuId, keyId)));

            var asymmetrics = ciphers.Select(cph => Cipher.Asymmetric(cph)).ToList();
            var sessionKeys = challenges.Select(ch => prv.DecryptKey(ch.Challenge)).ToList();
            var allCipher = asymmetrics.SelectMany(asy => asy).ToArray();
            var signs = sessionKeys.Select(key => key.Hash(allCipher)).ToList();

            var cipherPartials = await Task.WhenAll(Clients.Select((cli, i) =>
                cli.DecryptBulk(VuId, keyId, asymmetrics, challenges[i].Token, signs[i])));

            var ciphs = asymmetrics.Select(asy => Cipher.CipherFromAsymmetric(asy)).ToList();
            var ids = await Task.WhenAll(Clients.Select(cln => cln.GetId()));

            var plains = new byte[ciphers.Count][];
            for (var j = 0; j < ciphers.Count; j++)
            {
                var partials = cipherPartials.Select((cph, i) => C25519Point.From(sessionKeys[i].Decrypt(cph[j])))
                    .Select(pnt => new C25519Cipher(pnt, ciphs[j].C2)).ToList();

                var plain = C25519Cipher.DecryptShares(partials, ids);

                var symmetric = Cipher.Symmetric(ciphers[j]);
                if (symmetric.Length == 0) {
                    plains[j] = Cipher.UnPad32(plain);
                    continue;
                }

                var symmetricKey = AesSherableKey.Parse(plain);
                plains[j] = symmetricKey.Decrypt(symmetric);
            }

            return plains;
        }
    }
}