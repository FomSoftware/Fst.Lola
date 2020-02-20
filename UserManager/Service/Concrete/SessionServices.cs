namespace UserManager.Service.Concrete
{
    public class SessionServices
    {
        #region Common Method

        public static void SetValue<T>(string key, T value)
        {
            System.Web.HttpContext.Current.Session[key] = value;
        }

        public static void RemoveValue(string key)
        {
            System.Web.HttpContext.Current.Session.Remove(key);
        }

        public static T GetValue<T>(string key)
        {
            if (System.Web.HttpContext.Current.Session != null && null != System.Web.HttpContext.Current.Session[key])
            {
                return (T)System.Web.HttpContext.Current.Session[key];
            }
            else
            {
                return default(T);
            }
        }

        public static void Clear()
        {
            System.Web.HttpContext.Current.Session.Clear();
            System.Web.HttpContext.Current.Session.Abandon();
        }

        #endregion
    }
}
