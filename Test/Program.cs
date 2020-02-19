using Interfaces;
using Autofac;
using Microsoft.Extensions.Configuration;
using System;
using Autofac.Configuration;

namespace UnityAutofac5ContainerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder();
            config.AddJsonFile("config.json");

            var module = new ConfigurationModule(config.Build());

            var builder = new ContainerBuilder();

            builder.RegisterModule(module);
            //builder.RegisterType<ImplementationN>().As<InterfaceN>();

            var container = builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var impl = scope.Resolve<InterfaceN>();
                Console.WriteLine(impl.GetString());
                Console.ReadKey();
            }

        }
    }
}
