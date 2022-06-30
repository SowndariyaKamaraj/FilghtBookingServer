using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using BookingApi.Model;
using BookingApi.Services;
using RabbitMQ.Client;
using System.Text;

namespace BookingApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        public Ticket tcktbk = new Ticket();
        private readonly IConfiguration _config;
        private readonly ITicketRepository _ticketrepository;

        public BookingController(IConfiguration configuration, ITicketRepository ticketrepo)
        {
            _config = configuration;
            _ticketrepository = ticketrepo;

        }



        [HttpPost("Ticket")]
        public ActionResult TicketBook(Ticket ts)
        {

            tcktbk.InventoryId = ts.InventoryId;
            tcktbk.EmailId = ts.EmailId;
            tcktbk.ModifiedId = ts.ModifiedId;
            tcktbk.Rountrip = ts.Rountrip;
            tcktbk.OptMeal = ts.OptMeal;
            tcktbk.NoofSeatedBooked = ts.NoofSeatedBooked;
            tcktbk.SeatNumbers = ts.SeatNumbers;
            tcktbk.Type = ts.Type;
            tcktbk.IsActive = true;
            tcktbk.DiscountCode = ts.DiscountCode;
            tcktbk.cost = ts.cost;
            tcktbk.ModifiedDate = DateTime.Now;
            tcktbk.ModifiedId = ts.ModifiedId;
     
            var discount = _ticketrepository.DiscountCalculation(ts.DiscountCode);

            if (discount != null)
            {
                var percentage = Convert.ToInt32(discount.DiscountPercent);
                tcktbk.cost -= ((tcktbk.cost * percentage) / 100);
            }
            
            var receipt = _ticketrepository.BookTickets(tcktbk);
            //string data = (ts.InventoryId , ts.NoofSeatedBooked).ToString();
            string data = ts.InventoryId.ToString() +',' + ts.NoofSeatedBooked.ToString() + ',' + ts.Type.ToString() + ',' + "Book";
            InventoryProducer( data);
            return Ok(receipt);
        }


        [HttpGet("GetDiscount")]
        public ActionResult DiscountCalculation(string discountCode)
        {

            var discount = _ticketrepository.DiscountCalculation(discountCode);

            return Ok(discount);

        }

        [HttpGet("History")]
        public ActionResult TicketHistory(string TicketPNR, string EmailId)
        {

            tcktbk.TicketPNR = TicketPNR;
            tcktbk.EmailId = EmailId;
            var History = _ticketrepository.TicketHistory(tcktbk);
            return Ok(History);
        }

        [HttpGet("SearchManageHistory")]
        public ActionResult SearchManageHistory(string TicketPNR, string EmailId)
        {

            tcktbk.TicketPNR = TicketPNR;
            tcktbk.EmailId = EmailId;
            var History = _ticketrepository.SearchManageHistory(tcktbk);
            return Ok(History);
        }



        [HttpGet("ManageBooking")]
        public ActionResult ManageBooking(int userid)
        {

            tcktbk.ModifiedId = userid;
            var History = _ticketrepository.ManageBooking(tcktbk);

            if (History != null)
            {
                return Ok(History);
            }
            else
            {
                return Ok("No Data Found");
            }
        }
       

        [HttpGet("ManageHistory")]
        public ActionResult ManageHistory(int userid)
        {

            tcktbk.ModifiedId = userid;
            var History = _ticketrepository.ManageHistory(tcktbk);

            if (History != null)
            {
                return Ok(History);
            }
            else
            {
                return Ok("No Data Found");
            }
        }


        [HttpGet("AllHistory")]
        public ActionResult AllHistory(Ticket pnt)
        {

            tcktbk.ModifiedId = pnt.ModifiedId;
            var History = _ticketrepository.AllHistory(tcktbk);
            return Ok(History);
        }

        [HttpPost("Cancel")]
        public ActionResult Bookingcancel(Ticket pnr)
        {

            var History = _ticketrepository.Bookingcancel(pnr);
       
            string data = pnr.InventoryId.ToString() + ',' + pnr.NoofSeatedBooked.ToString() + ',' + pnr.Type.ToString() + ',' + "Cancel";

            InventoryProducer(data);
            return Ok(History);
        }

        [HttpPost("DownloadTicket")]
        public ActionResult DownloadTicket(Ticket pnr)
        {

            var History = _ticketrepository.TicketPdf(pnr);
            return Ok("Ticket Downloaded Successfully");
        }


        public void InventoryProducer(string tr)
        {
            var factory = new ConnectionFactory() { Uri = new System.Uri("amqp://guest:guest@localhost:5672") };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "UpdateInventory",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            string message = tr;
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "",
                                 routingKey: "UpdateInventory",
                                 basicProperties: null,
                                 body: body);
          

        }


    }
}
