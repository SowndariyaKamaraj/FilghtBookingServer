
using Airline.Model;
using AirlineApiService.Model;
using AirlineApiService.Services;
using AirlineAPIServices.Model;
using AirlineAPIServices.Service;
using System;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace AirlineApiService
{
    public class SqlRepository : IInventoryRepository, IAirlineRepository, IDiscountRepository
    {
        private readonly AppDbContext _appdbcontext;

        public SqlRepository(AppDbContext appdb)
        {
            _appdbcontext = appdb;
        }
        public string AddInventory(Inventory inv)
        {
            try
            {
                _appdbcontext.Inventory.Add(inv);
                _appdbcontext.SaveChanges();
                return "Inventory Added Successfully";
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

     

        public IList<AirlineLogo> GetAllAirline()
        {
            return _appdbcontext.Airline.ToList();
        }

        public IList<Inventory> GetAllInventory()
        {
            var inventoryItems = (from invent in _appdbcontext.Inventory
                                  //where invent.IsActive == true
                                  select new Inventory
                                  {
                                      InventoryId = invent.InventoryId,
                                      AirlineId = invent.AirlineLogo.AirlineId,
                                      No_Booked_BC = invent.No_Booked_BC,
                                      No_Booked_NonBC = invent.No_Booked_NonBC,
                                      Total_BusinessClass_Seat = invent.Total_BusinessClass_Seat,
                                      Total_NonBusinessClass_Seat = invent.Total_NonBusinessClass_Seat,
                                      FromPlace = invent.FromPlace,
                                      ToPlace = invent.ToPlace,
                                      StartDate = invent.StartDate,
                                      EndDate = invent.EndDate,
                                      InstrumentUsed = invent.InstrumentUsed,
                                      ModifiedDate = invent.ModifiedDate,
                                      ModifieduserId = invent.ModifieduserId,
                                      TicketCost = invent.TicketCost,
                                      AirlineLogo = invent.AirlineLogo,
                                      IsActive = invent.IsActive
                                  }).ToList();

            return inventoryItems;
        }

        public IList<Inventory> SearchFlight(Inventory invt)
        {
            var inventoryItems = (from invent in _appdbcontext.Inventory
                                  where invent.FromPlace == invt.FromPlace && invent.ToPlace == invt.ToPlace
                                  select new Inventory
                                  {
                                      InventoryId = invent.InventoryId,
                                      AirlineId = invent.AirlineLogo.AirlineId,
                                      No_Booked_BC = invent.No_Booked_BC,
                                      No_Booked_NonBC = invent.No_Booked_NonBC,
                                      Total_BusinessClass_Seat = invent.Total_BusinessClass_Seat,
                                      Total_NonBusinessClass_Seat = invent.Total_NonBusinessClass_Seat,
                                      FromPlace = invent.FromPlace,
                                      ToPlace = invent.ToPlace,
                                      StartDate = invent.StartDate,
                                      EndDate = invent.EndDate,
                                      InstrumentUsed = invent.InstrumentUsed,
                                      ModifiedDate = invent.ModifiedDate,
                                      ModifieduserId = invent.ModifieduserId,
                                      TicketCost = invent.TicketCost,
                                      AirlineLogo = invent.AirlineLogo,
                                      IsActive = invent.IsActive
                                  }).ToList();

            return inventoryItems;
        }

        public AirlineLogo BlockAirline(AirlineLogo air)
        {

            try
            {
                var block = _appdbcontext.Airline.FirstOrDefault(x => x.AirlineId == air.AirlineId);

                block.ModifiedDate = DateTime.Now;
                block.ModifiedId = air.ModifiedId;
                block.IsActive = air.IsActive;

                _appdbcontext.Inventory.Where(item => item.AirlineId == air.AirlineId).ToList().ForEach(items =>
                {
                    items.IsActive = air.IsActive;
                });

                _appdbcontext.SaveChanges();

                return block;

            }
            catch (Exception ex)
            {
                throw ex;
            }




        }


        public Inventory BlockInventory(Inventory invt)
        {

            try
            {
                var block = _appdbcontext.Inventory.FirstOrDefault(x => x.AirlineId == invt.AirlineId);
                block.AirlineId = invt.AirlineId;
                block.ModifiedDate = DateTime.Now;
                block.ModifieduserId = invt.ModifieduserId;
                block.IsActive = invt.IsActive;

                _appdbcontext.SaveChanges();
                return block;

            }
            catch (Exception ex)
            {
                throw ex;
            }




        }
        public AirlineLogo UnBlockAirline(AirlineLogo air)
        {


            var unblock = _appdbcontext.Airline.FirstOrDefault(x => x.AirlineId == air.AirlineId);
            unblock.IsActive = true;
            unblock.ModifiedDate = DateTime.Now;
            unblock.ModifiedId = air.ModifiedId;
            //logic for minus in inentory
            _appdbcontext.SaveChanges();


            return unblock;

        }

        public string AddAirline(AirlineLogo air)
        {
            try
            {
                _appdbcontext.Airline.Add(air);
                _appdbcontext.SaveChanges();
                return "Airline Added Successfully";
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }



        public string AddDiscount(Discount dis)
        {
            try
            {
                _appdbcontext.Discount.Add(dis);
                _appdbcontext.SaveChanges();
                return "Discount Added Successfully";
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

     
       
        public List<Discount> GetDiscounts()
        {
            return _appdbcontext.Discount.ToList();
        }
    }
}



