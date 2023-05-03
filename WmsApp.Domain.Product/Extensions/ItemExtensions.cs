using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WmsApp.Domain.Common.Enities.Events;
using WmsApp.Domain.Items.DomainEvents;

namespace WmsApp.Domain.Items.Extensions
{
    public static class ItemExtensions
    {
        public static IStatusValidator CashedReviewsDataRecalculate_Add(
            this ItemCashed cashed,
            int newVotedStars)
        {
            var totalStars = Math.Round(cashed.ReviewsAverageVote
                    * cashed.ReviewsCount)
                + newVotedStars;

            var newReviewsCount = cashed.ReviewsCount + 1;
            var newReviewsAverageVote = totalStars / newReviewsCount;

            return cashed.UpdateReviewsData(newReviewsAverageVote, newReviewsCount);
        }

        public static IStatusValidator CashedReviewsDataRecalculate_Remove(
            this ItemCashed cashed,
            int dellVotedStars)
        {
            var totalStars = Math.Round(cashed.ReviewsAverageVote
                    * cashed.ReviewsCount)
                - dellVotedStars;

            var newReviewsCount = cashed.ReviewsCount - 1;
            var newReviewsAverageVote = newReviewsCount == 0 
                ? 0
                : totalStars / newReviewsCount;

            return cashed.UpdateReviewsData(newReviewsAverageVote, newReviewsCount);
        }
    }
}
