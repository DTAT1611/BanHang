namespace BanHang.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AccountFixed : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Role", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Role");
        }
    }
}
