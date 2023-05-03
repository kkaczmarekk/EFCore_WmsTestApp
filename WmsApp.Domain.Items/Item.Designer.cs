
namespace WmsApp.Domain.Items
{
    public partial class Item
    {
        public static class CONST
        {
            public const int NAME_LEN = 50;
            public const int DESCRIPTION_LEN = 200;
        }


        public int Id { get; private set; }

        [MaxLength(CONST.NAME_LEN)]
        [Required]
        public string Name { get; private set; }

        [MaxLength(CONST.DESCRIPTION_LEN)]
        public string Description { get; private set; }



        public Category Category { get; private set; }
        public ItemCashed Cashed { get; private set; }
        private HashSet<ItemReview> _reviews;


        public bool IsDeleted { get; private set; }

        [MaxLength(Common.Enities.CONST.USERNAME_LEN)]
        public string DeletedByUserName { get; private set; }

        public DateTime? DeletedDateUtc { get; private set; }

        [MaxLength(Common.Enities.CONST.USERNAME_LEN)]
        public string UpdatedByUserName { get; private set; }

        public DateTime? UpdatedDateUtc { get; private set; }

        [MaxLength(Common.Enities.CONST.USERNAME_LEN)]
        [Required]
        public string CreatedByUserName { get; private set; }

        public DateTime CreatedDateUtc { get; private set; }
    }
}
