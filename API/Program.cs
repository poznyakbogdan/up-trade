using API.Models;
using API.Validation;
using FluentValidation;
using Implementations;
using Interfaces;
using Nethereum.Web3;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IWeb3>(new Web3(builder.Configuration.GetValue<string>("NodeUrl")));
builder.Services.AddTransient<IBalanceService, BalanceService>();
builder.Services.AddScoped<IValidator<PostBalances>, PostBalancesValidator>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateBootstrapLogger();

builder.Host.UseSerilog();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();