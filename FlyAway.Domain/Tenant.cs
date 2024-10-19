using FlyAway.Domain.ValueObjects;

namespace FlyAway.Domain;

public abstract class Tenant
{
    public string Name { get; set; }
    public abstract TenantType Type { get; }

    public List<FlightDefinition> FlightDefinitions { get; set; } = [];

    public FlightRecord FindFlightRecordById(FlightId flightId)
        => FlightDefinitions.Select(x => x.FindFlightRecordById(flightId)).OfType<FlightRecord>().FirstOrDefault();
    
    public FlightDefinition AddDefinition(string from, string to, TimeSpan duration)
    {
        if (FlightDefinitions.Any(x => x.Departure == from && x.Arrival == to))
        {
            throw new InvalidOperationException("Flight definition already exists");
        }
        
        var definition = new FlightDefinition()
        {
            Departure = from,
            Arrival = to,
            PlannedDuration = duration
        };
        FlightDefinitions.Add(definition);
        return definition;
    }

    public abstract List<CustomerFlight> BookFlight(FlightRecord flightRecord, List<Discount> discounts, Customer customer,
        short amount);
}

public enum TenantType
{
    A,
    B
}