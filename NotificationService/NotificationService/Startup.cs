using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.PostgreSql;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Queries;
using NotificationService.Model;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RemoteProcedureInvokation.Controllers;

namespace NotificationService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHangfire(config =>
                config.UsePostgreSqlStorage("Host=pg-docker;Database=aspnethangfiredb;Username=postgres;Password=gigaming"));
            services.AddMediatR(typeof(GetNotificationsHandler).GetTypeInfo().Assembly);
            services.AddDbContext<NotificationContext>(opt
                => opt.UseNpgsql(Configuration.GetConnectionString("defaultConnection")));
            services.AddControllers();
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseHangfireServer();

            app.UseHangfireDashboard();
            RecurringJob.AddOrUpdate<RabbitListener>(x => x.Register(), Cron.Minutely);
        }
    }

    public class RabbitListener
    {
        public void Register()
        {
            var client = new HttpClient();
            var factory = new ConnectionFactory() { HostName = "some-rabbit" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("directExchange", "direct");
                channel.QueueDeclare("notification", true, false, false, null);
                channel.QueueBind("notification", "directExchange", "notificationKey");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    var content = new StringContent(message, Encoding.UTF8, "application/json");
                    if (content != null)
                    {

                        Console.WriteLine($"Processing data from queue");
                        await client.PostAsync("http://notificationservice/notification", content);
                    }
                    else
                    {
                        Console.WriteLine("No data");
                    }
                };
                channel.BasicConsume(queue: "notification",
                                     autoAck: true,
                                     consumer: consumer);

                Thread.Sleep(10000);
            }
        }
    }
}
