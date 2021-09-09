using Nop.Core.Configuration;
using Nop.Plugin.Widgets.PowerReviews.Models;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.PowerReviews
{
    public class PowerReviewsSettings : ISettings
    {
        // settings
        public string APIKey { get; private set; }
        public string MerchantGroupId { get; private set; }
        public string MerchantId { get; private set; }

        // advanced
        public string CustomStyles { get; private set; }
        public string OnReadReviewsClickCode { get; private set; }
        public string ProductListingWidgetZone { get; private set; }
        public string ProductDetailWidgetZone { get; private set; }
        public string ProductDetailReviewsWidgetZone { get; private set; }

        public static PowerReviewsSettings DefaultValues()
        {
            return new PowerReviewsSettings()
            {
                CustomStyles = @"/* cleans category snippet for DefaultClean */
                                 .p-w-r .pr-category-snippet {
                                   margin-left: 0;
                                   margin-bottom: 0.5rem;
                                 }
                                 
                                 /* hides review count for category snippets */
                                 .p-w-r .pr-category-snippet__total {
                                   display: none;
                                 }
                                 
                                 /* centers review snippet */
                                 .p-w-r .pr-review-snippet-container {
                                   text-align: center;
                                   margin-bottom: 1rem;
                                 }
                                 
                                 /* removes 'recommended to friends' message from review snippet */
                                 .p-w-r .pr-snippet-stars-reco-reco {
                                    display: none;   
                                 }
                                 
                                 @media(min-width: 1001px) {
                                   .p-w-r .pr-review-snippet-container {
                                     text-align: left;
                                   }
                                 }",
                OnReadReviewsClickCode = "document.getElementById('pr-reviewdisplay').scrollIntoView({block: 'start', behavior: 'smooth'});",
                ProductListingWidgetZone = PublicWidgetZones.ProductBoxAddinfoBefore,
                ProductDetailWidgetZone = PublicWidgetZones.ProductDetailsOverviewTop,
                ProductDetailReviewsWidgetZone = PublicWidgetZones.ProductDetailsBeforeCollateral,
            };
        }

        public static PowerReviewsSettings FromModel(PowerReviewsConfigModel model)
        {
            return new PowerReviewsSettings()
            {
                APIKey = model.APIKey,
                MerchantGroupId = model.MerchantGroupId,
                MerchantId = model.MerchantId,
                CustomStyles = model.CustomStyles,
                OnReadReviewsClickCode = model.OnReadReviewsClickCode,
                ProductListingWidgetZone = model.ProductListingWidgetZone,
                ProductDetailWidgetZone = model.ProductDetailWidgetZone,
                ProductDetailReviewsWidgetZone = model.ProductDetailReviewsWidgetZone
            };
        }

        public PowerReviewsConfigModel ToModel()
        {
            return new PowerReviewsConfigModel
            {
                APIKey = APIKey,
                MerchantGroupId = MerchantGroupId,
                MerchantId = MerchantId,
                CustomStyles = CustomStyles,
                OnReadReviewsClickCode = OnReadReviewsClickCode,
                ProductListingWidgetZone = ProductListingWidgetZone,
                ProductDetailWidgetZone = ProductDetailWidgetZone,
                ProductDetailReviewsWidgetZone = ProductDetailReviewsWidgetZone
            };
        }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(APIKey) &&
                   !string.IsNullOrWhiteSpace(MerchantGroupId) &&
                   !string.IsNullOrWhiteSpace(MerchantId);
        }
    }
}