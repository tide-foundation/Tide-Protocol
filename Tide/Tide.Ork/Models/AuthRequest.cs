using System;
using System.Text.Json;

namespace Tide.Ork.Models {
     public class AuthRequest
    {
        public string UserId {get; set;}
        public string CertTime {get; set;}
        public string BlurHCmkMul {get; set;} 
    }
}