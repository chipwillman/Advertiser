namespace Advertiser.Mongo.Models
{
    using System;

    using MongoDB.Bson;

    public class CampaignSummary
    {
        public ObjectId Id { get; set; }
        public Guid CampaignFk { get; set; }
        public decimal Budget { get; set; }
        public decimal Spend { get; set; }
        public int Impressions { get; set; }
        public bool Completed { get; set; }

    }
}
