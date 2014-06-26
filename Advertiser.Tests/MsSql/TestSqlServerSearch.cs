namespace Advertiser.Tests.MsSql
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Spatial;
    using System.Linq;

    using Advertiser.Sql.Models;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using wwDrink.data;
    using wwDrink.data.Models;

    [TestClass]
    public class TestSqlServerSearch
    {
        [TestMethod]
        public void TestEstablishmentSearch()
        {
            var db = new AdvertisingCampaignContext();
            //db.Database.Initialize(false);

            db.Campaigns.RemoveRange(db.Campaigns.ToArray());

            AddSampleCampaigns(db);

            var startTime = DateTime.Now;

            //["Kensington, Australia", "-37.7949955", "144.92515895", "1500"],
            var campaigns = CampaignCampaigns(-37.7949955, 144.92515895, db);
            //["Melbourne, Australia (AU)", "-37.81407309669506", "144.96439579895014", "1500"],
            campaigns = CampaignCampaigns(-37.81407309669506, 144.96439579895014, db);
            Assert.IsTrue(campaigns.Length > 0);
            //["Belfast, Northern Ireland", "54.59258586598787", "-5.932437428588863", "1100"],
            campaigns = CampaignCampaigns(54.59258586598787, -5.932437428588863, db);
            Assert.IsTrue(campaigns.Length > 0);
            //["Derry, Northern Ireland", "54.999243520707154", "-7.322133534208317", "300"],
            campaigns = CampaignCampaigns(54.999243520707154, -7.322133534208317, db);
            Assert.IsTrue(campaigns.Length > 0);
            //["Oxford, England", "51.752100602253314", "-1.2567392470825833", "1200"],
            campaigns = CampaignCampaigns(51.752100602253314, -1.2567392470825833, db);
            Assert.IsTrue(campaigns.Length > 0);
            //["Amsterdam, Neatherlands", "52.37558253868254", "4.8953170978763705", "600"],
            campaigns = CampaignCampaigns(52.37558253868254, 4.8953170978763705, db);
            Assert.IsTrue(campaigns.Length > 0);
            //["Copenhagen, Denmark", "55.676205694010136", "12.568444388360591", "550"],
            campaigns = CampaignCampaigns(55.676205694010136, 12.56844438836059, db);
            Assert.IsTrue(campaigns.Length > 0);
            //["Clarke Quay, Singapore", "1.2892135032791805", "103.84820018900233", "250"]
            campaigns = CampaignCampaigns(1.2892135032791805, 103.84820018900233, db);
            Assert.IsTrue(campaigns.Length > 0);
            var time = DateTime.Now - startTime;
            Assert.AreEqual(0.1, time.ToString());

        }

        private static Campaign[] CampaignCampaigns(double latitude, double longitude, AdvertisingCampaignContext db)
        {
            var point = string.Format("POINT({1} {0})", latitude, longitude);
            var campaigns = (from e in db.Campaigns
                             where e.Bounds.Intersects(DbGeography.FromText(point))
                             orderby e.Bounds.Distance(DbGeography.FromText(point))
                             select e).Take(Constants.RecordCount).ToArray();
            return campaigns;
        }

        private static void AddSampleCampaigns(AdvertisingCampaignContext db)
        {
            var latitude = -37.788081;
            var longitude = 144.93672;
            var context = new RandomNightsContext();
            var searchLocation = DbGeography.FromText(string.Format("POINT({1} {0})", latitude, longitude));

            var establishments = (from e in context.Establishments
                                  //where e.Location.Distance(searchLocation) < 1000000
                                  //orderby e.Rating, e.Location.Distance(searchLocation)
                                  select e).Take(Constants.RecordCount).ToArray();
            var campaigns = new List<Campaign>();
            foreach (var establishment in establishments)
            {
                var c = AddSampleCampaign(establishment);
                if (c != null)
                {
                    campaigns.Add(c);
                }
            }
            db.Campaigns.AddRange(campaigns);
            db.SaveChanges();
        }

        private static Campaign AddSampleCampaign(Establishment establishment)
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
                string polyText;
                if (maxLatitude - minLatitude < 180 && (maxLongitude - minLongitude > 0) || (maxLongitude - minLongitude < 0 && maxLatitude - minLatitude < 0))
                {
                    polyText = string.Format(
                        "POLYGON(({0} {1}, {2} {1}, {2} {3}, {0} {3}, {0} {1}))",
                        minLongitude.ToString("0.######"),
                        minLatitude.ToString("0.######"),
                        maxLongitude.ToString("0.######"),
                        maxLatitude.ToString("0.######"));
                }
                else
                {
                    polyText = string.Format(
                        "POLYGON(({0} {1}, {0} {3}, {2} {3}, {2} {1}, {0} {1}))",
                        minLongitude.ToString("0.######"),
                        minLatitude.ToString("0.######"),
                        maxLongitude.ToString("0.######"),
                        maxLatitude.ToString("0.######"));
                }
                var bounds =
                    DbGeography.FromText(polyText, 4326);

                var result = new Campaign
                {
                    CampaignPk = Guid.NewGuid(),
                    OwnerFk = Guid.NewGuid(),
                    Cpm = cost,
                    Budget = budget,
                    StartUtc = DateTime.UtcNow,
                    EndUtc = DateTime.UtcNow,
                    Range = range,
                    Bounds = bounds,
                    AdText = establishment.Name
                };

                return result;
            }
            return null;
        }
    }
}
