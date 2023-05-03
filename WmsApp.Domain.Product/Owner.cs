
using System.Xml.Linq;

namespace WmsApp.Domain.Items
{
    public partial class Owner : IFullAuditModel
    {
        public IReadOnlyCollection<CategoryOwner> CategoriesOwnedLinks => _categoriesOwnedLinks?.ToList();

        private Owner() { }

        public static IStatusValidator<Owner> CreateOwner(
            string firstName,
            string lastName,
            string email,
            string userName,
            IActualDateTime timeGenerator)
        {
            var status = new StatusValidatorHandler<Owner>();

            status.StringNullOrWhiteSpaceCheck(firstName, nameof(FirstName));
            status.PropertyAttributeCheckByName<Owner>(firstName, nameof(FirstName));

            status.StringNullOrWhiteSpaceCheck(lastName, nameof(LastName));
            status.PropertyAttributeCheckByName<Owner>(lastName, nameof(LastName));

            status.StringNullOrWhiteSpaceCheck(email, nameof(Email));
            status.PropertyAttributeCheckByName<Owner>(email, nameof(Email));

            status.StringNullOrWhiteSpaceCheck(userName, nameof(CreatedByUserName));
            status.PropertyAttributeCheckByName<Owner>(userName, nameof(CreatedByUserName));

            if (!status.IsValid) return status.SetResult(null);
            

            var owner = new Owner()
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                CreatedByUserName = userName,
                CreatedDateUtc = timeGenerator.GetActualDateTime(),
                _categoriesOwnedLinks = new HashSet<CategoryOwner>()
            };

            return status.SetResult(owner);
        }

        public IStatusValidator SoftDelete_Update(
            bool softDeleted,
            string userName,
            IActualDateTime timeGenerator)
        {
            var status = new StatusValidatorHandler();

            status.StringNullOrWhiteSpaceCheck(userName, nameof(DeletedByUserName));
            status.PropertyAttributeCheckByRef<Owner>(userName, DeletedByUserName);

            if (!status.IsValid) return status;


            if (!IsDeleted && softDeleted)
            {
                DeletedDateUtc = timeGenerator.GetActualDateTime();
                DeletedByUserName = userName;
            }

            IsDeleted = softDeleted;

            return status;
        }

        public IStatusValidator FirstName_Update(
            string firstName,
            string userName,
            IActualDateTime timeGenerator)
        {
            var status = new StatusValidatorHandler();

            status.StringNullOrWhiteSpaceCheck(firstName, nameof(FirstName));
            status.PropertyAttributeCheckByName<Owner>(firstName, nameof(FirstName));

            status.StringNullOrWhiteSpaceCheck(userName, nameof(UpdatedByUserName));
            status.PropertyAttributeCheckByName<Owner>(userName, nameof(UpdatedByUserName));

            if (!status.IsValid) return status;


            FirstName = firstName;
            SetUpdateValues(userName, timeGenerator);

            return status;
        }

        public IStatusValidator LastName_Update(
            string lastName,
            string userName,
            IActualDateTime timeGenerator)
        {
            var status = new StatusValidatorHandler();

            status.StringNullOrWhiteSpaceCheck(lastName, nameof(LastName));
            status.PropertyAttributeCheckByName<Owner>(lastName, nameof(LastName));

            status.StringNullOrWhiteSpaceCheck(userName, nameof(UpdatedByUserName));
            status.PropertyAttributeCheckByName<Owner>(userName, nameof(UpdatedByUserName));

            if (!status.IsValid) return status;


            LastName = lastName;
            SetUpdateValues(userName, timeGenerator);

            return status;
        }

        public IStatusValidator Email_Update(
            string emial,
            string userName,
            IActualDateTime timeGenerator)
        {
            var status = new StatusValidatorHandler();

            status.StringNullOrWhiteSpaceCheck(emial, nameof(Email));
            status.PropertyAttributeCheckByName<Owner>(emial, nameof(Email));

            status.StringNullOrWhiteSpaceCheck(userName, nameof(UpdatedByUserName));
            status.PropertyAttributeCheckByName<Owner>(userName, nameof(UpdatedByUserName));

            if (!status.IsValid) return status;


            Email = emial;
            SetUpdateValues(userName, timeGenerator);

            return status;
        }

        public IStatusValidator OwnedCategories_Add(
            Category category,
            OwnerType ownedType)
        {
            var status = new StatusValidatorHandler();

            status.ObjectNullCheck(category, nameof(category));
            status.ObjectNullCheck(ownedType, nameof(ownedType));

            status.CollectionLoadCheck(_categoriesOwnedLinks, nameof(CategoriesOwnedLinks));

            var searchQuery = _categoriesOwnedLinks
                .Where(co => co.Category == category);
            if (_categoriesOwnedLinks != null && searchQuery.Any())
                status.AddError($"The \"{this.ToString()}\" is already owner of \"{category.ToString()}\" category.");

            if (!status.IsValid) return status;


            _categoriesOwnedLinks.Add(new CategoryOwner(category, this, ownedType));

            return status;
        }

        public IStatusValidator OwnedCategories_Update(
            Category category,
            OwnerType ownedType)
        {
            var status = new StatusValidatorHandler();

            status.ObjectNullCheck(category, nameof(category));
            status.ObjectNullCheck(ownedType, nameof(ownedType));

            status.CollectionLoadCheck(_categoriesOwnedLinks, nameof(CategoriesOwnedLinks));

            var searchQuery = _categoriesOwnedLinks
                .Where(co => co.Category == category);
            if (_categoriesOwnedLinks != null && category != null && !searchQuery.Any())
                status.AddError($"The \"{this.ToString()}\" is not the owner of \"{category.ToString()}\" category.");

            if(!status.IsValid) return status;


            searchQuery.FirstOrDefault()?.Type_Update(ownedType);

            return status;
        }

        public IStatusValidator OwnedCategories_Delete(
            Category category)
        {
            var status = new StatusValidatorHandler();

            status.ObjectNullCheck(category, nameof(category));

            status.CollectionLoadCheck(_categoriesOwnedLinks, nameof(CategoriesOwnedLinks));

            var searchQuery = _categoriesOwnedLinks
                .Where(co => co.Category == category);
            if (_categoriesOwnedLinks != null && category != null && !searchQuery.Any())
                status.AddError($"The \"{this.ToString()}\" is not the owner of \"{category.ToString()}\" category.");

            if (!status.IsValid) return status;


            _categoriesOwnedLinks?.Remove(searchQuery.FirstOrDefault());

            return status;
        }

        public override string ToString()
        {
            return FirstName + " " + LastName;
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
