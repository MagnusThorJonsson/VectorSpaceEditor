using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace VectorSpace
{
    static class Extensions
    {
        /// <summary>
        /// Sorting helper for ObservableCollections
        /// </summary>
        /// <typeparam name="TSource">Source item</typeparam>
        /// <typeparam name="TKey">Search parameters</typeparam>
        /// <param name="source">Source item initiator</param>
        /// <param name="keySelector">Search parameter</param>
        /// <param name="direction">Sorting direction</param>
        public static void Sort<TSource, TKey>(this ObservableCollection<TSource> source, Func<TSource, TKey> keySelector, ListSortDirection direction = ListSortDirection.Ascending)
        {
            List<TSource> sortedList = null;

            if (direction == ListSortDirection.Ascending)
                sortedList = source.OrderBy(keySelector).ToList();
            else if (direction == ListSortDirection.Descending)
                sortedList = source.OrderByDescending(keySelector).ToList();
            
            source.Clear();
            for (int i = 0; i < sortedList.Count; i++)
            {
                source.Add(sortedList[i]);
            }
        }

    }
}
