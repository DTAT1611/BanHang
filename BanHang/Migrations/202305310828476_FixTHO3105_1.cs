namespace BanHang.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixTHO3105_1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.tb_post", "Posts_Id", "dbo.tb_post");
            DropIndex("dbo.tb_post", new[] { "Posts_Id" });
            DropColumn("dbo.tb_post", "Posts_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tb_post", "Posts_Id", c => c.Int());
            CreateIndex("dbo.tb_post", "Posts_Id");
            AddForeignKey("dbo.tb_post", "Posts_Id", "dbo.tb_post", "Id");
        }
    }
}
