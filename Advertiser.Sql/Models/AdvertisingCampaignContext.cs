namespace Advertiser.Sql.Models
{
    using System.Data.Entity;

    public class AdvertisingCampaignContext : DbContext
    {
        public AdvertisingCampaignContext()
            : base("AdvertisingCampaignConnection")
        {
            
        }

        public DbSet<Campaign> Campaigns { get; set; }
    }
}
