using System;
using System.Collections.Generic;
using System.IO;
using Autofac;
using Bit.Persistence;
using Bit.To.EmailSending;
using Bit.To.SampleEmail;
using Bit.To.SampleEmailSending;
using Bit.Validation;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Responses;
using Nancy.Swagger;
using Nancy.Swagger.Modules;
using Nancy.Swagger.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog;
using Swagger.ObjectModel;
using Topshelf;
using Topshelf.LibLog;
using Topshelf.Nancy;

namespace Bit.To.Email
{
    class Program
    {
        static void Main(string[] args)
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Converters = new JsonConverter[] {new StringEnumConverter()}
            };

            var config = new AppConfigurationBuilder()
                .AddJsonFile("cfg\\default.json")
                .AddJsonFile("cfg\\app.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .WriteTo.File(
                    Path.Combine(String.Format(config["LogPath"].AsString(), config["ServiceName"]), $"{config["ServiceName"]}.txt"),
                    rollingInterval: RollingInterval.Day)
                .CreateLogger();

            var container = CreateContainer(config);

            void LogError(ILifetimeScope c, IPipelines pipelines) =>
                pipelines.OnError.AddItemToStartOfPipeline(
                    (ctx, ex) =>
                    {
                        Log.Logger.Error("NancyHost Exception {@ex}", ex);

                        switch (ex)
                        {
                            case ValidationException validationException:
                                return new TextResponse(ex.Message) {StatusCode = HttpStatusCode.UnprocessableEntity, }; 
                        }

                        return null;
                    });

            // ConfigurableAutofacBootstrapper реализован локально, т.к. его package не публикуется
            var bootstrapper = new ConfigurableAutofacBootstrapper(
                container,
                new List<Action<ILifetimeScope, IPipelines>> { LogError },
                typeof(SendEmailModule), typeof(GetEmailItemModule), typeof(SwaggerModule));

            InitSwagger(config);

            HostFactory.Run(x =>
            {
                x.UseLibLog();
                x.Service<BaseService>(
                    s =>
                    {
                        s.ConstructUsing(settings => new BaseService());
                        s.WhenStarted(service => service.Start());
                        s.WhenStopped(service => service.Stop());
                        s.WithNancyEndpoint(
                            x,
                            c =>
                            {
                                c.UseBootstrapper(bootstrapper);
                                var uri = new Uri(config["HostUrl"]);
                                {
                                    c.AddHost(uri.Scheme, uri.Host, uri.Port, uri.AbsolutePath);
                                }
                                c.CreateUrlReservationsOnInstall();
                                c.OpenFirewallPortsOnInstall(config["ServiceName"]);
                            });
                    });
                x.StartAutomaticallyDelayed();
                x.SetServiceName(config["ServiceName"]);
                x.SetDisplayName(config["ServiceName"]);
                x.SetDescription($"{config["ServiceDescription"]} | {config["HostUrl"]}");
                x.RunAsNetworkService();
            });
        }

        private static void InitSwagger(IAppConfiguration config)
        {
            SwaggerMetadataProvider.SetInfo(
                config["SvcMetaTitle"],
                config["SvcMetaVer"],
                $"[ Base URL: {config["HostUrl"]} ]",
                new Contact { EmailAddress = config["SvcMetaContact"] });
            
            SwaggerConfig.ResourceListingPath = config["SwaggerResourceListingPath"];

            SwaggerTypeMapping.AddTypeMapping(typeof(DateTime), typeof(string));
            SwaggerTypeMapping.AddTypeMapping(typeof(DateTime?), typeof(string));
            SwaggerTypeMapping.AddTypeMapping(typeof(TimeSpan), typeof(string));
            SwaggerTypeMapping.AddTypeMapping(typeof(TimeSpan?), typeof(string));
            SwaggerTypeMapping.AddTypeMapping(typeof(Guid), typeof(string));
        }

        private static IContainer CreateContainer(IAppConfiguration config)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<EmailItemRepository>()
                .WithParameter("connectionFactory", new SqlConnectionFactory(config.ConnectionString("email"))).AsImplementedInterfaces();
            builder.RegisterType<EmailItemsDbContext.Mapper>();
            
            builder.RegisterType<FakeSmtpClient>()
                .WithParameter("userName", config["FromAddress"].AsString())
                .WithParameter("password", config["Password"].AsString())
                .WithParameter("smtpHost", config["SmtpHost"].AsString())
                .WithParameter("smtpPort", config["SmtpPort"].AsInt())
                .AsImplementedInterfaces();

            builder.RegisterType<MailMessageFactory>()
                .WithParameter("fromAddress", config["FromAddress"].AsString())
                .WithParameter("fromDisplayName", config["FromDisplayName"].AsString())
                .AsImplementedInterfaces();

            builder.RegisterType<SendEmailModule>();
            builder.RegisterType<SendEmailHandler>();
            builder.RegisterType<SendEmailValidator>().AsImplementedInterfaces();
            builder.Register(c => new ValidatedCommandHandler<SendEmail>(c.Resolve<SendEmailHandler>(), c.Resolve<IValidator<SendEmail>>()))
                .AsImplementedInterfaces();

            builder.RegisterType<GetEmailItemModule>();
            builder.RegisterType<GetEmailItemHandler>().AsImplementedInterfaces();

            builder.RegisterType<SwaggerModule>();

            return builder.Build();
        }

        private class BaseService
        {
            public void Start()
            {
            }

            public void Stop()
            {
            }
        }
    }
}
