using System;
using Microsoft.AspNetCore.Mvc;
using Tide.Core;

namespace Tide.Simulator.Contracts
{
    public interface IContract
    {
        bool Matchs(Transaction transaction);
        IActionResult Process(Transaction transaction);
    }
}