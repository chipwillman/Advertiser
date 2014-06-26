namespace Advertiser.Mongo.Models
{
    using System;

    using MongoDB.Bson;
    using MongoDB.Driver.GeoJsonObjectModel;

    public class Impression
    {
        public ObjectId Id { get; set; }
        public Guid CampaignFk { get; set; }
        public string Referrer { get; set; }
        public string IpAddress { get; set; }
        public GeoJson2DGeographicCoordinates SearchLocation { get; set; }
        public DateTime CreatedUtc { get; set; }
    }
}
