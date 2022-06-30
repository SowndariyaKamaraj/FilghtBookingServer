using AirlineApiService.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AirlineAPIServices.Service
{
    public class Consumer : BackgroundService
    {
        private IModel _channel;
        private IConnection _connection;
        private AppDbContext _appDbContext;

        public Consumer(IServiceProvider serviceProvider)
        {
            var factory = new ConnectionFactory() { Uri = new System.Uri("amqp://guest:guest@localhost:5672") };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "UpdateInventory",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

            var scope = serviceProvider.CreateScope();
            _appDbContext = scope.ServiceProvider.GetService<AppDbContext>();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var msg = message;

                if (msg != null || msg != "")
                {

                    string[] strlist = msg.Split(',');

                    int InventoryId = Convert.ToInt32(strlist[0]);
                    int totalseat = Convert.ToInt32(strlist[1]);
                    string type = strlist[2];
                    string data = strlist[3];

                    var update = _appDbContext.Inventory.FirstOrDefault(x => x.InventoryId == InventoryId);
                    //Inventory.InventoryId = InventoryId;
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


                    _appDbContext.SaveChanges();
                }
            };

            _channel.BasicConsume(queue: "UpdateInventory",
                                            autoAck: true,
                                            consumer: consumer);

            return Task.CompletedTask;
        }

    }
}
