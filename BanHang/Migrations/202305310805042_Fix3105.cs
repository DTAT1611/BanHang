﻿namespace BanHang.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Fix3105 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.tb_Order", "ShipperID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tb_Order", "ShipperID", c => c.String());
        }
    }
}
