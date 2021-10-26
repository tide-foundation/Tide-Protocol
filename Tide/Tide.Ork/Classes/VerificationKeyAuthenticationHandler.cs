using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tide.Core;

namespace Tide.Ork.Classes {

    internal class VerificationKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private const string bearerTag = "bearer";
        private readonly IVerificationKeyRepo _repo;

        public VerificationKeyAuthenticationHandler(
            IVerificationKeyRepo repo,
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock):base(options, logger, encoder, clock)
        {
            _repo = repo;
        }

        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string token = null;
            if (Request.Headers.TryGetValue("Authorization", out var bearers) && bearers.Any()) {
                var bearer = bearers.FirstOrDefault();
                if (!bearer.StartsWith(bearerTag, StringComparison.OrdinalIgnoreCase)) {
                    Logger.LogDebug("Path: {path} - Header: {header}", this.Request.Path.ToString() ,bearer);
                    Logger.LogInformation("Authorization failed: header is not in the correct format");
                    return AuthenticateResult.Fail("Authorization header is not in the correct format");
                }
                
                token = bearer.Substring(bearerTag.Length).Trim();
            }
            else if (Request.Query.TryGetValue("access_token", out var tokens) && tokens.Any()) {
                token = tokens.FirstOrDefault();
            }
            else if (Request.HasFormContentType && Request.Form.TryGetValue("access_token", out var tokens2) && tokens2.Any()) {
                token = tokens2.FirstOrDefault();
            }

            if (string.IsNullOrWhiteSpace(token)) {
                Logger.LogInformation("Authorization: no token was provided for: [{method}] {path}", Request.Method, Request.Path);
                return AuthenticateResult.NoResult();
            }

            if (!TideJwt.TryParse(token, out TideJwt jwt))
            {
                Logger.LogInformation("Authorization failed: JWT is not in the correct format {jwt}", token);
                return AuthenticateResult.Fail($"JWT is not in the correct format");
            }

            if (!jwt.IsTide)
            {
                Logger.LogInformation("Authorization failed: JWT signature algorithm is not allowed {jwt}", token);
                return AuthenticateResult.Fail($"JWT signature algorithm is not allowed");
            }

            var delayCheck = Task.Delay(500);
            if (!jwt.IsOnTime())
            {
                Logger.LogInformation("Authorization failed: Invalid Token time for {subject} | {issuedAt} {validFrom}, {validTo}",
                    jwt.Subject, jwt.IssuedAt, jwt.ValidFrom, jwt.ValidTo);
                await delayCheck;
                return AuthenticateResult.Fail($"Invalid Token");
            }

            var verificationKey = await _repo.GetVerificationKey(jwt.Subject);
            if (verificationKey == null) {
                Logger.LogWarning("Authorization failed: no VerificationKey was found for {subject}", jwt.Subject);
                await delayCheck;
                return AuthenticateResult.Fail($"Invalid Token");
            }

            if (!jwt.Verify(verificationKey.Key))
            {
                Logger.LogWarning("Authorization failed: Invalid Token for {subject} | IsOnTime: {onTime} ", jwt.Subject, jwt.IsOnTime());
                await delayCheck;
                return AuthenticateResult.Fail($"Invalid Token");
            }

            var identity = new ClaimsIdentity(jwt.Claims, Scheme.Name);
            identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Subject.ToString()));

            var principal = new GenericPrincipal(identity, null);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            Logger.LogInformation("Authenticated request for {subject} to the endpint [{method}] {path} ", jwt.Subject, Request.Method, Request.Path);
            return AuthenticateResult.Success(ticket);
        }
    }
}