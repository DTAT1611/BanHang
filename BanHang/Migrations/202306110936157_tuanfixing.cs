namespace BanHang.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tuanfixing : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.tb_post", "Posts_Id", "dbo.tb_post");
            DropIndex("dbo.tb_post", new[] { "Posts_Id" });
            DropColumn("dbo.tb_post", "Posts_Id");
            DropTable("dbo.tb_Advs");
            DropTable("dbo.tb_Contact");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.tb_Contact",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 150),
                        Website = c.String(),
                        Email = c.String(maxLength: 150),
                        Message = c.String(maxLength: 4000),
                        IsRead = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifierDate = c.DateTime(nullable: false),
                        ModifierBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.tb_Advs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 250),
                        Description = c.String(maxLength: 500),
                        Link = c.String(maxLength: 500),
                        Type = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifierDate = c.DateTime(nullable: false),
                        ModifierBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.tb_post", "Posts_Id", c => c.Int());
            CreateIndex("dbo.tb_post", "Posts_Id");
            AddForeignKey("dbo.tb_post", "Posts_Id", "dbo.tb_post", "Id");
        }
    }
}
