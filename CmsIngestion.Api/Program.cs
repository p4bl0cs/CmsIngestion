using CmsIngestion.Api.Middleware;
using CmsIngestion.Application.Services;
using CmsIngestion.Infrastructure;
using CmsIngestion.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 1. Add Infrastructure (Encapsulates EF Core, SQLite, and Repositories cleanly)
builder.Services.AddInfrastructure(builder.Configuration);
// 2. Register Application services
builder.Services.AddScoped<ICmsEventService, CmsEventService>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// 4. Ensure database and tables are created automatically on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseAuthorization();
app.UseBasicAuth();

app.MapControllers();

app.Run();
