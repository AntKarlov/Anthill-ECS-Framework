namespace Anthill.Pools
{
	using System;
	using UnityEngine;

	[CreateAssetMenuAttribute(fileName = "PoolPreset", menuName = "Anthill/Pool Preset", order = 2)]
	public class AntPoolPreset : ScriptableObject
	{
		[Serializable]
		public struct Item
		{
			public GameObject prefab;
			public int initialSize;
			public bool isGrow;
			public bool isLimitCapacity;
			public int maxCapacity;
			public bool isOpened;
		}

		public Item[] items = new Item[0];
	}
}