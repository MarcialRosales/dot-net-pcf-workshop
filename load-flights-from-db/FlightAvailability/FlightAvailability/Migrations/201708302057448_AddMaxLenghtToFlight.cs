namespace FlightAvailability.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMaxLenghtToFlight : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Flights", "Origin", c => c.String(maxLength: 10));
            AlterColumn("dbo.Flights", "Destination", c => c.String(maxLength: 10));
            AlterColumn("dbo.Flights", "Date", c => c.String(maxLength: 250));
            AlterColumn("dbo.Flights", "Code", c => c.String(maxLength: 25));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Flights", "Code", c => c.String());
            AlterColumn("dbo.Flights", "Date", c => c.String());
            AlterColumn("dbo.Flights", "Destination", c => c.String());
            AlterColumn("dbo.Flights", "Origin", c => c.String());
        }
    }
}
