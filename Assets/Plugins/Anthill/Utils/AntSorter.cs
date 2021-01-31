namespace Anthill.Utils
{
	using System.Collections.Generic;

	public class SorterData<K>
	{
		public float sortIndex;
		public K userData;

		public SorterData(K aUserData, float aSortIndex)
		{
			sortIndex = aSortIndex;
			userData = aUserData;
		}
	}

	public static class AntSorterOrder
	{
		public const int ASC = -1;
		public const int DESC = 1;
	}

	public class AntSorter<T>
	{
		public List<SorterData<T>> list;

		public AntSorter()
		{
			list = new List<SorterData<T>>();
		}

		public void Add(T aUserData, float aSortIndex)
		{
			list.Add(new SorterData<T>(aUserData, aSortIndex));
		}

		public bool Contains(T aUserData)
		{
			for (int i = 0, n = list.Count; i < n; i++)
			{
				if (System.Object.ReferenceEquals(aUserData, list[i].userData))
				{
					return true;
				}
			}
			return false;
		}

		public void Remove(T aUserData)
		{
			int n = list.Count;
			for (int i = 0; i < n; i++)
			{
				if (System.Object.ReferenceEquals(aUserData, list[i].userData))
				{
					list.RemoveAt(i);
					break;
				}
			}
		}

		public void RemoveAt(int aIndex)
		{
			if (aIndex >= 0 && aIndex < list.Count)
			{
				list.RemoveAt(aIndex);
			}
		}

		public T this[int aIndex]
		{
			get => (aIndex >= 0 && aIndex < list.Count) 
				? list[aIndex].userData 
				: default(T);
		}

		public T GetAt(int aIndex)
		{
			return (aIndex >= 0 && aIndex < list.Count) 
				? list[aIndex].userData 
				: default(T);
		}

		public void SetIndex(int aIndex, float aSortIndex)
		{
			if (aIndex >= 0 && aIndex < list.Count)
			{
				list[aIndex].sortIndex = aSortIndex;
			}
		}

		public void Sort(int aSortOrder = AntSorterOrder.ASC)
		{
			list.Sort((x, y) => (x.sortIndex < y.sortIndex) 
				? aSortOrder 
				: (x.sortIndex > y.sortIndex) 
					? -aSortOrder 
					: 0);
		}

		public void Clear()
		{
			list.Clear();
		}

		public int Count
		{
			get => list.Count;
		}
	}
}