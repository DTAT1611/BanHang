namespace BanHang.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateava : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Ava", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Ava");
        }
    }
}
