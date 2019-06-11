using Microsoft.AspNetCore.Mvc;
using Tide.Library.Classes.Cryptide;
using Tide.Library.Models;
using Tide.Library.Models.Interfaces;

namespace Tide.Master.Controllers {
    public class HomeController : Controller {
        private readonly ITideProtocol _helper;

        public HomeController(ITideProtocol helper) {
            _helper = helper;
        }


        public IActionResult Index() {
            return View();
        }

        [HttpPost("/CheckAccount")]
        public bool CheckAccount([FromBody] AuthenticationModel model) {
            return _helper.AccountExists(model.Username);
        }

        [HttpPost("/InitializeUser")]
        public TideResponse InitializeUser([FromBody] AuthenticationModel model) {
            var accountResult = _helper.CreateBlockchainAccount(model.PublicKey);
            if (!accountResult.Success) return accountResult;

            var initResult = _helper.InitializeAccount(accountResult.Content.ToString(), model.Username);
            if (initResult.Success) initResult.Content = accountResult.Content;
            return initResult;
        }

        [HttpPost("/ConfirmUser")]
        public TideResponse ConfirmUser([FromBody] AuthenticationModel model) {
            return _helper.ConfirmAccount(Cryptide.Instance.HashUsername("tide").username, model.Username);
        }

        [HttpPost("/CreateVendor")]
        public TideResponse CreateVendor([FromBody] CreateVendorModel model) {
            return _helper.CreateVendor(model);
        }
    }
}