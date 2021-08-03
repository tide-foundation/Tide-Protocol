using System;
using Tide.Core;

namespace Tide.Ork.Repo
{
    public class MemoryDnsManager : MemoryManagerJson<DnsEntry, DnsEntry>, IDnsManager
    {
    }
}