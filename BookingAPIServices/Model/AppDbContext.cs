using BookingAPIServices.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApi.Model
{
    public class AppDBContext : DbContext

    {
        public AppDBContext(DbContextOptions<AppDBContext> option) : base(option)
        {

        }

        protected override void OnModelCreating(ModelBuilder mbuild)
        {
            mbuild.Entity<Ticket>().
                Property(x => x.TicketPNR).HasComputedColumnSql
                ("N'PNRNUM'+ right('00000'+ cast(TicketId as varchar(5) ),5)");
        }
        public DbSet<Ticket> TicketBooking { get; set; }

        public DbSet<Ticket> TicketHistory { get; set; }

        public DbSet<Ticket> Bookingcancel { get; set; }

        public DbSet<Ticket> ManageBooking { get; set; }

        public DbSet<Ticket> DownloadTicket { get; set; }

        public DbSet<Discount> Discount { get; set; }

       

    }
}
