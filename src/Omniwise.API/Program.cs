using Omniwise.API.Extensions;
using Omniwise.API.Handlers;
using Omniwise.Domain.Entities;
using Omniwise.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPresentation();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.MapGroup("api/identity")
    .WithTags("Identity") 
    .MapOmniwiseIdentityApi<User>();

app.UseAuthorization();

app.MapControllers();

app.Run();
