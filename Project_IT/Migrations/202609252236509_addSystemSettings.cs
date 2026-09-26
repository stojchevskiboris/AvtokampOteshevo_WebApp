namespace Project_IT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addSystemSettings : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SystemSettings",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 100),
                        Value = c.String(nullable: false),
                        Description = c.String(maxLength: 255),
                        LastModified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Key);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SystemSettings");
        }
    }
}
