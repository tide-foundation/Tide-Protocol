using System;
using System.Linq;
using System.Numerics;
using System.Text;
using Microsoft.AspNetCore.Http;
using Tide.Encryption.Tools;

namespace Tide.Ork.Classes {
    public class IdGenerator {
        private readonly IHttpContextAccessor _http;

        private HttpRequest Request => _http.HttpContext.Request;

        private string Host => Request.Host.Value;

        public BigInteger Id => GetId(Host);

        public Guid Guid => GetGuid(Host);

        public byte[] BufferId => GetBufferId(Host);

        public IdGenerator(IHttpContextAccessor http)
        {
            _http = http;
        }

        public static Guid GetGuid(string data) => new Guid(GetBufferId(data));

        public static BigInteger GetId(string data) => new BigInteger(GetBufferId(data), true, true);

        public static byte[] GetBufferId(string data) => Utils.Hash(Encoding.UTF8.GetBytes(data)).Take(16).ToArray();
    }
}