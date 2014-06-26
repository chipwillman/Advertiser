namespace Advertiser.Mongo.Models
{
    using System;

    using MongoDB.Bson;
    using MongoDB.Driver.GeoJsonObjectModel;

    public class Campaign
    {
        public ObjectId Id { get; set; }
        public Guid CampaignPk { get; set; }
        public Guid OwnerFk { get; set; }
        public int Cpm { get; set; }
        public int Budget { get; set; }
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public int Range { get; set; }
        public GeoJsonPolygon<GeoJson2DGeographicCoordinates> Bounds { get; set; }
        public string[] AdText { get; set; }
    }
}
