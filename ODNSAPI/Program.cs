
using System.Net;
using System.Reflection;
using System.Threading.RateLimiting;
using Asp.Versioning;
using HealthChecks.UI.Client;
using HealthCheckUtils;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NLog.Web;
using ODNSAPI.Swagger;
using ODNSBusiness;
using ODNSRepository;
using ODNSRepository.Repository;
using Swashbuckle.AspNetCore.SwaggerGen;
using Entities.ODNS.Request;
using Microsoft.Extensions.FileProviders;
using OpenTelemetry.Metrics;
using Metrics;

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
    builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
    builder.Services.AddSwaggerGen(c =>
    {
        c.OperationFilter<SwaggerDefaultValues>();
        c.EnableAnnotations();
        c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
        c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(DnsEntryFilter).Assembly.GetName().Name}.xml"));
    });

    builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1,0);
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

    #region healthcheck
    builder.Services.AddHealthChecks()
        .AddNpgSql(
            builder.Configuration.GetSection("Database:ConnectionString").Value, 
            healthQuery: "select 1", 
            name: "PostgresSql", 
            failureStatus: HealthStatus.Unhealthy, 
            tags: new[] { "Feedback", "Database" },
            timeout:TimeSpan.FromSeconds(Int32.Parse(builder.Configuration.GetSection("HealthCheck:DatabaseTimeoutInSeconds").Value))
        )
        .AddCheck("API", () => HealthCheckResult.Healthy("API is running"), tags: new[] { "ready" })
        .AddCheck<MemoryHealthChecker>("Feedback Service Memory Check", failureStatus: HealthStatus.Unhealthy, tags: new[] { "Feedback Service" });

    //builder.Services.AddHealthChecksUI(opt =>
    //{
    //    opt.SetEvaluationTimeInSeconds(Int32.Parse(builder.Configuration.GetSection("HealthCheck:EvaluationTimeInSeconds").Value));//time in seconds between check    
    //    opt.MaximumHistoryEntriesPerEndpoint(Int32.Parse(builder.Configuration.GetSection("HealthCheck:MaximumHistoryEntriesPerEndpoint").Value)); //maximum history of checks    
    //    opt.SetApiMaxActiveRequests(Int32.Parse(builder.Configuration.GetSection("HealthCheck:ApiMaxActiveRequests").Value)); //api requests concurrency
    //    opt.AddHealthCheckEndpoint("ODNS API", "/api/health"); //map health check api
    //})
    //    .AddInMemoryStorage();
    #endregion

    #region Metrics

    builder.Services.AddOpenTelemetry()
        .WithMetrics(metrics =>
        {
            metrics.AddAspNetCoreInstrumentation();
            metrics.AddHttpClientInstrumentation();
            metrics.AddRuntimeInstrumentation();
            metrics.AddMeter(builder.Configuration.GetValue<string>("Metrics:MeterName"));
            metrics.AddOtlpExporter(options =>
            {
                options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                options.Endpoint = new Uri(builder.Configuration.GetValue<string>("Metrics:EndpointExporter"));
                
            });
        });

    builder.Services.AddSingleton<IMetricsManager,MetricsManager>();
    #endregion

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (enableSwagger)
    {
        //app.UseStaticFiles(new StaticFileOptions()
        //{
        //    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Content")),
        //    RequestPath = "/Content"
        //});

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
            //c.InjectStylesheet("/Content/swagger-custom.css");
            c.HeadContent = builder.Configuration.GetValue<string>("Settings:DocsSwaggerOptions:SwaggerHeaderCss");
            //c.SwaggerEndpoint($"/swagger/{apiVersion}/swagger.json", $"ODNSAPI {apiVersion}");
            //c.RoutePrefix = builder.Configuration.GetValue<string>("Settings:DocsEndpoint");
        });
    }

    //app.UseHttpsRedirection();

    app.UseRouting();
    app.UseRateLimiter();
    app.UseAuthorization();

    app.MapControllers();
    app.MapHealthChecks("/api/health",new HealthCheckOptions()
    {
        Predicate= _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
    //app.UseHealthChecksUI(config => {
    //    config.UIPath = "/api/health-ui";
    //});
    app.Run();
}
catch(Exception ex)
{
    Console.WriteLine(ex.ToString());
}

