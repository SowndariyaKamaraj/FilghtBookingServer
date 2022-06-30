using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Airline.Model;
using AirlineApiService.Services;
using AirlineApiService.Model;
using AirlineAPIServices.Model;
using AirlineAPIServices.Service;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace AirlineApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AirlineController : ControllerBase
    {
        public Inventory Inventory = new Inventory();
        public AirlineLogo Airline = new AirlineLogo();
        public Discount Discount = new Discount();
        private readonly IConfiguration _config;
        private readonly IInventoryRepository _inventoryrepository;
        private readonly IAirlineRepository _airlinerepository;
        
        private readonly IServiceScopeFactory _servicefact;
        private readonly AppDbContext _appdbcontext;
        public AirlineController(IConfiguration configuration, IInventoryRepository inventoryrepo, IAirlineRepository airlinerepo, IServiceScopeFactory servicefact, AppDbContext appdbcontext)
        {
            _config = configuration;
            _inventoryrepository = inventoryrepo;
            _airlinerepository = airlinerepo;
            _servicefact = servicefact;
            _appdbcontext = appdbcontext;

        }



        [HttpPost("AddInventory"), Authorize(Roles = "Admin")]
        public ActionResult AddInventory(Inventory invt)
        {

            Inventory.AirlineId = invt.AirlineId;
            Inventory.IsActive = true;
            Inventory.InstrumentUsed = invt.InstrumentUsed;
            Inventory.FromPlace = invt.FromPlace;
            Inventory.ToPlace = invt.ToPlace;
            Inventory.StartDate = invt.StartDate;
            Inventory.EndDate = invt.EndDate;
            Inventory.TicketCost = invt.TicketCost;
            Inventory.No_Booked_BC = invt.Total_BusinessClass_Seat;
            Inventory.No_Booked_NonBC = invt.Total_NonBusinessClass_Seat;
            Inventory.Total_BusinessClass_Seat = invt.Total_BusinessClass_Seat;
            Inventory.Total_NonBusinessClass_Seat = invt.Total_NonBusinessClass_Seat;
            Inventory.ModifiedDate = DateTime.Now;
            Inventory.ModifieduserId = invt.ModifieduserId;
            _inventoryrepository.AddInventory(Inventory);
            return Ok(new { Message = "Inventory Added Successfully" });

        }



        [HttpPost("AddAirline"), Authorize(Roles = "Admin")]
        public ActionResult AddAirline(AirlineLogo airline)
        {
            Airline.AirlineName = airline.AirlineName;
            Airline.ContactAddress = airline.ContactAddress;
            Airline.ContactNumber = airline.ContactNumber;
            Airline.ModifiedDate = DateTime.Now;
            Airline.ModifiedId = airline.ModifiedId;
            Airline.UploadLogo = airline.UploadLogo;
            Airline.IsActive = true;

            _airlinerepository.AddAirline(Airline);
            return Ok(new { Message = "Airline Added Successfully" });


        }

        [HttpPost("AddDiscount"), Authorize(Roles = "Admin")]
        public ActionResult AddDiscount(Discount dis)
        {

            Discount.DiscountCode = dis.DiscountCode;
            Discount.DiscountPercent = dis.DiscountPercent;
            Discount.ModifiedDate = DateTime.Now;
            Discount.ModifiedId = dis.ModifiedId;
            Discount.IsActive = true;

            _airlinerepository.AddDiscount(Discount);
            return Ok(new { Message = "Discount Added Successfully" });
        }

        [HttpGet("ShowDiscount"), Authorize(Roles = "Admin")]
        public ActionResult ShowDiscount()
        {
            var discounts = _airlinerepository.GetDiscounts();
            return Ok(discounts);
        }

        [HttpGet("AllAirline")]
        public ActionResult AllAirline()
        {
            var airlines = _airlinerepository.GetAllAirline();
            return Ok(airlines);
        }

        [HttpGet("AllInventory")]
        public ActionResult AllInventory()
        {
            var airlines = _inventoryrepository.GetAllInventory();
            return Ok(airlines);
        }

        [HttpGet("SearchFlight")]
        public ActionResult SearchFlight(string Source, string Destination)
        {
            //Source = "Bangalore";
            //Destination = "Hydrabad";
            Inventory.FromPlace = Source;
            Inventory.ToPlace = Destination;
            var search = _inventoryrepository.SearchFlight(Inventory);
            return Ok(search);
        }


        [HttpPost("BlockAirline"), Authorize(Roles = "Admin")]
        public ActionResult BlockAirline(AirlineLogo air)
        {

            Airline.AirlineId = air.AirlineId;
            Airline.ModifiedId = air.ModifiedId;
            Airline.IsActive = air.IsActive;
            var block = _airlinerepository.BlockAirline(Airline);
            // BlockInventory(air.ModifiedId, air.IsActive, air.AirlineId);
            return Ok(block);


        }



        [HttpPost("BlockInventory"), Authorize(Roles = "Admin")]


        public ActionResult BlockInventory(int ModifieduserId, bool IsActive, int AirlineId)
        {

            Inventory.AirlineId = AirlineId;
            Inventory.ModifieduserId = ModifieduserId;
            Inventory.IsActive = IsActive;
            var block = _inventoryrepository.BlockInventory(Inventory);
            return Ok(block);


        }
        [HttpPost("UnBlockAirline"), Authorize(Roles = "Admin")]
        public ActionResult UnBlockAirline(int AirlineId, int ModifiedId)
        {
            Airline.AirlineId = AirlineId;
            Airline.ModifiedId = ModifiedId;

            var block = _airlinerepository.UnBlockAirline(Airline);
            return Ok(block);


        }



        [HttpGet("Consumer")]

        public void UpdateInventoryConsumer()
        {
            string msg = "";
            var factory = new ConnectionFactory() { Uri = new System.Uri("amqp://guest:guest@localhost:5672") };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "UpdateInventory",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    msg = message;


                };



                channel.BasicConsume(queue: "UpdateInventory",
                                                autoAck: true,
                                                consumer: consumer);
            }



            //char[] spearator = { ',' };
            if (msg != null || msg != "")
            {

                string[] strlist = msg.Split(',');

                int InventoryId = Convert.ToInt32(strlist[0]);
                int totalseat = Convert.ToInt32(strlist[1]);
                string type = strlist[2];
                string data = strlist[3];

                using (var scope = _servicefact.CreateScope())
                {

                    var update = _appdbcontext.Inventory.FirstOrDefault(x => x.InventoryId == InventoryId);
                    Inventory.InventoryId = InventoryId;
                    if (type == "Business Class" && data == "Book")
                    {
                        update.No_Booked_BC = update.Total_BusinessClass_Seat - totalseat;
                    }
                    else if (type != "Business Class" && data == "Book")
                    {
                        update.No_Booked_NonBC = update.Total_NonBusinessClass_Seat - totalseat;
                    }


                    if (type == "Business Class" && data == "Cancel")
                    {
                        update.No_Booked_BC = update.No_Booked_BC + totalseat;
                    }
                    else if (type != "Business Class" && data == "Cancel")
                    {
                        update.No_Booked_NonBC = update.No_Booked_BC + totalseat;
                    }


                    _appdbcontext.SaveChanges();

                }


            }



        }
    }



}

