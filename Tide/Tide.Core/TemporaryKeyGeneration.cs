using System;
using System.Linq;
using System.Numerics;
using System.Dynamic;
using System.Text.Json;
using System.Collections.Generic;
using Tide.Encryption.SecretSharing;
using Tide.Encryption.Ed;
using Tide.Encryption.Tools;
using Tide.Encryption.AesMAC;

public class KeyGenerator
{
    private BigInteger MSecOrki {get;} // this ork's private scalar
    private Ed25519Point MgOrki {get;} // this ork's public point
    private string From_Username {get;} // this ork's username
    public int Threshold => 3; // change me

    public string GenShard(Guid keyID, Ed25519Key[] mgOrkij, int numKeys, Ed25519Point[] gMultiplier, string[] to_userNames){

        if (gMultiplier.All(multipler => multipler.IsSafePoint()))
        {
            throw new Exception("Not all points supplied are safe");
        }

        // Generate DiffieHellman Keys based on this ork's priv and other Ork's Pubs
        AesKey[] ECDHij = mgOrkij.Select(key => createKey(key.Y)).ToArray();

        // Here we generate the X values of the polynomial through creating GUID from other orks publics, then generating a bigInt (the X) from those GUIDs
        // This was based on how the JS creates the X values from publics in ClientBase.js and IdGenerator.js
        var mgOrkj_Xs = mgOrkij.Select(pub => new BigInteger(new Guid(Utils.Hash(pub.ToByteArray())).ToByteArray(), true, true)); 

        long timestampi = DateTime.UtcNow.Ticks;
        RandomField rdm = new RandomField(Ed25519.N);

        BigInteger[] k = new BigInteger[numKeys - 1];
        Ed25519Point[] gK = new Ed25519Point[numKeys - 1];
        List<IReadOnlyList<Point>> Yij = new List<IReadOnlyList<Point>>();
        IEnumerable<ShareEncrypted> YCiphers;
        List<Ed25519Point> gMultiplied = new List<Ed25519Point>();

        for(int i = 0; i < numKeys; i++){
            // Generate random k shard
            k[i] = rdm.Generate(BigInteger.One);

            // Calculate public shard
            gK[i] = Ed25519.G * k[i];

            // For each ORK, secret share value ki
            Yij.Add(EccSecretSharing.Share(k[i], mgOrkj_Xs, Threshold, Ed25519.N));

            // Multiply the required multipliers
            foreach(Ed25519Point multiplier in gMultiplier){
                gMultiplied.Add(multiplier * k[i]);
            }
        }
        // Encrypt shares and partial public with each ork key
        YCiphers = to_userNames.Select((username, i) => encryptShares(ECDHij, Yij, i, username));

        GenShardResponse response = new GenShardResponse(gK[0], YCiphers, gMultiplied, timestampi);

        return JsonSerializer.Serialize(response);
    }

    private AesKey createKey(Ed25519Point point){
        if(MgOrki.IsEquals(point)){  // TODO: create more efficient isEquals function
            return AesKey.Seed(MSecOrki.ToByteArray(true, true));
        }
        else{
            return AesKey.Seed((point * MSecOrki).ToByteArray());
        }
    }

    private ShareEncrypted encryptShares(AesKey[] DHKeys, List<IReadOnlyList<Point>> shares, int index, string to_username){

        var data_to_encrypt = new { shares = shares.Select(pointShares => pointShares[index]) };

        var orkShare = new ShareEncrypted{
            To = to_username,
            From = From_Username,
            EncryptedData = DHKeys[index].EncryptStr(JsonSerializer.Serialize(data_to_encrypt))
        };
        
        return orkShare;
    }

    private class ShareEncrypted {
        public string To {get;set;} /// Ork Username the share will go to
        public string From {get;set;} /// Ork Username the share is sent from
        public string EncryptedData {get;set;} // this is the DataToEncrypt object encrypted
    }

    private class GenShardResponse {
        public byte[] GK {get; set;} // represents G * k[i]  ToByteArray()
        public ShareEncrypted[] EncryptedOrkShares {get; set;}
        public byte[][] GMultipliers {get; set;}
        public string Timestampi {get; set;}
        public GenShardResponse(Ed25519Point gK, IEnumerable<ShareEncrypted> encryptedOrkShares, List<Ed25519Point> gMultipliers, long timestampi){
            GK = gK.ToByteArray();
            EncryptedOrkShares = encryptedOrkShares.ToArray();
            GMultipliers = gMultipliers.Select(multiplier => multiplier.ToByteArray()).ToArray();
            Timestampi = timestampi.ToString();
        }

    }
}

