using SampledStreamCollector;
using SampledStreamCommon;

namespace SampledStreamCollector
{
    /// <summary>
    /// Class for startup configuration of the application
    /// </summary>
    public class Startup
    {
        // Application configuration
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Construct the startup instance
        /// </summary>
        /// <param name="configuration">Current application configuration</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Register services with the application
        /// </summary>
        /// <param name="services">Current services</param>
        public void RegisterServices(IServiceCollection services)
        {
            // Add service controllers to the container
            services.AddControllers();

            services
                .AddHostedService<TweetCollector>()
                .AddSingleton<IBackgroundQueue<Tweet>, BackgroundQueue<Tweet>>();
        }

        /// <summary>
        /// Configure the HTTP request pipeline and other settings
        /// </summary>
        /// <param name="app">Current Web application</param>
        /// <param name="env">Current environment settings</param>
        public void Configure(WebApplication app, IWebHostEnvironment env)
        {
            // Configure the HTTP request pipeline
            if (env.IsDevelopment())
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
        }
    }
}