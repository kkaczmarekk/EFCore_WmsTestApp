using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WmsApp.Domain.Items.SupportedTypes;
using static WmsApp.Tests.Common.Stubs.CommonStubs;

namespace WmsApp.Tests.Common.Factories.Items
{
    public static class CategoryFactory
    {
        public class FashionCompexCategory
        {
            public FashionCompexCategory()
            {
                SetupCategories();
            }

            private void SetupCategories()
            {
                Fashion = Category.CreateCategory(nameof(Fashion), null, null, CreateStub.ActualDateTimeStub, CreateStub.UserNameStub,
                    new List<Category>()
                    {
                    Category.CreateCategory(nameof(Fashion_Male), null, null, CreateStub.ActualDateTimeStub, CreateStub.UserNameStub,
                        new List<Category>()
                        {
                            Category.CreateCategory(nameof(Fashion_Male_Shoe), null, null, CreateStub.ActualDateTimeStub, CreateStub.UserNameStub).Result
                        }).Result,
                    Category.CreateCategory(nameof(Fashion_Female), null, null, CreateStub.ActualDateTimeStub, CreateStub.UserNameStub,
                        new List<Category>()
                        {
                            Category.CreateCategory(nameof(Fashion_Female_Shoe), null, null, CreateStub.ActualDateTimeStub, CreateStub.UserNameStub).Result
                        }).Result,
                    }).Result;

                Fashion_Male = Fashion.CategoriesOwned.Where(c => c.Name == nameof(Fashion_Male)).FirstOrDefault();
                Fashion_Female = Fashion.CategoriesOwned.Where(c => c.Name == nameof(Fashion_Female)).FirstOrDefault();

                Fashion_Male_Shoe = Fashion_Male.CategoriesOwned.Where(c => c.Name == nameof(Fashion_Male_Shoe)).FirstOrDefault();
                Fashion_Female_Shoe = Fashion_Female.CategoriesOwned.Where(c => c.Name == nameof(Fashion_Female_Shoe)).FirstOrDefault();
            }


            public Category Fashion;
            public Category Fashion_Male;
            public Category Fashion_Male_Shoe;
            public Category Fashion_Female;
            public Category Fashion_Female_Shoe;
        }

        public class SimpleCategory
        {
            public Category Cat { get; set; }

            public string NameInit { get; init; }
            public Category ParentCatInit { get; init; }
            public IReadOnlyCollection<CategoryOwner> OwnersLinksInit { get; init; }
            public IReadOnlyCollection<Category> CategoriesOwnedInit { get; init; }


            public SimpleCategory(int simpleId,
                Category paterntCat = null,
                ICollection<OwnerWithTypeDto> ownersWithTypes = null,
                ICollection<Category> categoriesOwned = null)
            {
                var name = "SIMPLE_CAT_" + simpleId;
                Cat = Category.CreateCategory(name, paterntCat, ownersWithTypes, CreateStub.ActualDateTimeStub, CreateStub.UserNameStub, categoriesOwned).Result;

                NameInit = Cat.Name;
                ParentCatInit = paterntCat;
                OwnersLinksInit = Cat.OwnersLinks;
                CategoriesOwnedInit = Cat.CategoriesOwned;
            }
        }
    }
}
