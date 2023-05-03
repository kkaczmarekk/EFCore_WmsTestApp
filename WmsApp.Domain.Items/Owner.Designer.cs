
namespace WmsApp.Domain.Items
{
    public partial class Owner
    {
        public static class CONST
        {
            public const int FIRST_NAME_LEN = 50;
            public const int LAST_NAME_LEN = 50;
            public const int EMAIL_LEN = 50;
        }


        public int Id { get; private set; }

        [MaxLength(CONST.FIRST_NAME_LEN)]
        [Required]
        public string FirstName { get; private set; }

        [MaxLength(CONST.LAST_NAME_LEN)]
        [Required]
        public string LastName { get; private set; }

        [MaxLength(CONST.EMAIL_LEN)]
        [Required]
        public string Email { get; private set; }

        public HashSet<CategoryOwner> _categoriesOwnedLinks;

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
