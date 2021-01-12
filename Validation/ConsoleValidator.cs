using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Validation
{
    public static class ConsoleValidator
    {
        private static bool Parse(string objToRead, TypeCode typeCode, out dynamic value)
        {
            dynamic conversion = null;
            bool converted = false;
            
            switch (typeCode)
            {
                case TypeCode.String:
                    conversion = objToRead;
                    converted = true;
                    break;
                
                case TypeCode.Int32:
                    converted = int.TryParse(objToRead, out int i);
                    if (converted) conversion = i; else conversion = objToRead;
                    break;
                        
                case TypeCode.Double:
                    converted = double.TryParse(objToRead, out double d);
                    if (converted) conversion = d; else conversion = objToRead;
                    break;
                        
                case TypeCode.Single:
                    converted = float.TryParse(objToRead, out float f);
                    if (converted) conversion = f; else conversion = objToRead;
                    break;
                
                case TypeCode.Boolean:
                    converted = bool.TryParse(objToRead, out bool b);
                    if (converted) conversion = b; else conversion = objToRead;
                    break;
            }
            
            value = conversion;
            return converted;
        }
        
        private static bool ParseDate(string strDate, string format, out DateTime date, DateTimeStyles timeStyles = DateTimeStyles.None)
        {
            DateTime conversion;
            bool converted = false;

            converted = DateTime.TryParseExact(strDate, format, null, timeStyles, out DateTime d);
            if (converted) conversion = d; else conversion = DateTime.MinValue;

            date = conversion;
            return converted;
        }

        public static dynamic Read(TypeCode typeCode)
        {
            dynamic obj = null;
            int left = Console.CursorLeft;
            bool converted = false;
            
            do
            {
                try
                {
                    converted = Parse(Console.ReadLine(), typeCode, out obj);
                    if (string.IsNullOrEmpty(obj.ToString()) || string.IsNullOrWhiteSpace(obj.ToString()) || !converted)
                    {
                        Console.SetCursorPosition(left, Console.CursorTop - 1);
                        Console.Write(new string(' ', (obj.ToString()).Length));
                        Console.SetCursorPosition(left, Console.CursorTop);
                    }
                }
                catch (Exception)
                { }
            } while (string.IsNullOrEmpty(obj.ToString()) || string.IsNullOrWhiteSpace(obj.ToString()) || !converted);
            return obj;
        }
        
        public static DateTime ReadDateTime(string date, string format, DateTimeStyles styles = DateTimeStyles.None)
        {
            DateTime dateObj = default;
            int left = Console.CursorLeft;
            bool converted = false;
            
            try
            {
                converted = ParseDate(date, format, out dateObj, styles);
                if (string.IsNullOrEmpty(dateObj.ToString()) || string.IsNullOrWhiteSpace(dateObj.ToString()) || !converted)
                {
                    Console.SetCursorPosition(left, Console.CursorTop - 1);
                    Console.Write(new string(' ', (dateObj.ToString()).Length));
                    Console.SetCursorPosition(left, Console.CursorTop);
                }
            }
            catch (Exception)
            { }

            return dateObj;
        }
        
        public static dynamic ReadWithConditions(bool obeyAll, TypeCode typeCode, int cursorPosition, params Func<int, bool>[] conditions)
        {
            dynamic obj = null;
            bool obeysConditions = false;
            bool converted = false;
            
            do
            {
                converted = Parse(Console.ReadLine(), typeCode, out obj);
                if (string.IsNullOrEmpty(obj.ToString()) && string.IsNullOrWhiteSpace(obj.ToString()))
                {
                    Console.SetCursorPosition(cursorPosition, Console.CursorTop - 1);
                    Console.Write(new string(' ', (obj.ToString()).Length));
                    Console.SetCursorPosition(cursorPosition, Console.CursorTop);
                    continue;
                };
                
                if (!converted) continue;
                
                if (obeyAll)
                {
                    foreach (var condition in conditions)
                    {
                        if (condition(obj)) obeysConditions = true;
                        else
                        {
                            obeysConditions = false;
                            break;
                        }
                    }
                }
                else
                {
                    foreach (var condition in conditions)
                    {
                        obeysConditions = condition(obj);
                        break;
                    }
                }
            } while (string.IsNullOrEmpty(obj.ToString()) || string.IsNullOrWhiteSpace(obj.ToString()));
            
            if (string.IsNullOrEmpty(obj.ToString()) || string.IsNullOrWhiteSpace(obj.ToString()) || !obeysConditions)
            {
                Console.SetCursorPosition(cursorPosition, Console.CursorTop - 1);
                Console.Write(new string(' ', (obj.ToString()).Length));
                Console.SetCursorPosition(cursorPosition, Console.CursorTop);
            }
            
            if (!obeysConditions)
                obj = ReadWithConditions(obeyAll, typeCode, cursorPosition, conditions);

            return obj;
        }
        
        public static string ReadEmail()
        {
            Regex reg = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,})+)$");
            bool isValid = false;

            int left = Console.CursorLeft;
            string email = string.Empty;
            do
            {
                try
                {
                    email = Console.ReadLine();
                    isValid = reg.Match(email).Success;

                    if (string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email) || !isValid)
                    {
                        Console.SetCursorPosition(left, Console.CursorTop - 1);
                        Console.Write(new string(' ', email.Length));
                        Console.SetCursorPosition(left, Console.CursorTop);
                    }
                }
                catch (Exception )
                { }

            } while ((string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email)) || !isValid);

            return email;
        }
    }
}
