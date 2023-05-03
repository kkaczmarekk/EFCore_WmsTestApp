using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace WmsApp.Domain.Items
{
    public partial class ItemReview
    {
        public static class CONST
        {
            public const int VOTE_STARS_MIN = 1;
            public const int VOTE_STARS_MAX = 5;
            public const int VOTER_NAME_LEN = 50;
            public const int COMMENT_LEN = 400;
        }
        [Key]
        public int Id { get; private set; }

        [Range(CONST.VOTE_STARS_MIN, CONST.VOTE_STARS_MAX)]
        public int VoteStars { get; private set; }

        [MaxLength(CONST.VOTER_NAME_LEN)]
        [Required]
        public string VoterName { get; private set; }

        public DateTime VoteDate { get; private set; }

        [MaxLength(CONST.COMMENT_LEN)]
        public string Comment { get; private set; }

        public int ItemId { get; private set; }
    }
}
