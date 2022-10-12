using System;
using Tide.Core;
using Tide.Encryption.Ed;

namespace Tide.Ork.DTOs
{
    public class KeyIdVaultDTO
    {
        public Guid KeyId { get; set; }
        public string Key { get; set; }

        public KeyIdVaultDTO() { }

        public KeyIdVaultDTO(KeyIdVault key)
        {
            KeyId = key.KeyId;
            Key = Convert.ToBase64String(key.Key.ToByteArray());
        }

        public KeyIdVault Map() 
        {
            return new KeyIdVault
            {
                KeyId = this.KeyId,
                Key = Ed25519Key.ParsePublic(Convert.FromBase64String(Key))
            };
        }

        public static implicit operator KeyIdVault(KeyIdVaultDTO k) => k.Map();
        public static implicit operator KeyIdVaultDTO(KeyIdVault k) => new KeyIdVaultDTO(k);
    }
}
