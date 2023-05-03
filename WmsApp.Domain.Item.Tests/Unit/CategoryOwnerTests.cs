using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WmsApp.Domain.Items.Tests.Factories;
using WmsApp.Tests.Common.Extensions;

namespace WmsApp.Domain.Items.Tests.Unit
{
    public class CategoryOwnerTests
    {
        [Fact]
        private void CategoryOwner_Ctor()
        {
            //SETUP
            var category = new CategoryFactory.SimpleCategory(1).Cat;
            var owner = new OwnerFactory.SimpleOwner(1).Own;
            var type = OwnerType.GetCopywriterType();

            //ATTEMPT
            var catOwnerTest = new CategoryOwner(category, owner, type);

            //VERIFY
            Assert.Equal(category, catOwnerTest.Category);
            Assert.Equal(owner, catOwnerTest.Owner);
            Assert.Equal(type, catOwnerTest.Type);
        }

        [Fact]
        private void Type_Update_WithNullOrWhiteSpace()
        {
            //SETUP
            var category = new CategoryFactory.SimpleCategory(1).Cat;
            var owner = new OwnerFactory.SimpleOwner(1).Own;
            var type = OwnerType.GetCopywriterType();
            var catOwnerTest = new CategoryOwner(category, owner, type);

            //ATTEMPT
            var catOwnerTestResult = catOwnerTest.Type_Update(null);

            //VERIFY
            AssertExt.StatusValidator_Fail(catOwnerTestResult);
            Assert.Equal(type, catOwnerTest.Type);
        }

        [Fact]
        private void Type_Update_Correct()
        {
            //SETUP
            var category = new CategoryFactory.SimpleCategory(1).Cat;
            var owner = new OwnerFactory.SimpleOwner(1).Own;
            var type = OwnerType.GetCopywriterType();
            var new_Type = OwnerType.GetDirectorType();
            var catOwnerTest = new CategoryOwner(category, owner, type);

            //ATTEMPT
            var catOwnerTestResult = catOwnerTest.Type_Update(new_Type);

            //VERIFY
            AssertExt.StatusValidator_OK(catOwnerTestResult);
            Assert.Equal(new_Type, catOwnerTest.Type);
        }
    }
}
