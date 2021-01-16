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
            bool converted = false;
            Dictionary<TypeCode, Func<dynamic>> parseActions = new Dictionary<TypeCode, Func<dynamic>>()
            {
                {TypeCode.String, () => { converted = true; return objToRead; }},
                {TypeCode.Int16, () => { converted = short.TryParse(objToRead, out var sh); return converted ? (dynamic) sh : objToRead; }},
                {TypeCode.Int32, () => { converted = int.TryParse(objToRead, out var i); return converted ? (dynamic) i : objToRead; }},
                {TypeCode.Int64, () => { converted = long.TryParse(objToRead, out var lo); return converted ? (dynamic) lo : objToRead; }},
                {TypeCode.Double, () => { converted = double.TryParse(objToRead, out var d); return converted ? (dynamic) d : objToRead; }},
                {TypeCode.Single, () => { converted = float.TryParse(objToRead, out var f); return converted ? (dynamic) f : objToRead; }},
                {TypeCode.Boolean, () => { converted = bool.TryParse(objToRead, out var tf); return converted ? (dynamic) tf : objToRead; }},
                {TypeCode.Char, () => { converted = char.TryParse(objToRead, out var c); return converted ? (dynamic) c : objToRead; }},
                {TypeCode.Byte, () => { converted = byte.TryParse(objToRead, out var by); return converted ? (dynamic) @by : objToRead; }},
                {TypeCode.DateTime, () => { converted = DateTime.TryParse(objToRead, out var dt); return converted ? (dynamic) dt : objToRead; }},
            };

            value = parseActions[typeCode]();
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
        
        public static DateTime ReadDate(string date, string format, DateTimeStyles styles = DateTimeStyles.None)
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

        public static ConsoleColor ReadColor()
        {
            ConsoleColor col = default;
            int left = Console.CursorLeft;
            bool converted = false;

            do
            {
                try
                {
                    converted = Enum.TryParse(Console.ReadLine(), true, out col);
                    if (string.IsNullOrEmpty(col.ToString()) || string.IsNullOrWhiteSpace(col.ToString()) || !converted)
                    {
                        Console.SetCursorPosition(left, Console.CursorTop - 1);
                        Console.Write(new string(' ', (col.ToString()).Length));
                        Console.SetCursorPosition(left, Console.CursorTop);
                    }
                }
                catch (Exception)
                { }
            } while (string.IsNullOrEmpty(col.ToString()) || string.IsNullOrWhiteSpace(col.ToString()) || !converted);
            
            return col;
        }
        
        public static dynamic ReadWithConditions(bool obeyAll, TypeCode typeCode, int cursorPosition, params Func<dynamic, bool>[] conditions)
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
                }

                if (!converted || typeCode != Type.GetTypeCode(obj.GetType())) continue;
                
                if (obeyAll && converted)
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
                    isValid = reg.Match(email ?? string.Empty).Success;

                    if (string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email) || !isValid)
                    {
                        Console.SetCursorPosition(left, Console.CursorTop - 1);
                        if (email != null) Console.Write(new string(' ', email.Length));
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
