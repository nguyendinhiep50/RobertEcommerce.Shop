using Identity.API;
using Identity.API.apis;
using Identity.API.Data;
using Identity.API.Identity;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddApplicationServices();
builder.Services.AddProblemDetails();

//builder.AddNpgsqlDbContext<ApplicationDbContext>("identitydb");
//builder.EnrichNpgsqlDbContext<ApplicationDbContext>(settings => settings.DisableHealthChecks = true);
//builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

builder.Services.AddMigration<ApplicationDbContext, UsersSeed>();

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

var withApiVersioning = builder.Services.AddApiVersioning();

builder.AddDefaultOpenApi(withApiVersioning);

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseStatusCodePages();

app.MapIdentityApi();

app.UseDefaultOpenApi();
app.Run();
