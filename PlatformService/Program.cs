using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var env = builder.Environment;
var configuration = builder.Configuration;
// Add services to the container.

// if (env.IsProduction())
// {
Console.WriteLine("--> UseSqlServer Db");
services.AddDbContext<AppDbContext>(opt =>
  opt.UseSqlServer(configuration.GetConnectionString("Database")));
// }
// else
// {
//     Console.WriteLine("--> UseInMemoryDatabase Db");
//     services.AddDbContext<AppDbContext>(opt =>
//         opt.UseInMemoryDatabase("PlatformService"));
// }

services.AddScoped<IPlatformRepo, PlatformRepo>();
services.AddGrpc();
services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();

services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
services.AddSingleton<IMessageBusClient, MessageBusClient>();

services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<GrpcPlatformService>();
app.MapGet("/protos/platforms.proto", async context =>
{
    await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
});

PrepDb.PrepPopulation(builder);
Console.WriteLine(configuration["CommandService"]);

app.Run();