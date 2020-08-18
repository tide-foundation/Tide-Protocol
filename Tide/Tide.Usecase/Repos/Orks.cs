using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Tide.Encryption.AesMAC;
using Tide.VendorSdk.Classes;

public class OrkRepo : IOrkRepo
{
    protected readonly ConcurrentDictionary<Guid, AesKey> _items;

    public OrkRepo()
    {
        _items = new ConcurrentDictionary<Guid, AesKey>();
    }

    public Task AddUser(Guid vuid, AesKey auth)
    {
        _items[vuid] = auth;
        return Task.CompletedTask;
    }

    public Task<AesKey> GetKey(Guid vuid)
    {
        if (!_items.ContainsKey(vuid))
            return Task.FromResult<AesKey>(null);

        return Task.FromResult(_items[vuid]);
    }

    public Task<string[]> GetListOrks()
    {
        //var list = new List<string>();
        //for (int i = 0; i < 10; i++)
        //{
        //    list.Add($"https://ork-${i}.azurewebsites.net");
        //}
        //return Task.FromResult(list.ToArray());
        return Task.FromResult(new[] {
            "http://localhost:5001",
            "http://localhost:5002",
            "http://localhost:5003"
        });
    }
}
