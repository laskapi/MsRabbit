using CommonsLib;
using DataApiService.Db;
using DataApiService.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ReadingDb>(opt => opt.UseSqlServer(connectionString));

var app = builder.Build();

app.MapGet("/", async (ReadingDb db) =>
{
   
    return "There are " + db.Readings.Count()+"readings in database now.";

});

app.MapPost("/", async(RabbitMessageModel message, ReadingDb db) =>
{

 
    var reading=new Reading { TimeStamp=message.Timestamp, Value=message.Value };
    Console.WriteLine(reading.ToString());
    await db.Readings.AddAsync(reading);
    await db.SaveChangesAsync();

    return Results.Created($"/readings/{reading.Id}",message);
});

app.Run();
