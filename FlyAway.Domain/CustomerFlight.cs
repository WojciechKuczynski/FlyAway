namespace FlyAway.Domain;

public class CustomerFlight
{
    public List<Discount> AppliedDiscounts { get; set; }
    public decimal FinalPrice { get; set; }
}