using System.ComponentModel.DataAnnotations.Schema;

namespace FlyAway.Domain;

[Table("Tenants")]
public class Tenant_B : Tenant
{
    public override TenantType Type => TenantType.B;
    
    public override List<CustomerFlight> BookFlight(FlightRecord flightRecord, List<Discount> discounts, Customer customer, short amount)
    {
        return flightRecord.BookFlights(amount, customer, discounts, false);
    }
}