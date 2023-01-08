namespace Project_IT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addVremeRezervacija : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservations", "VremeRezervacija", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reservations", "VremeRezervacija");
        }
    }
}
