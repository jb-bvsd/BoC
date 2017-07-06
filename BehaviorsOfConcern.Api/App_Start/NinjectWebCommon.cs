[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(BehaviorsOfConcern.Api.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(BehaviorsOfConcern.Api.App_Start.NinjectWebCommon), "Stop")]

namespace BehaviorsOfConcern.Api.App_Start {
    using System;
    using System.Text;
    using System.Web;
    using System.Web.Configuration;
    using System.Web.Http;
    using Domain.RepoServices;
    using Domain.RepoServices.Abstract;
    using ICCipher.Service;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using Ninject.Web.WebApi;

    public static class NinjectWebCommon {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop() {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel() {
            var kernel = new StandardKernel();
            try {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                GlobalConfiguration.Configuration.DependencyResolver = new NinjectDependencyResolver(kernel);  //Jun2017, added at the advice of:  ht tp://nodogmablog.bryanhogan.net/2016/04/web-api-2-and-ninject-how-to-make-them-work-together/
                return kernel;
            } catch {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel) {
            kernel.Bind<ICipher>()
                .To<BlowFish>()
                .WithConstructorArgument("cipherKey", Encoding.ASCII.GetBytes(WebConfigurationManager.AppSettings["cipherSalt_IC"]));

            kernel.Bind<IBoCAuthorizationService>()
                .To<BoCAuthorizationService>()
                .WithConstructorArgument("connectionString", WebConfigurationManager.ConnectionStrings["connICBoulderValley"].ConnectionString);
        }
    }
}
