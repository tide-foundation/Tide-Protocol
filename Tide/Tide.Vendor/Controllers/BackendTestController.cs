
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly VendorConfig _config;
        public IVendorRepo Repo { get; set; }
        public BackendTestController(IVendorRepo repo, VendorDbContext context, VendorConfig config)
        {
            Repo = repo;
            _context = context;
            _config = config;
        }

        [HttpGet]
        public ActionResult<List<User>> GetUsers()
        {
            return _context.Users.Include(u=>u.RentalApplications).ToList();
        }


        [HttpGet("full/{applicationId}")]
        public ActionResult<RentalApplication> DecryptFullApplication([FromRoute] int applicationId) {
            var application = _context.Applications.First(a => a.Id == applicationId);
            var user = _context.Users.First(u => u.Id == application.UserId);

            var userGuid = Guid.Parse(user.Id);
            var properties = typeof(RentalApplication).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property.Name == "Id" || property.Name == "UserId" || property.Name == "DateSubmitted") continue;
                try
                {
                    var currentValue = (string)property.GetValue(application);
                    var newValue = Decrypt(userGuid, currentValue).Result;
                    property.SetValue(application, newValue);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return application;
        }

        [HttpPost("single/{userId}")]
        public ActionResult<string> DecryptSingleField([FromRoute] Guid userId,[FromBody] DecryptRequest request) {
            return Decrypt(userId, request.Field).Result;
        }

        private async Task<string> Decrypt(Guid vuid, string cipher)
        {
            //    var orks = new List<Uri>() {
            //        new Uri("https://ork-0.azurewebsites.net"),
            //        new Uri("https://ork-1.azurewebsites.net"),
            //        new Uri("https://ork-2.azurewebsites.net")
            //};

            var orks = new List<Uri>() {
                new Uri("http://localhost:5001"),
                new Uri("http://localhost:5002"),
                new Uri("http://localhost:5003")
            };

            //  var uris = (await Repo.GetListOrks(vuid)).Select(url => new Uri(url)).ToList();
            var flow = new DCryptFlow(vuid, orks);

            var bytes = Convert.FromBase64String(cipher.DecodeBase64Url());

            return System.Text.Encoding.UTF8.GetString(Cipher.UnPad32(await  flow.Decrypt(bytes, _config.PrivateKey)));
        }
    }
}

public class DecryptRequest {
    public string Field { get; set; }
}