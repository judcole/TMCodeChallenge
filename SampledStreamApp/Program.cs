try
{
    // Create and initialize the application builder
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddRazorPages();

    // Add an HTTP client to the container
    builder.Services.AddHttpClient();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        // Set the default exception handler route
        app.UseExceptionHandler("/Error");

        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    // Redirect HTTP to HTTPS
    app.UseHttpsRedirection();

    // Enable serving of static files
    app.UseStaticFiles();

    // Set up routing
    app.UseRouting();

    // Enable authorization capabilities
    app.UseAuthorization();

    // Map any Razor pages
    app.MapRazorPages();

    // Run the Web application
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine(String.Format("Unhandled Exception: {0}", ex.ToString()));
}