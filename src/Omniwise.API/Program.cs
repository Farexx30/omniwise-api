using Microsoft.AspNetCore.Identity;
using Omniwise.API.Extensions;
using Omniwise.API.Handlers;
using Omniwise.Application.Extensions;
using Omniwise.Domain.Entities;
using Omniwise.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPresentation(builder.Host);
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddApplication();


var app = builder.Build();

await app.Services.InitializeDatabaseAsync();

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
