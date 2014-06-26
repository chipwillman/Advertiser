using System;

namespace Advertiser.PosGis.Models
{
    public class Campaign
    {
        public Guid CampaignPk { get; set; }
        public Guid OwnerFk { get; set; }
        public int Cpm { get; set; }
        public int Budget { get; set; }
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public int Range { get; set; }
        public double MinLatitude { get; set; }
        public double MaxLatitude { get; set; }
        public double MinLongitude { get; set; }
        public double MaxLongitude { get; set; }
        public string AdText { get; set; }
    }
}
