using Airline.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineApiService.Services
{
    public interface IInventoryRepository
    {
        String AddInventory(Inventory invdetails);

        IList<Inventory> GetAllInventory();

        IList<Inventory> SearchFlight(Inventory air);

        Inventory BlockInventory(Inventory air);

        
    }
}
