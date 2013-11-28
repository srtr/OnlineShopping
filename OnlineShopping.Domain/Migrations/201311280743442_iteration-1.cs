namespace OnlineShopping.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class iteration1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        categoryID = c.Int(nullable: false, identity: true),
                        categoryName = c.String(),
                    })
                .PrimaryKey(t => t.categoryID);
            
            CreateTable(
                "dbo.Manufacturers",
                c => new
                    {
                        manufacturerID = c.Int(nullable: false, identity: true),
                        manufacturerName = c.String(),
                    })
                .PrimaryKey(t => t.manufacturerID);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        productID = c.Int(nullable: false, identity: true),
                        productName = c.String(),
                        barcode = c.String(),
                        categoryID = c.Int(nullable: false),
                        manufacturerID = c.Int(nullable: false),
                        costPrice = c.Single(nullable: false),
                        maxPrice = c.Single(nullable: false),
                        currentStock = c.Int(nullable: false),
                        minimumStock = c.Int(nullable: false),
                        bundleUnit = c.Int(nullable: false),
                        discountPercentage = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.productID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Products");
            DropTable("dbo.Manufacturers");
            DropTable("dbo.Categories");
        }
    }
}
