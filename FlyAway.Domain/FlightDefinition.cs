using FlyAway.Domain.ValueObjects;

namespace FlyAway.Domain;

public class FlightDefinition
{
    public Airport Departure { get; set; }
    public Airport Arrival { get; set; }
    public TimeSpan PlannedDuration { get; set; }
    public decimal BasePrice { get; set; }
    public string Currency { get; set; }
    public List<Flight> Flights { get; set; } = [];
    
    private uint _flightNumber = 0;
    private uint NextFlightNumber => Interlocked.Increment(ref _flightNumber);
    
    public FlightId GenerateFlightId() => $"{Departure}" +
                                           $"{NextFlightNumber.ToString().PadLeft(5, '0')}" +
                                           $"{Arrival}";
    
    public Flight AddSingleFlight(DateTime departureTime, decimal? price = null)
    {
        var flight = new Flight()
        {
            FlightDefinition = this,
            DepartureTime = departureTime,
            Price = price ?? BasePrice,
            Currency = Currency 
        };
        Flights.Add(flight);
        flight.AddSingleFlight(departureTime);
        return flight;
    }

    public List<Flight> DefineFlightSchedule(IEnumerable<(DayOfWeek, DateTime, decimal?)> schedule)
    {
        var addedFlights = new List<Flight>();
        foreach(var t in schedule)
        {
            var flight = new Flight()
            {
                FlightDefinition = this,
                DayOfWeek = t.Item1,
                DepartureTime = t.Item2,
                Price = t.Item3 ?? BasePrice,
                Currency = Currency
            };
            Flights.Add(flight);
            addedFlights.Add(flight);
        }

        return addedFlights;
    }
    
    public List<FlightRecord> GenerateFlights(DateTime from, DateTime to)
    {
        var addedFlights = new List<FlightRecord>();
        foreach(var flight in Flights)
        {
            addedFlights.AddRange(flight.GenerateFlights(from, to));
        }

        return addedFlights;
    }
    
    // public Flight AddSingleFlight()
    public FlightRecord FindFlightRecordById(FlightId flightId)
    {
        return Flights.Select(x => x.FindFlightRecordById(flightId)).OfType<FlightRecord>().FirstOrDefault();
    }
}