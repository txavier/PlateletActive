namespace PlateletActive.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNameFieldToClientModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Client", "name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Client", "name");
        }
    }
}
