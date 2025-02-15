using System;
using RepoDb;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Circular.Data.Repositories.Message;
using Circular.Services.Message;
using Circular.Data.Repositories.Notification;
using Circular.Services.Notifications;
using Circular.Data.Repositories.User;
using Circular.Services.User;
using Circular.Framework.Utility;
using Microsoft.AspNetCore.Http;
using NLog;
using Circular.Framework.Logger;
using Circular.Services.Email;
using Circular.Data.Repositories.Email;
using Circular.Framework.Emailer;
using CircularScheduledJobs;
using Circular.Data.Repositories.Planners;
using Circular.Services.Planners;
using Circular.Data.Repositories.Account;
using Circular.Services.Account;
using Circular.Data.Repositories.Home;
using Circular.Services.Master;
using Circular.Services.Finance;
using Circular.Data.Repositories.Finance;
using Circular.Data.Repositories.Storefront;
using Circular.Services.Storefront;

namespace ScheduledJobs
{
    class Program
    {
        private static IHost CreateHost(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                var logger = LogManager.GetLogger("database");
                var connectionString = context.Configuration.GetConnectionString("CircularDbConnectionString");
                string appRootPath = Convert.ToString(context.Configuration.GetValue(typeof(string), "ApplicationRootPath"));
                RepoDb.GlobalConfiguration.Setup().UseSqlServer();
                services.AddHttpContextAccessor();
                services.AddScoped<IEMail, EMail>();
                services.AddScoped<IHelper, Helper>();
                NLog.LogManager.LoadConfiguration(Path.Combine(appRootPath, "nlog.config"));
                services.AddSingleton<ILoggerManager, LoggerManager>();
                services.AddScoped<IMailRepository, MailRepository>(provider => new MailRepository(connectionString));
                services.AddScoped<IMailService, Mailservice>();
                services.AddScoped<IPlannerRepository, PlannerRepository>(provider => new PlannerRepository(connectionString, provider.GetRequiredService<IHelper>(),
                 provider.GetRequiredService<IHttpContextAccessor>()));
                services.AddSingleton<IPlannerService, PlannerService>();
                services.AddScoped<INotificationRepository, INotificationRepository>(provider => new NotificationRepository(connectionString));
                services.AddScoped<INotificationService, NotificationService>();
                services.AddSingleton<ICustomerRepository, CustomerRepository>(provider => new CustomerRepository(connectionString, provider.GetRequiredService<IHelper>(),
                provider.GetRequiredService<IHttpContextAccessor>()));
                services.AddSingleton<ICustomerService, CustomerService>();
                services.AddScoped<IMessageRepository, IMessageRepository>(provider => new MessageRepository(connectionString));
                services.AddScoped<IMessageService, MessageService>();
                services.AddScoped<IAccountRepository, IAccountRepository>(provider => new AccountRepository(connectionString));
                services.AddScoped<IAccountServices, AccountServices>();
                services.AddScoped<IMasterRepository, MasterRepository>(provider => new MasterRepository(connectionString));
                services.AddSingleton<IMasterService, MasterService>();

                 services.AddSingleton<IStorefrontRepository, StorefrontRepository>(provider => new StorefrontRepository(connectionString, provider.GetRequiredService<IHelper>(),
                 provider.GetRequiredService<IHttpContextAccessor>()));
                services.AddSingleton<IStorefrontServices, StorefrontServices>();



                services.AddScoped<IFinanceRepository, FinanceRepository>(provider => new FinanceRepository(connectionString));
                services.AddSingleton<IFinanceService, FinanceService>();

              



            })
            .Build();


        static void Main(string[] args)
        {
            var logger = LogManager.GetLogger("database");
            try
            {
                logger.Info("job started");
                IHost host = CreateHost(args);
                logger.Info("after CreateHost");

                NotificationWorker worker = ActivatorUtilities.CreateInstance<NotificationWorker>(host.Services);
                logger.Info("after worker");

                //EventNotification eve = ActivatorUtilities.CreateInstance<EventNotification>(host.Services);
                //eve.SendEventScheduleNotification();
                //logger.Info("after event notification");

                MessageWorker messageWorker = ActivatorUtilities.CreateInstance<MessageWorker>(host.Services);
                messageWorker.SendScheduleMessages();
                logger.Info("after Scheduled Messages");

                messageWorker.SendBroadcastMessages();
                logger.Info("after Broadcast Messages");

                AccountWorker accountWorker = ActivatorUtilities.CreateInstance<AccountWorker>(host.Services);
                accountWorker.SendScheduleAccountEmails();
                logger.Info("after account Email");

                MasterWorker masterWorker = ActivatorUtilities.CreateInstance<MasterWorker>(host.Services);
                masterWorker.ClearUsedOTP();
                logger.Info("after OTP ");
                masterWorker.UpdateDevices();


                EventWorker eventWorker = ActivatorUtilities.CreateInstance<EventWorker>(host.Services);
                eventWorker.SendScheduleEventEmails();
                logger.Info("after event Email");

                SubscriptionWorker subscriptionWorker = ActivatorUtilities.CreateInstance<SubscriptionWorker>(host.Services);
                subscriptionWorker.CheckExpiredSubscriptionAndNotify();
                logger.Info("after ExpiredSubscription Email");



                logger.Info("Job completed");

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                Console.ReadLine();
            }
        }

    }
}
