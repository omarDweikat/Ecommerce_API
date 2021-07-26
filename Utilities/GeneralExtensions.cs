using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace Ecommerce_API.Utilities
{
    public static class GeneralExtensions
    {
        public static T SetIfNUll<T>(this T obj, T value)
        {
            return obj == null ? value : obj;
        }

        public static DataTable ToDataTable<T>(this T data)
        {
            return new List<T>() { data }.ToDataTable();
        }

        public static DataTable ToDataTable<T>(this IEnumerable<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        public static string HashStream(this byte[] stream)
        {
            var sha512 = new SHA512Managed();
            sha512.Initialize();
            var hash = sha512.ComputeHash(stream);
            return Encoding.UTF8.GetString(hash);
        }
    }
}