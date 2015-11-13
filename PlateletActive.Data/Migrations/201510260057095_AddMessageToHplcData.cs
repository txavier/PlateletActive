namespace PlateletActive.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMessageToHplcData : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HplcData", "message", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.HplcData", "message");
        }
    }
}
