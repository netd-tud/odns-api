
using System.Reflection;
using System.Threading.RateLimiting;
using Asp.Versioning;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using NLog.Web;
using ODNSAPI.Swagger;
using ODNSBusiness;
using ODNSRepository;
using ODNSRepository.Repository;
using Swashbuckle.AspNetCore.SwaggerGen;

try
{
    var builder = WebApplication.CreateBuilder(args);
    bool enableSwagger = builder.Configuration.GetValue<bool>("Settings:EnableSwagger");
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
    builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
    builder.Services.AddSwaggerGen(c =>
    {
        c.OperationFilter<SwaggerDefaultValues>();
        c.EnableAnnotations();
        c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
    });

    builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    })
        .AddApiExplorer(options =>
        {
            options.SubstituteApiVersionInUrl = true;
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

    #region RateLimiting
    builder.Services.AddRateLimiter(_ =>
    {
        _.AddFixedWindowLimiter(policyName: builder.Configuration.GetValue<string>("RateLimiting:PolicyName"), options =>
        {
            options.PermitLimit = builder.Configuration.GetValue<int>("RateLimiting:PermitLimit");
            options.Window = TimeSpan.FromSeconds(builder.Configuration.GetValue<int>("RateLimiting:WindowInSeconds"));
            options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            options.QueueLimit = builder.Configuration.GetValue<int>("RateLimiting:QueueLimit");
        });
        _.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        _.OnRejected = async (ctx, cancelationToken) =>
        {
            ctx.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            return;

        };
    }

    );

    #endregion

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (enableSwagger)
    {
        //string apiVersion = builder.Configuration.GetValue<string>("Settings:DocsVersion");
        app.UseSwagger();
        //app.UseSwaggerUI();
        app.UseSwaggerUI(c =>
        {
            var descriptions = app.DescribeApiVersions();

            // build a swagger endpoint for each discovered API version
            foreach (var description in descriptions)
            {
                var url = $"/swagger/{description.ApiVersion}/swagger.json";
                var name = description.GroupName.ToUpperInvariant();
                c.SwaggerEndpoint(url, name);
                c.RoutePrefix = builder.Configuration.GetValue<string>("Settings:DocsEndpoint");
            }
            //c.SwaggerEndpoint($"/swagger/{apiVersion}/swagger.json", $"ODNSAPI {apiVersion}");
            //c.RoutePrefix = builder.Configuration.GetValue<string>("Settings:DocsEndpoint");
        });
    }

    //app.UseHttpsRedirection();

    

    app.UseAuthorization();

    app.MapControllers();

    app.UseRouting();
    app.UseRateLimiter();

    app.Run();
}
catch(Exception ex)
{
    Console.WriteLine(ex.ToString());
}

