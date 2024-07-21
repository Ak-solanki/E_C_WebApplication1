namespace E_C_WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateProductModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductImages",
                c => new
                    {
                        ImageId = c.Int(nullable: false, identity: true),
                        ImageUrl = c.String(),
                        ProductId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ImageId)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId);
            
            AddColumn("dbo.Products", "Category", c => c.String());
            AddColumn("dbo.Products", "StockQuantity", c => c.Int(nullable: false));
            AlterColumn("dbo.Products", "Description", c => c.String(maxLength: 500));
            DropColumn("dbo.Products", "ImageUrl");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Products", "ImageUrl", c => c.String(maxLength: 100));
            DropForeignKey("dbo.ProductImages", "ProductId", "dbo.Products");
            DropIndex("dbo.ProductImages", new[] { "ProductId" });
            AlterColumn("dbo.Products", "Description", c => c.String(nullable: false, maxLength: 500));
            DropColumn("dbo.Products", "StockQuantity");
            DropColumn("dbo.Products", "Category");
            DropTable("dbo.ProductImages");
        }
    }
}
