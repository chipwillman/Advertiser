namespace Advertiser.Sql.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserProfile")]
    public class Campaign
    {
        [Key]
        public Guid CampaignPk { get; set; }
        public Guid OwnerFk { get; set; }
        public int Cpm { get; set; }
        public int Budget { get; set; }
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public int Range { get; set; }
        public string AdText { get; set; }
        public DbGeography Bounds { get; set; }
    }
}
