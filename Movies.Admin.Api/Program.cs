using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Movies.Admin.Api.Data;
using Movies.Admin.Api.Events.RecieveEvents;
using Movies.Admin.Api.Models;
using Movies.Admin.Api.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddTransient<IRecieveEvents, RecieveEvents>();
builder.Services.AddTransient<IMovieServices, MovieServices>();

string connectionString = builder.Configuration.GetConnectionString("Connection");
string secretStr = builder.Configuration["Application:Secret"];
byte[] secret = Encoding.UTF8.GetBytes(secretStr);


builder.Services.AddDbContext<AccountsDbContext>(options =>
                                                options.UseSqlServer(connectionString));

builder.Services.AddIdentity<UserModel, IdentityRole>()
                    .AddRoleManager<RoleManager<IdentityRole>>()
                    .AddEntityFrameworkStores<AccountsDbContext>().AddDefaultTokenProviders();


var rbHost = builder.Configuration["RabbitMq:RabbitMqHost"];
var rbUserName = builder.Configuration["RabbitMq:RabbitMqUserName"];
var rbPass = builder.Configuration["RabbitMq:RabbitMqPassword"];
var rbPort = builder.Configuration["RabbitMq:RabbitMqPort"];

builder.Services.AddCap(x =>
{

    x.UseRabbitMQ(options =>
    {
        options.HostName = rbHost;
        options.UserName = rbUserName;
        options.Password = rbPass;
        options.Port = Int32.Parse(rbPort);
    });


    x.UseEntityFramework<AccountsDbContext>();

    x.UseSqlServer(connectionString);


});

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

app.Run();