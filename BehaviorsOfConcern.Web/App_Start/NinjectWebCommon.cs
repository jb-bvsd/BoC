[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(BehaviorsOfConcern.Web.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(BehaviorsOfConcern.Web.App_Start.NinjectWebCommon), "Stop")]

namespace BehaviorsOfConcern.Web.App_Start {
    using System;
    using System.Web;
    using System.Web.Configuration;
    using Domain.DomainServices;
    using Domain.DomainServices.Abstract;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Activation;
    using Ninject.Web.Common;

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
            kernel.Bind<Controllers.IncidentController>()
                  .ToSelf()
                  .WithConstructorArgument("clearAuthenticationPrefix", WebConfigurationManager.AppSettings["clearAuthenticationPrefix"]);  //passes null for missing setting

            kernel.Bind<ILookupCodeCache>()
                  .ToMethod(LoadLookups);

            kernel.Bind<ISchoolRepository>()
                  .To<SchoolRepository>()
                  .WithConstructorArgument("connectionString", WebConfigurationManager.ConnectionStrings["connICBoulderValley"].ConnectionString);
        }

        private static ILookupCodeCache LoadLookups(IContext arg) {
            ILookupCodeCache lookupCodeCache = null;
            System.Web.Caching.Cache cache = HttpContext.Current.Cache;

            if (cache["lookups"] == null) {
                lookupCodeCache = new LookupCodeCache(WebConfigurationManager.ConnectionStrings["connICBoulderValley"].ConnectionString);
                lookupCodeCache.Refresh();
                cache.Add("lookups", lookupCodeCache, null, DateTime.Now.AddHours(8),
                    System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Low, null);
            } else {
                lookupCodeCache = (ILookupCodeCache)cache["lookups"];
            }
            return lookupCodeCache;
        }
    }
}
