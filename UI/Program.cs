using FlazorTemplate;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var startup = new Startup(builder.Environment, builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

app.Run();
