using System.Text.RegularExpressions;

namespace FlyAway.Domain.ValueObjects;

public class FlightId
{
    private string Value { get; }
    
    public string IATACode { get; set; }
    public string FlightNumber { get; set; }
    public string Suffix { get; set; }

    public FlightId(string id)
    {
        if (!Regex.Match(id.Trim(), @"[a-zA-Z]{3}[0-9]{5}[a-zA-Z]{3}").Success)
        {
          throw new ArgumentException("FlightDefinition ID must be in correct format.");  
        }
        
        Value = id;

        IATACode = id[..3];
        FlightNumber = id[3..^3];
        Suffix = id[^3..];
    }
    
    public static implicit operator string(FlightId id) => $"{id.Value}";
    public static implicit operator FlightId(string id) => new(id);
    
    public override string ToString() => Value;
}