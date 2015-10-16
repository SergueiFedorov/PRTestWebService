namespace TestAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDefaultCreateDate : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Journeys", "CreatedDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Journeys", "CreatedDate", c => c.DateTime(nullable: false));
        }
    }
}
