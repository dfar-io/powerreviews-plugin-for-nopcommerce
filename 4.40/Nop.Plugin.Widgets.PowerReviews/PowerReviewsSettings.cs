using Nop.Core.Configuration;
using Nop.Plugin.Widgets.PowerReviews.Models;

namespace Nop.Plugin.Widgets.PowerReviews
{
    public class PowerReviewsSettings : ISettings
    {
        public string APIKey { get; private set; }
        public string MerchantGroupId { get; private set; }
        public string MerchantId { get; private set; }
        public string CustomStyles { get; private set; }

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
                                 }"
            };
        }

        public static PowerReviewsSettings FromModel(PowerReviewsConfigModel model)
        {
            return new PowerReviewsSettings()
            {
                APIKey = model.APIKey,
                MerchantGroupId = model.MerchantGroupId,
                MerchantId = model.MerchantId,
                CustomStyles = model.CustomStyles
            };
        }

        public PowerReviewsConfigModel ToModel()
        {
            return new PowerReviewsConfigModel
            {
                APIKey = APIKey,
                MerchantGroupId = MerchantGroupId,
                MerchantId = MerchantId,
                CustomStyles = CustomStyles
            };
        }

        public bool IsValid()
        {
            foreach (var property in this.GetType().GetProperties())
            {
                if (string.IsNullOrWhiteSpace((string)property.GetValue(this, null)))
                {
                    return false;
                }
            }

            return true;
        }
    }
}