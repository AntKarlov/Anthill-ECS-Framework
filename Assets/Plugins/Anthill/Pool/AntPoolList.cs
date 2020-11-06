namespace Anthill.Pool
{
	using System;
	using UnityEngine;

	[CreateAssetMenuAttribute(fileName = "PoolList", menuName = "Anthill/Pool List", order = 2)]
	public class AntPoolList : ScriptableObject
	{
		[Serializable]
		public struct Item
		{
			public GameObject prefab;
			public int initialSize;
			public bool isGrow;
			public int maxGrowing;
			public bool isOpened;
		}

		public Item[] items = new Item[0];
	}
}