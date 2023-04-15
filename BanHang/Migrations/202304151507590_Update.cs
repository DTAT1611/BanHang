namespace BanHang.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.tb_ProductCategories", "Icon", c => c.String(maxLength: 250));
            AlterColumn("dbo.tb_ProductCategories", "SeoTitle", c => c.String(maxLength: 250));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.tb_ProductCategories", "SeoTitle", c => c.String());
            AlterColumn("dbo.tb_ProductCategories", "Icon", c => c.String());
        }
    }
}
