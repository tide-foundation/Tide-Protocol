using System;
using System.Text.Json;

namespace Tide.Ork.Models {
     public class AuthRequest
    {
        public byte[] UserId {get; set;}
        public byte[] CertTime {get; set;}
        public string BlurHCmkMul {get; set;}
        public string BlurRMul {get; set;} 
    }
}