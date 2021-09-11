using Nop.Services.Logging;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;
using System;
using Nop.Plugin.Widgets.PowerReviews.Models;
using System.Linq;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Web.Models.Catalog;
using Nop.Core;
using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.PowerReviews.Components
{
    public class WidgetsPowerReviewsViewComponent : NopViewComponent
    {
        private readonly ILogger _logger;
        private readonly IWebHelper _webHelper;
        private readonly PowerReviewsSettings _settings;

        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public WidgetsPowerReviewsViewComponent(
            ILogger logger,
            IWebHelper webHelper,
            PowerReviewsSettings settings,
            IGenericAttributeService genericAttributeService,
            IManufacturerService manufacturerService,
            IProductService productService,
            ICategoryService categoryService
        )
        {
            _logger = logger;
            _webHelper = webHelper;
            _settings = settings;
            _genericAttributeService = genericAttributeService;
            _manufacturerService = manufacturerService;
            _productService = productService;
            _categoryService = categoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData = null)
        {
            if (!_settings.IsValid())
            {
                await _logger.ErrorAsync("PowerReviews settings are required to have " +
                              "reviews display for products, add the correct " +
                              "settings in configuration.");
                return Content("");
            }

            if (widgetZone == _settings.ProductListingWidgetZone)
            {
                return await Listing(additionalData as ProductOverviewModel);
            }
            if (widgetZone == _settings.ProductDetailWidgetZone)
            {
                return View("~/Plugins/Widgets.PowerReviews/Views/Detail.cshtml");
            }
            if (widgetZone == _settings.ProductDetailReviewsWidgetZone)
            {
                return View("~/Plugins/Widgets.PowerReviews/Views/DetailReviews.cshtml");
            }

            // Scripts
            if (widgetZone == PublicWidgetZones.CategoryDetailsBottom ||
                widgetZone == PublicWidgetZones.ManufacturerDetailsBottom)
            {
                return View("~/Plugins/Widgets.PowerReviews/Views/ListingScript.cshtml", _settings);
            }
            if (widgetZone == PublicWidgetZones.ProductDetailsBottom)
            {
                return await Detail(additionalData as ProductDetailsModel);
            }

            await _logger.ErrorAsync($"Widgets.PowerReviews: No view provided for widget zone {widgetZone}");
            return Content("");
        }

        private async Task<IViewComponentResult> Listing(ProductOverviewModel productOverviewModel)
        {
            var model = new ListingModel()
            {
                ProductId = productOverviewModel.Id,
                ProductSku = await GetPowerReviewsSkuAsync(productOverviewModel.Id, productOverviewModel.Sku)
            };

            return View(
                "~/Plugins/Widgets.PowerReviews/Views/Listing.cshtml",
                model
            );
        }

        private async Task<IViewComponentResult> Detail(ProductDetailsModel productDetailsModel)
        {
            var productId = productDetailsModel.Breadcrumb.ProductId;
            var product = await _productService.GetProductByIdAsync(productId);
            var priceEndDate = DateTime.Now;

            var feedlessModel = await GetFeedlessProductAsync(
                product,
                productDetailsModel.DefaultPictureModel.ImageUrl
            );

            var model = new DetailModel()
            {
                ProductSku = await GetPowerReviewsSkuAsync(productId, productDetailsModel.Sku),
                ProductId = productId,
                ProductPrice = productDetailsModel.ProductPrice.PriceValue.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture),
                PriceValidUntil = priceEndDate,
                Settings = _settings,
                FeedlessProduct = feedlessModel
            };

            return View(
                "~/Plugins/Widgets.PowerReviews/Views/DetailScript.cshtml",
                model
            );
        }

        private async Task<FeedlessProductModel> GetFeedlessProductAsync(
            Product product,
            string imageUrl
        ) {
            var productCategory = (await _categoryService.GetProductCategoriesByProductIdAsync(product.Id, true)).FirstOrDefault();
            var category = productCategory != null ?
                await _categoryService.GetCategoryByIdAsync(productCategory.CategoryId) :
                null;
            var productManufacturer = (await _manufacturerService.GetProductManufacturersByProductIdAsync(product.Id, true)).FirstOrDefault();
            var manufacturer = productManufacturer != null ?
                await _manufacturerService.GetManufacturerByIdAsync(productManufacturer.ManufacturerId) :
                null;

            var price = product.Price.ToString();
            var description = await _genericAttributeService.GetAttributeAsync<string>(product, "PowerReviewsDescription", 0, product.ShortDescription);

            return new FeedlessProductModel()
            {
                Name = product.Name,
                Url = _webHelper.GetThisPageUrl(true),
                ImageUrl = imageUrl,
                Description = description,
                CategoryName = category.Name,
                ManufacturerId = manufacturer != null ? manufacturer.Id : 0,
                Upc = product.Gtin,
                BrandName = manufacturer?.Name,
                InStock = !product.DisableBuyButton,
                Price = price
            };
        }

        // PowerReviews requires a SKU with only letters, numbers
        private async Task<string> GetPowerReviewsSkuAsync(int productId, string defaultSku)
        {
            var sku = await _genericAttributeService.GetAttributeAsync<Product, string>(productId, "PowerReviewsSku", 0, defaultSku);
            if (string.IsNullOrWhiteSpace(sku)) return "";

            char[] conversionString = sku.ToCharArray();
            conversionString = Array.FindAll<char>(conversionString, (c => (char.IsLetterOrDigit(c)
                                    || c == '-')));
            return new string(conversionString);
        }
    }
}
