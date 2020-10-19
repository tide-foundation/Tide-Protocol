using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Sec;
using Tide.Vendor.Models;

namespace Tide.Vendor.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class VerificationController : BaseController
    {
        protected VendorDbContext Context;
        public VerificationController(VendorDbContext context) : base(context)
        {
    
            //const string privateKey = "c711e5080f2b58260fe19741a7913e8301c1128ec8e80b8009406e5047e6e1ef";
            //const string publicKey = "04e33993f0210a4973a94c26667007d1b56fe886e8b3c2afdd66aa9e4937478ad20acfbdc666e3cec3510ce85d40365fc2045e5adb7e675198cf57c6638efa1bdb";

            const string privateKey = "nPfgXzfNORd6tulHSV0a1/iBAnviVKSrv7PSu5SJlIOrb89v2paVYgPwyU7d0uga2kICJTTYu9T8Ifoauxcdnbjh/6DerDvGU/hSi0HFZg/9EgC6nPyWUrfqfJEr0tw7";
            const string publicKey = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACrb89v2paVYgPwyU7d0uga2kICJTTYu9T8Ifoauxcdnbjh/6DerDvGU/hSi0HFZg/9EgC6nPyWUrfqfJEr0tw7";

            var privateECDsa = LoadPrivateKey(FromHexString(privateKey));
            var publicECDsa = LoadPublicKey(FromHexString(publicKey));

            var jwt = CreateSignedJwt(privateECDsa);
            var isValid = VerifySignedJwt(publicECDsa, jwt);

            Console.WriteLine(isValid ? "Valid!" : "Not Valid...");
        }

        [HttpGet]
        public bool Test() {
            return true;
        }

        //[HttpGet("{vuid}")]
        //public ActionResult Login([FromRoute]string vuid) {
        //    var token = Request.Headers["Authorization"];

        //    var isAuthenticated = VerifySignedJwt(ECDsa.Create(), token);
        //}




        #region Jwt

        private static byte[] FromHexString(string hex)
        {
            var numberChars = hex.Length;
            var hexAsBytes = new byte[numberChars / 2];
            for (var i = 0; i < numberChars; i += 2)
                hexAsBytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);

            return hexAsBytes;
        }

        private ECDsa LoadPrivateKey(byte[] key)
        {
            var privKeyInt = new Org.BouncyCastle.Math.BigInteger(+1, key);
            var parameters = SecNamedCurves.GetByName("secp256r1");
            var ecPoint = parameters.G.Multiply(privKeyInt);
            var privKeyX = ecPoint.Normalize().XCoord.ToBigInteger().ToByteArrayUnsigned();
            var privKeyY = ecPoint.Normalize().YCoord.ToBigInteger().ToByteArrayUnsigned();

            return ECDsa.Create(new ECParameters
            {
                Curve = ECCurve.NamedCurves.nistP256,
                D = privKeyInt.ToByteArrayUnsigned(),
                Q = new ECPoint
                {
                    X = privKeyX,
                    Y = privKeyY
                }
            });
        }

        private ECDsa LoadPublicKey(byte[] key)
        {
            var pubKeyX = key.Skip(1).Take(32).ToArray();
            var pubKeyY = key.Skip(33).ToArray();

            return ECDsa.Create(new ECParameters
            {
                Curve = ECCurve.NamedCurves.nistP256,
                Q = new ECPoint
                {
                    X = pubKeyX,
                    Y = pubKeyY
                }
            });
        }

        private string CreateSignedJwt(ECDsa eCDsa)
        {
            var now = DateTime.UtcNow;
            var tokenHandler = new JwtSecurityTokenHandler();

            var jwtToken = tokenHandler.CreateJwtSecurityToken(
                issuer: "me",
                audience: "you",
                subject: null,
                notBefore: now,
                expires: now.AddMinutes(30),
                issuedAt: now,
                signingCredentials: new SigningCredentials(
                    new ECDsaSecurityKey(eCDsa), SecurityAlgorithms.EcdsaSha256));

            return tokenHandler.WriteToken(jwtToken);
        }

        private bool VerifySignedJwt(ECDsa eCDsa, string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidIssuer = "me",
                ValidAudience = "you",
                IssuerSigningKey = new ECDsaSecurityKey(eCDsa)
            }, out var parsedToken);

            return claimsPrincipal.Identity.IsAuthenticated;
        }

        #endregion
    }
}
