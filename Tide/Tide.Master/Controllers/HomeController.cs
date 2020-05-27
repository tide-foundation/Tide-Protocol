using Microsoft.AspNetCore.Mvc;
using Tide.Library.Classes.Encryption;
using Tide.Library.Models;
using Tide.Library.Models.Interfaces;

namespace Tide.Master.Controllers {
    public class HomeController : Controller {
        private readonly ITideProtocol _tideProtocol;

        public HomeController(ITideProtocol tideProtocol) {
            _tideProtocol = tideProtocol;
        }


        public IActionResult Index() {
            return View();
        }

        [HttpPost("/CheckAccount")]
        public bool CheckAccount([FromBody] JsonData model) {
            return _tideProtocol.AccountExists(model.Username);
        }

        [HttpPost("/InitializeAccount")]
        public TideResponse InitializeAccount([FromBody] JsonData model) {
            return _tideProtocol.InitializeAccount(model.PublicKey, model.Username);
        }

        [HttpPost("/ConfirmAccount")]
        public TideResponse ConfirmAccount([FromBody] JsonData model) {
            return _tideProtocol.ConfirmAccount(model.Username);
        }

        [HttpPost("/CreateVendor")]
        public TideResponse CreateVendor([FromBody] CreateVendorModel model) {
            return _tideProtocol.CreateVendor(model.Account,model.Username,model.PublicKey,model.Description);
        }
    }
}