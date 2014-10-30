using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;
using NBlog.Web.Application.Infrastructure;
using NBlog.Web.Application.Service;
using NBlog.Web.Application.Service.Entity;
using NBlog.Web.Application.Service.Internal;
using NBlog.Web.Application.Storage;
using NBlog.Web.Application.Storage.Azure;
using NBlog.Web.Application.Storage.Json;
using NBlog.Web.Application.Storage.Mongo;
using NBlog.Web.Application.Storage.Sql;
using Quartz;
using Quartz.Impl;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace NBlog.Web
{
	public class ContainerConfig
	{
		private static readonly string DefaultRepositoryName = ConfigurationManager.AppSettings["DefaultRepositoryName"];

		public static void SetUpContainer()
		{
			var container = RegisterDependencies();

			DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

			// replace the default FilterAttributeFilterProvider with one that has Autofac property
			// injection
			FilterProviders.Providers.Remove(FilterProviders.Providers.Single(f => f is FilterAttributeFilterProvider));
			FilterProviders.Providers.Add(new AutofacFilterProvider());

			InitialiseJobScheduler(container);
		}

		private static ResolvedParameter GetResolvedParameterByName<T>(string key)
		{
			return new ResolvedParameter(
				(pi, c) => pi.ParameterType == typeof(T),
				(pi, c) => c.ResolveNamed<T>(key));
		}

		private static void InitialiseJobScheduler(IContainer container)
		{
			// Quartz.NET scheduler
			ISchedulerFactory factory = new StdSchedulerFactory();
			var scheduler = factory.GetScheduler();
			scheduler.JobFactory = new AutofacJobFactory(new Autofac.Integration.Mvc.RequestLifetimeScopeProvider(container));
			scheduler.Start();
		}

		private static IContainer RegisterDependencies()
		{
			var builder = new ContainerBuilder();

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

			builder.RegisterControllers(typeof(ContainerConfig).Assembly)
				.WithParameter(GetResolvedParameterByName<IRepository>(DefaultRepositoryName));
			builder.RegisterModelBinders(Assembly.GetExecutingAssembly());

			builder.RegisterType<ConfigService>().As<IConfigService>().InstancePerLifetimeScope()
				.WithParameter(GetResolvedParameterByName<IRepository>(DefaultRepositoryName));

			builder.RegisterType<EntryService>().As<IEntryService>().InstancePerLifetimeScope()
				.WithParameter(GetResolvedParameterByName<IRepository>(DefaultRepositoryName));

			builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();
			builder.RegisterType<MessageService>().As<IMessageService>().InstancePerLifetimeScope();
			builder.RegisterType<ThemeService>().As<IThemeService>().InstancePerLifetimeScope();
			builder.RegisterType<CloudService>().As<ICloudService>().InstancePerLifetimeScope();
			builder.RegisterType<Services>().As<IServices>().InstancePerLifetimeScope();

			var container = builder.Build();
			return container;
		}
	}
}