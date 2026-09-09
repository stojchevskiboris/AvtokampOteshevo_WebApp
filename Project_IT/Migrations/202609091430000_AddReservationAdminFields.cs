namespace Project_IT.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddReservationAdminFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservations", "FullName", c => c.String());
            AddColumn("dbo.Reservations", "Smestuvanje", c => c.String());
            AddColumn("dbo.Reservations", "Status", c => c.String(nullable: false, defaultValue: "New"));
            AddColumn("dbo.Reservations", "UserPlatform", c => c.String());
            AddColumn("dbo.Reservations", "UserOs", c => c.String());
            AddColumn("dbo.Reservations", "UserAgent", c => c.String());
            AddColumn("dbo.Reservations", "IPAddress", c => c.String());
            AddColumn("dbo.Reservations", "UserIp", c => c.String());
            AddColumn("dbo.Reservations", "UserBrowser", c => c.String());
            AddColumn("dbo.Reservations", "UserVersion", c => c.String());
            AddColumn("dbo.Reservations", "UserCountry", c => c.String());
            AddColumn("dbo.Reservations", "UserReferrer", c => c.String());
            AddColumn("dbo.Reservations", "CreatedOn", c => c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()"));
            AddColumn("dbo.Reservations", "ModifiedOn", c => c.DateTime());
        }

        public override void Down()
        {
            DropColumn("dbo.Reservations", "ModifiedOn");
            DropColumn("dbo.Reservations", "CreatedOn");
            DropColumn("dbo.Reservations", "UserReferrer");
            DropColumn("dbo.Reservations", "UserCountry");
            DropColumn("dbo.Reservations", "UserVersion");
            DropColumn("dbo.Reservations", "UserBrowser");
            DropColumn("dbo.Reservations", "UserIp");
            DropColumn("dbo.Reservations", "IPAddress");
            DropColumn("dbo.Reservations", "UserAgent");
            DropColumn("dbo.Reservations", "UserOs");
            DropColumn("dbo.Reservations", "UserPlatform");
            DropColumn("dbo.Reservations", "Status");
            DropColumn("dbo.Reservations", "Smestuvanje");
            DropColumn("dbo.Reservations", "FullName");
        }
    }
}
