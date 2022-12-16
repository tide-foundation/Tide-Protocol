using System;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Text.Json;
using System.Collections.Generic;
using Tide.Encryption.SecretSharing;
using Tide.Encryption.Ed;
using Tide.Encryption.Tools;
using Tide.Encryption.AesMAC;
using System.Text.Json.Serialization;
using Tide.Core;

public class KeyGenerator
{
    private BigInteger MSecOrki { get; } // this ork's private scalar
    internal AesKey MSecOrki_Key => AesKey.Seed(MSecOrki.ToByteArray(true, true));
    private Ed25519Point MgOrki { get; } // this ork's public point
    internal Ed25519Key mgOrki_Key => new Ed25519Key(0, MgOrki);
    private string My_Username { get; } // this ork's username
    public int Threshold { get; } // change me
    private readonly Caching _cachingManager;

    public KeyGenerator(BigInteger mSecOrki, Ed25519Point mgOrki, string my_Username, int threshold)
    {
        MSecOrki = mSecOrki;
        MgOrki = mgOrki;
        My_Username = my_Username;
        Threshold = threshold;
        _cachingManager = new Caching();
    }

    public string GenShard(string keyID, Ed25519Key[] mgOrkij, int numKeys, Ed25519Point[] gMultiplier, string[] to_userNames)
    {

        if (gMultiplier != null && !gMultiplier.All(multipler => multipler.IsSafePoint()))
        {
            throw new Exception("GenShard: Not all points supplied are safe");
        }
        if (mgOrkij.Count() != to_userNames.Count())
        {
            throw new Exception("GenShard: Length of keys supplied is not equal to length of supplied ork usernames");
        }
        if (mgOrkij.Count() < 2)
        {
            throw new Exception("GenShard: Number of ork keys provided must be greater than 1");
        }
        if (numKeys < 1)
        {
            throw new Exception("GenShard: Number of keys requested must be at minimum 1");
        }

        // Generate DiffieHellman Keys based on this ork's priv and other Ork's Pubs
        AesKey[] ECDHij = mgOrkij.Select(key => createKey(key.Y)).ToArray();

        // Here we generate the X values of the polynomial through creating GUID from other orks publics, then generating a bigInt (the X) from those GUIDs
        // This was based on how the JS creates the X values from publics in ClientBase.js and IdGenerator.js
        var mgOrkj_Xs = mgOrkij.Select(pub => new BigInteger(new Guid(Utils.Hash(pub.GetPublic().ToByteArray()).Take(16).ToArray()).ToByteArray(), true, true));

        long timestampi = DateTime.UtcNow.Ticks;
        RandomField rdm = new RandomField(Ed25519.N);

        BigInteger[] k = new BigInteger[numKeys];
        Ed25519Point[] gK = new Ed25519Point[numKeys];
        Point[][] Yij = new Point[numKeys][];
        Ed25519Point[] gMultiplied = new Ed25519Point[gMultiplier ==null ? 0 : gMultiplier.Count()];

        for (int i = 0; i < numKeys; i++)
        {
            // Generate random k shard
            k[i] = rdm.Generate(BigInteger.One);

            // Calculate public shard
            gK[i] = Ed25519.G * k[i];

            // For each ORK, secret share value ki
            Yij[i] = (EccSecretSharing.Share(k[i], mgOrkj_Xs, Threshold, Ed25519.N)).ToArray();

            // Multiply the required multipliers
            try{
                if(gMultiplier != null)
                    gMultiplied[i] = gMultiplier[i] * k[i];
            }catch(IndexOutOfRangeException e){} // only multiply the available multipliers
            
            
        }
        // Encrypt shares and partial public with each ork key
        ShareEncrypted[] YCiphers = to_userNames.Select((username, i) => encryptShares(ECDHij, Yij, gK, i, timestampi, username, keyID)).ToArray();

        GenShardResponse response = new GenShardResponse
        {
            GK = gK[0].ToByteArray(),
            EncryptedOrkShares = YCiphers,
            GMultipliers =  gMultiplier == null ? null : gMultiplied.Select(multiplier => multiplier.ToByteArray()).ToArray(),
            Timestampi = timestampi.ToString()
        };

        return JsonSerializer.Serialize(response);
    }
    /// <summary>
    /// Make sure orkShares provided are sorted in same order as mgOrkij. For example, orkshare[0].From = ork2 AND mgOrkij[0] = ork2's public.
    /// This function cannot correlate orkId to public key unless it's in the same order
    /// </summary>
    public (string,string) SetKey(string keyID, string[] orkShares, Ed25519Key[] mgOrkij)
    {
        IEnumerable<ShareEncrypted> encryptedShares = orkShares.Select(share => JsonSerializer.Deserialize<ShareEncrypted>(share)); // deserialize all ork shares back into objects
        if (!encryptedShares.All(share => share.To.Equals(My_Username)))
        {
            throw new Exception("SetKey: One or more of the shares were sent to the incorrect ork");
        }

        // Decrypts only the shares that were sent to itself and the partial publics
        AesKey[] ECDHij = mgOrkij.Select(key => createKey(key.Y)).ToArray();
        IEnumerable<DataToEncrypt> decryptedShares = encryptedShares.Select((share, i) => decryptShares(share, ECDHij[i]));
        if (!decryptedShares.All(share => share.KeyID.Equals(keyID))) // check that no one is attempting to recreate someone else's key for their own account
        {
            throw new Exception("SetKey: KeyID of this share does not equal KeyID supplied");
        }

        // Verify the time difference is not material (30min)
        long timestamp = Median(decryptedShares.Select(share => long.Parse(share.Timestampi)).ToArray()); // get median of timestamps
        if (!decryptedShares.All(share => VerifyDelay(long.Parse(share.Timestampi), timestamp)))
        {
            throw new Exception("SetKey: One or more of the shares has expired");
        }

        int numKeys = decryptedShares.First().PartialPubs.Count();
        Ed25519Point[] gK = new Ed25519Point[numKeys];
        BigInteger[] Y = new BigInteger[numKeys];
        Ed25519Point[] gKTest = new Ed25519Point[numKeys];
        for (int i = 0; i < numKeys; i++) // will iterate by the number of keys to build
        { 
            // Add own all previously encrypted gKs together to mitigate malicious user
            gK[i] = decryptedShares.Aggregate(Ed25519.Infinity, (total, next) => total + Ed25519Point.From(next.PartialPubs[i]));

            // Aggregate all shares to form final Y coordinate
            Y[i] = decryptedShares.Aggregate(BigInteger.Zero, (sum, point) => (sum + new BigInteger(point.Shares[i], true, true)) % Ed25519.N);

            // Generate sharded public key for final verification
            gKTest[i] = Ed25519.G * Y[i];
        }

        // Encrypt latest state with this ork's private key
        string data_to_encrypt = MSecOrki_Key.EncryptStr(JsonSerializer.Serialize(new StateData
        {
            KeyID = decryptedShares.First().KeyID,
            Timestampi = timestamp.ToString(),
            gKn = gK.Select(point => point.ToByteArray()).ToArray(),
            Yn = Y.Select(num => num.ToByteArray(true, true)).ToArray()
        }));

        // Generate EdDSA R from all the ORKs publics
        byte[] MData_To_Hash = gK[0].ToByteArray().Concat(BitConverter.GetBytes(timestamp).Concat(Encoding.ASCII.GetBytes(keyID))).ToArray(); // M = hash( gK[1] | timestamp | keyID )
        byte[] M = Utils.HashSHA512(MData_To_Hash);
        //byte[] rData_To_Hash = MSecOrki.ToByteArray(true, true).Concat(M).ToArray();
        //BigInteger ri = new BigInteger(Utils.HashSHA512(rData_To_Hash), true, false).Mod(Ed25519.N);
        RandomField rdm = new RandomField(Ed25519.N);
        var ri =  rdm.Generate(BigInteger.One);
        var RKey = rdm.Generate(BigInteger.One);
        _cachingManager.AddOrGetCache(RKey.ToString(),ri.ToString()).GetAwaiter().GetResult();
        
        Ed25519Point gRi = Ed25519.G * ri;

        var response = new SetKeyResponse
        {
            gKTesti = gKTest.Select(point => point.ToByteArray()).ToArray(),
            gRi = gRi.ToByteArray(),
            EncryptedData = data_to_encrypt
        };
        return (JsonSerializer.Serialize(response), MSecOrki_Key.EncryptStr(RKey.ToString()));
    }
    public PreCommitResponse PreCommit(string keyID, Ed25519Point[] gKntest, Ed25519Key[] mgOrkij, Ed25519Point R2, string EncSetKeyStatei, string randomKey)
    {
        var key =  MSecOrki_Key.DecryptStr(randomKey);
        string r =  _cachingManager.AddOrGetCache(key, string.Empty).GetAwaiter().GetResult();
        if(r == null || r == ""){
            throw new Exception("PreCommit: Random not found in cache");          
        }
        _cachingManager.Remove(randomKey);
        
        // Reastablish state
        StateData state = JsonSerializer.Deserialize<StateData>(MSecOrki_Key.DecryptStr(EncSetKeyStatei)); // decrypt encrypted state in response

        if(!state.KeyID.Equals(keyID))
        {
            throw new Exception("PreCommit: KeyID of instanciated object does not equal that of previous state");
        }
        if(!VerifyDelay(long.Parse(state.Timestampi), DateTime.UtcNow.Ticks))
        {
            throw new Exception("PreCommit: State has expired");
        }
        Ed25519Point[] gKn = state.gKn.Select(bytes => Ed25519Point.From(bytes)).ToArray();
        byte[] MData_To_Hash = gKn[0].ToByteArray().Concat(Encoding.ASCII.GetBytes(state.Timestampi)).Concat(Encoding.ASCII.GetBytes(keyID)).ToArray(); // M = hash( gK[1] | timestamp | keyID )
        byte[] M = Utils.Hash(MData_To_Hash);
      
        BigInteger ri = BigInteger.Parse(r);

        // Verifying both publics
        if(!gKntest.Select((gKtest, i) => gKtest.IsEquals(gKn[i])).All(verify => verify == true)){ // check all elements of gKtest[n] == gK[n]
             throw new Exception("PreCommit: gKtest failed");
        }

        // This is done only on the first key
        Ed25519Point R = mgOrkij.Aggregate(Ed25519.Infinity, (sum, next) => next.Y + sum) + R2;

        // Prepare the signature message
        byte[] HData_To_Hash = R.ToByteArray().Concat(gKn[0].ToByteArray()).Concat(M).ToArray();
        BigInteger H = new BigInteger(Utils.HashSHA512(HData_To_Hash), true, false).Mod(Ed25519.N);


        // Calculate the lagrange coefficient for this ORK
        var mgOrkj_Xs = mgOrkij.Select(pub => new BigInteger(new Guid(Utils.Hash(pub.GetPublic().ToByteArray()).Take(16).ToArray()).ToByteArray(), true, true));
        BigInteger my_X = new BigInteger(new Guid(Utils.Hash(this.mgOrki_Key.ToByteArray()).Take(16).ToArray()).ToByteArray(), true, true);
        BigInteger li = EccSecretSharing.EvalLi(my_X, mgOrkj_Xs, Ed25519.N);
        // Generate the partial signature
        BigInteger Y = new BigInteger(state.Yn[0], true, true);
        BigInteger Si = this.MSecOrki + ri + (H * Y * li);
       // BigInteger Si = this.MSecOrki + BigInteger.Parse(r) + (H * Y * li);
        
        
        return new PreCommitResponse{
                Timestampi = state.Timestampi,
                gKn = state.gKn.Select(gK => Ed25519Point.From(gK)).ToArray(),
                Yn = state.Yn.Select(Y => new BigInteger(Y, true, true)).ToArray(),
                S = Si
        };
    }

    public CommitResponse Commit(string keyID, BigInteger S, Ed25519Key[] mgOrkij, Ed25519Point R2, string EncSetKeyStatei)
    {
        // Reastablish state
       // SetKeyResponse decryptedResponse = JsonSerializer.Deserialize<SetKeyResponse>(EncSetKeyStatei);  // deserialize reponse
        StateData state = JsonSerializer.Deserialize<StateData>(MSecOrki_Key.DecryptStr(EncSetKeyStatei)); // decrypt encrypted state in response

        if(!state.KeyID.Equals(keyID))
        {
            throw new Exception("Commit: KeyID of instanciated object does not equal that of previous state");
        }
        if(!VerifyDelay(long.Parse(state.Timestampi), DateTime.UtcNow.Ticks))
        {
            throw new Exception("Commit: State has expired");
        }

        Ed25519Point gK = Ed25519Point.From(state.gKn[0]);
        byte[] MData_To_Hash = gK.ToByteArray().Concat(Encoding.ASCII.GetBytes(state.Timestampi).Concat(Encoding.ASCII.GetBytes(keyID))).ToArray(); // M = hash( gK[1] | timestamp | keyID )
        byte[] M = Utils.Hash(MData_To_Hash);

        Ed25519Point R = mgOrkij.Aggregate(Ed25519.Infinity, (sum, next) => next.Y + sum) + R2;

        byte[] HData_To_Hash = R.ToByteArray().Concat(gK.ToByteArray()).Concat(M).ToArray();
        BigInteger H = new BigInteger(Utils.HashSHA512(HData_To_Hash), true, false).Mod(Ed25519.N);

        // Verify the Signature 
        bool valid = (Ed25519.G * S).IsEquals(R + (gK * H));

        if(!valid)
        {
            throw new Exception("Commit: Validation failed");
        }

        return new CommitResponse{
            Timestampi = long.Parse(state.Timestampi),
            gKn = state.gKn.Select(gK => Ed25519Point.From(gK)).ToArray(),
            Yn = state.Yn.Select(Y => new BigInteger(Y, true, true)).ToArray()
        };
    }
    private AesKey createKey(Ed25519Point point)
    {
        if (MgOrki.IsEquals(point))
        {  // TODO: create more efficient isEquals function
            return MSecOrki_Key;
        }
        else
        {
            return AesKey.Seed((point * MSecOrki).ToByteArray());
        }
    }
    private ShareEncrypted encryptShares(AesKey[] DHKeys, Point[][] shares, Ed25519Point[] gK, int index, long timestampi, string to_username, string keyID)
    {
        var data_to_encrypt = new DataToEncrypt
        {
            KeyID = keyID,
            Timestampi = timestampi.ToString(),
            Shares = shares.Select(pointShares => pointShares[index].Y.ToByteArray(true, true)).ToArray(),
            PartialPubs = gK.Select(partialPub => partialPub.ToByteArray()).ToArray()
        };
        var orkShare = new ShareEncrypted
        {
            To = to_username,
            From = My_Username,
            EncryptedData = DHKeys[index].EncryptStr(JsonSerializer.Serialize(data_to_encrypt))
        };
        return orkShare;
    }



    private DataToEncrypt decryptShares(ShareEncrypted encryptedShare, AesKey DHKey)
    {
        return JsonSerializer.Deserialize<DataToEncrypt>(DHKey.DecryptStr(encryptedShare.EncryptedData)); // decrypt encrypted share and create DataToEncrypt object
    }
    private bool VerifyDelay(long timestamp, long timestampi)
    {
        return (Math.Abs(timestamp - timestampi) < 18000000000); // Checks different between timestamps is less than 30 min
    }
    private long Median(long[] data)  // TODO: implement this somewhere better in Cryptide
    {
        Array.Sort(data);
        if (data.Length % 2 == 0)
            return (data[data.Length / 2 - 1] + data[data.Length / 2]) / 2;
        else
            return data[data.Length / 2];
    }

    // USE BETTER OOP HERE> ITS DISGUSTING
    public class CommitResponse
    {
        public long Timestampi {get; set;}
        public Ed25519Point[] gKn {get; set;}
        public BigInteger[] Yn {get; set;}
    }
     public class PreCommitResponse
    {
        public string Timestampi {get; set;}
        public Ed25519Point[] gKn {get; set;}
        public BigInteger[] Yn {get; set;}
        public BigInteger S {get; set;}
    }
    internal class StateData
    {
        public string KeyID { get; set; } // Guid of key to string()
        public string Timestampi { get; set; }
        public byte[][] gKn { get; set; }
        public byte[][] Yn { get; set; }
    }
    internal class SetKeyResponse
    {
        public byte[][] gKTesti { get; set; } //ed25519Points
        public byte[] gRi { get; set; } //ed25519Point
        public string EncryptedData { get; set; } // encrypted StateData
    }

    internal class DataToEncrypt
    {
        public string KeyID { get; set; } // Guid of key to string()
        public string Timestampi { get; set; }
        public byte[][] Shares { get; set; }
        public byte[][] PartialPubs { get; set; }
    }
    internal class ShareEncrypted
    {
        public string To { get; set; } /// Ork Username the share will go to
        public string From { get; set; } /// Ork Username the share is sent from
        public string EncryptedData { get; set; } // this is the DataToEncrypt object encrypted
    }
    internal class GenShardResponse
    {
        public byte[] GK { get; set; } // represents G * k[i]  ToByteArray()
        public ShareEncrypted[] EncryptedOrkShares { get; set; }
        public byte[][] GMultipliers { get; set; }
        public string Timestampi { get; set; }
    }
}

