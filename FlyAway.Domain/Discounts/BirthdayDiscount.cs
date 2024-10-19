namespace FlyAway.Domain.Discounts;

public class BirthdayDiscount : Discount
{
    public BirthdayDiscount()
    {
        Type = DiscountType.Fixed;
        Amount = 10;
    }
    
    public override (bool, decimal) Apply(FlightRecord flightRecord, decimal price, Customer customer)
    {
        if (customer.BirthDate.Month == DateTime.Now.Month && customer.BirthDate.Day == DateTime.Now.Day)
        {
            return base.Apply(flightRecord, price, customer);
        }
        
        return (false, price);
    }
}