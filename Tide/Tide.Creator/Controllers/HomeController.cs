using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tide.Creator.Models;
using Tide.Library.Models;
using Tide.Library.Models.Interfaces;

namespace Tide.Creator.Controllers
{
    public class HomeController : Controller
    {
        // Normally, a vendor would use the more abstract IVendorAuthentication, but as this is the master creator we will directly use IBlockchainHelper
        private readonly IBlockchainHelper _blockchainHelper;
        public HomeController(IBlockchainHelper blockchainHelper) {
            _blockchainHelper = blockchainHelper;
        }

        public IActionResult Index() {
            return View();
        }

        [HttpPost("/Create")]
        public TideResponse CreateVendor(CreateVendorModel model) {
            return _blockchainHelper.CreateVendor(model);
        }

        [HttpPost("/Init")]
        public TideResponse InitializeAccount([FromBody]CreateVendorModel model)
        {
            return _blockchainHelper.InitializeAccount(model.Username);
        }

        [HttpPost("/Confirm")]
        public TideResponse ConfirmAccount(CreateVendorModel model)
        {
            return _blockchainHelper.ConfirmAccount(model.Username);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
