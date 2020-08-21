using System;
namespace Project.Webframeworks.Api
{
    public static class Routes
    {
        public const string BaseRootAddress = "api/v{version:apiVersion}/[controller]";

        public static class Users
        {
            public static string RootAddress = "";
            public static class Get
            {

            }
        }

        //Test routes
        public static class MyEntity
        {
            public const string RootAddress = "entities";
        }
    }
}
