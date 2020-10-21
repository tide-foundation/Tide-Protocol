using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Tide.Vendor.Controllers;

namespace Tide.Vendor.Classes {
    public class Authentication : IAuthentication {
        private readonly Settings _settings;

        private readonly Dictionary<string, VendorUser> _users = new Dictionary<string, VendorUser>();

        public Authentication(Settings settings) {
            _settings = settings;
        }

        public string Register(string tideToken, string publicKey) {
            var key = GetPublicKey(publicKey);
            if (!ValidateTideToken(tideToken, key)) return null;

            var claims = ExtractClaims(tideToken, key);
            var vuid = claims.First(c => c.Type == "vuid").Value;

            _users.Add(vuid, new VendorUser {PublicKey = publicKey, Vuid = vuid});

            return GenerateVendorToken(vuid);
        }

        public string Exchange(string tideToken, string vuid) {
            if (_users.TryGetValue(vuid, out var user)) {
                var key = GetPublicKey(user.PublicKey);
                if (!ValidateTideToken(tideToken, key)) return null;
                return GenerateVendorToken(vuid);
            }

            return null;
        }

        public VendorUser GetUser(string stringToken) {
 
                if (!ValidateVendorToken(stringToken)) return null;
          
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadToken(stringToken) as JwtSecurityToken;
               
                var vuid = token.Claims.First(c => c.Type == "vuid").Value;
              
                return _users.First(u => u.Key == vuid).Value;
          
        }

        #region Vendor Token

        public bool ValidateVendorToken(string token)
        {
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_settings.BearerKey));
          
            var tokenHandler = new JwtSecurityTokenHandler();
            
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _settings.Audience,
                    ValidAudience = _settings.Audience,
                    IssuerSigningKey = key
                }, out SecurityToken validatedToken);
            }
            catch(Exception e)
            {
                return false;
            }

       
            return true;
        }

        private string GenerateVendorToken(string vuid) {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_settings.BearerKey);
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(new[] {
                    new Claim("vuid", vuid)
                }),
                Expires = DateTime.Now.AddDays(14),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
                Audience = _settings.Audience,
                Issuer = _settings.Audience,
                IssuedAt = DateTime.Now
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        #endregion


        #region TideToken

        private ECDsaCng GetPublicKey(string stringKey) {
            var publicKeyBytes = Convert.FromBase64String(stringKey);
            var keyType = new byte[] {0x45, 0x43, 0x53, 0x31};
            var keyLength = new byte[] {0x20, 0x00, 0x00, 0x00};
            var key = keyType.Concat(keyLength).Concat(publicKeyBytes.Skip(publicKeyBytes.Length - 64)).ToArray();

            var cngKey = CngKey.Import(key, CngKeyBlobFormat.EccPublicBlob);
            return new ECDsaCng(cngKey);
        }


        private bool ValidateTideToken(string tokenString, ECDsa pubKey) {
            try {
                var securityToken = new JwtSecurityToken(tokenString);
                var securityTokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters {
                    //ValidIssuer = securityToken.Issuer,
                    //ValidAudience = securityToken.Audiences.First(),
                    IssuerSigningKey = new ECDsaSecurityKey(pubKey),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RequireExpirationTime = false,
                    ValidateLifetime = true
                };
                SecurityToken stoken;
                securityTokenHandler.ValidateToken(tokenString, validationParameters, out stoken);
                return true;
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        private List<Claim> ExtractClaims(string tokenString, ECDsa pubKey) {
            return new JwtSecurityToken(tokenString).Claims.ToList();
        }

        #endregion
    }
}