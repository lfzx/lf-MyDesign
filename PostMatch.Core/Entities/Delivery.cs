using System;
using System.Collections.Generic;
using System.Text;

namespace PostMatch.Core.Entities
{
    public class Delivery
    {
        public string DeliveryId { get; set; }
        public string ResumeId { get; set; }
        public string PostId { get; set; }
        public DateTime DeliveryUpdateTime { get; set; }
    }
}
