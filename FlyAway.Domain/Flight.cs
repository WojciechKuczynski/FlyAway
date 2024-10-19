using System.ComponentModel.DataAnnotations.Schema;
using FlyAway.Domain.ValueObjects;

namespace FlyAway.Domain;

public class Flight
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    
    public DayOfWeek? DayOfWeek { get; set; }
    public DateTime DepartureTime { get; set; }
    
    public decimal Price { get; set; }
    public string Currency { get; set; }

    public bool SingleEntry => DayOfWeek == null;

    public virtual List<FlightRecord> FlightRecords { get; set; } = [];
    public virtual FlightDefinition FlightDefinition { get; set; }

    public List<FlightRecord> GenerateFlights(DateTime from, DateTime to)
    {
        var addedFlights = new List<FlightRecord>();
        // get all dates from 'from' to 'to' that are on the same day of the week as this flight
        var dates = Enumerable.Range(0, 1 + to.Subtract(from).Days)
            .Select(offset => from.AddDays(offset))
            .Where(date => date.DayOfWeek == DayOfWeek)
            .ToList();

        var flightRecords = dates.Select(date => new FlightRecord()
        {
            DepartureTime = date.Date.AddHours(DepartureTime.Hour).AddMinutes(DepartureTime.Minute),
            Flight = this,
            FlightId = FlightDefinition.GenerateFlightId()
        });
        foreach(var flightRecord in flightRecords)
        {
            if (FlightRecords.Any(x => x.DepartureTime == flightRecord.DepartureTime))
                continue;
            
            FlightRecords.Add(flightRecord);
            addedFlights.Add(flightRecord);
        }

        return addedFlights;
    }

    public FlightRecord AddSingleFlight(DateTime departureTime)
    {
        var departure = departureTime.Date.AddHours(departureTime.Hour).AddMinutes(departureTime.Minute);
        if (FlightRecords.Any(x => x.DepartureTime == departure))
        {
            // We assume there might be logic like this.
            throw new InvalidOperationException("Flight already exists for this time.");
        }
        
        var flightRecord = new FlightRecord()
        {
            DepartureTime = departureTime,
            Flight = this,
            FlightId = FlightDefinition.GenerateFlightId()
        };
        FlightRecords.Add(flightRecord);
        return flightRecord;
    }

    public FlightRecord FindFlightRecordById(FlightId flightId)
    {
     return FlightRecords.FirstOrDefault(x => x.FlightId.ToString() == flightId);
    }
}