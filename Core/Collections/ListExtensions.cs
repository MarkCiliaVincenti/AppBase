﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CarinaStudio.Collections
{
	/// <summary>
	/// Extensions for <see cref="IList{T}"/>.
	/// </summary>
	public static class ListExtensions
	{
		/// <summary>
		/// Make given <see cref="IList{T}"/> as read-only <see cref="IList{T}"/>.
		/// </summary>
		/// <typeparam name="T">Type of element.</typeparam>
		/// <param name="list"><see cref="IList{T}"/> to make as read-only.</param>
		/// <returns>Read-only <see cref="IList{T}"/>.</returns>
		public static IList<T> AsReadOnly<T>(this IList<T> list)
		{
			if (list.IsReadOnly)
				return list;
			return new ReadOnlyCollection<T>(list);
		}


		/// <summary>
		/// Find given element by binary-search.
		/// </summary>
		/// <typeparam name="T">Type of element.</typeparam>
		/// <param name="list">List to find element.</param>
		/// <param name="element">Element to be found.</param>
		/// <param name="comparer"><see cref="IComparer{T}"/> to compare elements.</param>
		/// <returns>Index of found element, or bitwise complement of index of proper position to put element.</returns>
		public static int BinarySearch<T>(this IList<T> list, T element, IComparer<T> comparer) => BinarySearch<T>(list, 0, list.Count, element, comparer.Compare);


		/// <summary>
		/// Find given element by binary-search.
		/// </summary>
		/// <typeparam name="T">Type of element.</typeparam>
		/// <param name="list">List to find element.</param>
		/// <param name="element">Element to be found.</param>
		/// <param name="comparer"><see cref="IComparer{T}"/> to compare elements.</param>
		/// <returns>Index of found element, or bitwise complement of index of proper position to put element.</returns>
		public static int BinarySearch<T>(this IReadOnlyList<T> list, T element, IComparer<T> comparer) => BinarySearch<T>(list, 0, list.Count, element, comparer.Compare);


		/// <summary>
		/// Find given element by binary-search.
		/// </summary>
		/// <typeparam name="T">Type of element.</typeparam>
		/// <param name="list">List to find element.</param>
		/// <param name="element">Element to be found.</param>
		/// <param name="comparison">Comparison function.</param>
		/// <returns>Index of found element, or bitwise complement of index of proper position to put element.</returns>
		public static int BinarySearch<T>(this IList<T> list, T element, Comparison<T> comparison) => BinarySearch<T>(list, 0, list.Count, element, comparison);


		/// <summary>
		/// Find given element by binary-search.
		/// </summary>
		/// <typeparam name="T">Type of element.</typeparam>
		/// <param name="list">List to find element.</param>
		/// <param name="element">Element to be found.</param>
		/// <param name="comparison">Comparison function.</param>
		/// <returns>Index of found element, or bitwise complement of index of proper position to put element.</returns>
		public static int BinarySearch<T>(this IReadOnlyList<T> list, T element, Comparison<T> comparison) => BinarySearch<T>(list, 0, list.Count, element, comparison);


		/// <summary>
		/// Find given element by binary-search.
		/// </summary>
		/// <typeparam name="T">Type of element.</typeparam>
		/// <param name="list">List to find element.</param>
		/// <param name="element">Element to be found.</param>
		/// <returns>Index of found element, or bitwise complement of index of proper position to put element.</returns>
		public static int BinarySearch<T>(this IList<T> list, T element) where T : IComparable<T> => BinarySearch<T>(list, 0, list.Count, element);


		/// <summary>
		/// Find given element by binary-search.
		/// </summary>
		/// <typeparam name="T">Type of element.</typeparam>
		/// <param name="list">List to find element.</param>
		/// <param name="element">Element to be found.</param>
		/// <returns>Index of found element, or bitwise complement of index of proper position to put element.</returns>
		public static int BinarySearch<T>(this IReadOnlyList<T> list, T element) where T : IComparable<T> => BinarySearch<T>(list, 0, list.Count, element);


		// Binary search.
		static int BinarySearch<T>(IList<T> list, int start, int end, T element, Comparison<T> comparison)
		{
			if (start >= end)
				return ~start;
			var middle = (start + end) / 2;
			var result = comparison(element, list[middle]);
			if (result == 0)
				return middle;
			if (result < 0)
				return BinarySearch<T>(list, start, middle, element, comparison);
			return BinarySearch<T>(list, middle + 1, end, element, comparison);
		}
		static int BinarySearch<T>(IReadOnlyList<T> list, int start, int end, T element, Comparison<T> comparison)
		{
			if (start >= end)
				return ~start;
			var middle = (start + end) / 2;
			var result = comparison(element, list[middle]);
			if (result == 0)
				return middle;
			if (result < 0)
				return BinarySearch<T>(list, start, middle, element, comparison);
			return BinarySearch<T>(list, middle + 1, end, element, comparison);
		}
		static int BinarySearch<T>(IList<T> list, int start, int end, T element) where T : IComparable<T>
		{
			if (start >= end)
				return ~start;
			var middle = (start + end) / 2;
			var result = element.CompareTo(list[middle]);
			if (result == 0)
				return middle;
			if (result < 0)
				return BinarySearch<T>(list, start, middle, element);
			return BinarySearch<T>(list, middle + 1, end, element);
		}
		static int BinarySearch<T>(IReadOnlyList<T> list, int start, int end, T element) where T : IComparable<T>
		{
			if (start >= end)
				return ~start;
			var middle = (start + end) / 2;
			var result = element.CompareTo(list[middle]);
			if (result == 0)
				return middle;
			if (result < 0)
				return BinarySearch<T>(list, start, middle, element);
			return BinarySearch<T>(list, middle + 1, end, element);
		}
	}
}
