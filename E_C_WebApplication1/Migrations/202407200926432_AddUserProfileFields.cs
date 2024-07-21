namespace E_C_WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserProfileFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ApplicationUsers", "FirstName", c => c.String(maxLength: 50));
            AddColumn("dbo.ApplicationUsers", "LastName", c => c.String(maxLength: 50));
            AddColumn("dbo.ApplicationUsers", "Address", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ApplicationUsers", "Address");
            DropColumn("dbo.ApplicationUsers", "LastName");
            DropColumn("dbo.ApplicationUsers", "FirstName");
        }
    }
}
