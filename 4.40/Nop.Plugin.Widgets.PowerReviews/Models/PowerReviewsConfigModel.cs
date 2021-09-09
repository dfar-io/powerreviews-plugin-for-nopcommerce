using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.PowerReviews.Models
{
    public class PowerReviewsConfigModel
    {
        [NopResourceDisplayName(PowerReviewsLocales.APIKey)]
        public string APIKey { get; set; }

        [NopResourceDisplayName(PowerReviewsLocales.MerchantGroupId)]
        public string MerchantGroupId { get; set; }

        [NopResourceDisplayName(PowerReviewsLocales.MerchantId)]
        public string MerchantId { get; set; }

        [NopResourceDisplayName(PowerReviewsLocales.CustomStyles)]
        public string CustomStyles { get; set; }

        [NopResourceDisplayName(PowerReviewsLocales.OnReadReviewsClickCode)]
        public string OnReadReviewsClickCode { get; set; }

        [NopResourceDisplayName(PowerReviewsLocales.ProductListingWidgetZone)]
        public string ProductListingWidgetZone { get; set; }

        [NopResourceDisplayName(PowerReviewsLocales.ProductDetailWidgetZone)]
        public string ProductDetailWidgetZone { get; set; }

        [NopResourceDisplayName(PowerReviewsLocales.ProductDetailReviewsWidgetZone)]
        public string ProductDetailReviewsWidgetZone { get; set; }
    }
}
