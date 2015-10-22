namespace PlateletActive.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeNameFieldToDisplayNameField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Client", "displayName", c => c.String());
            DropColumn("dbo.Client", "name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Client", "name", c => c.String());
            DropColumn("dbo.Client", "displayName");
        }
    }
}
