
// Create and initialize the application builder
var builder = WebApplication.CreateBuilder(args);

// Add service controllers to the container.
builder.Services.AddControllers();

// Add and configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Build the Web API application
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    // It is a development build so enable Swagger
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirect HTTP to HTTPS
app.UseHttpsRedirection();

// Enable authorization capabilities
app.UseAuthorization();

// Add endpoints for controller actions
app.MapControllers();

// Run the Web API application
app.Run();
