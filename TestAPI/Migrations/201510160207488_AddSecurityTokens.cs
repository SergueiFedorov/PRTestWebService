namespace TestAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSecurityTokens : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SecurityTokens",
                c => new
                    {
                        SecurityTokenId = c.Int(nullable: false, identity: true),
                        Token = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.SecurityTokenId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SecurityTokens");
        }
    }
}
