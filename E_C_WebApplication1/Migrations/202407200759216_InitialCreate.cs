namespace E_C_WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        RoleId = c.Int(nullable: false, identity: true),
                        RoleName = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.RoleId);
            
            CreateTable(
                "dbo.ApplicationUsers",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        Username = c.String(nullable: false, maxLength: 100),
                        Password = c.String(nullable: false, maxLength: 100),
                        Email = c.String(nullable: false, maxLength: 100),
                        RoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Roles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.RoleId);

            // Seed initial roles
            Sql("INSERT INTO Roles (RoleName) VALUES ('Admin')");
            Sql("INSERT INTO Roles (RoleName) VALUES ('Seller')");
            Sql("INSERT INTO Roles (RoleName) VALUES ('Buyer')");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ApplicationUsers", "RoleId", "dbo.Roles");
            DropIndex("dbo.ApplicationUsers", new[] { "RoleId" });
            DropTable("dbo.ApplicationUsers");
            DropTable("dbo.Roles");
        }
    }
}
