using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Web.Areas.Admin.Factories;
using Nop.Plugin.Widgets.PowerReviews.Factories;

namespace Nop.Plugin.Widgets.PowerReviews.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(IServiceCollection services, ITypeFinder typeFinder, AppSettings appSettings)
        {
            services.AddScoped<IProductModelFactory, CustomProductModelFactory>();
        }

        // +1 from Nop.Web's DependencyRegistrar
        public int Order => 3;
    }
}