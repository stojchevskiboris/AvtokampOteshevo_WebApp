namespace Project_IT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddFeedItemsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FeedItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 200),
                        Subtitle = c.String(maxLength: 500),
                        Description = c.String(),
                        ImageUrl = c.String(),
                        Category = c.Int(nullable: false),
                        LabelBadge = c.String(),
                        OfferText = c.String(),
                        ButtonText = c.String(),
                        ActionUrl = c.String(),
                        ValidTo = c.DateTime(),
                        IsFeatured = c.Boolean(nullable: false),
                        IsPublished = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);

        }

        public override void Down()
        {
            DropTable("dbo.FeedItems");
        }
    }
}
