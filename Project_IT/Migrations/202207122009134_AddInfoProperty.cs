namespace Project_IT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddInfoProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservations", "Info", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reservations", "Info");
        }
    }
}
