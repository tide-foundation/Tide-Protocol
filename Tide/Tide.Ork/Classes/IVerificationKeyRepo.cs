using System;
using System.Threading.Tasks;
using Tide.Core;
using Tide.Encryption.Ed;

namespace Tide.Ork.Classes
{
    internal interface IVerificationKeyRepo
    {
        Task<VerificationKey> GetVerificationKey(Guid id);
    }

    internal class VerificationKey : IGuid
    {
        public Guid  Id { get; set; }
        public Ed25519Key Key { get; set; }
    }
}