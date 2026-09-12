namespace Project_IT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class feedGallery : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FeedItems", "GalleryPath", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.FeedItems", "GalleryPath");
        }
    }
}
