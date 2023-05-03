using WmsApp.Domain.Common.DateTimeGenerator;

namespace WmsApp.Tests.Common.Stubs
{
    public static class CommonStubs
    {
        public static class CreateStub
        {
            public static IActualDateTime ActualDateTimeStub = new FixedDateTimeGenerator(14, 04, 2023);
            public static string UserNameStub = "USERNAME_CREATE";
        }

        public static class UpdateStub
        {
            public static IActualDateTime ActualDateTimeStub = new FixedDateTimeGenerator(17, 04, 2023);
            public static string UserNameStub = "USERNAME_UPDATE";
        }

        public static class DeleteStub
        {
            public static IActualDateTime ActualDateTimeStub = new FixedDateTimeGenerator(20, 04, 2023);
            public static string UserNameStub = "USERNAME_DELETE";
        }
    }
}
