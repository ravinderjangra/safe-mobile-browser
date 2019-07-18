using System.Collections.Generic;

namespace SafeMobileBrowser.Models
{
    public class WebFetchOptions
    {
        public List<ByteRangeHeader> RangeHeader { get; set; }
    }

    public class ByteRangeHeader
    {
        public ulong Start { get; }

        public ulong End { get; }

        public ByteRangeHeader(ulong startByteIndex, ulong endByteIndex)
        {
            Start = startByteIndex;
            End = endByteIndex;
        }
    }
}
