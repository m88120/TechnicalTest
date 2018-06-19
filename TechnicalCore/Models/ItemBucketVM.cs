using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TechnicalCore.Models
{
    public class ItemBucketVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public double Priority { get; set; }
        public double Version { get; set; }
        public string MockupLink { get; set; }
        public string BucketDescription { get; set; }
        public double OrderInLane { get; set; }
        public double OrderInVertical { get; set; }
    }
}