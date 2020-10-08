using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Tide.Encryption.Ecc;
using Tide.Vendor.Models;
using Tide.VendorSdk.Classes;

namespace Tide.Vendor.Controllers
{
  
    [ApiController]
    [Route("[controller]")]
    public class AccountController : BaseController
    {

        private readonly Settings _settings;

        public AccountController(VendorDbContext context,Settings settings) : base(context) {
            _settings = settings;
        }

        [HttpGet]
        public bool Test() {
            return true;
        }

        [HttpPost]
        public ActionResult CreateAccount([FromBody] User user) {
            Context.Add(user);
            Context.SaveChanges();

            return Ok(GenerateToken(user.Id));
            //var msg = Encoding.ASCII.GetBytes(GenerateToken(user.Id));
            //return Ok(C25519Key.Parse(user.CvkPub).Encrypt(msg));
        }

        [HttpGet("{vuid}")]
        public ActionResult Login([FromRoute]string vuid) {
            return Ok(GenerateToken(vuid)); // TODO: Encrypt this with the cvk pub
        }

        private string GenerateToken(string vuid)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_settings.BearerKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, vuid)
                }),
                Expires = DateTime.UtcNow.AddDays(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
