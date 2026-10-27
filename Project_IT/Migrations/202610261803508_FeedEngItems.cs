namespace Project_IT.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class FeedEngItems : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FeedItems", "TitleEn", c => c.String(maxLength: 200));
            AddColumn("dbo.FeedItems", "SubtitleEn", c => c.String(maxLength: 500));
            AddColumn("dbo.FeedItems", "DescriptionEn", c => c.String());
            AddColumn("dbo.FeedItems", "LabelBadgeEn", c => c.String());
            AddColumn("dbo.FeedItems", "OfferTextEn", c => c.String());
            AddColumn("dbo.FeedItems", "ButtonTextEn", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.FeedItems", "ButtonTextEn");
            DropColumn("dbo.FeedItems", "OfferTextEn");
            DropColumn("dbo.FeedItems", "LabelBadgeEn");
            DropColumn("dbo.FeedItems", "DescriptionEn");
            DropColumn("dbo.FeedItems", "SubtitleEn");
            DropColumn("dbo.FeedItems", "TitleEn");
        }
    }
}
