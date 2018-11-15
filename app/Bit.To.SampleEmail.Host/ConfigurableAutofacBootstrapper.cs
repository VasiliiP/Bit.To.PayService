using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Autofac;
using Nancy.Configuration;

// ReSharper disable once CheckNamespace
namespace Bit.To.SampleEmail
{
    public class ConfigurableAutofacBootstrapper : AutofacNancyBootstrapper
    {
        private readonly IContainer _container;
        private readonly IDictionary<Type, List<Action<ILifetimeScope, NancyModule>>> _configureModules;
        private readonly ICollection<Action<ILifetimeScope, IPipelines>> _configureAppPipelines;
        private readonly ICollection<Action<ILifetimeScope, NancyContext>> _configureRequestContainer;

        public ConfigurableAutofacBootstrapper(
            IContainer container,
            ICollection<Action<ILifetimeScope, IPipelines>> configureAppPipelines,
            IDictionary<Type, List<Action<ILifetimeScope, NancyModule>>> configureModules, 
            ICollection<Action<ILifetimeScope, NancyContext>> configureRequestContainer)
        {
            _container = container;
            _configureAppPipelines = configureAppPipelines;
            _configureModules = configureModules;
            _configureRequestContainer = configureRequestContainer;
        }

        public ConfigurableAutofacBootstrapper(
            IContainer container,
            ICollection<Action<ILifetimeScope, IPipelines>> configureAppPipelines,
            params Type[] allowedModules)
        {
            _container = container;
            _configureAppPipelines = configureAppPipelines;
            _configureModules = allowedModules.ToDictionary(x => x, x => new List<Action<ILifetimeScope, NancyModule>>());
            _configureRequestContainer = new List<Action<ILifetimeScope, NancyContext>>();
        }

        protected override ILifetimeScope GetApplicationContainer()
        {
            return _container;
        }

        protected override INancyModule GetModule(ILifetimeScope container, Type moduleType)
        {
            var module = container.Resolve(moduleType) as NancyModule;
            if (_configureModules.TryGetValue(moduleType, out var actions))
            {
                foreach (var action in actions)
                {
                    action(container, module);
                }
            }

            return module;
        }

        public override void Configure(INancyEnvironment environment)
        {
            base.Configure(environment);
            environment.Tracing(enabled: false, displayErrorTraces: true);
        }

        protected override void ApplicationStartup(ILifetimeScope container, IPipelines pipelines)
        {
            // No registrations should be performed in here, however you may
            // resolve things that are needed during application startup.

            foreach (var configure in _configureAppPipelines)
            {
                configure(container, pipelines);
            }

            pipelines.AfterRequest.AddItemToEndOfPipeline(x => x.Response.Headers.Add("Access-Control-Allow-Origin", "*"));
        }

        protected override void ConfigureApplicationContainer(ILifetimeScope existingContainer)
        {
            // Perform registration that should have an application lifetime
        }

        protected override IEnumerable<INancyModule> GetAllModules(ILifetimeScope container)
        {
            foreach (var moduleType in _configureModules.Keys)
            {
                yield return GetModule(container, moduleType);
            }
        }

        protected override void ConfigureRequestContainer(ILifetimeScope container, NancyContext context)
        {
            // Perform registrations that should have a request lifetime
            foreach (var action in _configureRequestContainer)
            {
                action(container, context);
            }
        }

        protected override void RequestStartup(ILifetimeScope container, IPipelines pipelines, NancyContext context)
        {
            // No registrations should be performed in here, however you may
            // resolve things that are needed during request startup.
        }
    }
}