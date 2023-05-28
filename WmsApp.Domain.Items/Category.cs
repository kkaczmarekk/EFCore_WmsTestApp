using System.Text;
using System.Xml.Linq;
using WmsApp.Domain.Common.Enities.Events;
using WmsApp.Domain.Items.DomainEvents;
using WmsApp.Domain.Items.SupportedTypes;

namespace WmsApp.Domain.Items
{
    public partial class Category : EntityWithEvents, IFullAuditModel
    {
        public IReadOnlyCollection<CategoryOwner> OwnersLinks => _ownersLinks?.ToList();
        public IReadOnlyCollection<Item> Items => _items?.ToList();
        public IReadOnlyCollection<Category> CategoriesOwned => _categoriesOwned?.ToList();

        private Category() { }

        public static IStatusValidator<Category> CreateCategory(
            string name,
            Category? parentCategory,
            ICollection<OwnerWithTypeDto>? ownersWithType,
            IActualDateTime timeGenerator,
            string userName,
            ICollection<Category>? categoriesOwned = null)
        {
            var status = new StatusValidatorHandler<Category>();

            status.StringNullOrWhiteSpaceCheck(name, nameof(Name));
            status.PropertyAttributeCheckByName<Category>(name, nameof(Name));

            if(parentCategory != null)
                status.CollectionLoadCheck(parentCategory.CategoriesOwned, "Parent.CategoriesOwned");

            status.StringNullOrWhiteSpaceCheck(userName, nameof(CreatedByUserName));
            status.PropertyAttributeCheckByName<Category>(userName, nameof(CreatedByUserName));

            if (!status.IsValid) return status;


            var category = new Category()
            {
                Name = name,
                ParentCategory = parentCategory,
                _items = new HashSet<Item>(),
                _categoriesOwned = categoriesOwned is null ?  new HashSet<Category>() : new HashSet<Category>(categoriesOwned),
                CreatedDateUtc = timeGenerator.GetActualDateTime(),
                CreatedByUserName = userName
            };

            foreach (var cat in category._categoriesOwned)
                cat.ParentCategory = category;

            category._ownersLinks = ownersWithType is null 
                ? new HashSet<CategoryOwner>()
                : new HashSet<CategoryOwner>(
                    ownersWithType.Select(a =>
                        new CategoryOwner(category, a.Owner, a.Type)));

            parentCategory?._categoriesOwned.Add(category);

            category.AddEvent(EventRunType.After, EventRunScope.Context, new CategoryBranchStringUpdateEvent());
            category.AddEvent(EventRunType.After, EventRunScope.Context, new CategoryOwnedStringUpdateEvent());


            return status.SetResult(category);
        }

        public IStatusValidator SoftDelete_Update(
            bool softDeleted, 
            string userName,
            IActualDateTime timeGenerator)
        {
            var status = new StatusValidatorHandler();

            status.StringNullOrWhiteSpaceCheck(userName, nameof(DeletedByUserName));
            status.PropertyAttributeCheckByRef<Category>(userName, DeletedByUserName);
            status.CollectionLoadCheck(_categoriesOwned, nameof(CategoriesOwned));

            if (ParentCategory != null)
                status.CollectionLoadCheck(ParentCategory.CategoriesOwned, "Parent.CategoriesOwned");

            if (_categoriesOwned != null 
                && _categoriesOwned.Count > 0
                && softDeleted)
            {
                status.AddError("Category cannot contains other categories before deletion.");
            }

            if (!status.IsValid) return status;


            if (!IsDeleted && softDeleted)
            {
                DeletedDateUtc = timeGenerator.GetActualDateTime();
                DeletedByUserName = userName;
            }

            if(IsDeleted != softDeleted)
                AddEvent(EventRunType.Before, EventRunScope.Context, new CategoryOwnedStringUpdateEvent());

            IsDeleted = softDeleted;

            return status;
        }

        public IStatusValidator Name_Update(
            string newName,
            string userName,
            IActualDateTime timeGenerator)
        {
            var status = new StatusValidatorHandler();

            status.StringNullOrWhiteSpaceCheck(newName, nameof(Name));
            status.PropertyAttributeCheckByRef<Category>(newName, Name);

            status.StringNullOrWhiteSpaceCheck(userName, nameof(UpdatedByUserName));
            status.PropertyAttributeCheckByRef<Category>(userName, UpdatedByUserName);

            if (!status.IsValid) return status;

            AddEvent(EventRunType.Before, EventRunScope.Context, new CategoryBranchStringUpdateEvent());
            Name = newName;
            SetUpdateValues(userName, timeGenerator);

            return status;
        }

        public IStatusValidator ParentCategory_Update(
            Category newParentCategory,
            string userName,
            IActualDateTime timeGenerator)
        {
            var status = new StatusValidatorHandler();

            status.StringNullOrWhiteSpaceCheck(userName, nameof(userName));
            status.PropertyAttributeCheckByRef<Category>(userName, UpdatedByUserName);

            if (!status.IsValid) return status;

            AddEvent(EventRunType.After, EventRunScope.Context, new CategoryBranchStringUpdateEvent());
            AddEvent(EventRunType.After, EventRunScope.Context, new CategoryOwnedStringUpdateEvent());

            ParentCategory = newParentCategory;
            SetUpdateValues(userName, timeGenerator);

            return status;
        }

        public IStatusValidator Owners_AddOwner(
            Owner owner,
            OwnerType ownerType,
            string userName,
            IActualDateTime timeGenerator)
        {
            var status = new StatusValidatorHandler();

            status.ObjectNullCheck(owner, nameof(owner));
            status.ObjectNullCheck(ownerType, nameof(ownerType));

            status.StringNullOrWhiteSpaceCheck(userName, nameof(userName));
            status.PropertyAttributeCheckByRef<Category>(userName, UpdatedByUserName);

            status.CollectionLoadCheck(_ownersLinks, nameof(OwnersLinks));

            if (_ownersLinks != null && owner != null && _ownersLinks.Where(ol => ol.Owner == owner).Any())
                status.AddError($"The collection already contains item: {owner?.ToString()}.");

            if(!status.IsValid) return status;


            _ownersLinks.Add(new CategoryOwner(this, owner, ownerType));
            SetUpdateValues(userName, timeGenerator);
            
            return status;
        }

        public IStatusValidator Owners_RemoveOwner(
            Owner owner,
            string userName,
            IActualDateTime timeGenerator)
        {
            var status = new StatusValidatorHandler();

            status.ObjectNullCheck(owner, nameof(owner));

            status.StringNullOrWhiteSpaceCheck(userName, nameof(userName));
            status.PropertyAttributeCheckByRef<Category>(userName, UpdatedByUserName);

            status.CollectionLoadCheck(_ownersLinks, nameof(OwnersLinks));

            var searchQuery = _ownersLinks
                .Where(ol => ol.Owner == owner);

            if (_ownersLinks != null && owner != null && !searchQuery.Any())
                status.AddError($"The collection does not contain item: {owner?.ToString()}.");

            if (!status.IsValid) return status;


            _ownersLinks?.Remove(searchQuery.FirstOrDefault());
            SetUpdateValues(userName, timeGenerator);

            return status;
        }


        private void SetUpdateValues(
            string userName,
            IActualDateTime timeGenerator)
        {
            UpdatedDateUtc = timeGenerator.GetActualDateTime();
            UpdatedByUserName = userName;
        }
    }
}
