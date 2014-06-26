// http://docs.mongodb.org/ecosystem/tutorial/use-csharp-driver/
// http://code.msdn.microsoft.com/Getting-started-with-37dbd5bd
// Start Mongo DB 
// C:\my_mongo_dir\bin> mongod
// 
namespace Advertiser.Tests.Mongo
{
    using System;
    using System.Data.Entity.Spatial;
    using System.Linq;

    using Advertiser.Mongo.Models;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;
    using MongoDB.Driver.GeoJsonObjectModel;

    using wwDrink.data;
    using wwDrink.data.Models;

    [TestClass]
    public class TestCampaignStats
    {
        [TestMethod]
        public void ImpressionsTest()
        {
            var mongoClient = new MongoClient();
            var mongo = mongoClient.GetServer();
            var db = mongo.GetDatabase("advertiser");
            using (mongo.RequestStart(db))
            {
                var collection = db.GetCollection<BsonDocument>("campaigns");
                collection.RemoveAll();
            }
            AddSampleCampaigns(mongo, db);

            var latitude = -37.788081;
            var longitude = 144.93672;

            var startTime = DateTime.Now;
            var readOnlyCollection = db.GetCollection<Campaign>("campaigns");

            var campaigns = Campaigns(latitude, longitude, readOnlyCollection);
            //["Melbourne, Australia (AU)", "-37.81407309669506", "144.96439579895014", "1500"],
            campaigns = Campaigns(-37.81407309669506, 144.96439579895014, readOnlyCollection);
            Assert.IsTrue(campaigns.Length > 0);
            //["Belfast, Northern Ireland", "54.59258586598787", "-5.932437428588863", "1100"],
            campaigns = Campaigns(54.59258586598787, -5.932437428588863, readOnlyCollection);
            Assert.IsTrue(campaigns.Length > 0);
            //["Derry, Northern Ireland", "54.999243520707154", "-7.322133534208317", "300"],
            campaigns = Campaigns(54.999243520707154, -7.322133534208317, readOnlyCollection);
            Assert.IsTrue(campaigns.Length > 0);
            //["Oxford, England", "51.752100602253314", "-1.2567392470825833", "1200"],
            campaigns = Campaigns(51.752100602253314, -1.2567392470825833, readOnlyCollection);
            Assert.IsTrue(campaigns.Length > 0);
            //["Amsterdam, Neatherlands", "52.37558253868254", "4.8953170978763705", "600"],
            campaigns = Campaigns(52.37558253868254, 4.8953170978763705, readOnlyCollection);
            Assert.IsTrue(campaigns.Length > 0);
            //["Copenhagen, Denmark", "55.676205694010136", "12.568444388360591", "550"],
            campaigns = Campaigns(55.676205694010136, 12.56844438836059, readOnlyCollection);
            Assert.IsTrue(campaigns.Length > 0);
            //["Clarke Quay, Singapore", "1.2892135032791805", "103.84820018900233", "250"]
            campaigns = Campaigns(1.2892135032791805, 103.84820018900233, readOnlyCollection);

            Assert.IsTrue(campaigns.Length > 0);
            var time = DateTime.Now - startTime;
            Assert.AreEqual(0.1, time.ToString());
        }

        private static Campaign[] Campaigns(double latitude, double longitude, MongoCollection<Campaign> readOnlyCollection)
        {
            var location = GeoJson.Point(new GeoJson2DGeographicCoordinates(longitude, latitude));
            var query = Query.GeoIntersects("Bounds", location);
            var campaigns = readOnlyCollection.Find(query).ToArray();
            return campaigns;
        }

        private static void AddSampleCampaigns(MongoServer mongo, MongoDatabase db)
        {
            var latitude = -37.788081;
            var longitude = 144.93672;
            var context = new RandomNightsContext();
            var searchLocation = DbGeography.FromText(string.Format("POINT({1} {0})", latitude, longitude));

            var establishments = (from e in context.Establishments
                                  //where e.Location.Distance(searchLocation) < 1000000
                                  //orderby e.Rating, e.Location.Distance(searchLocation)
                                  select e).Take(Constants.RecordCount).ToArray();

            foreach (var establishment in establishments)
            {
                //if (DistanceInMeters(latitude, longitude, establishment.Location.Latitude.Value, establishment.Location.Longitude.Value) < 5000)
                {
                    AddSampleCampaign(mongo, db, establishment);
                }
            }
        }

        private static void AddSampleCampaign(MongoServer mongo, MongoDatabase db, Establishment establishment)
        {
            if (establishment.Location.Latitude.HasValue && establishment.Location.Longitude.HasValue)
            {
                var range = 5000;
                var budget = 10000;
                var cost = 1;
                var bounds = CreateAdvertisingBounds(establishment.Location.Latitude.Value, establishment.Location.Longitude.Value, range);
                using (mongo.RequestStart(db))
                {
                    var campaign = new Campaign
                                       {
                                           CampaignPk = Guid.NewGuid(),
                                           OwnerFk = Guid.NewGuid(),
                                           Cpm = cost,
                                           Budget = budget,
                                           StartUtc = DateTime.UtcNow,
                                           EndUtc = DateTime.UtcNow,
                                           Range = range,
                                           Bounds = bounds
                                       };

                    var collection = db.GetCollection<Campaign>("campaigns");

                    collection.Insert(campaign);
                }
            }
        }

        private static GeoJsonPolygon<GeoJson2DGeographicCoordinates> CreateAdvertisingBounds(double latitude, double longitude, double range)
        {
            var radiusOfEarthMeters = 6371000;
            var radiansPerKilometer = 0.0096316678795954989 / 1070.9410769514438;
            var offset = radiansPerKilometer * range;

            var result = GeoJson.Polygon(
                new GeoJson2DGeographicCoordinates(longitude - offset, latitude - offset),
                new GeoJson2DGeographicCoordinates(longitude - offset, latitude + offset),
                new GeoJson2DGeographicCoordinates(longitude + offset, latitude + offset),
                new GeoJson2DGeographicCoordinates(longitude + offset, latitude - offset),
                new GeoJson2DGeographicCoordinates(longitude - offset, latitude - offset));
            return result;
        }

        public static double DistanceInMeters(double lat1, double lon1, double lat2, double lon2)
        {
            double theta = lon1 - lon2;
            double dist = Math.Sin(Deg2Rad(lat1)) * Math.Sin(Deg2Rad(lat2)) + Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) * Math.Cos(Deg2Rad(theta));
            dist = Math.Acos(dist);
            dist = Rad2Deg(dist);
            dist = dist * 60 * 1.1515 * 1.609344 * 1000;
            return (dist);
        } 

        private static double Deg2Rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        private static double Rad2Deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }

    }
}
