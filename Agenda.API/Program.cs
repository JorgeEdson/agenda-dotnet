using Agenda.Infraestrutura.Contextos;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configurando o acesso ao Banco pelo Contexto na API. E a conex�o padr�o vem do "appsettings.json"
// ServiceLifeTime � o "tempo de vida" do objeto, que ser� por transa��o (Transient), se quisesse que ele ficasse no ar todo o tempo seria (Singleton)
builder.Services.AddDbContext<Contexto>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("conexaoPadrao")),
               ServiceLifetime.Transient, ServiceLifetime.Transient

                                        );
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Configurando Mediator
builder.Services.AddMediatR(x =>
{
    x.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
    x.Lifetime = ServiceLifetime.Transient;
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
