using System.Threading.Tasks;
using Tide.VendorSdk.Classes;

public class OrkRepo : IOrkRepo
{
    public Task<string[]> GetListOrks()
    {
        return Task.FromResult(new[] {
            "http://localhost:5001",
            "http://localhost:5002",
            "http://localhost:5003"
        });
    }
}