namespace Advertiser.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Data.Entity.Spatial;

    public class AdContext : DbContext
    {
        public AdContext()
            : base("DefaultConnection")
        {
            
        }

        public DbSet<EstablishmentModel> Establishments { get; set; }

        public DbSet<AdvertisementModel> Advertisements { get; set; }
        
        public DbSet<CampaignModel> Campaigns { get; set; }
    }

    [Table("Advertisements")]
    public class AdvertisementModel
    {
        public Guid AdvertisementPk { get; set; }

        public Guid UserFk { get; set; }

        public string AdvertisementType { get; set; }

        public string AdvertisementHtml { get; set; }
    }

    [Table("Establishments")]
    public class EstablishmentModel
    {
        public Guid EstablishmentPk { get; set; }

        public string Name { get; set; }
        
        public string Description { get; set; }

        public Guid UserFk { get; set; }
    }

    [Table("Campaigns")]
    public class CampaignModel
    {
        public Guid CampaignPk { get; set; }

        public Guid UserFk { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public DateTime SpecialStarttime { get; set; }

        public DateTime SpecialEndTime { get; set; }

        public Guid AdvertisementFk { get; set; }

        public decimal MaximumSpend { get; set; }

        public DbGeography Location { get; set; }

        public decimal Zone1Price { get; set; } // 1 km - nearby

        public decimal Zone2Price { get; set; } // 5 km - suburb

        public decimal Zone3Price { get; set; } // 20 km - city

        public decimal Zone4Price { get; set; } // 200 km - state

        public decimal Zone5Price { get; set; } // 500 km - country

        public decimal Zone6Price { get; set; } // 2000 km - contenent

        public string CampaignType { get; set; }
    }

    [Table("ActiveNearbySpecials")]
    public class ActiveNearbySpecialModel
    {
        public Guid ActiveNearbySpecialPk { get; set; }

        public decimal MaximumSpend { get; set; }

        public DbGeography Location { get; set; }

        public decimal Price { get; set; }

        public Guid AdvertisementPk { get; set; }

        public Guid EstablishmentPk { get; set; }
    }
}