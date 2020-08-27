
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tide.Encryption.Tools;
using Tide.Vendor.Models;
using Tide.VendorSdk.Classes;

namespace Tide.Vendor.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BackendTestController : ControllerBase
    {
        private readonly VendorDbContext _context;
        private VendorConfig _config;
        public IVendorRepo Repo { get; set; }
        public BackendTestController(IVendorRepo repo, VendorDbContext context, VendorConfig config)
        {
            Repo = repo;
            _context = context;
            _config = config;
        }


        [HttpGet("{vuid}/{decrypt}")]
        public ActionResult<ApplicationUser> PartiallyDecryptAccount([FromRoute] Guid vuid,bool decrypt) {
            var user = _context.GetAccount(vuid);
            if (user == null) return null;

            if (decrypt) {
                user.Field1 = Decrypt(vuid, user.Field1).Result;
                user.Field2 = Decrypt(vuid, user.Field2).Result;
            }
         

            return user;
        }

        private async Task<string> Decrypt(Guid vuid, string cipher)
        {
            var uris = (await Repo.GetListOrks(vuid)).Select(url => new Uri(url)).ToList();
            var flow = new DCryptFlow(vuid, uris);

            var bytes = Convert.FromBase64String(cipher.DecodeBase64Url());

            return System.Text.Encoding.UTF8.GetString(Cipher.UnPad32(await  flow.Decrypt(bytes, _config.PrivateKey)));
        }
    }
}
