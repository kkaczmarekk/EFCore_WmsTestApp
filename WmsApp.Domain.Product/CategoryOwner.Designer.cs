using System.ComponentModel.DataAnnotations.Schema;

namespace WmsApp.Domain.Items
{
    [Table("CategoriesOwners")]
    public partial class CategoryOwner
    {
        public int CategoryId { get; private set; }
        public int OwnerId { get; private set; }

        public Category Category { get; private set; }
        public Owner Owner { get; private set; }
        public OwnerType Type { get; private set; }
    }
}
