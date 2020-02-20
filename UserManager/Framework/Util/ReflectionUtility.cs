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
                PropertyInfo property = root.GetType().GetProperty(prop);
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

        //public Expression Eval(object root, string propertyString)
        //{
        //    var propertyNames = propertyString.Split('.');
        //    ParameterExpression param = Expression.Parameter(root.GetType(), "_");
        //    Expression property = param;
        //    foreach (var prop in propertyNames)
        //    {
        //        property = Expression.PropertyOrField(property, prop);
        //    }

        //    return Expression.Lambda(property, param);
        //}

        //public static Expression Eval(Expression expression, string property)
        //{
        //    if (property.IndexOf('.') > 0)
        //    {
        //        return Expression.PropertyOrField(expression, property);
        //    }
        //    else
        //    {
        //        return Eval(Expression.PropertyOrField(expression, property.Substring(0, property.IndexOf('.'))), property.Replace(property.Substring(0, property.IndexOf('.')) + ".", ""));
        //    }
        //}

    }
}
