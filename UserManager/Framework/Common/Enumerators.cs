namespace UserManager.Framework.Common
{
    public static class Enumerators
    {
        //Common Role And Group

        public enum UserRole
        {
            NA = -1,
            Admin = 0,
            User = 1,
            Guest = 2,
            Assistance = 3,
            Customer = 4,
            UserApi = 5
        }

        public enum UserGroup
        {    
            NA = -1,
            Admin = 0,
            User = 1,
            Guest = 2
        }

        //--------------------
    }
}
