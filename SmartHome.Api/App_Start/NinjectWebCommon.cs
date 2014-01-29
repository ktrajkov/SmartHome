[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(SmartHome.Web.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(SmartHome.Web.App_Start.NinjectWebCommon), "Stop")]

namespace SmartHome.Web.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using SmartHome.Api.Infrastructure.Abstract;
    using SmartHome.Api.Infrastructure.Concrete;
    using SmartHome.Api.Models;
    using SmartHome.Api.Properties;
    using SmartHome.Data;
    using System.Data.Entity;

    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

            RegisterServices(kernel);
            System.Web.Http.GlobalConfiguration.Configuration.DependencyResolver = new Ninject.WebApi.DependencyResolver.NinjectDependencyResolver(kernel);
            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            EmailSettings emailSettings = new EmailSettings
            {
                ServerName = Settings.Default.EmailServerName,
                ServerPort = Settings.Default.EmailServerPort,
                UseSsl = Settings.Default.EmailUseSsl,
                Username = Settings.Default.EmailUsername,
                Password = Settings.Default.EmailPassword
            };
            kernel.Bind<IEmailSender>().To<EmailSender>().WithConstructorArgument("settings", emailSettings);
            kernel.Bind<DbContext>().To<ApplicationDbContext>().InRequestScope();
            kernel.Bind<IUowData>().To<UowData>().InRequestScope();
        }

    }
}
