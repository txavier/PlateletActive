namespace PlateletActive.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeSampleNumberToSampleLocation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HplcData", "SampleLocation", c => c.String(unicode: false));
            DropColumn("dbo.HplcData", "SampleNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.HplcData", "SampleNumber", c => c.String(unicode: false));
            DropColumn("dbo.HplcData", "SampleLocation");
        }
    }
}
