namespace BanHang.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tb_Comments", "hide", c => c.Boolean(nullable: false));
            AddColumn("dbo.tb_Order", "ApplicationUsers_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.tb_Order", "ApplicationUsers_Id");
            AddForeignKey("dbo.tb_Order", "ApplicationUsers_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tb_Order", "ApplicationUsers_Id", "dbo.AspNetUsers");
            DropIndex("dbo.tb_Order", new[] { "ApplicationUsers_Id" });
            DropColumn("dbo.tb_Order", "ApplicationUsers_Id");
            DropColumn("dbo.tb_Comments", "hide");
        }
    }
}
