using Implementations;
using Interfaces;
using Nethereum.Web3;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IWeb3>(new Web3(builder.Configuration.GetValue<string>("NodeUrl")));
builder.Services.AddTransient<IBalanceService, BalanceService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();