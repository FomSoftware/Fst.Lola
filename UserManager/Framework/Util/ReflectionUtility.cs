using System;
using System.Reflection;

namespace UserManager.Framework.Util
{
    public class ReflectionUtility
    {
        public static object EvalProperty(object root, string propertyString)
        {
            var propertyNames = propertyString.Split('.');
            foreach (var prop in propertyNames)
            {
                var property = root.GetType().GetProperty(prop);
                if (null == property) { continue; }
                root = property.GetValue(root, null);
            }
            return root;
        }

        public static void SetValueToProperty(object root, string propertyString, object propertyValue)
        {
            var propertyNames = propertyString.Split('.');
            foreach (var prop in propertyNames)
            {
                var property = root.GetType().GetProperty(prop);
                if (null == property) { continue; }

                property.SetValue(root, ConvertToPropertyType(property, propertyValue), null);
            }
        }

        private static object ConvertToPropertyType(PropertyInfo property, object propertyValue)
        {
            switch (property.PropertyType.FullName)
            {
                case "System.Boolean":
                    return Convert.ToBoolean(propertyValue);
                default:
                    return propertyValue;
            }
        }


    }
}
