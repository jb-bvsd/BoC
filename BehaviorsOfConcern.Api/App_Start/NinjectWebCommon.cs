[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(BehaviorsOfConcern.Api.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(BehaviorsOfConcern.Api.App_Start.NinjectWebCommon), "Stop")]

namespace BehaviorsOfConcern.Api.App_Start {
    using System;
    using System.Text;
    using System.Web;
    using System.Web.Configuration;
    using System.Web.Http;
    using BvsdSecurity.Service;
    using Domain.DomainServices;
    using Domain.DomainServices.Abstract;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Ninject;
    using Ninject.Activation;
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
            //introduced in .net3.5, MS gives us HttpContextBase/HttpContextWrapper to promote loose coupling, better enable unit testing, allow for mocking, etc.
            kernel.Bind<HttpContextBase>().ToMethod(nc => new HttpContextWrapper(HttpContext.Current));

            kernel.Bind<ILookupCodeCache>().ToMethod(LoadLookups);

            kernel.Bind<ICipher>()
                .To<BlowFish>()
                .WithConstructorArgument("cipherKey", Encoding.ASCII.GetBytes(WebConfigurationManager.AppSettings["cipherSalt_IC"]));

            kernel.Bind<IBoCAuthorizationService>()
                .To<BoCAuthorizationService>()
                .WithConstructorArgument("connectionString", WebConfigurationManager.ConnectionStrings["connICBoulderValley"].ConnectionString)
                .WithConstructorArgument("clearAuthenticationPrefix", WebConfigurationManager.AppSettings["clearAuthenticationPrefix"]);

            kernel.Bind<IIncidentRepository>()
                .To<IncidentRepository>()
                .WithConstructorArgument("connectionString", WebConfigurationManager.ConnectionStrings["connICBoulderValley"].ConnectionString);

            kernel.Bind<ISchoolRepository>()
                .To<SchoolRepository>()
                .WithConstructorArgument("connectionString", WebConfigurationManager.ConnectionStrings["connICBoulderValley"].ConnectionString);
        }


        private static ILookupCodeCache LoadLookups(IContext arg) {
            ILookupCodeCache lookupCodeCache = null;
            //HttpContextBase context = new HttpContextWrapper(HttpContext.Current);
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
