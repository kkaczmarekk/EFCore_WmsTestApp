using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WmsApp.Domain.Items
{
    public partial class ItemCashed
    {
        [Key]
        public int ItemId { get; private set; }

        [ConcurrencyCheck]
        [Range(ItemReview.CONST.VOTE_STARS_MIN, ItemReview.CONST.VOTE_STARS_MAX)]
        public double ReviewsAverageVote { get; private set; }

        [ConcurrencyCheck]
        [Range(0, Int32.MaxValue)]
        public int ReviewsCount { get; private set; }
    }
}
