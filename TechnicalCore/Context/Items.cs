using System;
using System.Collections.Generic;

namespace TechnicalCore.Context
{
    public partial class Items
    {
        public int ItemId { get; set; }
        public string ItemTitle { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public double? Priority { get; set; }
        public double? Version { get; set; }
        public string MockUpLink { get; set; }
        public int? BucketId { get; set; }
        public double? OrderInLane { get; set; }

        public Buckets Bucket { get; set; }
    }
}
