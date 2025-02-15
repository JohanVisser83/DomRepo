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
using Circular.Data.Repositories.User;
using Circular.Framework.Emailer;
using Circular.Framework.Logger;
using Circular.Framework.Utility;
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
using Circular.Services.User;
using CircularWeb.filters;
using CircularWeb.Middleware;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.FileProviders;
using NLog;
using OpenIddict.Client;
using Quartz;
using RepoDb;
using System.Reflection;

var logger = LogManager.GetLogger("database");
try
{
    var builder = WebApplication.CreateBuilder(args);
    RepoDb.GlobalConfiguration.Setup().UseSqlServer();


    var connectionString = builder.Configuration.GetConnectionString("CircularDbConnectionString");
    var OIDCUrl = builder.Configuration.GetSection("OIDCUrl");
    CircularWeb.Static.AppSettings.ChallengeUrl = builder.Configuration.GetSection("Challange").Value;
    CircularWeb.Static.AppSettings.ServerTimeZone = builder.Configuration.GetSection("ServerTimeZone").Value;
    CircularWeb.Static.AppSettings.DefaultUserTimeZone = builder.Configuration.GetSection("DefaultUserTimeZone").Value;


    string OIDCClient = builder.Configuration.GetSection("OIDCClient").Value;
    string OIDCSecret = builder.Configuration.GetSection("OIDCSecret").Value;


    // Add services to the container.
    builder.Services.AddControllersWithViews(options =>
    {
        options.Filters.Add<TimeZoneFilter>();

    });

    builder.Services.AddControllersWithViews();

    builder.Services.AddAutoMapper(Assembly.Load("Circular.Core"));
    builder.Services.AddHttpContextAccessor();

    builder.Services.AddScoped<IMailRepository, MailRepository>(provider => new MailRepository(connectionString));
    builder.Services.AddScoped<IMailService, Mailservice>();

    builder.Services.AddScoped<IMessageRepository, MessageRepository>(provider => new MessageRepository(connectionString));
    builder.Services.AddScoped<IMessageService, MessageService>();
    builder.Services.AddScoped<IMasterRepository, MasterRepository>(provider => new MasterRepository(connectionString));
    builder.Services.AddScoped<IMasterService, MasterService>();
    builder.Services.AddScoped<ISafetyRepository, SafetyRepository>(provider => new SafetyRepository(connectionString,
        provider.GetRequiredService<IHelper>(), provider.GetRequiredService<IHttpContextAccessor>()));
    builder.Services.AddScoped<ISafetyService, SafetyService>();
    builder.Services.AddScoped<IPlannerRepository, IPlannerRepository>(provider => new PlannerRepository(connectionString,
    provider.GetRequiredService<IHelper>(), provider.GetRequiredService<IHttpContextAccessor>()));
    builder.Services.AddScoped<IPlannerService, PlannerService>();
    builder.Services.AddScoped<IFinanceRepository, FinanceRepository>(provider => new FinanceRepository(connectionString));
    builder.Services.AddScoped<IFinanceService, FinanceService>();
    builder.Services.AddScoped<INotificationRepository, NotificationRepository>(provider => new NotificationRepository(connectionString));
    builder.Services.AddScoped<INotificationService, NotificationService>();
    builder.Services.AddScoped<ICommunityRepository, CommunityRepository>(provider => new CommunityRepository(connectionString));
    builder.Services.AddScoped<ICommunityService, CommunityService>();
    builder.Services.AddScoped<IHelper, Helper>();
    builder.Services.AddScoped<ISettingRepository, SettingRepository>(provider => new SettingRepository(connectionString));
    builder.Services.AddScoped<ISettingService, SettingService>();
    builder.Services.AddScoped<ICustomerRepository, CustomerRepository>(provider => new CustomerRepository(connectionString,
    provider.GetRequiredService<IHelper>(), provider.GetRequiredService<IHttpContextAccessor>()));
    builder.Services.AddScoped<ICustomerService, CustomerService>();
    builder.Services.AddScoped<IStorefrontServices, StorefrontServices>();
    builder.Services.AddScoped<IStorefrontRepository, StorefrontRepository>(provider => new StorefrontRepository(connectionString,
        provider.GetRequiredService<IHelper>(), provider.GetRequiredService<IHttpContextAccessor>()));
    builder.Services.AddScoped<INotificationRepository, NotificationRepository>(provider => new NotificationRepository(connectionString));
    builder.Services.AddScoped<INotificationService, NotificationService>();

    builder.Services.AddScoped<IAccountRepository, AccountRepository>(provider => new AccountRepository(connectionString));
    builder.Services.AddScoped<IAccountServices, AccountServices>();

    builder.Services.AddScoped<ISportsRepository, SportsRepository>(provider => new SportsRepository(connectionString));
    builder.Services.AddScoped<ISportsService, SportsService>();


    //builder.Services.AddScoped<ILoggerManager, LoggerManager>();
    // builder.Services.AddControllers().AddControllersAsServices();

    NLog.LogManager.LoadConfiguration(Path.Combine(Directory.GetCurrentDirectory(), "nlog.config"));
    builder.Services.AddSingleton<ILoggerManager, LoggerManager>();
    builder.Services.AddScoped<IEMail, EMail>();

    builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(options =>
{
    options.LoginPath = "/Login/Login/";
    options.LogoutPath = "/Login/Logout/";
});


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
            });
        });


    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }
    else
    {
        app.UseDeveloperExceptionPage();
    }
    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseStaticFiles(new StaticFileOptions()
    {
        FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Uploads")),
        RequestPath = new PathString("/Uploads")
    });

    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

   

    if (Convert.ToBoolean(app.Configuration.GetValue(typeof(bool), "RequestTracker")) == true)
    {
        app.UseMiddleware<HttpRequestResponseMiddleware>();
    }

    app.MapControllerRoute(
       name: "default",
       pattern: "{controller=Login}/{action=Login}/{id?}");

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







