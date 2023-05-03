using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WmsApp.Domain.Common.Enities.Events;

namespace WmsApp.Domain.Items.DomainEvents
{
    public class ItemReviewAddEvent : IEntityEvent
    {
        public ItemReviewAddEvent(int votedStars)
        {
            VotedStars = votedStars;
        }

        public int VotedStars { get; }
    }
}
