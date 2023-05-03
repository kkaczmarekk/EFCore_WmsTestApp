using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WmsApp.Domain.Common.StatusValidator.Extensions;

namespace WmsApp.Domain.Items
{
    public partial class ItemCashed
    {
        internal ItemCashed() 
        {
            ReviewsAverageVote = 0;
            ReviewsCount = 0;
        }

        public IStatusValidator UpdateReviewsData(double reviewsAverageVote, int reviewsCount)
        {
            var status = new StatusValidatorHandler();

            status.PropertyAttributeCheckByRef<ItemCashed>(reviewsAverageVote, ReviewsAverageVote);
            status.PropertyAttributeCheckByRef<ItemCashed>(reviewsCount, ReviewsCount);

            if (!status.IsValid) return status;

            ReviewsAverageVote = reviewsAverageVote;
            ReviewsCount = reviewsCount;

            return status;
        }
    }
}
