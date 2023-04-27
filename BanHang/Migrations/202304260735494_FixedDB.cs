namespace BanHang.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixedDB : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tb_Category", "Link", c => c.String());
            AlterColumn("dbo.tb_ProductCategories", "Icon", c => c.String());
            AlterColumn("dbo.tb_ProductCategories", "SeoTitle", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.tb_ProductCategories", "SeoTitle", c => c.String(maxLength: 250));
            AlterColumn("dbo.tb_ProductCategories", "Icon", c => c.String(maxLength: 250));
            DropColumn("dbo.tb_Category", "Link");
        }
    }
}
