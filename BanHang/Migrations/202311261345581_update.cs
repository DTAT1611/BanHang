namespace BanHang.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Sales",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        codesale = c.String(),
                        productid = c.Int(nullable: false),
                        percent = c.Int(nullable: false),
                        userid = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifierDate = c.DateTime(nullable: false),
                        ModifierBy = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Sales");
        }
    }
}
