using Circular.Framework.Utility;
using Microsoft.Extensions.FileProviders;
using RepoDb;
using OpenIddict.Client;
using Microsoft.AspNetCore.Authentication.Cookies;
using Circular.Data.Repositories.User;
using Circular.Services.User;
using Circular.Services.Storefront;
using Circular.Data.Repositories.Storefront;
using Circular.Core.Entity;
using System.Reflection;
using Circular.Data.Repositories.Notification;
using Circular.Services.Notifications;
using Circular.Framework.Logger;
using NLog;
using Circular.Framework.Emailer;
using Circular.Data.Repositories.Email;
using Circular.Services.Email;
using Circular.Services.CreateCommunity;
using Circular.Data.Repositories.CreateCommunity;
using Circular.Data.Repositories.CommunityFeatures;
using Circular.Services.CommunityFeatures;
using CircularSubscriptions.Business;
using Stripe;
using Circular.Data.Repositories.Finance;
using Circular.Services.Finance;
using Circular.Data.Repositories.Message;
using Circular.Services.Message;
using Circular.Framework.ShortMessages;
using Circular.Data.Repositories.Planners;
using Circular.Services.Planners;

var logger = LogManager.GetLogger("database");
try
{
    var builder = WebApplication.CreateBuilder(args);
    RepoDb.GlobalConfiguration.Setup().UseSqlServer();


    var connectionString = builder.Configuration.GetConnectionString("CircularDbConnectionString");
    var OIDCUrl = builder.Configuration.GetSection("OIDCUrl");
    var SecretKey = builder.Configuration.GetSection("SecretKey");
    StripeConfiguration.ApiKey = SecretKey.Value;

    // Add services to the container.
    builder.Services.AddControllersWithViews();

    builder.Services.AddAutoMapper(Assembly.Load("Circular.Core"));
    builder.Services.AddHttpContextAccessor();

    builder.Services.AddScoped<IMailRepository, MailRepository>(provider => new MailRepository(connectionString));
    builder.Services.AddScoped<IMailService, Mailservice>();

    builder.Services.AddScoped<IHelper, Helper>();
    builder.Services.AddScoped<IBulkSMS, BulkSMS>();

    builder.Services.AddScoped<INotificationRepository, NotificationRepository>(provider => new NotificationRepository(connectionString));
    builder.Services.AddScoped<INotificationService, NotificationService>();

    builder.Services.AddScoped<IHelper, Helper>();

    builder.Services.AddScoped<ICustomerRepository, CustomerRepository>(provider => new CustomerRepository(connectionString,
    provider.GetRequiredService<IHelper>(), provider.GetRequiredService<IHttpContextAccessor>()));
    builder.Services.AddScoped<ICustomerService, Circular.Services.User.CustomerService>();
    builder.Services.AddScoped<IStorefrontServices, StorefrontServices>();
    builder.Services.AddScoped<IStorefrontRepository, StorefrontRepository>(provider => new StorefrontRepository(connectionString,
        provider.GetRequiredService<IHelper>(), provider.GetRequiredService<IHttpContextAccessor>()));
    builder.Services.AddScoped<INotificationRepository, NotificationRepository>(provider => new NotificationRepository(connectionString));
    builder.Services.AddScoped<INotificationService, NotificationService>();

    builder.Services.AddScoped<ICreateCommunityRepository, CreateCommunityRepository>(provider => new CreateCommunityRepository(connectionString));
    builder.Services.AddScoped<ICreateCommunityServices, CreateCommunityServices>();

    builder.Services.AddScoped<ICommunityFeaturesRepositories, CommunityFeaturesRepositories>(provider => new CommunityFeaturesRepositories(connectionString));
    builder.Services.AddScoped<ICommunityFeaturesServices, CommunityFeaturesServices>();

    builder.Services.AddScoped<IFinanceRepository, FinanceRepository>(provider => new FinanceRepository(connectionString));
    builder.Services.AddScoped<IFinanceService, FinanceService>();

    builder.Services.AddScoped<IMessageRepository, MessageRepository>(provider => new MessageRepository(connectionString));
    builder.Services.AddScoped<IMessageService, MessageService>();

    builder.Services.AddScoped<IPlannerRepository, IPlannerRepository>(provider => new PlannerRepository(connectionString,
    provider.GetRequiredService<IHelper>(), provider.GetRequiredService<IHttpContextAccessor>()));
    builder.Services.AddScoped<IPlannerService, PlannerService>();


    builder.Services.AddScoped<ILoggerManager, LoggerManager>();
    // builder.Services.AddControllers().AddControllersAsServices();

    NLog.LogManager.LoadConfiguration(Path.Combine(Directory.GetCurrentDirectory(), "nlog.config"));
    builder.Services.AddScoped<ILoggerManager, LoggerManager>();
    builder.Services.AddScoped<IGeneric, Generic>();
    builder.Services.AddScoped<IEMail, EMail>();

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    }).AddCookie(options =>
    {
        options.LoginPath = "/EmailVerification/EmailVerification/";
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
                ClientId = "E558BDE2-0BA9-4C19-BADA-1C498D786246",
                ClientSecret = "1345480a-d6cb-4e39-cda2-08db72ea2159"
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
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=EmailVerification}/{action=EmailVerification}/{id?}");

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








