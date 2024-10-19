using FlyAway.Domain.ValueObjects;

namespace FlyAway.Domain;

public class FlightRecord
{
    public FlightId FlightId { get; set; }
    public Flight Flight { get; set; }
    
    public DateTime DepartureTime { get; set; }
    public DateTime DesiredArrivalTime => DepartureTime.Add(Flight.FlightDefinition.PlannedDuration);

    public List<CustomerFlight> CustomerFlights { get; set; } = [];
    
    public List<CustomerFlight> BookFlights(short amount,Customer customer, IEnumerable<Discount> discounts, bool preserveDiscounts = true)
    {
        var price = Flight.Price;
        var appliedDiscounts = new List<Discount>();
        var bookedFlights = new List<CustomerFlight>();
        foreach(var discount in discounts)
        {
            var (applied, discountedPrice) = discount.Apply(this, price, customer);
            if (applied)
            {
                appliedDiscounts.Add(discount);
            }
            
            price = discountedPrice;
        }
        
        for(var i = 0; i < amount; i++)
        {
            var customerFlight = new CustomerFlight()
            {
                AppliedDiscounts = preserveDiscounts ? appliedDiscounts : [],
                FinalPrice = price            
            };
            
            CustomerFlights.Add(customerFlight);
            bookedFlights.Add(customerFlight);
        }
        
        return bookedFlights;
    }
}