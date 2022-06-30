
using BookingApi.Model;
using BookingApi.Services;
using BookingAPIServices.Model;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApi.Services
{
    public class SQLRepository : ITicketRepository
    {
        private readonly AppDBContext _appdbcontext;

        public SQLRepository(AppDBContext appdb)
        {
            _appdbcontext = appdb;
        }
        public Ticket BookTickets(Ticket tckt)
        {
            try
            {

                _appdbcontext.TicketBooking.Add(tckt);
                _appdbcontext.SaveChanges();
                //var ticketpnr = TicketHistory(tckt);
                return tckt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public Discount DiscountCalculation(string discountCode)
        {
            var discount = _appdbcontext.Discount.FirstOrDefault(x => x.DiscountCode == discountCode);

            return discount;

        }


        public List<Ticket> ManageBooking(Ticket pnt)
        {
            var history = (from th in _appdbcontext.ManageBooking
                           where th.ModifiedId == pnt.ModifiedId && th.IsActive == true
                           select new Ticket
                           {
                               InventoryId = th.InventoryId,

                               //AirlineName= th.AirlineName,
                               EmailId = th.EmailId,
                               TicketPNR = th.TicketPNR,
                               FromDate = th.FromDate,
                               ToDate = th.ToDate,
                               Type = th.Type,
                               OptMeal = th.OptMeal,
                               cost = th.cost,
                               DiscountCode = th.DiscountCode,
                               IsActive = th.IsActive,
                               NoofSeatedBooked = th.NoofSeatedBooked,

                               Inventory = th.Inventory


                           }
                           ).ToList();

            return history;

        }

        public List<Ticket> ManageHistory(Ticket pnt)
        {
            var history = (from th in _appdbcontext.ManageBooking
                           where th.ModifiedId == pnt.ModifiedId
                           //where th.ModifiedId == pnt.ModifiedId || ((th.TicketPNR== pnt.TicketPNR || th.EmailId==pnt.EmailId ) )
                           select new Ticket
                           {
                               InventoryId = th.InventoryId,

                               //AirlineName= th.AirlineName,
                               EmailId = th.EmailId,
                               TicketPNR = th.TicketPNR,
                               FromDate = th.FromDate,
                               ToDate = th.ToDate,
                               Type = th.Type,
                               OptMeal = th.OptMeal,
                               cost = th.cost,
                               //DiscountCode= th.Inventory.AirlineLogo.AirlineName,
                               IsActive = th.IsActive,
                               Inventory = th.Inventory

                           }
                           ).ToList();

            return history;

        }
        public List<Ticket> TicketHistory(Ticket ts)
        {
            var history = _appdbcontext.TicketHistory.Where(x => x.EmailId == ts.EmailId || x.TicketPNR == ts.TicketPNR && x.IsActive == true).ToList();

            return history;

        }

        public List<Ticket> SearchManageHistory(Ticket ts)
        {
            //var history = _appdbcontext.TicketHistory.Where(x => x.EmailId == ts.EmailId || x.TicketPNR == ts.TicketPNR && x.IsActive == true).ToList();

            var history = (from th in _appdbcontext.ManageBooking

                           where th.TicketPNR == ts.TicketPNR || th.EmailId == ts.EmailId
                           select new Ticket
                           {
                               InventoryId = th.InventoryId,

                               //AirlineName= th.AirlineName,
                               EmailId = th.EmailId,
                               TicketPNR = th.TicketPNR,
                               FromDate = th.FromDate,
                               ToDate = th.ToDate,
                               Type = th.Type,
                               OptMeal = th.OptMeal,
                               cost = th.cost,
                               //DiscountCode= th.Inventory.AirlineLogo.AirlineName,
                               IsActive = th.IsActive,
                               Inventory = th.Inventory

                           }
                         ).ToList();
            return history;

        }



        public List<Ticket> AllHistory(Ticket ts)
        {
            var history = _appdbcontext.TicketHistory.Where(x => x.ModifiedId == ts.ModifiedId).ToList();

            return history;

        }



        public Ticket Bookingcancel(Ticket pnr)
        {
            var cancel = _appdbcontext.Bookingcancel.FirstOrDefault(x => x.TicketPNR == pnr.TicketPNR && x.EmailId == pnr.EmailId);
            cancel.IsActive = false;
            _appdbcontext.SaveChanges();
            return cancel;

        }



        public Ticket DownloadTicket(Ticket ts)
        {

            var download = (from th in _appdbcontext.ManageBooking

                            where th.TicketPNR == ts.TicketPNR || th.EmailId == ts.EmailId
                            select new Ticket
                            {
                                InventoryId = th.InventoryId,
                                EmailId = th.EmailId,
                                TicketPNR = th.TicketPNR,
                                FromDate = th.FromDate,
                                ToDate = th.ToDate,
                                Type = th.Type,
                                OptMeal = th.OptMeal,
                                cost = th.cost,
                                IsActive = th.IsActive,
                                Inventory = th.Inventory

                            }
                         ).FirstOrDefault();
            return download;




        }


        public string TicketPdf(Ticket ts)
        {
            string result;
            try
            {
                var res = (from th in _appdbcontext.ManageBooking

                           where th.TicketPNR == ts.TicketPNR || th.EmailId == ts.EmailId
                           select new Ticket
                           {
                               InventoryId = th.InventoryId,
                               EmailId = th.EmailId,
                               TicketPNR = th.TicketPNR,
                               FromDate = th.FromDate,
                               ToDate = th.ToDate,
                               Type = th.Type,
                               OptMeal = th.OptMeal,
                               cost = th.cost,
                               IsActive = th.IsActive,
                               Inventory = th.Inventory

                           }
                         ).ToList();


                string g = res.FirstOrDefault().Inventory.StartDate.ToShortDateString();
                DataTable ds = new DataTable();
                ds.Columns.Add("EmailId");
                ds.Columns.Add("Class Type");
                ds.Columns.Add("Meal");
                ds.Columns.Add("Cost");
                //ds.Columns.Add("Discount");
                ds.Columns.Add("From Date");
                ds.Columns.Add("To Date");

                foreach (var ps in res)
                {
                    ds.Rows.Add(new object[] { ps.EmailId, ps.Type, ps.OptMeal, ps.Inventory.TicketCost, ps.Inventory.StartDate.ToShortDateString(), ps.Inventory.EndDate.ToShortDateString() });
                }

                string filename = "C://Users//cogdotnet907//Downloads//" + res.FirstOrDefault().TicketPNR + "_" + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.Hour +
                    "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + ".pdf";

                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                iTextSharp.text.Rectangle rec = new iTextSharp.text.Rectangle(PageSize.A4);
                rec.BackgroundColor = new BaseColor(System.Drawing.Color.Olive);
                Document doc = new Document(rec);
                doc.SetPageSize(iTextSharp.text.PageSize.A4);
                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(filename, FileMode.Create));
                doc.Open();

                //Creating paragraph for header  
                BaseFont bfntHead = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                iTextSharp.text.Font fntHead = new iTextSharp.text.Font(bfntHead, 16, 1, iTextSharp.text.BaseColor.BLACK);
                Paragraph prgHeading = new Paragraph();
                prgHeading.Alignment = Element.ALIGN_LEFT;
                prgHeading.Add(new Chunk("Flight Ticket".ToUpper(), fntHead));
                doc.Add(prgHeading);

                Paragraph wlc = new Paragraph();
                //  BaseFont btnAuthor = BaseFont.CreateFont(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                iTextSharp.text.Font fnttAuthor = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 12, 2, iTextSharp.text.BaseColor.BLACK);
                wlc.Alignment = Element.ALIGN_LEFT;
                //prgGeneratedBY.Add(new Chunk("Report Generated by : ASPArticles", fntAuthor));  
                //prgGeneratedBY.Add(new Chunk("\nGenerated Date : " + DateTime.Now.ToShortDateString(), fntAuthor));
                wlc.Add(new Chunk("Hello,", fnttAuthor));

                doc.Add(new Chunk("\n", fntHead));

                //Adding paragraph for report generated by  
                Paragraph prgGeneratedBY = new Paragraph();
                //  BaseFont btnAuthor = BaseFont.CreateFont(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                iTextSharp.text.Font fntAuthor = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 12, 2, iTextSharp.text.BaseColor.BLACK);
                prgGeneratedBY.Alignment = Element.ALIGN_LEFT;
                //prgGeneratedBY.Add(new Chunk("Report Generated by : ASPArticles", fntAuthor));  
                //prgGeneratedBY.Add(new Chunk("\nGenerated Date : " + DateTime.Now.ToShortDateString(), fntAuthor));
                prgGeneratedBY.Add(new Chunk("Your ticket has confirmed, please find the below  PNR number and passenger details for reference.", fntAuthor));


                //Adding a line  
                Paragraph p = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, iTextSharp.text.BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                doc.Add(p);
                doc.Add(wlc);
                doc.Add(prgGeneratedBY);


                //Adding line break  
                doc.Add(new Chunk("\n", fntHead));


                Paragraph pnr = new Paragraph();
                BaseFont fntpr = BaseFont.CreateFont(BaseFont.TIMES_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                iTextSharp.text.Font ftpnr = new iTextSharp.text.Font(fntpr, 13, 2, iTextSharp.text.BaseColor.BLACK);
                pnr.Alignment = Element.ALIGN_LEFT;
                pnr.Add(new Chunk("PNR: " + res.FirstOrDefault().TicketPNR, ftpnr));
                doc.Add(pnr);

                doc.Add(new Chunk("\n", fntHead));


                Paragraph doj = new Paragraph();
                BaseFont fndoj = BaseFont.CreateFont(BaseFont.TIMES_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                iTextSharp.text.Font ftdoj = new iTextSharp.text.Font(fndoj, 13, 2, iTextSharp.text.BaseColor.BLACK);
                doj.Alignment = Element.ALIGN_LEFT;
                doj.Add(new Chunk("DateofJourney: " + res.FirstOrDefault().Inventory.StartDate.ToShortDateString(), ftdoj));
                doc.Add(doj);

                doc.Add(new Chunk("\n", fntHead));

                //Adding  PdfPTable  
                PdfPTable table = new PdfPTable(ds.Columns.Count);
                table.WidthPercentage = 100;
                for (int i = 0; i < ds.Columns.Count; i++)
                {
                    string cellText = ds.Columns[i].ColumnName;
                    PdfPCell cell = new PdfPCell();
                    cell.Phrase = new Phrase(cellText, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 10, 1));
                    // cell.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#C8C8C8"));
                    //cell.Phrase = new Phrase(cellText, new Font(Font.FontFamily.TIMES_ROMAN, 10, 1, new BaseColor(grdStudent.HeaderStyle.ForeColor)));  
                    //cell.BackgroundColor = new BaseColor(grdStudent.HeaderStyle.BackColor);  
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.PaddingBottom = 5;
                    table.AddCell(cell);
                }

                //writing table Data  
                for (int i = 0; i < ds.Rows.Count; i++)
                {
                    for (int j = 0; j < ds.Columns.Count; j++)
                    {
                        table.AddCell(ds.Rows[i][j].ToString());
                    }
                }

                doc.Add(table);

                doc.Add(new Chunk("\n", fntHead));

                Paragraph imp = new Paragraph();
                //BaseFont fnimp = BaseFont.CreateFont(BaseFont.TIMES_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                iTextSharp.text.Font ftimp = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 11, 2, iTextSharp.text.BaseColor.RED);
                imp.Alignment = Element.ALIGN_LEFT;
                imp.Add(new Chunk("Important: It is compulsory for all passengers to wear face mask at the airport and throughout the journey inside the aircraft.", ftimp));
                doc.Add(imp);

                doc.Close();
                //File.Open(filename,FileMode.Open);
                //FileStream SourceStream = File.Open(filename, FileMode.Open);
                // return result;


                System.Diagnostics.Process.Start(filename);
                //string FilePath = Server.MapPath("javascript1-sample.pdf");
                //WebClient User = new WebClient();
                //Byte[] FileBuffer = User.DownloadData(FilePath);
                //if (FileBuffer != null)
                //{
                //    Response.ContentType = "application/pdf";
                //    Response.AddHeader("content-length", FileBuffer.Length.ToString());
                //    Response.BinaryWrite(FileBuffer);
                //}







                result = "Booked Sucessfully";
            }

            catch (Exception e)
            {
                result = "Booking Failed";
            }
            return result;
        }













    }
}
