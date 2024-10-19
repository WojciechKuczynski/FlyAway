namespace FlyAway.Domain.Discounts;

public class AfricaTripDiscount : Discount
{
    public AfricaTripDiscount()
    {
        Type = DiscountType.Fixed;
        Amount = 20;
    }
    
    public override (bool, decimal) Apply(FlightRecord flightRecord ,decimal price, Customer customer)
    {
        if (flightRecord.FlightId.Suffix == "AFR") 
        {
            return base.Apply(flightRecord, price, customer);
        }
        
        return (false, price);
    }
}