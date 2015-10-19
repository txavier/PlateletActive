namespace PlateletActive.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeFermNumberAndAddUserToDatabaseModel2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HplcData", "SampleNumber", c => c.String(unicode: false));
            AddColumn("dbo.HplcData", "User", c => c.String());
            DropColumn("dbo.HplcData", "FermNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.HplcData", "FermNumber", c => c.String(unicode: false));
            DropColumn("dbo.HplcData", "User");
            DropColumn("dbo.HplcData", "SampleNumber");
        }
    }
}
