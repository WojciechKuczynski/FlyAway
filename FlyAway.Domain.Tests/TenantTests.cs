using FlyAway.Domain.Discounts;

namespace FlyAway.Domain.Tests;

public class TenantTests
{
    [Fact]
    public void Tenant_AddSingleFlight_Success()
    {
        Tenant tenant = new Tenant_A();

        var definition = tenant.AddDefinition("ABC", "XYZ", TimeSpan.FromHours(3));

        definition.AddSingleFlight(DateTime.Now);
        
        Assert.Single(definition.Flights);
        Assert.Single(definition.Flights[0].FlightRecords);
    }
    
    [Fact]
    public void Tenant_FlightIsEveryDay_ShouldGenerate14FlightsIn2Weeks()
    {
        Tenant tenant = new Tenant_A();

        var definition = tenant.AddDefinition("ABC", "XYZ", TimeSpan.FromHours(3));

        // generate tuple inline
        var flightDefinitions = definition.DefineFlightSchedule(new (DayOfWeek, DateTime, decimal?)[]
        {
            (DayOfWeek.Monday, DateTime.Now, null),
            (DayOfWeek.Tuesday, DateTime.Now, null),
            (DayOfWeek.Wednesday, DateTime.Now, null),
            (DayOfWeek.Thursday, DateTime.Now, null),
            (DayOfWeek.Friday, DateTime.Now, null),
            (DayOfWeek.Saturday, DateTime.Now, null),
            (DayOfWeek.Sunday, DateTime.Now, null)
        });
        definition.GenerateFlights(DateTime.Now.AddDays(1), DateTime.Now.AddDays(14));
        
        Assert.Equal(7, definition.Flights.Count);
        Assert.Equal(14, definition.Flights.Sum(x => x.FlightRecords.Count));
    }

    [Fact]
    public void Tenant_GivenFlightId_ShouldReturnExactFlightRecord()
    {
        Tenant tenant = new Tenant_A();

        var definition = tenant.AddDefinition("ABC", "XYZ", TimeSpan.FromHours(3));

        definition.AddSingleFlight(DateTime.Now);
        definition.AddSingleFlight(DateTime.Now.AddDays(1));
        definition.AddSingleFlight(DateTime.Now.AddDays(2));
        definition.AddSingleFlight(DateTime.Now.AddDays(3));

        var flightRecord = definition.FindFlightRecordById("ABC00002XYZ");
        Assert.NotNull(flightRecord);
    }

    [Theory]
    [InlineData(TenantType.A, true)]
    [InlineData(TenantType.B, false)]
    public void Tenant_BookFlight_ShouldApplyDiscounts(TenantType type, bool preservedDiscounts)
    {
        Tenant tenant = type switch
        {
            TenantType.A => new Tenant_A(),
            TenantType.B => new Tenant_B()
        };

        var definition = tenant.AddDefinition("ABC", "XYZ", TimeSpan.FromHours(3));
        definition.AddSingleFlight(DateTime.Now, 40);
        var flightRecord = definition.FindFlightRecordById("ABC00001XYZ");
        var booked = tenant.BookFlight(flightRecord, new List<Discount> { new BirthdayDiscount() }, new Customer() {BirthDate = DateTime.Now}, 1);
        
        Assert.Equal(30, booked[0].FinalPrice);
        Assert.Equal(preservedDiscounts, booked[0].AppliedDiscounts.Any());
    }
    
    [Fact]
    public void Tenant_BookFlight_ShouldNotApplyDiscounts()
    {
        Tenant tenant = new Tenant_A();

        var definition = tenant.AddDefinition("ABC", "XYZ", TimeSpan.FromHours(3));
        definition.AddSingleFlight(DateTime.Now, 25);
        var flightRecord = definition.FindFlightRecordById("ABC00001XYZ");
        var booked = tenant.BookFlight(flightRecord, new List<Discount> { new BirthdayDiscount() }, new Customer() {BirthDate = DateTime.Now}, 1);
        
        Assert.Equal(25, booked[0].FinalPrice);
    }
}