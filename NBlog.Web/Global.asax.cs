﻿using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;
using Elmah;
using NBlog.Web.Application.Infrastructure;
using NBlog.Web.Application.Service;
using NBlog.Web.Application.Service.Entity;
using NBlog.Web.Application.Service.Internal;
using NBlog.Web.Application.Storage;
using NBlog.Web.Application.Storage.Json;
using NBlog.Web.Application.Storage.Mongo;
using NBlog.Web.Application.Storage.Sql;
using Quartz;
using Quartz.Impl;
using NBlog.Web.Application.Storage.Azure;

namespace NBlog.Web
{
    public class MvcApplication : HttpApplication, IRequestAuthorizationHandler
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.RouteExistingFiles = false;
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // homepage
            routes.MapRouteLowercase("", "", new { controller = "Home", action = "Index" });

            // favicon
            routes.MapRouteLowercase("", "favicon.ico", new { controller = "Resource", action = "Favicon" });

            // combined scripts
            routes.MapRouteLowercase("", "min.css", new { controller = "Resource", action = "Css" });
            routes.MapRouteLowercase("", "min.js", new { controller = "Resource", action = "JavaScript" });

            // feed
            routes.MapRouteLowercase("", "feed", new { controller = "Feed", action = "Index" });

            // search
            routes.MapRouteLowercase("", "search", new { controller = "Search", action = "Index" });

            // search
            routes.MapRouteLowercase("", "contact", new { controller = "Contact", action = "Index" });

            // entry pages
            routes.MapRouteLowercase("", "{id}", new { controller = "Entry", action = "Show" });

            // general route
            routes.MapRouteLowercase("", "{controller}/{action}/{id}", new { id = UrlParameter.Optional });
        }


        protected IContainer RegisterDependencies()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterModelBinders(Assembly.GetExecutingAssembly());
            
            builder.RegisterType<ThemeableRazorViewEngine>().As<IViewEngine>().InstancePerLifetimeScope().WithParameter(
                new NamedParameter("tenantSelector", new HttpTenantSelector())
            );

            var repositoryKeys = new RepositoryKeys();
            repositoryKeys.Add<Entry>(e => e.Slug);
            repositoryKeys.Add<Config>(c => c.Site);
            repositoryKeys.Add<User>(u => u.Username);

            builder.RegisterType<JsonRepository>().Named<IRepository>("json").InstancePerLifetimeScope().WithParameters(new[] {
                new NamedParameter("keys", repositoryKeys),
                new NamedParameter("tenantSelector", new HttpTenantSelector())
            });

            builder.RegisterType<SqlRepository>().Named<IRepository>("sql").InstancePerLifetimeScope().WithParameters(new[] {
                new NamedParameter("keys", repositoryKeys),
                new NamedParameter("connectionString", "Server=.;Trusted_Connection=True;"),
                new NamedParameter("databaseName", "NBlog")
            });

            builder.RegisterType<MongoRepository>().Named<IRepository>("mongo").InstancePerLifetimeScope().WithParameters(new[] {
                new NamedParameter("keys", repositoryKeys),
                new NamedParameter("connectionString", "mongodb://localhost"),
                new NamedParameter("databaseName", "nblog")
            });

            builder.RegisterType<AzureBlobRepository>().Named<IRepository>("azure").InstancePerHttpRequest().WithParameters(new[] {
                new NamedParameter("keys", repositoryKeys),
                new NamedParameter("tenantSelector", new HttpTenantSelector())
            });

            builder.RegisterType<ConfigService>().As<IConfigService>().InstancePerLifetimeScope()
                .WithParameter(GetResolvedParameterByName<IRepository>("json"));

            builder.RegisterType<EntryService>().As<IEntryService>().InstancePerLifetimeScope()
                .WithParameter(GetResolvedParameterByName<IRepository>("json"));

            builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();
            builder.RegisterType<MessageService>().As<IMessageService>().InstancePerLifetimeScope();
            builder.RegisterType<ThemeService>().As<IThemeService>().InstancePerLifetimeScope();
            builder.RegisterType<CloudService>().As<ICloudService>().InstancePerLifetimeScope();
            builder.RegisterType<Services>().As<IServices>().InstancePerLifetimeScope();

            var container = builder.Build();
            return container;
        }


        public static ResolvedParameter GetResolvedParameterByName<T>(string key)
        {
            return new ResolvedParameter(
                (pi, c) => pi.ParameterType == typeof(T),
                (pi, c) => c.ResolveNamed<T>(key));
        }


        protected void Application_Start()
        {
            var container = RegisterDependencies();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            // replace the default FilterAttributeFilterProvider with one that has Autofac property injection
            FilterProviders.Providers.Remove(FilterProviders.Providers.Single(f => f is FilterAttributeFilterProvider));
            FilterProviders.Providers.Add(new AutofacFilterProvider(new Autofac.Integration.Web.ContainerProvider(container)));

            HtmlHelper.ClientValidationEnabled = true;
            HtmlHelper.UnobtrusiveJavaScriptEnabled = true;

            AreaRegistration.RegisterAllAreas();
            RegisterRoutes(RouteTable.Routes);

            InitialiseJobScheduler(container);
        }


        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            EnforceLowercaseUrl();
        }


        protected void Application_Error(object sender, EventArgs e)
        {
            if (!HttpContext.Current.IsCustomErrorEnabled)
                return;

            var exception = Server.GetLastError();
            var httpException = new HttpException(null, exception);

            var routeData = new RouteData();
            routeData.Values.Add("controller", "Error");
            routeData.Values.Add("action", "Index");
            routeData.Values.Add("httpException", httpException);

            Server.ClearError();

            var errorController = ControllerBuilder.Current.GetControllerFactory().CreateController(
                new RequestContext(new HttpContextWrapper(Context), routeData), "Error");

            errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
        }


        protected void InitialiseJobScheduler(IContainer container)
        {
            // Quartz.NET scheduler
            ISchedulerFactory factory = new StdSchedulerFactory();
            var scheduler = factory.GetScheduler();
            scheduler.JobFactory = new AutofacJobFactory(new Autofac.Integration.Web.ContainerProvider(container));
            scheduler.Start();
        }


        // (Elmah.IRequestAuthorizationHandler.Authorize)
        public bool Authorize(HttpContext context)
        {
            var userService = (IUserService)DependencyResolver.Current.GetService(typeof(IUserService));
            if (!userService.Current.IsAdmin)
            {
                throw new HttpException(403, "Forbidden");
            }

            return true;
        }


        protected void EnforceLowercaseUrl()
        {
            var path = Request.Url.AbsolutePath;
            var verbIsGet = string.Equals(Request.HttpMethod, "GET", StringComparison.CurrentCultureIgnoreCase);

            if (!verbIsGet || !path.Any(c => char.IsUpper(c))) return;

            Response.RedirectPermanent(path.ToLowerInvariant() + Request.Url.Query);
        }
    }
}