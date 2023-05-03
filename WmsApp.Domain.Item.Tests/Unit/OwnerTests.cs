using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using WmsApp.Domain.Common.Enities;
using WmsApp.Domain.Items.Tests.Factories;
using WmsApp.Tests.Common.Extensions;

namespace WmsApp.Domain.Items.Tests.Unit
{
    public class OwnerTests
    {
        #region CreateOwner
        [Theory]
        [InlineData("", "LAST_NAME", "EMAIL", "USER_NAME")]
        [InlineData(null, "LAST_NAME", "EMAIL", "USER_NAME")]
        [InlineData("FIRST_NAME", "", "EMAIL", "USER_NAME")]
        [InlineData("FIRST_NAME", null, "EMAIL", "USER_NAME")]
        [InlineData("FIRST_NAME", "LAST_NAME", "", "USER_NAME")]
        [InlineData("FIRST_NAME", "LAST_NAME", null, "USER_NAME")]
        [InlineData("FIRST_NAME", "LAST_NAME", "EMAIL", "")]
        [InlineData("FIRST_NAME", "LAST_NAME", "EMAIL", null)]
        private void CreateOwner_WithNullOrWhiteSpace(string firstName, string lastName, string email, string userName)
        {
            //SETUP


            //ATTEMPT
            var ownerTestResult = Owner.CreateOwner(firstName, lastName, email, userName, CreateStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_Fail(ownerTestResult);
            Assert.Null(ownerTestResult.Result);
        }

        [Fact]
        private void CreateOwner_Correct()
        {
            //SETUP
            var firstName = "FIRST_NAME";
            var lastName = "LAST_NAME";
            var email = "EMAIL";

            //ATTEMPT
            var ownerTestResult = Owner.CreateOwner(firstName, lastName, email, CreateStub.UserNameStub, CreateStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_OK(ownerTestResult);
            Assert.Equal(firstName, ownerTestResult.Result.FirstName);
            Assert.Equal(lastName, ownerTestResult.Result.LastName);
            Assert.Equal(email, ownerTestResult.Result.Email);
            Assert.Equal(CreateStub.UserNameStub, ownerTestResult.Result.CreatedByUserName);
            Assert.Equal(CreateStub.ActualDateTimeStub.GetActualDateTime(), ownerTestResult.Result.CreatedDateUtc);
        }

        [Fact]
        private void CreateOwner_FirstNameExcedeedLen()
        {
            //SETUP
            var firstName = new StringBuilder().Append('f', Owner.CONST.FIRST_NAME_LEN + 1).ToString();
            var lastName = new StringBuilder().Append('l', Owner.CONST.LAST_NAME_LEN).ToString();
            var email = new StringBuilder().Append('e', Owner.CONST.EMAIL_LEN).ToString();
            var userName = new StringBuilder().Append('u', CONST.USERNAME_LEN).ToString();

            //ATTEMPT
            var ownerTestResult = Owner.CreateOwner(firstName, lastName, email, userName, CreateStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_Fail(ownerTestResult);
            Assert.Null(ownerTestResult.Result);
        }

        [Fact]
        private void CreateOwner_LastNameExcedeedLen()
        {
            //SETUP
            var firstName = new StringBuilder().Append('f', Owner.CONST.FIRST_NAME_LEN).ToString();
            var lastName = new StringBuilder().Append('l', Owner.CONST.LAST_NAME_LEN + 1).ToString();
            var email = new StringBuilder().Append('e', Owner.CONST.EMAIL_LEN).ToString();
            var userName = new StringBuilder().Append('u', CONST.USERNAME_LEN).ToString();

            //ATTEMPT
            var ownerTestResult = Owner.CreateOwner(firstName, lastName, email, userName, CreateStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_Fail(ownerTestResult);
            Assert.Null(ownerTestResult.Result);
        }

        [Fact]
        private void CreateOwner_EmailExcedeedLen()
        {
            //SETUP
            var firstName = new StringBuilder().Append('f', Owner.CONST.FIRST_NAME_LEN).ToString();
            var lastName = new StringBuilder().Append('l', Owner.CONST.LAST_NAME_LEN).ToString();
            var email = new StringBuilder().Append('e', Owner.CONST.EMAIL_LEN + 1).ToString();
            var userName = new StringBuilder().Append('u', CONST.USERNAME_LEN).ToString();

            //ATTEMPT
            var ownerTestResult = Owner.CreateOwner(firstName, lastName, email, userName, CreateStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_Fail(ownerTestResult);
            Assert.Null(ownerTestResult.Result);
        }

        [Fact]
        private void CreateOwner_UserNameExcedeedLen()
        {
            //SETUP
            var firstName = new StringBuilder().Append('f', Owner.CONST.FIRST_NAME_LEN).ToString();
            var lastName = new StringBuilder().Append('l', Owner.CONST.LAST_NAME_LEN).ToString();
            var email = new StringBuilder().Append('e', Owner.CONST.EMAIL_LEN).ToString();
            var userName = new StringBuilder().Append('u', CONST.USERNAME_LEN + 1).ToString();

            //ATTEMPT
            var ownerTestResult = Owner.CreateOwner(firstName, lastName, email, userName, CreateStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_Fail(ownerTestResult);
            Assert.Null(ownerTestResult.Result);
        }
        #endregion

        #region SoftDelete_Update
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        private void SoftDelete_Update_WithNullOrWhiteSpace(string userName)
        {
            //SETUP
            var ownerTest = new OwnerFactory.SimpleOwner(1).Own;

            //ATTEMPT
            var ownerTestResult = ownerTest.SoftDelete_Update(false, userName, DeleteStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_Fail(ownerTestResult);
            Assert.False(ownerTest.IsDeleted);
            Assert.Null(ownerTest.DeletedDateUtc);
            Assert.Null(ownerTest.DeletedByUserName);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        private void SoftDelete_Update_AlterMultipleTime(int alterCount)
        {
            //SETUP
            var ownerTest = new OwnerFactory.SimpleOwner(1).Own;


            //ATTEMPT
            for (int i = 0; i <= alterCount; i++)
                ownerTest.SoftDelete_Update(!ownerTest.IsDeleted, DeleteStub.UserNameStub, DeleteStub.ActualDateTimeStub);

            //VERIFY
            Assert.Equal((alterCount & 1) == 1, !ownerTest.IsDeleted);
            Assert.Equal(DeleteStub.UserNameStub, ownerTest.DeletedByUserName);
            Assert.Equal(DeleteStub.ActualDateTimeStub.GetActualDateTime(), ownerTest.DeletedDateUtc);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        private void SoftDelete_Update_SetTheSameValue(bool softDeleted)
        {
            //SETUP
            var ownerTest = new OwnerFactory.SimpleOwner(1).Own;


            if (ownerTest.IsDeleted != softDeleted)
                ownerTest.SoftDelete_Update(!ownerTest.IsDeleted, CreateStub.UserNameStub, CreateStub.ActualDateTimeStub);

            //ATTEMPT
            ownerTest.SoftDelete_Update(softDeleted, DeleteStub.UserNameStub, DeleteStub.ActualDateTimeStub);

            //VERIFY
            Assert.Equal(softDeleted, ownerTest.IsDeleted);
            Assert.NotEqual(DeleteStub.UserNameStub, ownerTest.DeletedByUserName);
            Assert.NotEqual(DeleteStub.ActualDateTimeStub.GetActualDateTime(), ownerTest.DeletedDateUtc);
        }

        [Fact]
        private void SoftDelete_Update_UserNameExcedeedLen()
        {
            //SETUP
            var userName = new StringBuilder().Append('u', CONST.USERNAME_LEN + 1).ToString();
            var ownerTest = new OwnerFactory.SimpleOwner(1).Own;


            //ATTEMPT
            var ownerTestResult = ownerTest.SoftDelete_Update(true, userName, DeleteStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_Fail(ownerTestResult);
            Assert.Equal(false, ownerTest.IsDeleted);
            Assert.NotEqual(DeleteStub.UserNameStub, ownerTest.DeletedByUserName);
            Assert.NotEqual(DeleteStub.ActualDateTimeStub.GetActualDateTime(), ownerTest.DeletedDateUtc);
        }
        #endregion

        #region FirstName_Update
        [Theory]
        [InlineData("NEW_FIRST_NAME", null)]
        [InlineData("NEW_FIRST_NAME", "")]
        [InlineData(null, "USER_NAME")]
        [InlineData("", "USER_NAME")]
        private void FirstName_Update_WithNullOrWhiteSpace(string newFirstName, string userName)
        {
            //SETUP
            var ownerTestInit = new OwnerFactory.SimpleOwner(1);

            //ATTEMPT
            var ownerTestResult = ownerTestInit.Own.FirstName_Update(newFirstName, userName, UpdateStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_Fail(ownerTestResult);
            Assert.Equal(ownerTestInit.FirstNameInit, ownerTestInit.Own.FirstName);
            Assert.Null(ownerTestInit.Own.UpdatedByUserName);
            Assert.Null(ownerTestInit.Own.UpdatedDateUtc);
        }

        [Fact]
        private void FirstName_Update_Correct()
        {
            //SETUP
            string new_FirstName = "NEW_FIRST_NAME";
            var ownerTestInit = new OwnerFactory.SimpleOwner(1);

            //ATTEMPT
            var ownerTestResult = ownerTestInit.Own.FirstName_Update(new_FirstName, UpdateStub.UserNameStub, UpdateStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_OK(ownerTestResult);
            Assert.Equal(new_FirstName, ownerTestInit.Own.FirstName);
            Assert.Equal(UpdateStub.UserNameStub, ownerTestInit.Own.UpdatedByUserName);
            Assert.Equal(UpdateStub.ActualDateTimeStub.GetActualDateTime(), ownerTestInit.Own.UpdatedDateUtc);
        }

        [Fact]
        private void FirstName_Update_FirstNameExcedeedLen()
        {
            //SETUP
            var new_FirstName = new StringBuilder().Append('f', Owner.CONST.FIRST_NAME_LEN + 1).ToString();
            var userName = new StringBuilder().Append('u', CONST.USERNAME_LEN).ToString();
            var ownerTestInit = new OwnerFactory.SimpleOwner(1);

            //ATTEMPT
            var ownerTestResult = ownerTestInit.Own.FirstName_Update(new_FirstName, userName, UpdateStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_Fail(ownerTestResult);
            Assert.Equal(ownerTestInit.FirstNameInit, ownerTestInit.Own.FirstName);
            Assert.Null(ownerTestInit.Own.UpdatedByUserName);
            Assert.Null(ownerTestInit.Own.UpdatedDateUtc);
        }

        [Fact]
        private void FirstName_Update_UserNameExcedeedLen()
        {
            //SETUP
            var new_FirstName = new StringBuilder().Append('f', Owner.CONST.FIRST_NAME_LEN).ToString();
            var userName = new StringBuilder().Append('u', CONST.USERNAME_LEN + 1).ToString();
            var ownerTestInit = new OwnerFactory.SimpleOwner(1);

            //ATTEMPT
            var ownerTestResult = ownerTestInit.Own.FirstName_Update(new_FirstName, userName, UpdateStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_Fail(ownerTestResult);
            Assert.Equal(ownerTestInit.FirstNameInit, ownerTestInit.Own.FirstName);
            Assert.Null(ownerTestInit.Own.UpdatedByUserName);
            Assert.Null(ownerTestInit.Own.UpdatedDateUtc);
        }
        #endregion

        #region LastName_Update
        [Theory]
        [InlineData("NEW_LAST_NAME", null)]
        [InlineData("NEW_LAST_NAME", "")]
        [InlineData(null, "USER_NAME")]
        [InlineData("", "USER_NAME")]
        private void LastName_Update_WithNullOrWhiteSpace(string newLastName, string userName)
        {
            //SETUP
            var ownerTestInit = new OwnerFactory.SimpleOwner(1);

            //ATTEMPT
            var ownerTestResult = ownerTestInit.Own.LastName_Update(newLastName, userName, UpdateStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_Fail(ownerTestResult);
            Assert.Equal(ownerTestInit.LastNameInit, ownerTestInit.Own.LastName);
            Assert.Null(ownerTestInit.Own.UpdatedByUserName);
            Assert.Null(ownerTestInit.Own.UpdatedDateUtc);
        }

        [Fact]
        private void LastName_Update_Correct()
        {
            //SETUP
            string new_LastName = "NEW_LAST_NAME";
            var ownerTestInit = new OwnerFactory.SimpleOwner(1);

            //ATTEMPT
            var ownerTestResult = ownerTestInit.Own.LastName_Update(new_LastName, UpdateStub.UserNameStub, UpdateStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_OK(ownerTestResult);
            Assert.Equal(new_LastName, ownerTestInit.Own.LastName);
            Assert.Equal(UpdateStub.UserNameStub, ownerTestInit.Own.UpdatedByUserName);
            Assert.Equal(UpdateStub.ActualDateTimeStub.GetActualDateTime(), ownerTestInit.Own.UpdatedDateUtc);
        }

        [Fact]
        private void LastName_Update_LastNameExcedeedLen()
        {
            //SETUP
            string new_LastName = new StringBuilder().Append('l', Owner.CONST.FIRST_NAME_LEN + 1).ToString();
            var userName = new StringBuilder().Append('u', CONST.USERNAME_LEN).ToString();
            var ownerTestInit = new OwnerFactory.SimpleOwner(1);

            //ATTEMPT
            var ownerTestResult = ownerTestInit.Own.LastName_Update(new_LastName, userName, UpdateStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_Fail(ownerTestResult);
            Assert.Equal(ownerTestInit.LastNameInit, ownerTestInit.Own.LastName);
            Assert.Null(ownerTestInit.Own.UpdatedByUserName);
            Assert.Null(ownerTestInit.Own.UpdatedDateUtc);
        }

        [Fact]
        private void LastName_Update_UserNameExcedeedLen()
        {
            //SETUP
            string new_LastName = new StringBuilder().Append('l', Owner.CONST.FIRST_NAME_LEN).ToString();
            var userName = new StringBuilder().Append('u', CONST.USERNAME_LEN + 1).ToString();
            var ownerTestInit = new OwnerFactory.SimpleOwner(1);

            //ATTEMPT
            var ownerTestResult = ownerTestInit.Own.LastName_Update(new_LastName, userName, UpdateStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_Fail(ownerTestResult);
            Assert.Equal(ownerTestInit.LastNameInit, ownerTestInit.Own.LastName);
            Assert.Null(ownerTestInit.Own.UpdatedByUserName);
            Assert.Null(ownerTestInit.Own.UpdatedDateUtc);
        }
        #endregion

        #region Email_Update
        [Theory]
        [InlineData("NEW_EMAIL", null)]
        [InlineData("NEW_EMAIL", "")]
        [InlineData(null, "USER_NAME")]
        [InlineData("", "USER_NAME")]
        private void Email_Update_WithNullOrWhiteSpace(string newEmail, string userName)
        {
            //SETUP
            var ownerTestInit = new OwnerFactory.SimpleOwner(1);

            //ATTEMPT
            var ownerTestResult = ownerTestInit.Own.Email_Update(newEmail, userName, UpdateStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_Fail(ownerTestResult);
            Assert.Equal(ownerTestInit.EmailInit, ownerTestInit.Own.Email);
            Assert.Null(ownerTestInit.Own.UpdatedByUserName);
            Assert.Null(ownerTestInit.Own.UpdatedDateUtc);
        }

        [Fact]
        private void Email_Update_Correct()
        {
            //SETUP
            string new_Email = "NEW_EMAIL";
            var ownerTestInit = new OwnerFactory.SimpleOwner(1);

            //ATTEMPT
            var ownerTestResult = ownerTestInit.Own.Email_Update(new_Email, UpdateStub.UserNameStub, UpdateStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_OK(ownerTestResult);
            Assert.Equal(new_Email, ownerTestInit.Own.Email);
            Assert.Equal(UpdateStub.UserNameStub, ownerTestInit.Own.UpdatedByUserName);
            Assert.Equal(UpdateStub.ActualDateTimeStub.GetActualDateTime(), ownerTestInit.Own.UpdatedDateUtc);
        }

        [Fact]
        private void Email_Update_EmailExcedeedLen()
        {
            //SETUP
            var ownerTestInit = new OwnerFactory.SimpleOwner(1);
            var newEmail = new StringBuilder().Append('e', Owner.CONST.EMAIL_LEN + 1).ToString();
            var userName = new StringBuilder().Append('u', CONST.USERNAME_LEN).ToString();

            //ATTEMPT
            var ownerTestResult = ownerTestInit.Own.Email_Update(newEmail, userName, UpdateStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_Fail(ownerTestResult);
            Assert.Equal(ownerTestInit.EmailInit, ownerTestInit.Own.Email);
            Assert.Null(ownerTestInit.Own.UpdatedByUserName);
            Assert.Null(ownerTestInit.Own.UpdatedDateUtc);
        }

        [Fact]
        private void Email_Update_UserNameExcedeedLen()
        {
            //SETUP
            var ownerTestInit = new OwnerFactory.SimpleOwner(1);
            var newEmail = new StringBuilder().Append('e', Owner.CONST.EMAIL_LEN).ToString();
            var userName = new StringBuilder().Append('u', CONST.USERNAME_LEN + 1).ToString();

            //ATTEMPT
            var ownerTestResult = ownerTestInit.Own.Email_Update(newEmail, userName, UpdateStub.ActualDateTimeStub);

            //VERIFY
            AssertExt.StatusValidator_Fail(ownerTestResult);
            Assert.Equal(ownerTestInit.EmailInit, ownerTestInit.Own.Email);
            Assert.Null(ownerTestInit.Own.UpdatedByUserName);
            Assert.Null(ownerTestInit.Own.UpdatedDateUtc);
        }
        #endregion

        #region OwnedCategories_Add
        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        private void OwnedCategories_Add_WithNullOrWhiteSpace(bool categoryNull, bool categoryTypeNull)
        {
            //SETUP
            var ownerTest = new OwnerFactory.SimpleOwner(1);
            var category = categoryNull 
                ? null 
                : new CategoryFactory.SimpleCategory(1).Cat;
            var ownedType = categoryTypeNull 
                ? null 
                : OwnerType.GetSupplyChainType();

            //ATTEMPT
            var ownerTestResult = ownerTest.Own.OwnedCategories_Add(category, ownedType);

            //VERIFY
            AssertExt.StatusValidator_Fail(ownerTestResult);
            Assert.Equal(0, ownerTest.Own.CategoriesOwnedLinks.Count);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        private void OwnedCategories_Add_CategoryAlreadyOwned(bool theSameOwnedType)
        {
            //SETUP
            var ownerTestInit = new OwnerFactory.SimpleOwner(1);
            var category = new CategoryFactory.SimpleCategory(1).Cat;
            var ownedType = OwnerType.GetSupplyChainType();
            var new_OwnedType = OwnerType.GetCopywriterType();
            ownerTestInit.Own.OwnedCategories_Add(category, ownedType);

            //ATTEMPT
            var ownerTestResult = ownerTestInit.Own.OwnedCategories_Add(category, theSameOwnedType ? ownedType : new_OwnedType);

            //VERIFY
            AssertExt.StatusValidator_Fail(ownerTestResult);
            Assert.Equal(1, ownerTestInit.Own.CategoriesOwnedLinks.Count);
            Assert.True(ownerTestInit.Own.CategoriesOwnedLinks
                .Where(ol => ol.Category == category && ol.Type == ownedType)
                .Any());
        }

        [Fact]
        private void OwnedCategories_Add_Correct()
        {
            //SETUP
            var ownerTestInit = new OwnerFactory.SimpleOwner(1);
            var category = new CategoryFactory.SimpleCategory(1).Cat;
            var ownedType = OwnerType.GetSupplyChainType();

            //ATTEMPT
            var ownerTestResult = ownerTestInit.Own.OwnedCategories_Add(category, ownedType);

            //VERIFY
            AssertExt.StatusValidator_OK(ownerTestResult);
            Assert.Equal(1, ownerTestInit.Own.CategoriesOwnedLinks.Count);
            Assert.True(ownerTestInit.Own.CategoriesOwnedLinks
                .Where(ol => ol.Category == category && ol.Type == ownedType)
                .Any());
        }
        #endregion

        #region OwnedCategories_Update
        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        private void OwnedCategories_Update_WithNullOrWhiteSpace(bool categoryNull, bool categoryTypeNull)
        {
            //SETUP
            var ownerTestInit = new OwnerFactory.SimpleOwner(1);
            var category = new CategoryFactory.SimpleCategory(1).Cat;
            var ownedType = OwnerType.GetSupplyChainType();

            ownerTestInit.Own.OwnedCategories_Add(category, ownedType);

            //ATTEMPT
            var ownerTestResult = ownerTestInit.Own.OwnedCategories_Update(categoryNull ? null : category, categoryTypeNull ? null : ownedType);

            //VERIFY
            AssertExt.StatusValidator_Fail(ownerTestResult);
            Assert.Equal(1, ownerTestInit.Own.CategoriesOwnedLinks.Count);
            Assert.True(ownerTestInit.Own.CategoriesOwnedLinks
                .Where(ol => ol.Category == category && ol.Type == ownedType)
                .Any());
        }

        [Fact]
        private void OwnedCategories_Update_CategoryNotOwned()
        {
            //SETUP
            var ownerTestInit = new OwnerFactory.SimpleOwner(1);
            var category = new CategoryFactory.SimpleCategory(1).Cat;
            var ownedType = OwnerType.GetSupplyChainType();

            //ATTEMPT
            var ownerTestResult = ownerTestInit.Own.OwnedCategories_Update(category, ownedType);

            //VERIFY
            AssertExt.StatusValidator_Fail(ownerTestResult);
            Assert.Equal(0, ownerTestInit.Own.CategoriesOwnedLinks.Count);
        }

        [Fact]
        private void OwnedCategories_Update_Correct()
        {
            //SETUP
            var ownerTestInit = new OwnerFactory.SimpleOwner(1);
            var category = new CategoryFactory.SimpleCategory(1).Cat;
            var ownedType = OwnerType.GetSupplyChainType();
            var newOwnedType = OwnerType.GetCopywriterType();

            ownerTestInit.Own.OwnedCategories_Add(category, ownedType);

            //ATTEMPT
            var ownerTestResult = ownerTestInit.Own.OwnedCategories_Update(category, newOwnedType);

            //VERIFY
            AssertExt.StatusValidator_OK(ownerTestResult);
            Assert.Equal(1, ownerTestInit.Own.CategoriesOwnedLinks.Count);
            Assert.True(ownerTestInit.Own.CategoriesOwnedLinks
                .Where(ol => ol.Category == category && ol.Type == newOwnedType)
                .Any());
        }
        #endregion

        #region OwnedCategories_Delete
        [Fact]
        private void OwnedCategories_Delete_WithNull()
        {
            //SETUP
            var ownerTestInit = new OwnerFactory.SimpleOwner(1);
            var category = new CategoryFactory.SimpleCategory(1).Cat;
            var ownedType = OwnerType.GetSupplyChainType();
            ownerTestInit.Own.OwnedCategories_Add(category, ownedType);

            //ATTEMPT
            var ownerTestResult = ownerTestInit.Own.OwnedCategories_Delete(null);

            //VERIFY
            AssertExt.StatusValidator_Fail(ownerTestResult);
            Assert.Equal(1, ownerTestInit.Own.CategoriesOwnedLinks.Count);
            Assert.True(ownerTestInit.Own.CategoriesOwnedLinks
                .Where(ol => ol.Category == category && ol.Type == ownedType)
                .Any());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        private void OwnedCategories_Delete_NotContains(bool categoryListEmpty)
        {
            //SETUP
            var ownerTestInit = new OwnerFactory.SimpleOwner(1);
            var dell_category = new CategoryFactory.SimpleCategory(1).Cat;
            var ownedType = OwnerType.GetSupplyChainType();
            var start_category = new CategoryFactory.SimpleCategory(2).Cat;

            if (!categoryListEmpty) 
                ownerTestInit.Own.OwnedCategories_Add(start_category, ownedType);

            var countBeforeAttempt = ownerTestInit.Own.CategoriesOwnedLinks.Count;

            //ATTEMPT
            var ownerTestResult = ownerTestInit.Own.OwnedCategories_Delete(dell_category);

            //VERIFY
            AssertExt.StatusValidator_Fail(ownerTestResult);
            Assert.Equal(countBeforeAttempt, ownerTestInit.Own.CategoriesOwnedLinks.Count);

            if(countBeforeAttempt > 0)
                Assert.True(ownerTestInit.Own.CategoriesOwnedLinks
                    .Where(ol => ol.Category == start_category && ol.Type == ownedType)
                    .Any());
        }

        [Fact]
        private void OwnedCategories_Delete_Correct()
        {
            //SETUP
            var ownerTestInit = new OwnerFactory.SimpleOwner(1);
            var category = new CategoryFactory.SimpleCategory(1).Cat;
            var ownedType = OwnerType.GetSupplyChainType();
            ownerTestInit.Own.OwnedCategories_Add(category, ownedType);

            //ATTEMPT
            var ownerTestResult = ownerTestInit.Own.OwnedCategories_Delete(category);

            //VERIFY
            AssertExt.StatusValidator_OK(ownerTestResult);
            Assert.Equal(0, ownerTestInit.Own.CategoriesOwnedLinks.Count);
        }
        #endregion
    }
}
