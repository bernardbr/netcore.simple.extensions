namespace Dotnet.Simple.Extensions.Data
{
    using System;
    using System.Data;
    using System.Diagnostics;
    using System.Reflection;

    /// <summary>
    /// Extension of <see cref="IDataReader"/> with a set of simpple utilities.
    /// </summary>
    public static class DataReaderExtension
    {
        /// <summary>
        /// Get the field value by index.
        /// </summary>
        /// <param name="dataReader">
        /// The data reader.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <typeparam name="TType">
        /// The type of return.
        /// </typeparam>
        /// <returns>
        /// The field value.
        /// </returns>
        public static TType GetFieldValue<TType>(this IDataReader dataReader, int index)
        {
            return (TType)dataReader.GetFieldValue(typeof(TType), index);
        }

        /// <summary>
        /// Get the field value by name.
        /// </summary>
        /// <param name="dataReader">
        /// The data reader.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="fieldName">
        /// The field name.
        /// </param>
        /// <returns>
        /// The field value.
        /// </returns>
        public static object GetFieldValue(this IDataReader dataReader, Type type, string fieldName)
        {
            return dataReader.GetFieldValue(type, dataReader.GetOrdinal(fieldName));
        }

        /// <summary>
        /// Get the field value by name.
        /// </summary>
        /// <param name="dataReader">
        /// The data reader.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="fieldIndex">
        /// The field index.
        /// </param>
        /// <returns>
        /// The field value.
        /// </returns>
        public static object GetFieldValue(this IDataReader dataReader, Type type, int fieldIndex)
        {
            if (dataReader.IsDBNull(fieldIndex))
            {
                return GetDefaultValue(type);
            }

            if (typeof(DateTime?) == type)
            {
                throw new NotSupportedException(string.Format("The type of field is unsupported."));
            }

            try
            {
                var fieldValue = dataReader[fieldIndex];
                if (type == typeof(string))
                {
                    fieldValue = dataReader[fieldIndex].ToString().Trim();
                }

                return Convert.ChangeType(fieldValue, type);
            }
            catch (IndexOutOfRangeException)
            {
                throw;
            }
            catch (Exception ex)
            {
                // If not a IndexOutOfRangeException, returns default value of type.
                // In the future, I`ll try to put common.loging or log4net.
                Debug.WriteLine(ex);
                return GetDefaultValue(type);
            }
        }

        /// <summary>
        /// Get the field value by name.
        /// </summary>
        /// <param name="dataReader">
        /// The data reader.
        /// </param>
        /// <param name="fieldName">
        /// The field name.
        /// </param>
        /// <typeparam name="TType">
        /// The type of return.
        /// </typeparam>
        /// <returns>
        /// The field value.
        /// </returns>
        public static TType GetFieldValue<TType>(this IDataReader dataReader, string fieldName)
        {
            return (TType)dataReader.GetFieldValue(typeof(TType), fieldName);
        }

        /// <summary>
        /// Return whether the specified field is set to null.
        /// </summary>
        /// <param name="dataReader">
        /// The data reader.
        /// </param>
        /// <param name="fieldName">
        /// The field name.
        /// </param>
        /// <returns>
        /// true if the specified field is set to null; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        /// </exception>
        public static bool IsDBNull(this IDataReader dataReader, string fieldName)
        {
            return dataReader.IsDBNull(dataReader.GetOrdinal(fieldName));
        }

        /// <summary>
        /// The get default value.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        private static object GetDefaultValue(Type type)
        {
            if (type == typeof(string))
            {
                return string.Empty;
            }

            return type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}