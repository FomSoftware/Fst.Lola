using FomMonitoringCore.Service.API.Concrete;
using FomMonitoringResources;
using Mapster;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Resources;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http.Filters;
using System.Web.Mvc;

namespace FomMonitoringCore.Framework.Common
{
    public static class ExtensionMethods
    {

        #region API AUTHENTICATION

        public static void ChallengeWith(this HttpAuthenticationChallengeContext context, string scheme)
        {
            ChallengeWith(context, new AuthenticationHeaderValue(scheme));
        }

        public static void ChallengeWith(this HttpAuthenticationChallengeContext context, string scheme, string parameter)
        {
            ChallengeWith(context, new AuthenticationHeaderValue(scheme, parameter));
        }

        public static void ChallengeWith(this HttpAuthenticationChallengeContext context, AuthenticationHeaderValue challenge)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.Result = new AddChallengeOnUnauthorizedResult(challenge, context.Result);
        }

        #endregion

        /// <summary>
        /// Get enumerator description
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Enumerator description</returns>
        public static string GetDescription(this Enum value)
        {
            var da = (DescriptionAttribute[])(value.GetType().GetField(value.ToString())).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return da.Length > 0 ? da[0].Description : value.ToString();
        }

        public static string GetController(this Enum value)
        {
            var da = (ControllerAttribute[])(value.GetType().GetField(value.ToString())).GetCustomAttributes(typeof(ControllerAttribute), false);
            return da.Length > 0 ? da[0].Controller : value.ToString();
        }

        /// <summary>
        /// Get enumerator label
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Enumerator label</returns>
        public static string GetLabel(this Enum value)
        {
            var da = (LabelAttribute[])(value.GetType().GetField(value.ToString())).GetCustomAttributes(typeof(LabelAttribute), false);
            return da.Length > 0 ? da[0].Text : value.ToString();
        }



        /// <summary>
        /// Encript string with SHA256
        /// </summary>
        /// <param name="value"></param>
        /// <returns>String encripted</returns>
        public static string SHA256Encript(this string value)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(value);
            SHA256 mySHA256 = SHA256.Create();
            byte[] hashValue = mySHA256.ComputeHash(plainTextBytes);
            string result = "";
            foreach (byte element in hashValue)
            {
                result += string.Format("{0:X2}", element);
            }
            return result;
        }

        /// <summary>
        /// Get enumerator from enumerator attribute value. Example: 
        /// <para>stringVariable.GetValueFromAttribute&#60;enumTypeName, DescriptionAttribute>(a => a.Description);</para>
        /// </summary>
        /// <param name="text"></param>
        /// <param name="valueFunc"></param>
        /// <returns>Enumerator label</returns>
        public static TEnum GetValueFromAttribute<TEnum, TAttribute>(this string text, Func<TAttribute, string> valueFunc) where TAttribute : Attribute
        {
            var type = typeof(TEnum);
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field, typeof(TAttribute)) as TAttribute;
                if (attribute != null)
                {
                    if (valueFunc.Invoke(attribute) == text)
                    {
                        return (TEnum)field.GetValue(null);
                    }
                }
                else
                {
                    if (field.Name == text)
                    {
                        return (TEnum)field.GetValue(null);
                    }
                }
            }
            throw new ArgumentException("Not found.", "text");
            // or return default(T);
        }



        /// <summary>
        /// Extension method to handle localized URL using a dedicated, multi-language custom route.
        /// for additional info, read the following post:
        /// https://www.ryadel.com/en/setup-a-multi-language-website-using-asp-net-mvc/
        /// </summary>
        public static string Action(this UrlHelper helper, string actionName, string controllerName, object routeValues, CultureInfo cultureInfo)
        {
            string result = string.Empty;
            if (cultureInfo == null)
            {
                cultureInfo = CultureInfo.CurrentCulture;
            }

            result = helper.RouteUrl("DefaultLocalized", new { lang = cultureInfo.TwoLetterISOLanguageName, action = actionName, controller = controllerName });
            return result;
        }

        /// <summary>
        /// Convert period value to Datetime
        /// </summary>
        /// <param name="period"></param>
        /// <param name="typePeriod"></param>
        /// <returns></returns>
        public static DateTime? PeriodToDate(this int period, enAggregation typePeriod)
        {
            DateTime? result = null;
            int day = 0;
            int month = 0;
            int year = 0;
            switch (typePeriod)
            {
                case enAggregation.Day:
                    Math.DivRem(period, 10000, out year);
                    Math.DivRem(period % 10000, 100, out month);
                    day = period % 100;
                    result = new DateTime(year, month, day);
                    break;
                case enAggregation.Week:
                    Math.DivRem(period, 100, out year);
                    result = Common.StartOfWeek(year, period % 100, DayOfWeek.Monday);
                    break;
                case enAggregation.Month:
                    Math.DivRem(period, 100, out year);
                    month = period % 100;
                    day = 1;
                    result = new DateTime(year, month, day);
                    break;
            }
            return result;
        }

        /// <summary>
        /// Extension method to be used in MapsterConfig.cs to ignore to map virtual property
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static TypeAdapterSetter<TSource, TDestination> IgnoreAllVirtual<TSource, TDestination>(this TypeAdapterSetter<TSource, TDestination> expression)
        {
            var destType = typeof(TDestination);
            foreach (var property in destType.GetProperties().Where(p => p.GetGetMethod().IsVirtual))
            {
                expression.Ignore(property.Name);
            }
            return expression;
        }

        /// <summary>
        /// Convert enum to localized string value
        /// </summary>
        /// <param name="enumerator"></param>
        /// <returns></returns>
        public static string ToLocalizedString(this Enum enumerator, string initialsLanguage)
        {
            ResourceManager rm = new ResourceManager(typeof(Resource));
            CultureInfo cultureInfo = CultureInfo.GetCultureInfo(initialsLanguage);
            string resourceValue = rm.GetString(enumerator.ToString(), cultureInfo);
            return string.IsNullOrEmpty(resourceValue) || string.IsNullOrWhiteSpace(resourceValue) ? string.Format("{0}", enumerator.ToString().Trim()) : resourceValue;
        }

        /// <summary>
        /// Get error string with current class, method and parameters
        /// </summary>
        /// <param name="ex"></param>
        /// <returns>Error string with current class, method and parameters</returns>
        public static string GetStringLog(this Exception ex)
        {
            StackTrace st = new StackTrace(ex, true);
            StackFrame sf = st.GetFrames().Last();
            MethodBase method = sf.GetMethod();
            string parametersString = string.Empty;
            List<string> parameters = method.GetParameters().Length > 0 ? method.GetParameters().Select(s => s.Name).ToList() : new List<string>();
            int cont = 0;
            foreach (string parameter in parameters)
            {
                parametersString = string.Concat(parametersString, parameter, " = '{", cont++, "}', ");
            }
            parametersString = !string.IsNullOrEmpty(parametersString) ? parametersString.Substring(0, parametersString.Length - 2) : parametersString;
            string result = string.Concat(method.DeclaringType.Name,
                " => ", method.Name,
                "(",
                parametersString,
                ") at row ",
                sf.GetFileLineNumber().ToString(),
                " and column ",
                sf.GetFileColumnNumber().ToString()
            );
            return result;
        }

        public static int MonthDifference(this DateTime lValue, DateTime rValue)
        {
            return Math.Abs((lValue.Month - rValue.Month) + 12 * (lValue.Year - rValue.Year));
        }

        public static decimal MathRound(this decimal leftValue, int nDeciaml)
        {
            return Math.Round(leftValue, nDeciaml, MidpointRounding.AwayFromZero);
        }

        public static int RoundToInt(this decimal leftValue)
        {
            return (int)Math.Round(leftValue, 0, MidpointRounding.AwayFromZero);
        }

        public static int RoundToInt(this double leftValue)
        {
            return (int)Math.Round(leftValue, 0, MidpointRounding.AwayFromZero);
        }


    }

    public class ControllerAttribute:Attribute
    {
        protected string _controller;
        
        public ControllerAttribute(string v)
        {
            this._controller = v;
        }

        public string Controller
        {
            get { return _controller; }
            set { _controller = value; }
        }

    }

    public static class DbDateHelper
    {
        /// <summary>
        /// Replaces any date before 01.01.1753 with a Nullable of 
        /// DateTime with a value of null.
        /// </summary>
        /// <param name="date">Date to check</param>
        /// <returns>Input date if valid in the DB, or Null if date is 
        /// too early to be DB compatible.</returns>
        public static DateTime? ToNullIfTooEarlyForDb(this DateTime date)
        {
            return (date >= (DateTime)SqlDateTime.MinValue) ? date : (DateTime?)null;
        }
    }
}

