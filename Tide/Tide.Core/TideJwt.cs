using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Tide.Encryption.Ecc;

namespace Tide.Core {
    public class TideJwt
    {
        private const long _window = TimeSpan.TicksPerHour;
        private readonly JwtSecurityToken _jwt;

        public string Signature { get; set; }

        public string SignatureAlgorithm => _jwt.SignatureAlgorithm;
        public bool IsTide => _jwt.SignatureAlgorithm.ToUpper() == "TS256";

        public IEnumerable<Claim> Claims => _jwt.Claims;

        public Guid Subject => Guid.TryParse(_jwt.Subject, out var sub) ? sub : Guid.Empty;

        public DateTime ValidTo { get => _jwt.ValidTo;
            set => SetClaimDate(JwtRegisteredClaimNames.Exp, value); }

        public DateTime ValidFrom { get => _jwt.ValidFrom;
            set => SetClaimDate(JwtRegisteredClaimNames.Nbf, value); }
        
        public DateTime IssuedAt { get => _jwt.IssuedAt;
            set => SetClaimDate(JwtRegisteredClaimNames.Iat, value); }

        private TideJwt(string jwt) {
            _jwt = new JwtSecurityToken(jwt);
            Signature = _jwt.RawSignature;
        }

        public TideJwt(Guid subject)
        {
            var id = BitConverter.ToUInt64(Guid.NewGuid().ToByteArray().Take(8).ToArray());
            _jwt = new JwtSecurityToken(
                claims: new Claim[] {
                    new Claim(JwtRegisteredClaimNames.Jti, id.ToString(), ClaimValueTypes.Integer64),
                    new Claim(JwtRegisteredClaimNames.Sub, subject.ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                },
                notBefore: SystemTime.UtcNow,
                expires: SystemTime.UtcNow.AddTicks(_window)
            );
            _jwt.Header["alg"] = "TS256";
            Signature = string.Empty;
        }

        public bool IsOnTime() {
            var now = SystemTime.UtcNow;
            var min = DateTime.MinValue;

            if (ValidFrom != min && ValidTo != min) {
                return now >= ValidFrom && now <= ValidTo;
            }

            if (ValidTo != min)
                return now >= ValidTo.AddTicks(-_window) && now <= ValidTo;

            if (ValidFrom != min)
                return now >= ValidFrom && now <= ValidFrom.AddTicks(_window);

            if (IssuedAt != min)
                return now >= IssuedAt && now <= IssuedAt.AddTicks(_window);

            return false;
        }

        public bool Verify(C25519Key key) => key.Verify(GetMessageBytes(), GetSignatureBytes());

        public void Sign(C25519Key key) => Signature = Base64UrlEncoder.Encode(key.Sign(GetMessageBytes()));

        private byte[] GetMessageBytes() => Encoding.UTF8.GetBytes($"{_jwt.EncodedHeader}.{_jwt.EncodedPayload}");
        
        private byte[] GetSignatureBytes() => Base64UrlEncoder.DecodeBytes(Signature);

        public override string ToString() => $"{_jwt.EncodedHeader}.{_jwt.EncodedPayload}.{Signature}";

        public byte[] ToByteArray() => Encoding.UTF8.GetBytes(ToString());

        private void SetClaimDate(string key, DateTime value)
        {
            if (value == DateTime.MinValue)
                _jwt.Payload.Remove(key);
            else
                _jwt.Payload[key] = ((DateTimeOffset)value).ToUnixTimeSeconds();
        }

        public static TideJwt Parse(string jwt) => new TideJwt(jwt);

        public static bool TryParse(string token, out TideJwt jwt)
        {
            try
            {
               jwt = TideJwt.Parse(token);
               return Guid.TryParse(jwt._jwt.Subject, out var subject);
            }
            catch (ArgumentException)
            {
                jwt = null;
                return false;
            }
        }
    }
}
