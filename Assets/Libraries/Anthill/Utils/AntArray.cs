namespace Anthill.Utils
{
	using System;

	public static class AntArray
	{
		// int index = list.FindIndex(x => x.Key == "foobar");
		// if (index >= 0) {
    	// found!
    	//	UseResult(list[index]);
		// }
		// //EffectItem item = Array.Find(effects, p => p.effectId == aEffectID);

		public static int FindIndex<T>(ref T[] aSource, Func<T, bool> aClosure)
		{
			for (int i = 0, n = aSource.Length; i < n; i++)
			{
				if (aClosure(aSource[i]))
				{
					return i;
				}
			}
			return -1;
		}

		public static bool Contains<T>(ref T[] aSource, T aValue)
		{
			int index = System.Array.FindIndex(aSource, x => System.Object.ReferenceEquals(x, aValue));
			return (index >= 0 && index < aSource.Length);
		}

		public static void Add<T>(ref T[] aSource, T aValue)
		{
			var newArray = new T[aSource.Length + 1];
			for (int i = 0, n = aSource.Length; i < n; i++)
			{
				newArray[i] = aSource[i];
			}

			newArray[newArray.Length - 1] = aValue;
			aSource = newArray;
		}

		public static void RemoveAt<T>(ref T[] aSource, int aDelIndex)
		{
			int curIndex = 0;
			var newArray = new T[aSource.Length - 1];
			for (int i = 0, n = aSource.Length; i < n; i++)
			{
				if (i != aDelIndex)
				{
					newArray[curIndex] = aSource[i];
					curIndex++;
				}
			}

			aSource = newArray;
		}

		public static T GetRandom<T>(ref T[] aSource)
		{
			return (aSource.Length > 0)
				? aSource[AntRandom.Range(0, aSource.Length - 1)]
				: default(T);
		}

		public static T PopRandom<T>(ref T[] aSource)
		{
			T result = default(T);
			if (aSource.Length > 0)
			{
				int i = AntRandom.Range(0, aSource.Length - 1);
				result = aSource[i];
				RemoveAt(ref aSource, i);
			}
			return result;
		}

		public static T First<T>(ref T[] aSource)
		{
			return (aSource.Length > 0) ? aSource[0] : default(T);
		}

		public static T Last<T>(ref T[] aSource)
		{
			return (aSource.Length > 0) ? aSource[aSource.Length - 1] : default(T);
		}

		public static T PopFirst<T>(ref T[] aSource)
		{
			T result = default(T);
			if (aSource.Length > 0)
			{
				result = aSource[0];
				RemoveAt(ref aSource, 0);
			}
			return result;
		}

		public static T PopLast<T>(ref T[] aSource)
		{
			T result = default(T);
			if (aSource.Length > 0)
			{
				int i = aSource.Length - 1;
				result = aSource[i];
				RemoveAt(ref aSource, i);
			}
			return result;
		}

		public static T Pop<T>(ref T[] aSource, int aIndex)
		{
			T result = default(T);
			if (aIndex >= 0 && aIndex < aSource.Length)
			{
				result = aSource[aIndex];
				RemoveAt(ref aSource, aIndex);
			}
			return result;
		}

		public static T[] Clone<T>(ref T[] aSource)
		{
			var newArray = new T[aSource.Length];
			for (int i = 0, n = aSource.Length; i < n; i++)
			{
				newArray[i] = aSource[i];
			}
			return newArray;
		}

		public static void Shuffle<T>(ref T[] aSource)
		{
			T temp;
			int newPos;
			int index = aSource.Length;
			while (index > 1)
			{
				index--;
				newPos = AntRandom.Range(0, index);
				temp = aSource[newPos];
				aSource[newPos] = aSource[index];
				aSource[index] = temp;
			}
		}

		public static void Swap<T>(ref T[] aSource, int aIndexA, int aIndexB)
		{
			if (aIndexA < 0 || aIndexB < 0 ||
				aIndexA == aIndexB ||
				aIndexA >= aSource.Length || aIndexB >= aSource.Length)
			{
				return;
			}

			T temp = aSource[aIndexA];
			aSource[aIndexA] = aSource[aIndexB];
			aSource[aIndexB] = temp;
		}
	}
}