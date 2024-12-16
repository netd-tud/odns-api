
using System.Net;
using System.Reflection;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using NLog.Web;
using ODNSBusiness;
using ODNSRepository;
using ODNSRepository.Repository;

try
{
    var builder = WebApplication.CreateBuilder(args);
    bool enableSwagger = builder.Configuration.GetValue<bool>("Settings:EnableSwagger");

    #region RateLimiting
    builder.Services.AddRateLimiter(o =>
    {
        o.AddPolicy(policyName: builder.Configuration.GetValue<string>("RateLimiting:PolicyName"), context =>
        {
            
            return RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: context.Request.Headers.TryGetValue("X-Forwarded-For", out var ip) ? ip[0] : context.Connection.RemoteIpAddress?.ToString(),
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = builder.Configuration.GetValue<int>("RateLimiting:PermitLimit"),
                    Window = TimeSpan.FromSeconds(builder.Configuration.GetValue<int>("RateLimiting:WindowInSeconds")),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = builder.Configuration.GetValue<int>("RateLimiting:QueueLimit"),
                    AutoReplenishment = true
                }
            );
        });
        /*o.AddFixedWindowLimiter(policyName: builder.Configuration.GetValue<string>("RateLimiting:PolicyName"), options =>
        {
            options.PermitLimit = builder.Configuration.GetValue<int>("RateLimiting:PermitLimit");
            options.Window = TimeSpan.FromSeconds(builder.Configuration.GetValue<int>("RateLimiting:WindowInSeconds"));
            options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            options.QueueLimit = builder.Configuration.GetValue<int>("RateLimiting:QueueLimit");
            options.AutoReplenishment = true;
        });*/
        o.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        o.OnRejected = async (ctx, cancelationToken) =>
        {
            bool hasForwarded = ctx.HttpContext.Request.Headers.TryGetValue("X-Forwarded-For", out var ip);
            //ctx.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            string? text = " for " + ( hasForwarded ? ip[0] : ctx.HttpContext.Connection.RemoteIpAddress?.ToString() );
            await ctx.HttpContext.Response.WriteAsync($"Too many requests {text}", cancelationToken);
            //return;
        };
    });

    #endregion

    // Add services to the container.

    builder.Services.AddControllers();

    if (Environment.OSVersion.Platform == PlatformID.Unix)
    {
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(builder.Configuration.GetValue<int>("Settings:PortLinux")); // to listen for incoming http connection on port 5001
                                                                                            // options.ListenAnyIP(7001, configure => configure.UseHttps()); // to listen for incoming https connection on port 7001
        });//linux
    }

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.EnableAnnotations();
        c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
    });


    #region services

    builder.Services.AddTransient<IOdnsRepositoryFactory,OdnsRepositoryFactory>();
    builder.Services.AddSingleton<IOdnsRepository,OdnsPostgresqlRepository>();
    builder.Services.AddSingleton<IBusinessOdns, BusinessOdns>();
    
    #endregion

    #region Logger
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();
    #endregion

    

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (enableSwagger)
    {
        string apiVersion = builder.Configuration.GetValue<string>("Settings:DocsVersion");
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint($"/swagger/{apiVersion}/swagger.json", $"ODNSAPI {apiVersion}");
            c.RoutePrefix = builder.Configuration.GetValue<string>("Settings:DocsEndpoint");
        });
    }

    //app.UseHttpsRedirection();

    app.UseRouting();
    app.UseRateLimiter();
    app.UseAuthorization();

    app.MapControllers();
    app.Run();
}
catch(Exception ex)
{
    Console.WriteLine(ex.ToString());
}

