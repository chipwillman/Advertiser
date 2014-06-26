namespace RandomNights.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DbMigration100 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Aspect",
                c => new
                    {
                        AspectPk = c.Guid(nullable: false),
                        AspectName = c.String(),
                        PreferenceCategoryPk = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.AspectPk)
                .ForeignKey("dbo.PreferenceCategory", t => t.PreferenceCategoryPk, cascadeDelete: true)
                .Index(t => t.PreferenceCategoryPk);
            
            CreateTable(
                "dbo.PreferenceCategory",
                c => new
                    {
                        PreferenceCategoryPk = c.Guid(nullable: false),
                        CategoryName = c.String(),
                    })
                .PrimaryKey(t => t.PreferenceCategoryPk);
            
            CreateTable(
                "dbo.Crafter",
                c => new
                    {
                        CrafterPk = c.Guid(nullable: false),
                        Name = c.String(maxLength: 100),
                        Address = c.String(maxLength: 300),
                        Phone = c.String(maxLength: 50),
                        Fax = c.String(maxLength: 50),
                        Email = c.String(maxLength: 128),
                        Url = c.String(),
                        Description = c.String(),
                        Location = c.Geography(),
                    })
                .PrimaryKey(t => t.CrafterPk);
            
            CreateTable(
                "dbo.Drinks",
                c => new
                    {
                        DrinkPk = c.Guid(nullable: false),
                        CrafterFk = c.Guid(nullable: false),
                        Type = c.String(maxLength: 10),
                        Name = c.String(maxLength: 256),
                        MainImageUrl = c.String(maxLength: 256),
                        Vegan = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.DrinkPk)
                .ForeignKey("dbo.Crafter", t => t.CrafterFk, cascadeDelete: true)
                .Index(t => t.CrafterFk);
            
            CreateTable(
                "dbo.Establishments",
                c => new
                    {
                        EstablishmentPk = c.Guid(nullable: false),
                        Name = c.String(maxLength: 100),
                        Description = c.String(),
                        AddressFk = c.Guid(nullable: false),
                        MainImageUrl = c.String(),
                        Location = c.Geography(),
                        GoogleId = c.String(maxLength: 100),
                        GoogleReference = c.String(maxLength: 500),
                        OpenHours = c.String(),
                        Rating = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.EstablishmentPk)
                .ForeignKey("dbo.Address", t => t.AddressFk, cascadeDelete: true)
                .Index(t => t.AddressFk);
            
            CreateTable(
                "dbo.Address",
                c => new
                    {
                        AddressPk = c.Guid(nullable: false),
                        Description = c.String(),
                        AddressType = c.String(),
                        Suburb = c.String(),
                        Postcode = c.String(),
                        SubNumber = c.String(),
                        Number = c.String(),
                        Street = c.String(),
                        StreetType = c.String(),
                        State = c.String(),
                        Country = c.String(),
                        Location = c.Geography(),
                    })
                .PrimaryKey(t => t.AddressPk);
            
            CreateTable(
                "dbo.AspectEstablishmentLinks",
                c => new
                    {
                        AspectEstablishmentLinkPk = c.Guid(nullable: false),
                        AspectFk = c.Guid(nullable: false),
                        EstablishmentFk = c.Guid(nullable: false),
                        Rating = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Establishment_EstablishmentPk = c.Guid(),
                    })
                .PrimaryKey(t => t.AspectEstablishmentLinkPk)
                .ForeignKey("dbo.Aspect", t => t.AspectFk, cascadeDelete: true)
                .ForeignKey("dbo.Establishments", t => t.Establishment_EstablishmentPk)
                .Index(t => t.AspectFk)
                .Index(t => t.Establishment_EstablishmentPk);
            
            CreateTable(
                "dbo.EstablishmentImages",
                c => new
                    {
                        EstablishmentImagePk = c.Guid(nullable: false),
                        EstablishmentFk = c.Guid(nullable: false),
                        ImageUrl = c.String(),
                        Aspect_AspectPk = c.Guid(),
                        Establishment_EstablishmentPk = c.Guid(),
                    })
                .PrimaryKey(t => t.EstablishmentImagePk)
                .ForeignKey("dbo.Aspect", t => t.Aspect_AspectPk)
                .ForeignKey("dbo.Establishments", t => t.Establishment_EstablishmentPk)
                .Index(t => t.Aspect_AspectPk)
                .Index(t => t.Establishment_EstablishmentPk);
            
            CreateTable(
                "dbo.UserPreference",
                c => new
                    {
                        UserPreferencePk = c.Guid(nullable: false),
                        UserId = c.Int(nullable: false),
                        AspectFk = c.Guid(nullable: false),
                        Required = c.Boolean(nullable: false),
                        Excluded = c.Boolean(nullable: false),
                        Factor = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.UserPreferencePk)
                .ForeignKey("dbo.Aspect", t => t.AspectFk, cascadeDelete: true)
                .Index(t => t.AspectFk);
            
            CreateTable(
                "dbo.UserProfile",
                c => new
                    {
                        UserPk = c.Guid(nullable: false),
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(maxLength: 100),
                        ScreenName = c.String(maxLength: 100),
                        AgeRange = c.String(maxLength: 20),
                    })
                .PrimaryKey(t => t.UserPk);
            
            CreateTable(
                "dbo.Reviews",
                c => new
                    {
                        ReviewPk = c.Guid(nullable: false),
                        UserFk = c.Guid(nullable: false),
                        ParentFk = c.Guid(nullable: false),
                        ParentTable = c.String(nullable: false, maxLength: 32),
                        ReviewText = c.String(nullable: false, storeType: "ntext"),
                        Rating = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedDate = c.DateTime(nullable: false),
                        ReviewDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ReviewPk)
                .ForeignKey("dbo.UserProfile", t => t.UserFk, cascadeDelete: true)
                .Index(t => t.UserFk);
            
            CreateTable(
                "dbo.ReviewAspectLinks",
                c => new
                    {
                        ReviewAspectPk = c.Guid(nullable: false),
                        ReviewFk = c.Guid(nullable: false),
                        AspectFk = c.Guid(nullable: false),
                        Rating = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Review_ReviewPk = c.Guid(),
                    })
                .PrimaryKey(t => t.ReviewAspectPk)
                .ForeignKey("dbo.Aspect", t => t.AspectFk, cascadeDelete: true)
                .ForeignKey("dbo.Reviews", t => t.Review_ReviewPk)
                .Index(t => t.AspectFk)
                .Index(t => t.Review_ReviewPk);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reviews", "UserFk", "dbo.UserProfile");
            DropForeignKey("dbo.ReviewAspectLinks", "Review_ReviewPk", "dbo.Reviews");
            DropForeignKey("dbo.ReviewAspectLinks", "AspectFk", "dbo.Aspect");
            DropForeignKey("dbo.UserPreference", "AspectFk", "dbo.Aspect");
            DropForeignKey("dbo.EstablishmentImages", "Establishment_EstablishmentPk", "dbo.Establishments");
            DropForeignKey("dbo.EstablishmentImages", "Aspect_AspectPk", "dbo.Aspect");
            DropForeignKey("dbo.AspectEstablishmentLinks", "Establishment_EstablishmentPk", "dbo.Establishments");
            DropForeignKey("dbo.AspectEstablishmentLinks", "AspectFk", "dbo.Aspect");
            DropForeignKey("dbo.Establishments", "AddressFk", "dbo.Address");
            DropForeignKey("dbo.Drinks", "CrafterFk", "dbo.Crafter");
            DropForeignKey("dbo.Aspect", "PreferenceCategoryPk", "dbo.PreferenceCategory");
            DropIndex("dbo.Reviews", new[] { "UserFk" });
            DropIndex("dbo.ReviewAspectLinks", new[] { "Review_ReviewPk" });
            DropIndex("dbo.ReviewAspectLinks", new[] { "AspectFk" });
            DropIndex("dbo.UserPreference", new[] { "AspectFk" });
            DropIndex("dbo.EstablishmentImages", new[] { "Establishment_EstablishmentPk" });
            DropIndex("dbo.EstablishmentImages", new[] { "Aspect_AspectPk" });
            DropIndex("dbo.AspectEstablishmentLinks", new[] { "Establishment_EstablishmentPk" });
            DropIndex("dbo.AspectEstablishmentLinks", new[] { "AspectFk" });
            DropIndex("dbo.Establishments", new[] { "AddressFk" });
            DropIndex("dbo.Drinks", new[] { "CrafterFk" });
            DropIndex("dbo.Aspect", new[] { "PreferenceCategoryPk" });
            DropTable("dbo.ReviewAspectLinks");
            DropTable("dbo.Reviews");
            DropTable("dbo.UserProfile");
            DropTable("dbo.UserPreference");
            DropTable("dbo.EstablishmentImages");
            DropTable("dbo.AspectEstablishmentLinks");
            DropTable("dbo.Address");
            DropTable("dbo.Establishments");
            DropTable("dbo.Drinks");
            DropTable("dbo.Crafter");
            DropTable("dbo.PreferenceCategory");
            DropTable("dbo.Aspect");
        }
    }
}
