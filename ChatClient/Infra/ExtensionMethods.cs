using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ChatClient.Infra
{
    public static class ExtensionMethods
    {
        public static void AddRange<T>(this ObservableCollection<T> oc, IEnumerable<T> collection)
        {
            if (collection == null) { throw new ArgumentNullException("collection"); }
            foreach (var i in collection) { oc.Add(i); }
        }
    }
}
