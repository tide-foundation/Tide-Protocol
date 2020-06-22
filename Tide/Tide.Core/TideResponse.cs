using System;
using System.Collections.Generic;
using System.Text;

namespace Tide.Core
{
    public class TideResponse
    {
        public TideResponse() {
            Success = true;
        }

        public TideResponse(bool success, object content, string error)
        {
            Success = success;
            Content = content;
            Error = error;
        }

        public TideResponse(string error)
        {
            Success = false;
            Content = null;
            Error = error;
        }

        public TideResponse(object obj)
        {
            Success = true;
            Content = obj;
            Error = null;
        }

        public bool Success { get; set; }
        public object Content { get; set; }
        public string Error { get; set; }
    }
}

