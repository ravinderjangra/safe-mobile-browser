using System;
using System.Collections.Generic;
using System.Text;

namespace SafeMobileBrowser.Models
{
    public class WebFetchResponse
    {
        public Dictionary<string, string> Headers { get; set; }
        public string MimeType { get; set; }
        public byte[] Data { get; set; }

        public WebFetchResponse()
        {
            Headers = new Dictionary<string, string>();
        }
    }
}
