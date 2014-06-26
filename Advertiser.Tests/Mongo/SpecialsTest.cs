namespace Advertiser.Tests.Mongo
{
    using System;

    using Advertiser.Mongo.Models;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;
    using MongoDB.Driver.GeoJsonObjectModel;

    [TestClass]
    public class SpecialsTest
    {
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void Test()
        {
            var mongoClient = new MongoClient();
            var mongo = mongoClient.GetServer();
            
        }

        [TestMethod]
        public void TestAddSpecialCampaign()
        {
            var mongoClient = new MongoClient();
            var mongo = mongoClient.GetServer();
            var db = mongo.GetDatabase("advertiser");
            using (mongo.RequestStart(db))
            {
                var collection = db.GetCollection<BsonDocument>("campaigns");
                collection.RemoveAll();
            }
            AddSampleRecord(mongo, db);

            var readOnlyCollection = db.GetCollection<Campaign>("campaigns");

            var query = Query.GeoIntersects("Bounds", GeoJson.Point(new GeoJson2DGeographicCoordinates(144.95804505279535, -37.81088734804004)));

            // var query = new QueryDocument("CampaignPk", new Guid("552EBAB0-C879-4F81-A637-5B52AC4B8B25"));
            var campaigns = readOnlyCollection.Find(query);
            Assert.IsNotNull(campaigns);
            foreach (var campaign in campaigns)
            {
                Assert.IsNotNull(campaign);
            }
        }

        private static void AddSampleRecord(MongoServer mongo, MongoDatabase db)
        {
            var bounds = GeoJson.Polygon(
                new GeoJson2DGeographicCoordinates(144.94804505279535, -37.82088734804004),
                new GeoJson2DGeographicCoordinates(144.94804505279535, -37.807326029194684),
                new GeoJson2DGeographicCoordinates(144.97851494720453, -37.807326029194684),
                new GeoJson2DGeographicCoordinates(144.97851494720453, -37.82088734804004),
                new GeoJson2DGeographicCoordinates(144.94804505279535, -37.82088734804004));
            using (mongo.RequestStart(db))
            {
                var campaign = new Campaign
                                   {
                                       CampaignPk = new Guid("552EBAB0-C879-4F81-A637-5B52AC4B8B25"),
                                       OwnerFk = new Guid("2060EBBC-53DA-43BA-8548-20469F4A2F45"),
                                       Cpm = 1,
                                       Budget = 100000,
                                       StartUtc = DateTime.Parse("2014-01-31 16:00:00Z"),
                                       EndUtc = DateTime.Parse("2014-03-01 16:00:00Z"),
                                       Range = 5000,
                                       Bounds = bounds
                                   };

                var collection = db.GetCollection<Campaign>("campaigns");

                collection.Insert(campaign);
            }
        }
    }
}
