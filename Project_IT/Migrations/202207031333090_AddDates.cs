namespace Project_IT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservations", "DataNaPristignuvanje", c => c.DateTime(nullable: false));
            AddColumn("dbo.Reservations", "DataNaZaminuvanje", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reservations", "DataNaZaminuvanje");
            DropColumn("dbo.Reservations", "DataNaPristignuvanje");
        }
    }
}
