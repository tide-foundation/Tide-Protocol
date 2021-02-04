using System;
using System.Threading.Tasks;
using Tide.Encryption.Ecc;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Text;

namespace Tide.VendorSdk.Classes
{
    public class DCryptFlowBulk
    {
        private readonly DnsClient _dns;
        private readonly C25519Key _prv;
        private readonly Regex _rxBase64 = new Regex("^([A-Za-z0-9+/]{4})*([A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{2}==)?$", RegexOptions.Compiled);

        public DCryptFlowBulk(C25519Key @private, Uri homeOrk)
        {
            _prv = @private;
            _dns = new DnsClient(homeOrk);
        }

        private async Task<List<DCryptFlow>> GetFlows(IEnumerable<Guid> vuids) {
            return (await _dns.GetEntries(vuids)).Select(entry => new DCryptFlow(entry.Id, entry.GetUrls())).ToList();
        }

        public async Task<List<byte[]>> Decrypt(IReadOnlyCollection<Guid> vuids, IReadOnlyCollection<byte[]> fields) {
            if (vuids == null || fields == null || !vuids.Any() || !fields.Any())
                throw new Exception("Vuid and fields must be provided");
            
            if (vuids.Count != fields.Count) throw new Exception("The number of vuid and fields must be the same");

            var plains = await Decrypt(vuids, fields.Select(fld => new List<byte[]> { fld }).ToList());
            return plains.Select(pln => pln.FirstOrDefault()).ToList();
        }

        public async Task<List<List<byte[]>>> Decrypt(IReadOnlyCollection<Guid> vuids, List<List<byte[]>> fields) {
            if (vuids == null || fields == null || !vuids.Any() || !fields.Any())
                throw new Exception("Vuid and fields must be provided");
            
            if (vuids.Count != fields.Count) throw new Exception("The number of vuid and fields must be the same");

            var flows = await GetFlows(vuids);

            var plains = await Task.WhenAll(flows.Select((flw, i) => flw.DecryptBulk(_prv, fields[i])));

            return plains.ToList();
        }

        public async Task DecryptObject<T>(IReadOnlyCollection<Guid> vuids, IReadOnlyList<T> data) where T: class
        {
            if (vuids == null || data == null || !vuids.Any() || !data.Any())
                throw new Exception("Vuid and data must be provided");
            
            if (vuids.Count != data.Count) throw new Exception("The number of vuid and data must be the same");

            var propInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(inf => inf.PropertyType == typeof(string))
                .ToList();

            var propValues = data.Select(dta =>
                propInfos.Select(inf => new { info = inf, value = inf.GetValue(dta, null) as string })
                    .Where(prp => !string.IsNullOrWhiteSpace(prp.value))
                    .Where(prp => _rxBase64.IsMatch(prp.value))
                    .ToList()).ToList();
                    
            var fields = propValues.Select((prps, i) => prps.Select(prp => Convert.FromBase64String(prp.value)).ToList());
            
            var plains = await Decrypt(vuids, fields.ToList());

            for (int i = 0; i < plains.Count; i++)
            {
                for (int j = 0; j < plains[i].Count; j++)
                {
                    propValues[i][j].info.SetValue(data[i], Encoding.UTF8.GetString(plains[i][j]));
                }
            }
        }
    }
}