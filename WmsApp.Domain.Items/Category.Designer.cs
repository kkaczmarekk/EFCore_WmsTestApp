
namespace WmsApp.Domain.Items
{
    public partial class Category
    {
        public static class CONST
        {
            public const int NAME_LEN = 50;
            public const string BRANCH_SEPARATOR = " | ";
            public const string OWNED_SEPARATOR = ", ";
        }

        public int Id { get; private set; }
        [MaxLength(CONST.NAME_LEN)]
        [Required]
        public string Name { get; private set; }
        [MaxLength(4000)]
        public string CategoriesOwnedString { get; private set; }
        [MaxLength(4000)]
        public string CategoryBranchString { get; private set; }

        public Category? ParentCategory { get; private set; }


        //public CategoryCashed Cashed { get; private set; }

        private HashSet<CategoryOwner> _ownersLinks;
        private HashSet<Item> _items;
        private HashSet<Category> _categoriesOwned;

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
