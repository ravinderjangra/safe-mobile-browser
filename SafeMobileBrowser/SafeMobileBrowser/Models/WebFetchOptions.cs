using System.Collections.Generic;

namespace SafeMobileBrowser.Models
{
    public class WebFetchOptions
    {
        public List<ByteRange> Range { get; set; }

        public WebFetchOptions()
        {
            Range = new List<ByteRange>();
        }
    }

    public class ByteRange
    {
        public ulong Start { get; set; }

        public ulong End { get; set; }
    }
}
