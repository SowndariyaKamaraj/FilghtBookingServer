using AirlineApiService.Model;
using AirlineAPIServices.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineApiService.Services
{
    public interface IAirlineRepository
    {
        String AddAirline(AirlineLogo invdetails);
        IList<AirlineLogo> GetAllAirline();

        AirlineLogo BlockAirline(AirlineLogo air);

        AirlineLogo UnBlockAirline(AirlineLogo air);

        String AddDiscount(Discount dis);


        List<Discount> GetDiscounts();


    }
}
