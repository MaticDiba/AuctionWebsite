namespace AuctionWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuctionOffer",
                c => new
                    {
                        AuctionOfferId = c.Int(nullable: false, identity: true),
                        Amount = c.Double(nullable: false),
                        Email = c.String(),
                        ImageId = c.Int(nullable: false),
                        Guid = c.String(),
                        DateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.AuctionOfferId)
                .ForeignKey("dbo.Image", t => t.ImageId, cascadeDelete: true)
                .Index(t => t.ImageId);
            
            CreateTable(
                "dbo.Image",
                c => new
                    {
                        ImageId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Path = c.String(),
                        ThumbnailPath = c.String(),
                        CreatedBy = c.String(),
                        CreatedDateTime = c.DateTime(nullable: false),
                        GalleryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ImageId)
                .ForeignKey("dbo.Gallery", t => t.GalleryId, cascadeDelete: true)
                .Index(t => t.GalleryId);
            
            CreateTable(
                "dbo.Gallery",
                c => new
                    {
                        GalleryId = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        CreatedBy = c.String(),
                        CreatedDateTime = c.DateTime(nullable: false),
                        AuctionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.GalleryId)
                .ForeignKey("dbo.Auction", t => t.GalleryId)
                .Index(t => t.GalleryId);
            
            CreateTable(
                "dbo.Auction",
                c => new
                    {
                        AuctionId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        CreatedBy = c.String(),
                        CreatedDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.AuctionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AuctionOffer", "ImageId", "dbo.Image");
            DropForeignKey("dbo.Image", "GalleryId", "dbo.Gallery");
            DropForeignKey("dbo.Gallery", "GalleryId", "dbo.Auction");
            DropIndex("dbo.Gallery", new[] { "GalleryId" });
            DropIndex("dbo.Image", new[] { "GalleryId" });
            DropIndex("dbo.AuctionOffer", new[] { "ImageId" });
            DropTable("dbo.Auction");
            DropTable("dbo.Gallery");
            DropTable("dbo.Image");
            DropTable("dbo.AuctionOffer");
        }
    }
}
