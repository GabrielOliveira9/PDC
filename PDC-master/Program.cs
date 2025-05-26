using CS_CriptoCoinRest.Model;
using CS_CriptoCoinRest.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<CriptoCoinContext>(opt =>
    opt.UseInMemoryDatabase("CriptoCoinBD"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Adiciona gRPC
builder.Services.AddGrpc();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
// Mapeia endpoint gRPC
app.MapGrpcService<WalletGrpcService>();

app.Run();