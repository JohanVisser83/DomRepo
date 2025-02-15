using Circular;
using Circular.Data.Repositories.Account;
using Circular.Data.Repositories.Community;
using Circular.Data.Repositories.Email;
using Circular.Data.Repositories.Finance;
using Circular.Data.Repositories.Home;
using Circular.Data.Repositories.Message;
using Circular.Data.Repositories.Notification;
using Circular.Data.Repositories.Planners;
using Circular.Data.Repositories.Safety;
using Circular.Data.Repositories.Setting;
using Circular.Data.Repositories.Sports;
using Circular.Data.Repositories.Storefront;
using Circular.Data.Repositories.Transport;
using Circular.Data.Repositories.User;
using Circular.Filters;
using Circular.Framework.Emailer;
using Circular.Framework.Logger;
using Circular.Framework.Notifications;
using Circular.Framework.ShortMessages;
using Circular.Framework.Utility;
using Circular.Middleware;
using Circular.Services.Account;
using Circular.Services.Community;
using Circular.Services.Email;
using Circular.Services.Finance;
using Circular.Services.Master;
using Circular.Services.Message;
using Circular.Services.Notifications;
using Circular.Services.Planners;
using Circular.Services.Safety;
using Circular.Services.Setting;
using Circular.Services.Sports;
using Circular.Services.Storefront;
using Circular.Services.Transport;
using Circular.Services.User;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using NLog;
using OpenIddict.Client;
using RepoDb;
using RepoDb.Options;
using Stripe;


var logger = LogManager.GetLogger("database");

try
{
    GlobalConfigurationOptions options = new GlobalConfigurationOptions();

    var builder = WebApplication.CreateBuilder(args);
    RepoDb.GlobalConfiguration.Setup().UseSqlServer();

    // Add services to the container.
    // Add services to the container.

    var connectionString = builder.Configuration.GetConnectionString("CircularDbConnectionString");
    var OIDCUrl = builder.Configuration.GetSection("OIDCUrl");

    Circular.Static.AppSettings.ChallengeUrl = builder.Configuration.GetSection("Challange").Value;
    Circular.Static.AppSettings.ServerTimeZone = builder.Configuration.GetSection("ServerTimeZone").Value;
    Circular.Static.AppSettings.DefaultUserTimeZone = builder.Configuration.GetSection("DefaultUserTimeZone").Value;



    string OIDCClient = builder.Configuration.GetSection("OIDCClient").Value;
    string OIDCSecret = builder.Configuration.GetSection("OIDCSecret").Value;


    var SecretKey = builder.Configuration.GetSection("SecretKey");
    StripeConfiguration.ApiKey = SecretKey.Value;

    builder.Services.AddScoped<IMailRepository, MailRepository>(provider => new MailRepository(connectionString));
    builder.Services.AddScoped<IMailService, Mailservice>();

    builder.Services.AddScoped<ISettingRepository, SettingRepository>(provider => new SettingRepository(connectionString));
    builder.Services.AddScoped<ISettingService, SettingService>();


    builder.Services.AddScoped<IMasterRepository, MasterRepository>(provider => new MasterRepository(connectionString));
    builder.Services.AddScoped<IMasterService, MasterService>();

    builder.Services.AddScoped<IMessageRepository, MessageRepository>(provider => new MessageRepository(connectionString));
    builder.Services.AddScoped<IMessageService, MessageService>();


    builder.Services.AddScoped<IPlannerRepository, IPlannerRepository>(provider => new PlannerRepository(connectionString, 
    provider.GetRequiredService<IHelper>(),provider.GetRequiredService<IHttpContextAccessor>()));
    builder.Services.AddScoped<IPlannerService, PlannerService>();

    builder.Services.AddScoped<ICommunityRepository, ICommunityRepository>(provider => new CommunityRepository(connectionString));
    builder.Services.AddScoped<ICommunityService, CommunityService>();

    builder.Services.AddScoped<INotificationRepository, INotificationRepository>(provider => new NotificationRepository(connectionString));
    builder.Services.AddScoped<INotificationService, NotificationService>();

    builder.Services.AddScoped<IHelper, Helper>();
    builder.Services.AddScoped<IBulkSMS, BulkSMS>();

    builder.Services.AddScoped<ISafetyRepository, SafetyRepository>(provider => new SafetyRepository(connectionString, provider.GetRequiredService<IHelper>(),
		provider.GetRequiredService<IHttpContextAccessor>()));
	builder.Services.AddScoped<ISafetyService, SafetyService>();

    builder.Services.AddScoped<ICustomerRepository, CustomerRepository>
        (provider => new CustomerRepository(connectionString, provider.GetRequiredService<IHelper>(),
        provider.GetRequiredService<IHttpContextAccessor>()));
    builder.Services.AddScoped<ICustomerService, Circular.Services.User.CustomerService>();

    builder.Services.AddScoped<IAccountRepository, AccountRepository>(provider => new AccountRepository(connectionString));
    builder.Services.AddScoped<IAccountServices, AccountServices>();

	builder.Services.AddScoped<ITransportRepository, TransportRepository>
        (provider => new TransportRepository(connectionString, provider.GetRequiredService<IHelper>(),
		provider.GetRequiredService<IHttpContextAccessor>()));
	builder.Services.AddScoped<ITransportService, TransportService>();

    builder.Services.AddScoped<IFinanceRepository, FinanceRepository>(provider => new FinanceRepository(connectionString));
    builder.Services.AddScoped<IFinanceService, FinanceService>();

    builder.Services.AddScoped<IStorefrontRepository, StorefrontRepository>(provider => new StorefrontRepository(connectionString, provider.GetRequiredService<IHelper>(),
        provider.GetRequiredService<IHttpContextAccessor>()));
    builder.Services.AddScoped<IStorefrontServices, StorefrontServices>();

    builder.Services.AddScoped<ISportsRepository, SportsRepository>(provider => new SportsRepository(connectionString));
    builder.Services.AddScoped<ISportsService, SportsService>();

    builder.Services.AddHttpContextAccessor();

    builder.Services.AddScoped<ICommon, Common>();
    builder.Services.AddScoped<IEMail, EMail>();

    builder.Services.AddScoped<NotificationPayload>();

    builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    NLog.LogManager.LoadConfiguration(Path.Combine(Directory.GetCurrentDirectory(), "nlog.config"));
    builder.Services.AddSingleton<ILoggerManager, LoggerManager>();



    builder.Services.Configure<FormOptions>(o =>
    {
        o.ValueLengthLimit = int.MaxValue;
        o.MultipartBodyLengthLimit = int.MaxValue;
        o.MemoryBufferThreshold = int.MaxValue;
    });



    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<TimeZoneFilter>();

    }).AddControllersAsServices();
    //builder.Services.AddControllers().AddControllersAsServices();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(opt =>
    {
        opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Circular 3.0", Version = "v1" });
        opt.EnableAnnotations();
        opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "OIDC",
            Scheme = "bearer"
        });
        opt.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type=ReferenceType.SecurityScheme,
                        Id="Bearer"
                    }
                },
                new string[]{}
            }
        });
    });
    builder.Services.AddCors();

    builder.Services.AddOpenIddict() // Register the OpenIddict client components.
        .AddClient(options =>
        {
            // Allow grant_type=client_credentials to be negotiated.
            options.AllowClientCredentialsFlow().AllowPasswordFlow().AllowRefreshTokenFlow();

            // Disable token storage, which is not necessary for non-interactive flows like
            // grant_type=password, grant_type=client_credentials or grant_type=refresh_token.
            options.DisableTokenStorage();

            // Register the System.Net.Http integration and use the identity of the current
            // assembly as a more specific user agent, which can be useful when dealing with
            // providers that use the user agent as a way to throttle requests (e.g Reddit).
            options.UseSystemNetHttp()
                   .SetProductInformation(typeof(Program).Assembly);

            // Add a client registration matching the client application definition in the server project.
            options.AddRegistration(new OpenIddictClientRegistration
            {
                Issuer = new Uri(OIDCUrl.Value, UriKind.Absolute),
                ClientId = OIDCClient,
                ClientSecret = OIDCSecret
            }) ;
        });
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: "AllowAll",
            builder =>
            {
                builder.AllowAnyOrigin()
                                    .AllowAnyHeader()
                                    .AllowAnyMethod();
            });
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
    {
        app.UseDeveloperExceptionPage();

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "CircularAPI v1");
            c.RoutePrefix = String.Empty;
        });
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseStaticFiles(new StaticFileOptions()
    {
        FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Uploads")),
        RequestPath = new PathString("/Uploads")
    });
    app.UseCors("AllowAll");

    


    app.UseAuthentication();
    app.UseAuthorization();

    if (Convert.ToBoolean(app.Configuration.GetValue(typeof(bool), "RequestTracker")) == true)
    {
        app.UseWhen(context => context.Request.Path.StartsWithSegments("/api"), appBuilder =>
        {
            appBuilder.UseMiddleware<HttpRequestResponseMiddleware>();
        });
    }

    app.MapControllers();
    app.Run();
}
catch (Exception ex)
{

    logger.Error(ex);
    throw (ex);
}
finally
{
    //Ensure to flush and stop internal timers/threads before application-exit(Avoid segmentation fault)
    NLog.LogManager.Shutdown();
}
