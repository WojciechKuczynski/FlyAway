namespace FlyAway.Domain;

public class Discount
{
    public Guid Id { get; set; }
    public short Amount { get; set; }
    public DiscountType Type { get; set; }
    
    public virtual (bool, decimal) Apply(FlightRecord flightRecord, decimal price, Customer customer)
    {
        var discounted = Type switch
        {
            DiscountType.Percentage => AppliedDiscount(price, price * Amount / 100),
            DiscountType.Fixed => AppliedDiscount(price,Amount),
            _ => (false, price)
        };
        
        return discounted;
    }
    
    private (bool,decimal) AppliedDiscount(decimal price, decimal discount)
    {
        var newPrice =  price - discount;
        return (newPrice >= 20, newPrice < 20 ? price : newPrice);
    }
}

public enum DiscountType
{
    Percentage,
    Fixed
}