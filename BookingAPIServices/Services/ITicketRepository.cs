using BookingApi.Model;
using BookingAPIServices.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApi.Services
{
    public interface ITicketRepository
    {

        Ticket BookTickets(Ticket tickets);

        List<Ticket> TicketHistory(Ticket pnr);
        
        Ticket Bookingcancel(Ticket pnr);

        Ticket DownloadTicket(Ticket download);

        List<Ticket> AllHistory(Ticket pnr);

        List<Ticket> ManageBooking(Ticket pnr);

        List<Ticket> ManageHistory(Ticket pnr);


        List<Ticket> SearchManageHistory(Ticket pnr);
        Discount DiscountCalculation(string discountCode);

        string TicketPdf(Ticket ts);
    }
}
