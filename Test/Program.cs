using Interfaces;
using Autofac;
using Microsoft.Extensions.Configuration;
using System;
using Autofac.Configuration;
using System.Runtime.Loader;
using System.Reflection;
using System.IO;
using System.Linq;

namespace UnityAutofac5ContainerTest
{
    class Program
    {
        private class ModulePluginContext : AssemblyLoadContext
        {
            private readonly AssemblyDependencyResolver resolver;

            public string AssemblyName { get; }

            public ModulePluginContext(string baseDir, string assemblyName)
            {
                resolver = new AssemblyDependencyResolver(Path.Combine(baseDir, assemblyName + ".dll"));
                AssemblyName = assemblyName;
            }

            protected override Assembly Load(AssemblyName assemblyName)
            {
                // Can't load from default, use files instead.
                var path = resolver.ResolveAssemblyToPath(assemblyName);

                if (path is string)
                {
                    return LoadFromAssemblyPath(path);
                }

                return null;
            }

        }

        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder();
            config.AddJsonFile("config.json");

            var modulePlugin = new ModulePluginContext(
                Path.GetFullPath("..\\modules\\netcoreapp3.1\\", Environment.CurrentDirectory),
                "Implementations");

            AssemblyLoadContext.Default.Resolving += (ctxt, assemblyName) =>
            {
                if(assemblyName.Name == modulePlugin.AssemblyName)
                {
                    return modulePlugin.LoadFromAssemblyName(assemblyName);
                }

                return null;
            };

            var loadedDlls = AssemblyLoadContext.Default.Assemblies.ToList();

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
