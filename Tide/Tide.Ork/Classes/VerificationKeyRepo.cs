using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tide.Encryption.Ed;
using Tide.Ork.Controllers;
using Tide.Ork.Repo;

namespace Tide.Ork.Classes {

    internal class VerificationKeyRepo : IVerificationKeyRepo
    {
        private readonly IKeyManagerFactory _factory;
        private readonly ILogger<VerificationKeyRepo> _logger;
        private readonly string _endpoint;
        private readonly PathString _path;

        public VerificationKeyRepo(IKeyManagerFactory factory, IHttpContextAccessor http, ILogger<VerificationKeyRepo> logger)
        {
            _factory = factory;
            _logger = logger;
            _endpoint = http.HttpContext.GetEndpoint()?.DisplayName;
            _path = http.HttpContext.Request.Path;
        }

        private bool IsController<TController>() where TController : ControllerBase {
            var controller = typeof(TController).FullName;
            if (string.IsNullOrEmpty(_endpoint) || !_endpoint.Contains(controller))
                return false;

            _logger.LogDebug("VerificationKey repository found for {controller}", controller); 
            return true;
        }

        public async Task<VerificationKey> GetVerificationKey(Guid id)
        {
            if (IsController<KeyController>()) {
                var keyEntry = await _factory.BuildKeyIdManager().GetById(id);

                return keyEntry is null ? null : new VerificationKey {
                    Id = keyEntry.Id,
                    Key = keyEntry.Key
                };
            }

            if (IsController<DnsController>()) {
                var dnsEntry = await _factory.BuildDnsManager().GetById(id);

                return dnsEntry is null ? null : new VerificationKey {
                    Id = dnsEntry.Id,
                    Key = Ed25519Key.ParsePublic(dnsEntry.Public)
                };
            }

            if (IsController<RuleController>()) {
                var cvkEntry = await _factory.BuildManagerCvk().GetById(id);

                return cvkEntry is null ? null : new VerificationKey {
                    Id = cvkEntry.Id,
                    Key = cvkEntry.CvkPub
                };
            }

            _logger.LogError("No VerificationKey repository was found that matches the URL {path}.", _path);
            return null;
        }
    }
}