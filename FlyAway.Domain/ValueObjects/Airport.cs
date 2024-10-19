namespace FlyAway.Domain.ValueObjects;

public class Airport
{
    private string Value { get; }

    public Airport(string value)
    {
        if (value.Length != 3)
        {
            throw new ArgumentException("Airport code must be 3 characters long.");
        }
        
        Value = value;
    }
    
    public static implicit operator string(Airport airport) => $"{airport.Value}";
    public static implicit operator Airport(string airport) => new(airport);

    public override string ToString() => Value;
}