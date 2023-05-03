namespace WmsApp.Domain.Items
{
    public partial class OwnerType
    {
        public static class CONST
        {
            public const int NAME_LEN = 50;
        }

        public int Id { get; private set; }
        [MaxLength(CONST.NAME_LEN)]
        [Required]
        public string Name { get; private set; }
    }
}
