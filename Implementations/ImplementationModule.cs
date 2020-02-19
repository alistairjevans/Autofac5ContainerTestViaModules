using Autofac;
using Interfaces;

namespace Implementations
{
    class ImplementationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ImplementationN>().As<InterfaceN>();
        }
    }
}
