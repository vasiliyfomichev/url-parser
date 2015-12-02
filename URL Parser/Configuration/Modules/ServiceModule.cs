#region

using System.Reflection;
using Autofac;

#endregion

namespace URL_Parser.Configuration.Modules
{
    public class ServiceModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                   .Where(t => t.Name.EndsWith("Service"))
                   .AsImplementedInterfaces();
        }
    }
}