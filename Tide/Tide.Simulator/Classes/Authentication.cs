using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Tide.Core;
using Microsoft.IdentityModel.Tokens;
using Tide.Simulator.Models;

namespace Tide.Simulator.Classes {
    public class Authentication : IAuthentication {
        private readonly BlockchainContext _context;
        private readonly Settings _settings;

        public Authentication(BlockchainContext context, Settings settings) {
            _context = context;
            _settings = settings;
        }

        public AuthenticationResponse Register(AuthenticationRequest request) {
            try {
                if (_context.Account.Any(u => u.Username == request.Username)) {
                    return new AuthenticationResponse {Error = "Invalid username", Success = false};
                }

                if (string.IsNullOrEmpty(request.Password)) {
                    return new AuthenticationResponse {Error = "Invalid Password", Success = false};
                }

                var salt = GenerateSalt();

                var newAccount = new Account {
                    Username = request.Username,
                    Salt = Convert.ToBase64String(salt),
                    Hash = Convert.ToBase64String(ComputeHash(request.Password, salt))
                };
                newAccount.Token = GenerateToken(request.Username, newAccount.Hash);

                _context.Add(newAccount);
                _context.SaveChanges();

                return new AuthenticationResponse {Token = newAccount.Token, Success = true};
            }
            catch (Exception e) {
                return new AuthenticationResponse {Error = e.Message, Success = false};
            }
        }

        public AuthenticationResponse Login(AuthenticationRequest request) {
            try {
                var user = _context.Account.FirstOrDefault(u => u.Username == request.Username);
                if (user == null) {
                    return new AuthenticationResponse {Error = "Invalid Username", Success = false};
                }

                ;

                var hashedPassword = ComputeHash(request.Password, Convert.FromBase64String(user.Salt));
                if (user.Hash != Convert.ToBase64String(hashedPassword)) {
                    return new AuthenticationResponse {Error = "Incorrect Password", Success = false};
                }

                user.Token = GenerateToken(user.Username, user.Hash);
                _context.SaveChanges();

                return new AuthenticationResponse {Success = true, Token = user.Token};
            }
            catch (Exception e) {
                return new AuthenticationResponse {Error = e.Message, Success = false};
            }
        }

        private string GenerateToken(string identifier, string hash) {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_settings.BearerKey);
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, identifier),
                    new Claim(ClaimTypes.Hash, hash)
                }),
                Expires = DateTime.UtcNow.AddYears(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private byte[] GenerateSalt() {
            using (var saltGenerator = new RNGCryptoServiceProvider()) {
                var salt = new byte[24];
                saltGenerator.GetBytes(salt);
                return salt;
            }
        }

        private byte[] ComputeHash(string password, byte[] salt) {
            using (var hashGenerator = new Rfc2898DeriveBytes(password, salt)) {
                hashGenerator.IterationCount = 10101;
                return hashGenerator.GetBytes(24);
            }
        }
    }
}