namespace Advertiser.PosGis
{
    using System;
    using System.Collections.Generic;

    using Advertiser.PosGis.Models;

    using Npgsql;

    using NpgsqlTypes;

    public class CampaignRepository
    {
        public CampaignRepository(NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            this.Connection = connection;
            this.Transaction = transaction;
        }

        public NpgsqlConnection Connection { get; set; }
        public NpgsqlTransaction Transaction { get; set; }

        public Campaign[] Search(double latitude, double longitude, double range)
        {
            var sqlText = @"SELECT
""CampaignPK"", 
""OwnerFK"", 
""Cpm"", 
""Budget"", 
""Range"", 
ST_AsKML(""Bounds"") as BoundsKml, 
""AdText"", 
""StartUtc"", 
""EndUtc""
FROM
   ""Campaigns""
WHERE
    ""Bounds"" && ST_GeographyFromText('Point(" + longitude.ToString("0.######") + " " + latitude.ToString("0.######") + @")')
ORDER BY 
ST_Distance(""Bounds"", ST_GeographyFromText('Point(" + longitude.ToString("0.######") + " " + latitude.ToString("0.######") + @")'))
";
            var result = new List<Campaign>();
            var cmd = new NpgsqlCommand(sqlText, Connection, Transaction);
            try
            {
                AddCommandParameters(cmd, "@Longitude", longitude);
                AddCommandParameters(cmd, "@Latitude", latitude);
                AddCommandParameters(cmd, "@Range", range);
                using (NpgsqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var campaign = new Campaign();
                        campaign.CampaignPk = (Guid)dr["CampaignPK"];
                        campaign.OwnerFk = (Guid)dr["OwnerFK"];
                        campaign.Cpm = (int)dr["Cpm"];
                        campaign.Budget = (int)dr["Budget"];
                        campaign.Range = (int)dr["Range"];
                        var bounds = (string)dr["BoundsKml"];
                        campaign.AdText = (string)dr["AdText"];
                        campaign.StartUtc = (DateTime)dr["StartUtc"];
                        campaign.EndUtc = (DateTime)dr["EndUtc"];
                        result.Add(campaign);
                    }
                }
            }
            finally
            {
                cmd.Dispose();
            }
            return result.ToArray();
        }

        public bool Insert(Campaign bizO)
        {
            bool result = false;
            string sqlText = @"INSERT INTO ""Campaigns""(
""CampaignPK"", 
""OwnerFK"", 
""Cpm"", 
""Budget"", 
""Range"", 
""Bounds"", 
""AdText"", 
""StartUtc"", 
""EndUtc""
)
    VALUES 
(
@CampaignPK, 
@OwnerFK, 
@Cpm, 
@Budget,
@Range, 
ST_GeographyFromText('POLYGON((" + CreatePolygonText(bizO) + @"))'), 
@AdText, 
@StartUtc, 
@EndUtc
);
";
            // ST_GeographyFromText('POLYGON(@MinLongitude @MinLatitude, @MaxLongitude @MinLatitude, @MaxLongitude @MaxLatitude, @MinLongitude  @MaxLatitude, @MinLongitude @MinLatitude), 4326)'), 

            var cmd = new NpgsqlCommand(sqlText, Connection, Transaction);
            try
            {
                AddCommandParameters(cmd, "@CampaignPK", bizO.CampaignPk);
                AddCommandParameters(cmd, "@OwnerFK", bizO.OwnerFk);
                AddCommandParameters(cmd, "@Cpm", bizO.Cpm);
                AddCommandParameters(cmd, "@Budget", bizO.Budget);
                AddCommandParameters(cmd, "@Range", bizO.Range);
                AddCommandParameters(cmd, "@AdText", bizO.AdText ?? "");
                AddCommandParameters(cmd, "@StartUtc", bizO.StartUtc);
                AddCommandParameters(cmd, "@EndUtc", bizO.EndUtc);
                int rowsAffected = (int)cmd.ExecuteNonQuery();
                result = rowsAffected != 0;
            }
            finally
            {
                cmd.Dispose();
            }
            return result;
        }

        private string CreatePolygonText(Campaign bizO)
        {
            string format = "0.######";
            return bizO.MinLongitude.ToString(format) + " " + bizO.MinLatitude.ToString(format) + ", "
                   + bizO.MinLongitude.ToString(format) + " " + bizO.MaxLatitude.ToString(format) + ", "
                   + bizO.MaxLongitude.ToString(format) + " " + bizO.MaxLatitude.ToString(format) + ", "
                   + bizO.MaxLongitude.ToString(format) + " " + bizO.MinLatitude.ToString(format) + ", "
                   + bizO.MinLongitude.ToString(format) + " " + bizO.MinLatitude.ToString(format);
        }

        public bool Update(Campaign bizO)
        {
            bool result = false;
            string sqlText = @"UPDATE ""Campaigns""
   SET ""CampaignPK""=@CampaignPK, ""OwnerFK""=@OwnerFK, ""Cpm""=@Cpm, ""Budget""=@Budget, ""Range""=@Range, 
       ""Bounds""=ST_GeographyFromText(POLYGON(('@MinLongitude @MinLatitude, @MaxLongitude @MinLatitude, @MaxLongitude @MaxLatitude, @MinLongitude  @MaxLatitude, @MinLongitude @MinLatitude'), 4326),
""AdText""=@AdText, ""StartUtc""=@StartUtc, ""EndUtc""=@EndUtc
, 
WHERE
    ""CampaignPK"" = @CampaignPK;
";

            var cmd = new NpgsqlCommand(sqlText, Connection, Transaction);
            try
            {
                AddCommandParameters(cmd, "@CampaignPK", bizO.CampaignPk);
                AddCommandParameters(cmd, "@OwnerFK", bizO.OwnerFk);
                AddCommandParameters(cmd, "@Cpm", bizO.Cpm);
                AddCommandParameters(cmd, "@Budget", bizO.Budget);
                AddCommandParameters(cmd, "@Range", bizO.Range);
                AddCommandParameters(cmd, "@MinLongitude", bizO.MinLongitude);
                AddCommandParameters(cmd, "@MaxLongitude", bizO.MaxLongitude);
                AddCommandParameters(cmd, "@MinLatitude", bizO.MinLatitude);
                AddCommandParameters(cmd, "@MaxLatitude", bizO.MaxLatitude);
                AddCommandParameters(cmd, "@AdText", bizO.AdText ?? "");
                AddCommandParameters(cmd, "@StartUtc", bizO.StartUtc);
                AddCommandParameters(cmd, "@EndUtc", bizO.EndUtc);
                int rowsAffected = (int)cmd.ExecuteNonQuery();
                result = rowsAffected != 0;
            }
            finally
            {
                cmd.Dispose();
            }
            return result;
        }

        public bool DeleteAll()
        {
            bool result = false;
            string sqlText = @"DELETE FROM ""Campaigns""";

            var cmd = new NpgsqlCommand(sqlText, Connection, Transaction);
            try
            {
                int rowsAffected = (int)cmd.ExecuteNonQuery();
                result = rowsAffected != 0;
            }
            finally
            {
                cmd.Dispose();
            }
            return result;
        }

        public bool Delete(Campaign bizO)
        {
            bool result = false;
            string sqlText = @"DELETE FROM ""Campaigns""
WHERE
    ""Campaign"" = @CampaignPK

";

            var cmd = new NpgsqlCommand(sqlText, Connection, Transaction);
            try
            {
                AddCommandParameters(cmd, "@CampaignPK", bizO.CampaignPk);
                int rowsAffected = (int)cmd.ExecuteNonQuery();
                result = rowsAffected != 0;
            }
            finally
            {
                cmd.Dispose();
            }
            return result;
        }

        protected void AddCommandParameters(NpgsqlCommand command, string dataName, object value)
        {
            NpgsqlParameter dbParameter = command.CreateParameter();
            if (value is double)
            {
                dbParameter.NpgsqlDbType = NpgsqlDbType.Double;
            }
            dbParameter.ParameterName = dataName;
            dbParameter.Value = value ?? DBNull.Value;
            command.Parameters.Add(dbParameter);
        }
    }
}
