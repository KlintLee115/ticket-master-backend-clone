using dotenv.net;
using events_tickets_management_backend.Data;
using events_tickets_management_backend.Services;
using Microsoft.EntityFrameworkCore;

DotEnv.Load();
string? dbConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(options =>
options.UseNpgsql(dbConnectionString)
);

builder.Services.AddScoped<EventService>();
builder.Services.AddScoped<InitService>();
builder.Services.AddScoped<TicketService>();

var app = builder.Build();
app.MapControllers();
app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();