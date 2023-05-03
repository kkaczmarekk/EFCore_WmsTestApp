using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WmsApp.Domain.Common.Enities;
using WmsApp.Domain.Items.Tests.Factories;
using WmsApp.Tests.Common.Extensions;

namespace WmsApp.Domain.Items.Tests.Unit
{
    public class ItemTests
    {
        #region CreateItem
        [Theory]
        [InlineData("", "USERNAME_TEST", false)]
        [InlineData(null, "USERNAME_TEST", false)]
        [InlineData("NAME_TEST", "", false)]
        [InlineData("NAME_TEST", null, false)]
        [InlineData("NAME_TEST", "USERNAME_TEST", true)]

        private void CreateItem_WithNullOrWhiteSpace(string name, string userName, bool nullCategory)
        {
            //SETUP
            var category = nullCategory 
                ? null 
                : new CategoryFactory.SimpleCategory(1).Cat;

            //ATTEMPT
            var itemTestResult = Item.CreateItem(name, "DESCRIPTION_1", category, userName, CreateStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_Fail(itemTestResult);
            Assert.Null(itemTestResult.Result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("DESCRIPTION_1")]
        private void CreateItem_Correct(string description)
        {
            //SETUP
            var category = new CategoryFactory.SimpleCategory(1).Cat;
            var itemName = "ITEM_NAME_1";

            //ATTEMPT
            var itemResult = Item.CreateItem(itemName, description, category, CreateStub.UserNameStub, CreateStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_OK(itemResult);
            Assert.Equal(itemName, itemResult.Result.Name);
            Assert.Equal(description, itemResult.Result.Description);
            Assert.Equal(CreateStub.UserNameStub, itemResult.Result.CreatedByUserName);
            Assert.Equal(CreateStub.ActualDateTimeStub.GetActualDateTime(), itemResult.Result.CreatedDateUtc);
            Assert.Equal(category, itemResult.Result.Category);
        }

        [Fact]
        private void CreateItem_NameExcedeedLen()
        {
            //SETUP
            var category = new CategoryFactory.SimpleCategory(1).Cat;
            var itemName = new StringBuilder().Append('n', Item.CONST.NAME_LEN + 1).ToString();
            var description = new StringBuilder().Append('d', Item.CONST.DESCRIPTION_LEN).ToString();
            var userName = new StringBuilder().Append('u', CONST.USERNAME_LEN).ToString();


            //ATTEMPT
            var itemResult = Item.CreateItem(itemName, description, category, userName, CreateStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_Fail(itemResult);
            Assert.Null(itemResult.Result);
        }

        [Fact]
        private void CreateItem_DescExcedeedLen()
        {
            //SETUP
            var category = new CategoryFactory.SimpleCategory(1).Cat;
            var itemName = new StringBuilder().Append('n', Item.CONST.NAME_LEN).ToString();
            var description = new StringBuilder().Append('d', Item.CONST.DESCRIPTION_LEN + 1).ToString();
            var userName = new StringBuilder().Append('u', CONST.USERNAME_LEN).ToString();

            //ATTEMPT
            var itemResult = Item.CreateItem(itemName, description, category, userName, CreateStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_Fail(itemResult);
            Assert.Null(itemResult.Result);
        }

        [Fact]
        private void CreateItem_UserNameExcedeedLen()
        {
            //SETUP
            var category = new CategoryFactory.SimpleCategory(1).Cat;
            var itemName = new StringBuilder().Append('n', Item.CONST.NAME_LEN).ToString();
            var description = new StringBuilder().Append('d', Item.CONST.DESCRIPTION_LEN).ToString();
            var userName = new StringBuilder().Append('u', CONST.USERNAME_LEN + 1).ToString();

            //ATTEMPT
            var itemResult = Item.CreateItem(itemName, description, category, userName, CreateStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_Fail(itemResult);
            Assert.Null(itemResult.Result);
        }
        #endregion

        #region SoftDelete_Update
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        private void SoftDelete_Update_WithNullOrWhiteSpace(string userName)
        {
            //SETUP
            var cat = new CategoryFactory.SimpleCategory(0).Cat;
            var itemTest = new ItemFactory.SimpleItem(0, cat).Itm;

            //ATTEMPT
            var itemTestResult = itemTest.SoftDelete_Update(false, userName, DeleteStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_Fail(itemTestResult);
            Assert.False(itemTest.IsDeleted);
            Assert.Null(itemTest.DeletedDateUtc);
            Assert.Null(itemTest.DeletedByUserName);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        private void SoftDelete_Update_AlterMultipleTime(int alterCount)
        {
            //SETUP
            var cat = new CategoryFactory.SimpleCategory(0).Cat;
            var itemTest = new ItemFactory.SimpleItem(0, cat).Itm;

            //ATTEMPT
            for (int i = 0; i <= alterCount; i++)
                itemTest.SoftDelete_Update(!itemTest.IsDeleted, DeleteStub.UserNameStub, DeleteStub.ActualDateTimeStub);

            //VERIFY
            Assert.Equal((alterCount & 1) == 1, !itemTest.IsDeleted);
            Assert.Equal(DeleteStub.UserNameStub, itemTest.DeletedByUserName);
            Assert.Equal(DeleteStub.ActualDateTimeStub.GetActualDateTime(), itemTest.DeletedDateUtc);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        private void SoftDelete_Update_SetTheSameValue(bool softDeleted)
        {
            //SETUP
            var cat = new CategoryFactory.SimpleCategory(0).Cat;
            var itemTest = new ItemFactory.SimpleItem(0, cat).Itm;

            if (itemTest.IsDeleted != softDeleted)
                itemTest.SoftDelete_Update(!itemTest.IsDeleted, CreateStub.UserNameStub, CreateStub.ActualDateTimeStub);

            //ATTEMPT
            itemTest.SoftDelete_Update(softDeleted, DeleteStub.UserNameStub, DeleteStub.ActualDateTimeStub);

            //VERIFY
            Assert.Equal(softDeleted, itemTest.IsDeleted);
            Assert.NotEqual(DeleteStub.UserNameStub, itemTest.DeletedByUserName);
            Assert.NotEqual(DeleteStub.ActualDateTimeStub.GetActualDateTime(), itemTest.DeletedDateUtc);
        }

        [Fact]
        private void SoftDelete_Update_UserNameExcedeedLen()
        {
            //SETUP
            var userName = new StringBuilder().Append('u', CONST.USERNAME_LEN + 1).ToString();
            var cat = new CategoryFactory.SimpleCategory(0).Cat;
            var itemTest = new ItemFactory.SimpleItem(0, cat).Itm;

            //ATTEMPT
            var itemTestResult = itemTest.SoftDelete_Update(true, userName, DeleteStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_Fail(itemTestResult);
            Assert.Equal(false, itemTest.IsDeleted);
            Assert.NotEqual(DeleteStub.UserNameStub, itemTest.DeletedByUserName);
            Assert.NotEqual(DeleteStub.ActualDateTimeStub.GetActualDateTime(), itemTest.DeletedDateUtc);
        }
        #endregion

        #region Name_Update
        [Theory]
        [InlineData("", "USERNAME_TEST")]
        [InlineData(null, "USERNAME_TEST")]
        [InlineData("NAME_TEST", "")]
        [InlineData("NAME_TEST", null)]
        private void Name_Update_WithNullOrWhireSpace(string newItemName, string userName)
        {
            //SETUP
            var category = new CategoryFactory.SimpleCategory(1).Cat;
            var itemTestInit = new ItemFactory.SimpleItem(1, category);

            //ATTEMPT
            var itemTestResult = itemTestInit.Itm.Name_Update(newItemName, userName, UpdateStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_Fail(itemTestResult);
            Assert.Equal(itemTestInit.NameInit, itemTestInit.Itm.Name);
            Assert.Null(itemTestInit.Itm.UpdatedByUserName);
            Assert.Null(itemTestInit.Itm.UpdatedDateUtc);
        }

        [Fact]
        private void Name_Update_Correct()
        {
            //SETUP
            var category = new CategoryFactory.SimpleCategory(1).Cat;
            var new_Name = "NEW_ITEM_NAME_1";

            var itemTest = new ItemFactory.SimpleItem(1, category);
            

            //ATTEMPT
            var itemTestResult = itemTest.Itm.Name_Update(new_Name, UpdateStub.UserNameStub, UpdateStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_OK(itemTestResult);
            Assert.Equal(new_Name, itemTest.Itm.Name);
            Assert.Equal(UpdateStub.UserNameStub, itemTest.Itm.UpdatedByUserName);
            Assert.Equal(UpdateStub.ActualDateTimeStub.GetActualDateTime(), itemTest.Itm.UpdatedDateUtc);
        }

        [Fact]
        private void Name_Update_NameExcedeedLen()
        {
            //SETUP
            var category = new CategoryFactory.SimpleCategory(1).Cat;
            var new_Name = new StringBuilder().Append('n', Item.CONST.NAME_LEN + 1).ToString();
            var userName = new StringBuilder().Append('u', CONST.USERNAME_LEN).ToString();
            var itemTest = new ItemFactory.SimpleItem(1, category);

            //ATTEMPT
            var itemTestResult = itemTest.Itm.Name_Update(new_Name, userName, UpdateStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_Fail(itemTestResult);
            Assert.Equal(itemTest.NameInit, itemTest.Itm.Name);
            Assert.Null(itemTest.Itm.UpdatedByUserName);
            Assert.Null(itemTest.Itm.UpdatedDateUtc);
        }

        [Fact]
        private void Name_Update_UserNameExcedeedLen()
        {
            //SETUP
            var category = new CategoryFactory.SimpleCategory(1).Cat;
            var new_Name = new StringBuilder().Append('n', Item.CONST.NAME_LEN).ToString();
            var userName = new StringBuilder().Append('u', CONST.USERNAME_LEN + 1).ToString();
            var itemTest = new ItemFactory.SimpleItem(1, category);

            //ATTEMPT
            var itemTestResult = itemTest.Itm.Name_Update(new_Name, userName, UpdateStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_Fail(itemTestResult);
            Assert.Equal(itemTest.NameInit, itemTest.Itm.Name);
            Assert.Null(itemTest.Itm.UpdatedByUserName);
            Assert.Null(itemTest.Itm.UpdatedDateUtc);
        }
        #endregion

        #region Description_Update
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        private void Description_Update_WithNullOrWhiteSpace(string userName)
        {
            //SETUP
            var category = new CategoryFactory.SimpleCategory(1).Cat;
            var new_Desc = "NEW_DESCTIPTION_1";
            var itemTest = new ItemFactory.SimpleItem(1, category);

            //ATTEMPT
            var itemTestResult = itemTest.Itm.Description_Update(new_Desc, userName, UpdateStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_Fail(itemTestResult);
            Assert.Equal(itemTest.DescInit, itemTest.Itm.Description);
            Assert.Null(itemTest.Itm.UpdatedByUserName);
            Assert.Null(itemTest.Itm.UpdatedDateUtc);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("NEW_DESCTIPTION_1")]
        private void Description_Update_Correct(string newItemDesc)
        {
            //SETUP
            var category = new CategoryFactory.SimpleCategory(1).Cat;
            var itemTest = new ItemFactory.SimpleItem(1, category);

            //ATTEMPT
            var itemTestResult = itemTest.Itm.Description_Update(newItemDesc, UpdateStub.UserNameStub, UpdateStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_OK(itemTestResult);
            Assert.Equal(newItemDesc, itemTest.Itm.Description);
            Assert.Equal(UpdateStub.UserNameStub, itemTest.Itm.UpdatedByUserName);
            Assert.Equal(UpdateStub.ActualDateTimeStub.GetActualDateTime(), itemTest.Itm.UpdatedDateUtc);
        }

        [Fact]
        private void Description_Update_DescriptionExcedeedLen()
        {
            //SETUP
            var category = new CategoryFactory.SimpleCategory(1).Cat;
            var itemTest = new ItemFactory.SimpleItem(1, category);
            var new_Desc = new StringBuilder().Append('d', Item.CONST.DESCRIPTION_LEN + 1).ToString();
            var userName = new StringBuilder().Append('u', CONST.USERNAME_LEN).ToString();

            //ATTEMPT
            var itemTestResult = itemTest.Itm.Description_Update(new_Desc, userName, UpdateStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_Fail(itemTestResult);
            Assert.Equal(itemTest.DescInit, itemTest.Itm.Description);
            Assert.Null(itemTest.Itm.UpdatedByUserName);
            Assert.Null(itemTest.Itm.UpdatedDateUtc);
        }

        [Fact]
        private void Description_Update_UserNameExcedeedLen()
        {
            //SETUP
            var category = new CategoryFactory.SimpleCategory(1).Cat;
            var itemTest = new ItemFactory.SimpleItem(1, category);
            var new_Desc = new StringBuilder().Append('d', Item.CONST.DESCRIPTION_LEN).ToString();
            var userName = new StringBuilder().Append('u', CONST.USERNAME_LEN + 1).ToString();

            //ATTEMPT
            var itemTestResult = itemTest.Itm.Description_Update(new_Desc, userName, UpdateStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_Fail(itemTestResult);
            Assert.Equal(itemTest.DescInit, itemTest.Itm.Description);
            Assert.Null(itemTest.Itm.UpdatedByUserName);
            Assert.Null(itemTest.Itm.UpdatedDateUtc);
        }
        #endregion

        #region Category_Update
        [Theory]
        [InlineData("", false)]
        [InlineData(null, false)]
        [InlineData("USERNAME_TEST", true)]
        private void Category_Update_WithNullOrWhiteSpace(string userName, bool newCategoryNull)
        {
            //SETUP
            var category = new CategoryFactory.SimpleCategory(1).Cat;
            var new_Category = newCategoryNull 
                ? null 
                : new CategoryFactory.SimpleCategory(2).Cat;
            var itemTest = new ItemFactory.SimpleItem(1, category);

            //ATTEMPT
            var itemTestResult = itemTest.Itm.Category_Update(new_Category, userName, UpdateStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_Fail(itemTestResult);
            Assert.Equal(itemTest.CatInit, itemTest.Itm.Category);
            Assert.Null(itemTest.Itm.UpdatedByUserName);
            Assert.Null(itemTest.Itm.UpdatedDateUtc);
        }

        [Fact]
        private void Category_Update_Correct()
        {
            //SETUP
            var category = new CategoryFactory.SimpleCategory(1).Cat;
            var new_Category = new CategoryFactory.SimpleCategory(2).Cat;
            var itemTest = new ItemFactory.SimpleItem(1, category);

            //ATTEMPT
            var itemTestResult = itemTest.Itm.Category_Update(new_Category, UpdateStub.UserNameStub, UpdateStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_OK(itemTestResult);
            Assert.Equal(new_Category, itemTest.Itm.Category);
            Assert.Equal(UpdateStub.UserNameStub, itemTest.Itm.UpdatedByUserName);
            Assert.Equal(UpdateStub.ActualDateTimeStub.GetActualDateTime(), itemTest.Itm.UpdatedDateUtc);
        }

        [Fact]
        private void Category_Update_UserNameExcedeed()
        {
            //SETUP
            var category = new CategoryFactory.SimpleCategory(1).Cat;
            var new_Category = new CategoryFactory.SimpleCategory(2).Cat;
            var userName = new StringBuilder().Append('u', CONST.USERNAME_LEN + 1).ToString();
            var itemTest = new ItemFactory.SimpleItem(1, category);

            //ATTEMPT
            var itemTestResult = itemTest.Itm.Category_Update(new_Category, userName, UpdateStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_Fail(itemTestResult);
            Assert.Equal(itemTest.CatInit, itemTest.Itm.Category);
            Assert.Null(itemTest.Itm.UpdatedByUserName);
            Assert.Null(itemTest.Itm.UpdatedDateUtc);
        }
        #endregion
    }
}
