
// Create and initialize the application builder
var builder = WebApplication.CreateBuilder(args);

// Create startup instance and register services
var startup = new SampledStreamCollector.Startup(builder.Configuration);
startup.RegisterServices(builder.Services);

//builder.Services.AddControllers();

// Add and configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Build the Web API application
var app = builder.Build();

// Configure the HTTP request pipeline and other settings
startup.Configure(app, builder.Environment);

// Run the Web API application
app.Run();
