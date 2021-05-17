namespace Anthill.Pools
{
	using System.Collections.Generic;
	using UnityEngine;

	public static class AntPool
	{
	#region Private Variables

		private static Transform _parent;
		private readonly static List<AntPoolContainer> _pools = new List<AntPoolContainer>();

	#endregion

	#region Public Methods

		public static void SetParent(Transform aParent)
		{
			_parent = aParent;
		}

		public static void Add(AntPoolPreset.Item aSource)
		{
			var go = new GameObject();
			go.name = aSource.prefab.name;
			if (_parent != null)
			{
				go.transform.SetParent(_parent);
			}
			
			_pools.Add(new AntPoolContainer(aSource, go.transform));
		}

		public static string[] GetPoolNames()
		{
			var result = new string[_pools.Count];
			for (int i = 0, n = _pools.Count; i < n; i++)
			{
				result[i] = _pools[i].name;
			}
			return result;
		}

		public static List<string> GetPoolNames<T>()
		{
			var result = new List<string>();
			for (int i = 0, n = _pools.Count; i < n; i++)
			{
				if (_pools[i].Has<T>())
				{
					result.Add(_pools[i].name);
				}
			}
			return result;
		}

		public static T Get<T, E>(E aEnumName, Vector3 aPosition, Quaternion aRotation) where E : System.Enum
		{
			var go = Get(aEnumName.ToString(), aPosition, aRotation);
			return (go != null)
				? go.GetComponent<T>()
				: default(T);
		}

		public static T Get<T, E>(E aEnumName, Vector3 aPosition) where E : System.Enum
		{
			var go = Get(aEnumName.ToString(), aPosition);
			return (go != null)
				? go.GetComponent<T>()
				: default(T);
		}

		public static T Get<T, E>(E aEnumName) where E : System.Enum
		{
			var go = Get(aEnumName.ToString());
			return (go != null)
				? go.GetComponent<T>()
				: default(T);
		}

		public static T Get<T>(string aName, Vector3 aPosition, Quaternion aRotation)
		{
			var go = Get(aName, aPosition, aRotation);
			return (go != null)
				? go.GetComponent<T>()
				: default(T);
		}

		public static T Get<T>(string aName, Vector3 aPosition)
		{
			var go = Get(aName, aPosition);
			return (go != null)
				? go.GetComponent<T>()
				: default(T);
		}

		public static T Get<T>(string aName)
		{
			var go = Get(aName);
			return (go != null)
				? go.GetComponent<T>()
				: default(T);
		}

		public static GameObject Get<T>(T aEnumName, Vector3 aPosition, Quaternion aRotation) where T : System.Enum
		{
			return Get(aEnumName.ToString(), aPosition, aRotation);
		}

		public static GameObject Get<T>(T aEnumName, Vector3 aPosition) where T : System.Enum
		{
			return Get(aEnumName.ToString(), aPosition);
		}

		public static GameObject Get<T>(T aEnumName) where T : System.Enum
		{
			return Get(aEnumName.ToString());
		}

		public static GameObject Get(string aName, Vector3 aPosition, Quaternion aRotation)
		{
			var go = Get(aName);
			if (go != null)
			{
				go.transform.position = aPosition;
				go.transform.rotation = aRotation;
			}
			return go;
		}

		public static GameObject Get(string aName, Vector3 aPosition)
		{
			var go = Get(aName);
			if (go != null)
			{
				go.transform.position = aPosition;
			}
			return go;
		}

		public static GameObject Get(string aName)
		{
			int index = _pools.FindIndex(x => x.name.Equals(aName));
			if (index >= 0 && index < _pools.Count)
			{
				var poolable = _pools[index].Get();
				return (poolable != null)
					? poolable.gameObject
					: null;
			}
			
			A.Editor.Warning($"Can't find the `{aName}` in the pool!", "AntPool");
			return null;
		}

		public static AntPoolContainer GetContainer(string aName)
		{
			int index = _pools.FindIndex(x => x.name.Equals(aName));
			return (index >= 0 && index < _pools.Count)
				? _pools[index]
				: null;
		}

		public static void DebugInfo()
		{
			for (int i = 0, n = _pools.Count; i < n; i++)
			{
				if (_pools[i].GrowCount > 0)
				{
					A.LogForce($"Pool `{_pools[i].name}` grew by {_pools[i].GrowCount}", "AntPool");
				}
			}
		}

	#endregion
	}
}