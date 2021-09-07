using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Services.Cms;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;
using Task = System.Threading.Tasks.Task;
using Nop.Services.Configuration;
using Nop.Data;
using Nop.Core.Domain.Catalog;
using System.Linq;
using Nop.Services.Common;
using Nop.Services.Catalog;
using Nop.Core.Domain.Common;

namespace Nop.Plugin.Widgets.PowerReviews
{
    public class PowerReviewsPlugin : BasePlugin, IWidgetPlugin
    {
        private const string HasNopReviewsKey = "HasNopReviews";

        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IProductService _productService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;

        private readonly IWebHelper _webHelper;

        private readonly IRepository<GenericAttribute> _genericAttributeRepository;
        private readonly IRepository<Product> _productRepository;

        public PowerReviewsPlugin(
            IGenericAttributeService genericAttributeService,
            IProductService productService,
            ISettingService settingService,
            ILocalizationService localizationService,
            IWebHelper webHelper,
            IRepository<GenericAttribute> genericAttributeRepository,
            IRepository<Product> productRepository
        )
        {
            _genericAttributeService = genericAttributeService;
            _productService = productService;
            _settingService = settingService;
            _localizationService = localizationService;
            _webHelper = webHelper;
            _genericAttributeRepository = genericAttributeRepository;
            _productRepository = productRepository;
        }

        public bool HideInWidgetList => false;

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/PowerReviews/Configure";
        }

        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "WidgetsPowerReviews";
        }

        public System.Threading.Tasks.Task<IList<string>> GetWidgetZonesAsync()
        {
            // So this will get interesting, I think we'll need to read from a database
            // kind of like what HTML widgets does.
            return Task.FromResult<IList<string>>(new List<string>
            {
                // //PublicWidgetZones.ProductBoxAddinfoBefore,
                // CustomPublicWidgetZones.ProductBoxAddinfoReviews,
                // //PublicWidgetZones.ProductDetailsOverviewTop,
                // CustomPublicWidgetZones.ProductDetailsReviews,
                // CustomPublicWidgetZones.ProductDetailsReviewsTab,
                // CustomPublicWidgetZones.ProductDetailsReviewsTabContent,
                PublicWidgetZones.ProductBoxAddinfoBefore,
                PublicWidgetZones.ProductDetailsOverviewTop,

                // standard - used for scripts
                PublicWidgetZones.CategoryDetailsBottom,
                PublicWidgetZones.ManufacturerDetailsBottom,
                PublicWidgetZones.ProductDetailsBottom
            });
        }

        public override async Task InstallAsync()
        {
            await AddLocalesAsync();
            await _settingService.SaveSettingAsync(PowerReviewsSettings.DefaultValues());
            await DisallowNOPProductReviewsAsync();

            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await _settingService.DeleteSettingAsync<PowerReviewsSettings>();
            await _localizationService.DeleteLocaleResourcesAsync(PowerReviewsLocales.Base);

            await EnableNOPProductReviewsAsync();

            await base.UninstallAsync();
        }

        public override async Task UpdateAsync(string currentVersion, string targetVersion)
        {
            await AddLocalesAsync();

            await base.UpdateAsync(currentVersion, targetVersion);
        }

        private async Task AddLocalesAsync()
        {
            await _localizationService.AddLocaleResourceAsync(
                new Dictionary<string, string>
                {
                    [PowerReviewsLocales.APIKey] = "API Key",
                    [PowerReviewsLocales.MerchantGroupId] = "Merchant Group ID",
                    [PowerReviewsLocales.MerchantId] = "Merchant ID",
                    [PowerReviewsLocales.CustomStyles] = "Custom Styles"
                }
            );
        }

        private async Task DisallowNOPProductReviewsAsync()
        {
            var productsWithReviewsActivated = await _productRepository.GetAllAsync(query =>
                {
                    return from p in query
                        where p.AllowCustomerReviews
                        select p;
                },
                null,
                false
            );

            foreach (var product in productsWithReviewsActivated)
            {
                await _genericAttributeService.SaveAttributeAsync<bool>(product, HasNopReviewsKey, true);
                product.AllowCustomerReviews = false;
                await _productService.UpdateProductAsync(product);
            }
        }

        private async Task EnableNOPProductReviewsAsync()
        {
            var genericAttributes = await _genericAttributeRepository.GetAllAsync(query =>
                {
                    return from ga in query
                        where ga.KeyGroup == "Product" && ga.Key == HasNopReviewsKey
                        select ga;
                },
                null,
                false
            );

            foreach (var genericAttribute in genericAttributes)
            {
                var product = await _productService.GetProductByIdAsync(genericAttribute.EntityId);
                
                product.AllowCustomerReviews = true;
                await _productService.UpdateProductAsync(product);

                await _genericAttributeService.DeleteAttributeAsync(genericAttribute);
            }
        }
    }
}
