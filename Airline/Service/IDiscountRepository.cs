using AirlineAPIServices.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineAPIServices.Service
{
    public interface IDiscountRepository
    {
        String AddDiscount(Discount dis);


        List<Discount> GetDiscounts();
    }
}
