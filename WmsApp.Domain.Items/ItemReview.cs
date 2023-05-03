using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WmsApp.Domain.Items
{
    public partial class ItemReview
    {
        private ItemReview() { }

        internal static IStatusValidator<ItemReview> CreateReview(
            string voterName,
            int voteStars,
            string comment,
            DateTime voteDate)
        {
            var status = new StatusValidatorHandler<ItemReview>();

            status.PropertyAttributeCheckByName<ItemReview>(voterName, nameof(VoterName));
            status.PropertyAttributeCheckByName<ItemReview>(voteStars, nameof(VoteStars));
            status.PropertyAttributeCheckByName<ItemReview>(comment, nameof(Comment));

            if (!status.IsValid) return status;


            var review = new ItemReview()
            {
                VoterName = voterName,
                VoteStars = voteStars,
                VoteDate = voteDate,
                Comment = comment
            };

            return status.SetResult(review);
        }
    }
}
