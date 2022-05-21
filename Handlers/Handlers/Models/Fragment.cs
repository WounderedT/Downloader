using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handlers.Models
{
    internal struct Fragment
    {
        public Int32 Id { get; }
        public Int64 FragmentSize { get; }
        public Int64 ExpectedLengthInBytes { get; }

        internal Fragment(Int32 id, Int64 fragmentSize, Int64 expectedLengthInBytes)
        {
            Id = id;
            FragmentSize = fragmentSize;
            ExpectedLengthInBytes = expectedLengthInBytes;
        }
    }
}
