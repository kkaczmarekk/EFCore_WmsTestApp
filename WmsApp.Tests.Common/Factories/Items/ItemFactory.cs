using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WmsApp.Tests.Common.Stubs.CommonStubs;

namespace WmsApp.Domain.Items.Tests.Factories
{
    public static class ItemFactory
    {
        public class SimpleItem
        {
            public Item Itm { get; }

            public string NameInit { get; }
            public string DescInit { get; }
            public Category CatInit { get; }

            public SimpleItem(int id, Category cat)
            {
                Itm = Item.CreateItem("NAME_" + id, "DESC_" + id, cat, CreateStub.UserNameStub, CreateStub.ActualDateTimeStub).Result;

                NameInit = Itm.Name;
                DescInit = Itm.Description;
                CatInit = Itm.Category;
            }


        }
    }
}
