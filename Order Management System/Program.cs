
using Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.Data;
using Persistence.Repositories;
using Services;
using ServicesAbstractions;
using Stripe;
using InvoiceService = Services.InvoiceService;
using ProductService = Services.ProductService;

namespace Order_Management_System
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<OrderManagementDbContext>(options =>
            {

                //options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]);
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

            });
            
            builder.Services.AddScoped<IDbInitializer,DbInitializer> (); // Allow
            builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();

             builder.Services.AddScoped<IOrderService, OrderService>();
             builder.Services.AddScoped<IInvoiceService, InvoiceService>();
             builder.Services.AddScoped<IProductService, ProductService>();
             builder.Services.AddScoped<IAuthService, AuthService>();




            var app = builder.Build();

            // Initialize the database
            using var scope = app.Services.CreateScope();
            var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>(); // ASK CLR Object From DI
            await dbInitializer.InitializeAsync(); // Call the method to initialize the database






            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
