using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Aibot
{
    public static class ExtensionHelper
    { 
            /// <summary>
            /// Query Url 转 字典
            /// </summary>
            /// <param name="query"></param>
            /// <returns></returns>
            public static Dictionary<string, string> ParseQueryString(this string query)
            {
                Dictionary<string, string> queryDict = new Dictionary<string, string>();
                foreach (string token in query.TrimStart(new char[] { '?' }).Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string[] parts = token.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                        queryDict[parts[0].Trim()] = HttpUtility.UrlDecode(parts[1]).Trim();
                    else
                        queryDict[parts[0].Trim()] = "";
                }
                return queryDict;
            }

            /// <summary>
            /// 枚举转字典
            /// </summary>
            /// <param name="enumType"></param>
            /// <returns></returns>
            public static Dictionary<T, string> ToEnumDictionary<T>(this Type enumType)
            {
                if (!enumType.IsEnum)
                {
                    throw new ArgumentException($"{enumType} is not an enum type.");
                }

                var values = Enum.GetValues(enumType).Cast<T>();
                var dictionary = values.ToDictionary(
                    value => value,
                    value => Enum.GetName(enumType, value)
                        ?? throw new ArgumentException($"Could not get name for value {value}.")
                );

                foreach (var field in enumType.GetFields())
                {
                    if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                    {
                        var value = (T)field.GetValue(null);
                        if (dictionary.ContainsKey(value))
                        {
                            dictionary[value] = attribute.Description;
                        }
                    }
                }

                return dictionary;
            }

            public static int IntTryParse(this string str)
            {
                if (int.TryParse(str, out int result))
                {
                    return result;
                }
                return 0;
            }

            public static List<T> ToList<T>(this object obj)
            {
                return JsonConvert.DeserializeObject<List<T>>(JsonConvert.SerializeObject(obj)) ?? new();
            }

            /// <summary>
            /// 转换 A Class to B Class
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="obj"></param>
            /// <returns></returns>
            public static T CastTo<T>(this object obj)
            {
                if (obj is string)
                {
                    return JsonConvert.DeserializeObject<T>((obj as string));
                }
                else
                {
                    return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj));
                }
            }

            /// <summary>
            /// class TO JsonString
            /// </summary>
            /// <returns></returns>
            public static string ToJsonString(this object obj)
            {
                IsoDateTimeConverter converter = new IsoDateTimeConverter();
                converter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

                if (obj is string)
                {
                    return obj as string ?? "";
                }
                return JsonConvert.SerializeObject(obj, Formatting.Indented, converter);
            }

            /// <summary>
            /// Clss to ToDictionary
            /// </summary>
            /// <typeparam name="TKey"></typeparam>
            /// <typeparam name="TValue"></typeparam>
            /// <param name="obj"></param>
            /// <returns></returns>
            public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this object obj) where TKey : notnull
            {
                return JsonConvert.DeserializeObject<Dictionary<TKey, TValue>>(obj.ToJsonString()) ?? new();
            }

            public static Dictionary<string, string> ToDictionary(this object obj)
            {
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(obj.ToJsonString()) ?? new();
            }

        /// <summary>
        /// object to dictionary<string, string> 
        /// </summary>
        /// <typeparam name="T">要转换的类型</typeparam>keyPrefix
        /// <param name="obj">对象</param>
        /// <param name="keyPrefix">key的前缀</param>
        /// <returns>小写字母的key字典</returns>
        public static Dictionary<string, string> ObjToDic<T>(this T obj,string keyPrefix = "") where T : class
        {
            Dictionary<string, string> map = new Dictionary<string, string>();
            if (obj == null)
            {
                return map;
            }
            Type t = obj.GetType();
            PropertyInfo[] PropertyList = t.GetProperties();
            foreach (var item in PropertyList)
            {
                string name = item.Name;
                object value = item.GetValue(obj, null);
                if (value != null)
                {
                    map.Add($"{(string.IsNullOrEmpty(keyPrefix)?"":string.Concat(keyPrefix,"_"))}{name}".ToLower(), value.ToString());
                }
                else
                {
                    map.Add($"{(string.IsNullOrEmpty(keyPrefix) ? "" : string.Concat(keyPrefix, "_"))}{name}".ToLower(), "");
                }
            }
            return map;
        } 

        #region StringExtension
        /// <summary>
        /// 字符串是否为Null、空字符串组成。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 字符串是否为Null、空字符串或仅由空白字符组成。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static DateTime ToDateTime(this string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }

            return Convert.ToDateTime(str);
        }



        /// <summary>
        /// 从字符串的开头得到一个字符串的子串 len参数不能大于给定字符串的长度
        /// </summary>
        /// <param name="str"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static string Left(this string str, int len)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }

            if (str.Length < len)
            {
                throw new ArgumentException("len参数不能大于给定字符串的长度");
            }

            return str.Substring(0, len);
        }

        /// <summary>
        /// 从字符串的末尾得到一个字符串的子串 len参数不能大于给定字符串的长度
        /// </summary>
        /// <param name="str"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static string Right(this string str, int len)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }

            if (str.Length < len)
            {
                throw new ArgumentException("len参数不能大于给定字符串的长度");
            }

            return str.Substring(str.Length - len, len);
        }

        /// <summary>
        /// len参数大于给定字符串是返回原字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string MaxLeft(this string str, int len)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }

            if (str.Length < len)
            {
                return str;
            }

            return str.Substring(0, len);
        }

        /// <summary>
        /// 从指定位置截取字符串，如果小于指定位置，则返回空字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string Sub(this string str, int len)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }

            if (str.Length < len)
            {
                return string.Empty;
            }

            return str.Substring(len);
        }

        /// <summary>
        /// 从字符串的末尾得到一个字符串的子串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string MaxRight(this string str, int len)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }

            if (str.Length < len)
            {
                return str;
            }

            return str.Substring(str.Length - len, len);
        }

        //
        // 摘要:
        //     Bytes转String
        //
        // 参数:
        //   byteArray:
        public static string BytesToString(this byte[] byteArray)
        {
            if (byteArray == null)
            {
                throw new ArgumentNullException("byteArray");
            }

            return Encoding.Default.GetString(byteArray);
        }

        //
        // 摘要:
        //     String转Bytes
        //
        // 参数:
        //   str:
        public static byte[] ToBytes(this string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }

            return Encoding.Default.GetBytes(str);
        }
        #endregion 
    }
}