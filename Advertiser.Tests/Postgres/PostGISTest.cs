namespace Advertiser.Tests.Postgres
{
    using System;
    using System.Data.Entity.Spatial;
    using System.Linq;

    using Advertiser.PosGis;
    using Advertiser.PosGis.Models;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Npgsql;

    using wwDrink.data;
    using wwDrink.data.Models;

    [TestClass]
    public class PostGISTest
    {
        [TestMethod]
        public void TestPostgresTable()
        {
            NpgsqlConnection conn = new NpgsqlConnection("Server=localhost;Port=5432;User Id=postgres;Password=infotech;Database=AdvertisementCampaign;");
            conn.Open() ;
            var repository = new CampaignRepository(conn, null);
            repository.DeleteAll();
    
            AddSampleCampaigns(repository);

            var latitude = -37.788081;
            var longitude = 144.93672;

            // var query = new QueryDocument("CampaignPk", new Guid("552EBAB0-C879-4F81-A637-5B52AC4B8B25"));
            var startTime = DateTime.Now;
            var campaigns = repository.Search(latitude, longitude, 1000);
            //["Melbourne, Australia (AU)", "-37.81407309669506", "144.96439579895014", "1500"],
            campaigns = repository.Search(-37.81407309669506, 144.96439579895014, 1000);
            Assert.IsTrue(campaigns.Length > 0);
            //["Belfast, Northern Ireland", "54.59258586598787", "-5.932437428588863", "1100"],
            campaigns = repository.Search(54.59258586598787, -5.932437428588863, 1000);
            Assert.IsTrue(campaigns.Length > 0);
            //["Derry, Northern Ireland", "54.999243520707154", "-7.322133534208317", "300"],
            campaigns = repository.Search(54.999243520707154, -7.322133534208317, 1000);
            Assert.IsTrue(campaigns.Length > 0);
            //["Oxford, England", "51.752100602253314", "-1.2567392470825833", "1200"],
            campaigns = repository.Search(51.752100602253314, -1.2567392470825833, 1000);
            Assert.IsTrue(campaigns.Length > 0);
            //["Amsterdam, Neatherlands", "52.37558253868254", "4.8953170978763705", "600"],
            campaigns = repository.Search(52.37558253868254, 4.8953170978763705, 1000);
            Assert.IsTrue(campaigns.Length > 0);
            //["Copenhagen, Denmark", "55.676205694010136", "12.568444388360591", "550"],
            campaigns = repository.Search(55.676205694010136, 12.56844438836059, 1000);
            Assert.IsTrue(campaigns.Length > 0);
            //["Clarke Quay, Singapore", "1.2892135032791805", "103.84820018900233", "250"]
            campaigns = repository.Search(1.2892135032791805, 103.84820018900233, 1000);

            Assert.IsNotNull(campaigns);
            var time = DateTime.Now - startTime;
            Assert.AreEqual(0.1, time.ToString());
        }


        private static void AddSampleCampaigns(CampaignRepository repository)
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
                    AddSampleCampaign(repository, establishment);
                }
            }
        }

        private static void AddSampleCampaign(CampaignRepository repository, Establishment establishment)
        {
            if (establishment.Location.Latitude.HasValue && establishment.Location.Longitude.HasValue)
            {
                var range = 5000;
                var budget = 10000;
                var cost = 1;
                var radiansPerKilometer = 0.0096316678795954989 / 1070.9410769514438;
                var offset = radiansPerKilometer * range;
                var minLongitude = establishment.Location.Longitude.Value - offset;
                var minLatitude = establishment.Location.Latitude.Value - offset;
                var maxLongitude = establishment.Location.Longitude.Value + offset;
                var maxLatitude = establishment.Location.Latitude.Value + offset;

                var campaign = new Campaign
                {
                    CampaignPk = Guid.NewGuid(),
                    OwnerFk = Guid.NewGuid(),
                    Cpm = cost,
                    Budget = budget,
                    StartUtc = DateTime.UtcNow,
                    EndUtc = DateTime.UtcNow,
                    Range = range,
                    MinLatitude = minLatitude,
                    MinLongitude = minLongitude,
                    MaxLatitude = maxLatitude,
                    MaxLongitude = maxLongitude,
                    AdText = establishment.Name
                };

                repository.Insert(campaign);
            }
        }

    }
}
