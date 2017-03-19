using System;
using System.Collections.Generic;
using System.Reflection;
using GameStats.Server.Statistics;
using LiteDB;

namespace GameStats.Server.Helpers
{
    internal class FieldBackuper<T>
        where T : IBaseStatistics
    {
        private static readonly List<FieldInfo> _simpleFields;
        private static readonly List<Tuple<FieldInfo, MethodInfo>> _collectionFields; //second field is method clear

        static FieldBackuper()
        {
            _simpleFields = new List<FieldInfo>();
            _collectionFields = new List<Tuple<FieldInfo, MethodInfo>>();
            var fields = typeof(T).GetFields(
                BindingFlags.NonPublic |
                BindingFlags.Instance);
            foreach (var field in fields)
            {
                var isCollection = field.FieldType.GetInterface(typeof(ICollection<>).Name);
                if (isCollection != null)
                    _collectionFields.Add(Tuple.Create(field, field.FieldType.GetMethod("Clear")));
                else
                    _simpleFields.Add(field);
            }
        }

        public static void Save(IDictionary<string, BsonValue> dictionary, T obj)
        {
            foreach (var field in _simpleFields)
                dictionary.Add(typeof(T).Name + '.' + field.Name, new BsonValue(field.GetValue(obj)));
            foreach (var field in _collectionFields)
                dictionary.Add(typeof(T).Name + '.' + field.Item1.Name, new BsonValue(field.Item1.GetValue(obj)));
        }

        public static void Reset(object obj)
        {
            foreach (var field in _simpleFields)
                field.SetValue(obj, null);
            foreach (var field in _collectionFields)
                field.Item2.Invoke(field.Item1.GetValue(obj), null);
        }
    }
}