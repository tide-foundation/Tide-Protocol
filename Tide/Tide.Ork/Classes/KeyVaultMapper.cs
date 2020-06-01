using System;
using System.Numerics;
using Library;
using Tide.Encryption.AesMAC;
using Tide.Ork.Models;

//TODO: This sould be in library
namespace Tide.Ork.Classes {
    public class KeyVaultMapper {
        static public KeyVaultEntity Map(KeyVault vault)
        {
            return new KeyVaultEntity
            {
                User = Convert.ToBase64String(vault.User.ToByteArray()),
                AuthShare = Convert.ToBase64String(vault.AuthShare.ToByteArray(true, true)),
                KeyShare = Convert.ToBase64String(vault.KeyShare.ToByteArray(true, true)),
                Secret = vault.Secret.ToString()
            };
        }

        static public KeyVault Map(KeyVaultEntity vault)
        {
            return new KeyVault
            {
                User = new Guid(Convert.FromBase64String(vault.User)),
                AuthShare = new BigInteger(Convert.FromBase64String(vault.AuthShare), true, true),
                KeyShare = new BigInteger(Convert.FromBase64String(vault.KeyShare), true, true),
                Secret = AesKey.Parse(vault.Secret)
            };
        }
    }
}