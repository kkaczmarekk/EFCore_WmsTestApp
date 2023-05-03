using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WmsApp.Tests.Common.Stubs.CommonStubs;

namespace WmsApp.Tests.Common.Factories.Items
{
    public static class OwnerFactory
    {
        public class SimpleOwner
        {
            public Owner Own { get; }

            public string FirstNameInit { get; }
            public string LastNameInit { get; }
            public string EmailInit { get; }

            public SimpleOwner(int id)
            {
                Own = Owner.CreateOwner("FIRST_NAME_" + id, "LAST_NAME_" + id, "EMIAL_" + id, CreateStub.UserNameStub, CreateStub.ActualDateTimeStub).Result;

                FirstNameInit = Own.FirstName;
                LastNameInit = Own.LastName;
                EmailInit = Own.Email;
            }
        }

    }
}
