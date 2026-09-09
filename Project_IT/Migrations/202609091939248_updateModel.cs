namespace Project_IT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservations", "Days", c => c.Int(nullable: false));
            AddColumn("dbo.Reservations", "Guests", c => c.Int(nullable: false));
            AddColumn("dbo.Reservations", "CheckInDate", c => c.DateTime());
            AddColumn("dbo.Reservations", "CheckOutDate", c => c.DateTime());
            AddColumn("dbo.Reservations", "Price", c => c.Int(nullable: false));
            AddColumn("dbo.Reservations", "AccommodationType", c => c.String());
            DropColumn("dbo.Reservations", "Denovi");
            DropColumn("dbo.Reservations", "Lica");
            DropColumn("dbo.Reservations", "DataNaPristignuvanje");
            DropColumn("dbo.Reservations", "DataNaZaminuvanje");
            DropColumn("dbo.Reservations", "VremeRezervacija");
            DropColumn("dbo.Reservations", "Cena");
            DropColumn("dbo.Reservations", "Smestuvanje");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Reservations", "Smestuvanje", c => c.String());
            AddColumn("dbo.Reservations", "Cena", c => c.Int(nullable: false));
            AddColumn("dbo.Reservations", "VremeRezervacija", c => c.DateTime(nullable: false));
            AddColumn("dbo.Reservations", "DataNaZaminuvanje", c => c.DateTime(nullable: false));
            AddColumn("dbo.Reservations", "DataNaPristignuvanje", c => c.DateTime(nullable: false));
            AddColumn("dbo.Reservations", "Lica", c => c.Int(nullable: false));
            AddColumn("dbo.Reservations", "Denovi", c => c.Int(nullable: false));
            DropColumn("dbo.Reservations", "AccommodationType");
            DropColumn("dbo.Reservations", "Price");
            DropColumn("dbo.Reservations", "CheckOutDate");
            DropColumn("dbo.Reservations", "CheckInDate");
            DropColumn("dbo.Reservations", "Guests");
            DropColumn("dbo.Reservations", "Days");
        }
    }
}
