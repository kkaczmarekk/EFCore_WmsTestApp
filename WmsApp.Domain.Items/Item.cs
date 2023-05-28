using System.Xml.Linq;
using WmsApp.Domain.Common.Enities.Events;
using WmsApp.Domain.Items.DomainEvents;

namespace WmsApp.Domain.Items
{
    public partial class Item : EntityWithEvents, IFullAuditModel
    {
        public IReadOnlyCollection<ItemReview> Reviews => _reviews?.ToList();

        private Item() { }

        public static IStatusValidator<Item> CreateItem(
            string name,
            string description,
            Category category,
            string userName,
            IActualDateTime timeGenerator)
        {
            var status = new StatusValidatorHandler<Item>();

            status.StringNullOrWhiteSpaceCheck(name, nameof(Name));
            status.PropertyAttributeCheckByName<Item>(name, nameof(Name));

            status.PropertyAttributeCheckByName<Item>(description, nameof(Description));

            status.ObjectNullCheck(category, nameof(Category));

            status.StringNullOrWhiteSpaceCheck(userName, nameof(CreatedByUserName));
            status.PropertyAttributeCheckByName<Item>(userName, nameof(CreatedByUserName));

            if (!status.IsValid) return status.SetResult(null);


            var item = new Item()
            {
                Name = name,
                Description = description,
                Category = category,
                CreatedByUserName = userName,
                CreatedDateUtc = timeGenerator.GetActualDateTime()
            };

            item._reviews = new HashSet<ItemReview>();

            item.Cashed = new ItemCashed();
            
            return status.SetResult(item);
        }


        public IStatusValidator SoftDelete_Update(
            bool softDeleted,
            string userName,
            IActualDateTime timeGenerator)
        {
            var status = new StatusValidatorHandler();

            status.StringNullOrWhiteSpaceCheck(userName, nameof(DeletedByUserName));
            status.PropertyAttributeCheckByRef<Item>(userName, DeletedByUserName);

            if (!status.IsValid) return status;


            if (!IsDeleted && softDeleted)
            {
                DeletedDateUtc = timeGenerator.GetActualDateTime();
                DeletedByUserName = userName;
            }

            IsDeleted = softDeleted;

            return status;
        }

        public IStatusValidator Name_Update(
            string name,
            string userName,
            IActualDateTime timeGenerator)
        {
            var status = new StatusValidatorHandler();

            status.StringNullOrWhiteSpaceCheck(name, nameof(Name));
            status.PropertyAttributeCheckByRef<Item>(name, Name);

            status.StringNullOrWhiteSpaceCheck(userName, nameof(UpdatedByUserName));
            status.PropertyAttributeCheckByRef<Item>(userName, UpdatedByUserName);

            if (!status.IsValid) return status;
            

            Name = name;
            SetUpdateValues(userName, timeGenerator);

            return status;
        }

        public IStatusValidator Description_Update(
            string description,
            string userName,
            IActualDateTime timeGenerator)
        {
            var status = new StatusValidatorHandler();

            status.PropertyAttributeCheckByRef<Item>(description, Description);

            status.StringNullOrWhiteSpaceCheck(userName, nameof(UpdatedByUserName));
            status.PropertyAttributeCheckByRef<Item>(userName, UpdatedByUserName);

            if (!status.IsValid) return status;


            Description = description;
            SetUpdateValues(userName, timeGenerator);

            return status;
        }

        public IStatusValidator Category_Update(
            Category category,
            string userName,
            IActualDateTime timeGenerator)
        {
            var status = new StatusValidatorHandler();

            status.ObjectNullCheck(category, nameof(category));

            status.StringNullOrWhiteSpaceCheck(userName, nameof(UpdatedByUserName));
            status.PropertyAttributeCheckByRef<Item>(userName, UpdatedByUserName);

            if (!status.IsValid) return status;


            Category = category;
            SetUpdateValues(userName, timeGenerator);

            return status;
        }

        public IStatusValidator Reviews_Add(
            string voterName,
            int voteStars,
            string comment,
            IActualDateTime timeGenerator)
        {
            var collectionStatus = new StatusValidatorHandler();
            collectionStatus.CollectionLoadCheck(_reviews, nameof(Reviews));

            var status = ItemReview.CreateReview(voterName, voteStars, comment, timeGenerator.GetActualDateTime());
            status.CombineStatues(collectionStatus);
            
            if (!status.IsValid) return collectionStatus;

            _reviews.Add(status.Result);
            AddEvent(EventRunType.Before, EventRunScope.Transient, new ItemReviewAddEvent(voteStars));

            return collectionStatus;
        }

        public IStatusValidator Reviews_Remove(int reviewId)
        {
            var status = new StatusValidatorHandler();
            status.CollectionLoadCheck(_reviews, nameof(Reviews));

            var searchQuery = _reviews.Where(r => r.Id == reviewId);

            if (status.IsValid && !searchQuery.Any())
                status.AddError($"The collection does not contain review of id: {reviewId}.");

            if(!status.IsValid) return status;

            var reviewToDel = searchQuery.FirstOrDefault();

            AddEvent(EventRunType.Before, EventRunScope.Transient, new ItemReviewRemoveEvent(reviewToDel.VoteStars));

            _reviews.Remove(reviewToDel);

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