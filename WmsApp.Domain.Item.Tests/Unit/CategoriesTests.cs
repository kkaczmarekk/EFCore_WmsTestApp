using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WmsApp.Domain.Common;
using WmsApp.Domain.Common.Enities;
using WmsApp.Domain.Items.SupportedTypes;
using WmsApp.Tests.Common.Extensions;
using Xunit.Sdk;

namespace WmsApp.Domain.Items.Tests.Unit
{
    public class CategoriesTests
    {
        #region CreateCategory
        [Theory]
        [InlineData("TEST_NAME", null)]
        [InlineData("TEST_NAME", "")]
        [InlineData(null, "TEST_USER_NAME")]
        [InlineData("", "TEST_USER_NAME")]
        private void CreateCategory_WithNullOrWiteSpace(string new_Name, string userName)
        {
            //SETUP


            //ATTEMPT
            var catTestResult = Category.CreateCategory(new_Name, null, null, CreateStub.ActualDateTimeStub, userName);


            //VERIFY
            AssertExt.StatusValidator_Fail(catTestResult);
            Assert.Null(catTestResult.Result);
        }

        [Fact]
        private void CreateCategory_WithoutRelationships()
        {
            //SETUP
            var name = "CAT_NAME_1";

            //ATTEMPT
            var catTestResult = Category.CreateCategory(name, null, null, CreateStub.ActualDateTimeStub, CreateStub.UserNameStub);

            //VERIFY
            AssertExt.StatusValidator_OK(catTestResult);
            Assert.Equal(name, catTestResult.Result.Name);
            Assert.Equal(CreateStub.ActualDateTimeStub.GetActualDateTime(), catTestResult.Result.CreatedDateUtc);
            Assert.Equal(CreateStub.UserNameStub, catTestResult.Result.CreatedByUserName);
            AssertExt.EntityEvents_Check(catTestResult.Result, afterCount: 2);
        }

        [Fact]
        private void CreateCategory_WithRelationships()
        {
            //SETUP

            //ATTEMPT
            var parentCatTest = new CategoryFactory.SimpleCategory(0).Cat;
            var catTest = new CategoryFactory.SimpleCategory(0, parentCatTest).Cat;

            //VERIFY
            Assert.Equal(parentCatTest, catTest.ParentCategory);
        }

        [Fact]
        private void CreateCategory_WithComplexRelationships()
        {
            //SETUP

            //ATTEMPT
            var catTest = new CategoryFactory.FashionCompexCategory();

            //VERIFY
            Assert.Null(catTest.Fashion.ParentCategory);
            Assert.Equal(2, catTest.Fashion.CategoriesOwned.Count);
            Assert.True(catTest.Fashion.CategoriesOwned.Contains(catTest.Fashion_Male));
            Assert.True(catTest.Fashion.CategoriesOwned.Contains(catTest.Fashion_Female));

            Assert.True(catTest.Fashion_Male.ParentCategory == catTest.Fashion);
            Assert.Equal(1, catTest.Fashion_Male.CategoriesOwned.Count);
            Assert.True(catTest.Fashion_Male.CategoriesOwned.Contains(catTest.Fashion_Male_Shoe));

            Assert.True(catTest.Fashion_Female.ParentCategory == catTest.Fashion);
            Assert.Equal(1, catTest.Fashion_Female.CategoriesOwned.Count);
            Assert.True(catTest.Fashion_Female.CategoriesOwned.Contains(catTest.Fashion_Female_Shoe));

            Assert.True(catTest.Fashion_Male_Shoe.ParentCategory == catTest.Fashion_Male);
            Assert.Equal(0, catTest.Fashion_Male_Shoe.CategoriesOwned.Count);

            Assert.True(catTest.Fashion_Female_Shoe.ParentCategory == catTest.Fashion_Female);
            Assert.Equal(0, catTest.Fashion_Female_Shoe.CategoriesOwned.Count);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        private void CreateCategory_WithOwners(int ownersCount)
        {
            //SETUP
            var ownersWithType = new List<OwnerWithTypeDto>();

            for (int i = 0; i < ownersCount; i++)
                ownersWithType.Add(new OwnerWithTypeDto(new OwnerFactory.SimpleOwner(1).Own,
                    (i & 1) == 1 ? OwnerType.GetDirectorType() : OwnerType.GetMerchantType()));

            //ATTEMPT
            var catTestResult = Category.CreateCategory("PARENT", null, ownersWithType, CreateStub.ActualDateTimeStub, CreateStub.UserNameStub);

            //VERIFY
            Assert.Equal(ownersCount, catTestResult.Result.OwnersLinks.Count);
            
            foreach(var ownerWithType in ownersWithType)
            {
                Assert.True(catTestResult.Result.OwnersLinks.Where(o => o.Owner == ownerWithType.Owner
                    && o.Type == ownerWithType.Type).Any());
            }
        }

        [Fact]
        private void CreateCategory_NameExcedeedLen()
        {
            //SETUP
            var name = new StringBuilder().Append('n', Category.CONST.NAME_LEN + 1).ToString();
            var userName = new StringBuilder().Append('u', CONST.USERNAME_LEN).ToString();

            //ATTEMPT
            var catTestResult = Category.CreateCategory(name, null, null, CreateStub.ActualDateTimeStub, userName);

            //VERIFY
            AssertExt.StatusValidator_Fail(catTestResult);
            Assert.Null(catTestResult.Result);
        }

        [Fact]
        private void CreateCategory_UserNameExcedeedLen()
        {
            //SETUP
            var name = new StringBuilder().Append('n', Category.CONST.NAME_LEN).ToString();
            var userName = new StringBuilder().Append('u', CONST.USERNAME_LEN + 1).ToString();

            //ATTEMPT
            var catTestResult = Category.CreateCategory(name, null, null, CreateStub.ActualDateTimeStub, userName);

            //VERIFY
            AssertExt.StatusValidator_Fail(catTestResult);
            Assert.Null(catTestResult.Result);
        }
        #endregion

        #region SoftDelete_Update
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        private void SoftDelete_Update_WithNullOrWhiteSpace(string userName)
        {
            //SETUP
            var catTest = new CategoryFactory.SimpleCategory(0).Cat;
            catTest.ClearAllEntityEvents();

            //ATTEMPT
            var catTestResult = catTest.SoftDelete_Update(false, userName, DeleteStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_Fail(catTestResult);
            Assert.False(catTest.IsDeleted);
            Assert.Null(catTest.DeletedDateUtc);
            Assert.Null(catTest.DeletedByUserName);
            AssertExt.EntityEvents_Check(catTest);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        private void SoftDelete_Update_AlterMultipleTime(int alterCount)
        {
            //SETUP
            var catTest = new CategoryFactory.SimpleCategory(0).Cat;
            catTest.ClearAllEntityEvents();

            //ATTEMPT
            for (int i = 0; i <= alterCount; i++)
                catTest.SoftDelete_Update(!catTest.IsDeleted, DeleteStub.UserNameStub, DeleteStub.ActualDateTimeStub);

            //VERIFY
            Assert.Equal((alterCount & 1) == 1, !catTest.IsDeleted);
            Assert.Equal(DeleteStub.UserNameStub, catTest.DeletedByUserName);
            Assert.Equal(DeleteStub.ActualDateTimeStub.GetActualDateTime(), catTest.DeletedDateUtc);
            AssertExt.EntityEvents_Check(catTest, beforeCount: 1);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        private void SoftDelete_Update_SetTheSameValue(bool softDeleted)
        {
            //SETUP
            var catTest = new CategoryFactory.SimpleCategory(0).Cat;

            if (catTest.IsDeleted != softDeleted)
                catTest.SoftDelete_Update(!catTest.IsDeleted, CreateStub.UserNameStub, CreateStub.ActualDateTimeStub);

            catTest.ClearAllEntityEvents();

            //ATTEMPT
            catTest.SoftDelete_Update(softDeleted, DeleteStub.UserNameStub, DeleteStub.ActualDateTimeStub);

            //VERIFY
            Assert.Equal(softDeleted, catTest.IsDeleted);
            Assert.NotEqual(DeleteStub.UserNameStub, catTest.DeletedByUserName);
            Assert.NotEqual(DeleteStub.ActualDateTimeStub.GetActualDateTime(), catTest.DeletedDateUtc);
            AssertExt.EntityEvents_Check(catTest, beforeCount: 0);
        }

        [Fact]
        private void SoftDelete_Update_UserNameExcedeedLen()
        {
            //SETUP
            var userName = new StringBuilder().Append('u', CONST.USERNAME_LEN + 1).ToString();
            var catTest = new CategoryFactory.SimpleCategory(0).Cat;
            catTest.ClearAllEntityEvents();

            //ATTEMPT
            var catTestResult = catTest.SoftDelete_Update(true, userName, DeleteStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_Fail(catTestResult);
            Assert.Equal(false, catTest.IsDeleted);
            Assert.NotEqual(DeleteStub.UserNameStub, catTest.DeletedByUserName);
            Assert.NotEqual(DeleteStub.ActualDateTimeStub.GetActualDateTime(), catTest.DeletedDateUtc);
            AssertExt.EntityEvents_Check(catTest);
        }

        [Fact]
        private void SoftDelete_Update_WhitOwnedCategories()
        {
            //SETUP
            var catParent = new CategoryFactory.SimpleCategory(0).Cat;
            var catTest = new CategoryFactory.SimpleCategory(1, null, null, new List<Category>() { catParent }).Cat;
            catTest.ClearAllEntityEvents();

            //ATTEMPT
            var catTestResult = catTest.SoftDelete_Update(true, CreateStub.UserNameStub, DeleteStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_Fail(catTestResult);
            Assert.Equal(false, catTest.IsDeleted);
            Assert.NotEqual(DeleteStub.UserNameStub, catTest.DeletedByUserName);
            Assert.NotEqual(DeleteStub.ActualDateTimeStub.GetActualDateTime(), catTest.DeletedDateUtc);
            AssertExt.EntityEvents_Check(catTest);
        }
        #endregion

        #region Name_Update
        [Theory]
        [InlineData("TEST_NAME", null)]
        [InlineData("TEST_NAME", "")]
        [InlineData(null, "TEST_USER_NAME")]
        [InlineData("", "TEST_USER_NAME")]
        private void Name_Update_WithNullOrWiteSpace(string name, string userName)
        {
            //SETUP
            var catTestInit = new CategoryFactory.SimpleCategory(0);
            catTestInit.Cat.ClearAllEntityEvents();

            //ATTEMPT
            var catTestResult = catTestInit.Cat.Name_Update(name, userName, UpdateStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_Fail(catTestResult);
            Assert.Equal(catTestInit.NameInit, catTestInit.Cat.Name);
            Assert.Null(catTestInit.Cat.UpdatedByUserName);
            Assert.Null(catTestInit.Cat.UpdatedDateUtc);
            AssertExt.EntityEvents_Check(catTestInit.Cat);
        }

        [Fact]
        private void Name_Update_Correct()
        {
            //SETUP
            var catTestInit = new CategoryFactory.SimpleCategory(0);
            var new_Name = "CAT_NEW_NAME";
            catTestInit.Cat.ClearAllEntityEvents();

            //ATTEMPT
            var catTestResult = catTestInit.Cat.Name_Update(new_Name, UpdateStub.UserNameStub, UpdateStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_OK(catTestResult);
            Assert.Equal(new_Name, catTestInit.Cat.Name);
            Assert.Equal(UpdateStub.UserNameStub, catTestInit.Cat.UpdatedByUserName);
            Assert.Equal(UpdateStub.ActualDateTimeStub.GetActualDateTime(), catTestInit.Cat.UpdatedDateUtc);
            AssertExt.EntityEvents_Check(catTestInit.Cat, beforeCount: 1);
        }

        [Fact]
        private void Name_Update_NameExcedeedLen()
        {
            //SETUP
            var catTestInit = new CategoryFactory.SimpleCategory(0);
            var new_Name = new StringBuilder().Append('n', Category.CONST.NAME_LEN + 1).ToString();
            var userName = new StringBuilder().Append('n', CONST.USERNAME_LEN).ToString();
            catTestInit.Cat.ClearAllEntityEvents();

            //ATTEMPT
            var catTestResult = catTestInit.Cat.Name_Update(new_Name, userName, UpdateStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_Fail(catTestResult);
            Assert.Equal(catTestInit.NameInit, catTestInit.Cat.Name);
            Assert.Null(catTestInit.Cat.UpdatedByUserName);
            Assert.Null(catTestInit.Cat.UpdatedDateUtc);
            AssertExt.EntityEvents_Check(catTestInit.Cat);
        }

        [Fact]
        private void Name_Update_UserNameExcedeedLen()
        {
            //SETUP
            var catTestInit = new CategoryFactory.SimpleCategory(0);
            var new_Name = new StringBuilder().Append('n', Category.CONST.NAME_LEN).ToString();
            var userName = new StringBuilder().Append('n', CONST.USERNAME_LEN + 1).ToString();
            catTestInit.Cat.ClearAllEntityEvents();

            //ATTEMPT
            var catTestResult = catTestInit.Cat.Name_Update(new_Name, userName, UpdateStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_Fail(catTestResult);
            Assert.Equal(catTestInit.NameInit, catTestInit.Cat.Name);
            Assert.Null(catTestInit.Cat.UpdatedByUserName);
            Assert.Null(catTestInit.Cat.UpdatedDateUtc);
            AssertExt.EntityEvents_Check(catTestInit.Cat);
        }
        #endregion

        #region PatentCategory_Update
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        private void PatentCategory_Update_WithNullOrWhiteSpace(string userName)
        {
            //SETUP
            var parentCatTestInit = new CategoryFactory.SimpleCategory(2);
            var catTestInit = new CategoryFactory.SimpleCategory(0);
            catTestInit.Cat.ClearAllEntityEvents();

            //ATTEMPT
            var catTestResult = catTestInit.Cat.ParentCategory_Update(parentCatTestInit.Cat, userName, UpdateStub.ActualDateTimeStub);


            //VERIFY
            AssertExt.StatusValidator_Fail(catTestResult);
            Assert.Null(catTestInit.Cat.ParentCategory);
            Assert.Null(catTestInit.Cat.UpdatedByUserName);
            Assert.Null(catTestInit.Cat.UpdatedDateUtc);
            AssertExt.EntityEvents_Check(catTestInit.Cat);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        private void ParentCategory_Update_Correct(bool testChildWithParent)
        {
            //SETUP
            Category new_parentCatTest = new CategoryFactory.SimpleCategory(1).Cat;

            var parentCatTest = testChildWithParent
                ? new CategoryFactory.SimpleCategory(2).Cat
                : null;

            var catTestInit = new CategoryFactory.SimpleCategory(0, parentCatTest);
            catTestInit.Cat.ClearAllEntityEvents();

            //ATTEMPT
            var catTestResult = catTestInit.Cat.ParentCategory_Update(new_parentCatTest, UpdateStub.UserNameStub, UpdateStub.ActualDateTimeStub);


            //VERIFY
            AssertExt.StatusValidator_OK(catTestResult);
            Assert.Equal(new_parentCatTest, catTestInit.Cat.ParentCategory);
            Assert.Equal(UpdateStub.UserNameStub, catTestInit.Cat.UpdatedByUserName);
            Assert.Equal(UpdateStub.ActualDateTimeStub.GetActualDateTime(), catTestInit.Cat.UpdatedDateUtc);
            AssertExt.EntityEvents_Check(catTestInit.Cat, afterCount: 2);
        }

        [Fact]
        private void ParentCategory_Update_UserNameExcedeedLen()
        {
            //SETUP
            Category new_parentCatTest = new CategoryFactory.SimpleCategory(1).Cat;
            var parentCatTest = new CategoryFactory.SimpleCategory(2).Cat;
            var catTestInit = new CategoryFactory.SimpleCategory(0, parentCatTest);
            var userName = new StringBuilder().Append('u', CONST.USERNAME_LEN + 1).ToString();
            catTestInit.Cat.ClearAllEntityEvents();

            //ATTEMPT
            var catTestResult = catTestInit.Cat.ParentCategory_Update(new_parentCatTest, userName, UpdateStub.ActualDateTimeStub);


            //VERIFY
            AssertExt.StatusValidator_Fail(catTestResult);
            Assert.Equal(catTestInit.ParentCatInit, catTestInit.Cat.ParentCategory);
            Assert.Null(catTestInit.Cat.UpdatedByUserName);
            Assert.Null(catTestInit.Cat.UpdatedDateUtc);
            AssertExt.EntityEvents_Check(catTestInit.Cat);
        }
        #endregion

        #region Ownes_AddOwner
        [Theory]
        [InlineData("", false, false)]
        [InlineData(null, false, false)]
        [InlineData("USERNAME_TEST", true, false)]
        [InlineData("USERNAME_TEST", false, true)]
        private void Owners_AddOwner_WithNullOrWhiteSpace(string userName, bool ownerIsNull, bool ownerTypeIsNull)
        {
            //SETUP
            var catTestInit = new CategoryFactory.SimpleCategory(1).Cat;
            var new_Owner = ownerIsNull
                ? null 
                : new OwnerFactory.SimpleOwner(1).Own;
                
            var new_OwnerType = ownerTypeIsNull 
                ? null 
                : OwnerType.GetCopywriterType();


            //ATTEMPT
            var catTestResult = catTestInit.Owners_AddOwner(new_Owner, new_OwnerType, userName, UpdateStub.ActualDateTimeStub);


            //VERIFY
            AssertExt.StatusValidator_Fail(catTestResult);
            Assert.Equal(0, catTestInit.OwnersLinks.Count);
            Assert.Null(catTestInit.UpdatedByUserName);
            Assert.Null(catTestInit.UpdatedDateUtc);
        }

        [Fact]
        private void Owners_AddOwner_OwnerAlreadyInCollection()
        {
            //SETUP
            var new_Owner = new OwnerFactory.SimpleOwner(1).Own;
            var new_OwnerType = OwnerType.GetCopywriterType();
            var new_OwnerWithType = new OwnerWithTypeDto(new_Owner, new_OwnerType);

            var catTest = new CategoryFactory.SimpleCategory(1, null, new List<OwnerWithTypeDto>() { new_OwnerWithType }).Cat;


            //ATTEMPT
            var catTestResult = catTest.Owners_AddOwner(new_Owner, new_OwnerType, UpdateStub.UserNameStub, UpdateStub.ActualDateTimeStub);


            //VERIFY
            AssertExt.StatusValidator_Fail(catTestResult);
            Assert.Null(catTest.UpdatedByUserName);
            Assert.Null(catTest.UpdatedDateUtc);
        }

        [Fact]
        private void Owners_AddOwner_Correct()
        {
            //SETUP
            var new_Owner = new OwnerFactory.SimpleOwner(1).Own;
            var new_OwnerType = OwnerType.GetCopywriterType();
            var catTest = new CategoryFactory.SimpleCategory(1).Cat;


            //ATTEMPT
            var catTestResult = catTest.Owners_AddOwner(new_Owner, new_OwnerType, UpdateStub.UserNameStub, UpdateStub.ActualDateTimeStub);


            //VERIFY
            AssertExt.StatusValidator_OK(catTestResult);
            Assert.True(catTest.OwnersLinks
                .Where(ol => ol.Owner == new_Owner && ol.Type == new_OwnerType)
                .Any());
            Assert.Equal(UpdateStub.UserNameStub, catTest.UpdatedByUserName);
            Assert.Equal(UpdateStub.ActualDateTimeStub.GetActualDateTime(), catTest.UpdatedDateUtc);
        }

        [Fact]
        private void Owners_AddOwner_UserNameExcedeedLen()
        {
            //SETUP
            var new_Owner = new OwnerFactory.SimpleOwner(1).Own;
            var new_OwnerType = OwnerType.GetCopywriterType();
            var catTestInit = new CategoryFactory.SimpleCategory(1);
            var userName = new StringBuilder().Append('u', CONST.USERNAME_LEN + 1).ToString();

            //ATTEMPT
            var catTestResult = catTestInit.Cat.Owners_AddOwner(new_Owner, new_OwnerType, userName, UpdateStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_Fail(catTestResult);
            Assert.False(catTestInit.Cat.OwnersLinks
                .Where(ol => ol.Owner == new_Owner && ol.Type == new_OwnerType)
                .Any());
            Assert.Null(catTestInit.Cat.UpdatedByUserName);
            Assert.Null(catTestInit.Cat.UpdatedDateUtc);
        }
        #endregion

        #region Owners_RemoveOwner
        [Theory]
        [InlineData("", false)]
        [InlineData(null, false)]
        [InlineData("USERNAME_TEST", true)]
        private void Owners_RemoveOwner_WithNullOrWhiteSpace(string userName, bool ownerIsNull)
        {
            //SETUP
            var catTest = new CategoryFactory.SimpleCategory(1).Cat;

            var dell_Owner = ownerIsNull 
                ? null 
                : new OwnerFactory.SimpleOwner(1).Own;


            //ATTEMPT
            var catTestResult = catTest.Owners_RemoveOwner(dell_Owner, userName, UpdateStub.ActualDateTimeStub);


            //VERIFY
            AssertExt.StatusValidator_Fail(catTestResult);
            Assert.Equal(0, catTest.OwnersLinks.Count);
            Assert.Null(catTest.UpdatedByUserName);
            Assert.Null(catTest.UpdatedDateUtc);
        }

        [Fact]
        private void Owners_RemoveOwner_OwnerNotInCollection()
        {
            //SETUP
            var catTest = new CategoryFactory.SimpleCategory(1).Cat;

            var dell_Owner = new OwnerFactory.SimpleOwner(1).Own;


            //ATTEMPT
            var catTestResult = catTest.Owners_RemoveOwner(dell_Owner, UpdateStub.UserNameStub, UpdateStub.ActualDateTimeStub);


            //VERIFY
            AssertExt.StatusValidator_Fail(catTestResult);
            Assert.Equal(0, catTest.OwnersLinks.Count);
            Assert.Null(catTest.UpdatedByUserName);
            Assert.Null(catTest.UpdatedDateUtc);
        }

        [Fact]
        private void Owners_RemoveOwner_Correct()
        {
            //SETUP
            var dell_Owner = new OwnerFactory.SimpleOwner(1).Own;
            var dell_OwnerType = OwnerType.GetCopywriterType();
            var dell_OwnerWithType = new OwnerWithTypeDto(dell_Owner, dell_OwnerType);

            var catTest = new CategoryFactory.SimpleCategory(1, null, new List<OwnerWithTypeDto> { dell_OwnerWithType } ).Cat;


            //ATTEMPT
            var catTestResult = catTest.Owners_RemoveOwner(dell_Owner, UpdateStub.UserNameStub, UpdateStub.ActualDateTimeStub);


            //VERIFY
            AssertExt.StatusValidator_OK(catTestResult);
            Assert.False(catTest.OwnersLinks
                .Where(ol => ol.Owner == dell_Owner && ol.Type == dell_OwnerType)
                .Any());
            Assert.Equal(UpdateStub.UserNameStub, catTest.UpdatedByUserName);
            Assert.Equal(UpdateStub.ActualDateTimeStub.GetActualDateTime(), catTest.UpdatedDateUtc);
        }

        [Fact]
        private void Owners_RemoveOwner_UserNameExcedeedLen()
        {
            //SETUP
            var dell_Owner = new OwnerFactory.SimpleOwner(1).Own;
            var dell_OwnerType = OwnerType.GetCopywriterType();
            var dell_OwnerWithType = new OwnerWithTypeDto(dell_Owner, dell_OwnerType);
            var userName = new StringBuilder().Append('u', CONST.USERNAME_LEN + 1).ToString();
            var catTestInit = new CategoryFactory.SimpleCategory(1, null, new List<OwnerWithTypeDto> { dell_OwnerWithType });

            //ATTEMPT
            var catTestResult = catTestInit.Cat.Owners_RemoveOwner(dell_Owner, userName, UpdateStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_Fail(catTestResult);
            Assert.True(catTestInit.Cat.OwnersLinks
                .Where(ol => ol.Owner == dell_Owner && ol.Type == dell_OwnerType)
                .Any());
            Assert.Null(catTestInit.Cat.UpdatedByUserName);
            Assert.Null(catTestInit.Cat.UpdatedDateUtc);
        }
        #endregion
    }
}
