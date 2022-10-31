using System;


namespace Tide.Ork.Models {
     public class CmkResponse
    {
        
        public byte[] GBlureCMKi {get; set;}
        public byte[] GR {get; set;}
        public byte[] GCMK {get; set;}

        public CmkResponse()
        {     
           
        }

        public byte[] ToByteArray()
        {
            var buffer = new byte[192];
            GBlureCMKi.CopyTo(buffer, 0);
            GR.CopyTo(buffer, 64);
            GCMK.CopyTo(buffer, 128);

            return buffer;
        }


    }
}