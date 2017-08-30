namespace FlightAvailability.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialFlightTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Flights",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Origin = c.String(),
                        Destination = c.String(),
                        Date = c.String(),
                        Code = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Flights");
        }
    }
}
