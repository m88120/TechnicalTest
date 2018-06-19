using System;
using System.Collections.Generic;

namespace TechnicalCore.Context
{
    public partial class Buckets
    {
        public Buckets()
        {
            Items = new HashSet<Items>();
        }

        public int BucketId { get; set; }
        public string Description { get; set; }
        public double? OrderInVertical { get; set; }

        public ICollection<Items> Items { get; set; }
    }
}
