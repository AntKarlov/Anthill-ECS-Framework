namespace Anthill.Utils
{
	using System.Collections.Generic;
	
	public static class AntList
	{
		public static T GetRandom<T>(this IList<T> aSource)
		{
			T result = default(T);
			if (aSource.Count > 0)
			{
				result = aSource[AntMath.RandomRangeInt(0, aSource.Count - 1)];
			}
			return result;
		}

		public static T PopRandom<T>(this IList<T> aSource)
		{
			T result = default(T);
			if (aSource.Count > 0)
			{
				int i = AntMath.RandomRangeInt(0, aSource.Count - 1);
				result = aSource[i];
				aSource.RemoveAt(i);
			}
			return result;
		}

		public static T First<T>(this IList<T> aSource)
		{
			T result = default(T);
			if (aSource.Count > 0)
			{
				result = aSource[0];
			}
			return result;
		}

		public static T Last<T>(this IList<T> aSource)
		{
			T result = default(T);
			if (aSource.Count > 0)
			{
				result = aSource[aSource.Count - 1];
			}
			return result;
		}

		public static T PopFirst<T>(this IList<T> aSource)
		{
			T result = default(T);
			if (aSource.Count > 0)
			{
				result = aSource[0];
				aSource.RemoveAt(0);
			}
			return result;
		}

		public static T PopLast<T>(this IList<T> aSource)
		{
			T result = default(T);
			if (aSource.Count > 0)
			{
				int i = aSource.Count - 1;
				result = aSource[i];
				aSource.RemoveAt(i);
			}
			return result;
		}

		public static T Pop<T>(this IList<T> aSource, int aIndex)
		{
			T result = default(T);
			if (aIndex >= 0 && aIndex < aSource.Count)
			{
				result = aSource[aIndex];
				aSource.RemoveAt(aIndex);
			}
			return result;
		}

		public static List<T> Clone<T>(this List<T> aSource)
		{
			return new List<T>(aSource);
		}

		public static void Shuffle<T>(this IList<T> aList)
		{
			T temp;
			int newPos;
			int index = aList.Count;
			while (index > 1)
			{
				index--;
				newPos = AntMath.RandomRangeInt(0, index);
				temp = aList[newPos];
				aList[newPos] = aList[index];
				aList[index] = temp;
			}
		}

		public static void Swap<T>(this IList<T> aList, int aIndexA, int aIndexB)
		{
			if (aIndexA < 0 || aIndexB < 0 ||
				aIndexA == aIndexB ||
				aIndexA >= aList.Count || aIndexB >= aList.Count)
			{
				return;
			}

			T temp = aList[aIndexA];
			aList[aIndexA] = aList[aIndexB];
			aList[aIndexB] = temp;
		}
	}
}