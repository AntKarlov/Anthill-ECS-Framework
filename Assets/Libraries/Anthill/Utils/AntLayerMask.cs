namespace Anthill.Utils
{
	using System.Collections.Generic;
	using UnityEngine;

#if UNITY_EDITOR
	using UnityEditorInternal;
#endif

	public static class AntLayerMask
	{
		public static LayerMask Create(params string[] aLayerNames)
		{
			return NamesToMask(aLayerNames);
		}
	
		public static LayerMask Create(params int[] aLayerNumbers)
		{
			return LayerNumbersToMask(aLayerNumbers);
		}
	
		public static LayerMask NamesToMask(params string[] aLayerNames)
		{
			var result = (LayerMask) 0;
			foreach(var name in aLayerNames)
			{
				result |= (1 << LayerMask.NameToLayer(name));
			}
			return result;
		}
	
		public static LayerMask LayerNumbersToMask(params int[] aLayerNumbers)
		{
			var result = (LayerMask) 0;
			foreach(var layer in aLayerNumbers)
			{
				result |= (1 << layer);
			}
			return result;
		}
	
		public static LayerMask Inverse(this LayerMask aOriginal)
		{
			return ~aOriginal;
		}
	
		public static LayerMask AddToMask(this LayerMask aOriginal, params string[] aLayerNames)
		{
			return aOriginal | NamesToMask(aLayerNames);
		}
	
		public static LayerMask RemoveFromMask(this LayerMask aOriginal, params string[] aLayerNames)
		{
			LayerMask invertedOriginal = ~aOriginal;
			return ~(invertedOriginal | NamesToMask(aLayerNames));
		}
	
		public static string[] MaskToNames(this LayerMask aOriginal)
		{
			var output = new List<string>();
			for (int i = 0; i < 32; ++i)
			{
				int shifted = 1 << i;
				if ((aOriginal & shifted) == shifted)
				{
					string layerName = LayerMask.LayerToName(i);
					if (!string.IsNullOrEmpty(layerName))
					{
						output.Add(layerName);
					}
				}
			}
			return output.ToArray();
		}
	
		public static string MaskToString(this LayerMask aOriginal)
		{
			return MaskToString(aOriginal, ", ");
		}
	
		public static string MaskToString(this LayerMask aOriginal, string aDelimiter)
		{
			return string.Join(aDelimiter, MaskToNames(aOriginal));
		}

		public static bool IsInLayerMask(GameObject aObject, LayerMask aLayerMask)
		{
			return ((aLayerMask.value & (1 << aObject.layer)) > 0);
		}

		//LayerMask mask = LayerMask.NameToLayer("MyMask");
		//object.layer = Mathf.RoundToInt(Mathf.Log(mask.value, 2);

		/// <summary>
		/// Задает один или несколько слоев для LayerMask с которыми физические объекты могут взаимодействовать.
		/// LayerMask layer = SetLayerMask("MyLayerName", "Layer2");
		/// В результате будут выделенны только указанные слои.
		/// </summary>
		public static int SetLayerMask(params string[] aLayerNames)
		{
			int mask = 0;
			for (int i = 0, n = aLayerNames.Length; i < n; i++)
			{
				mask |= (1 << LayerMask.NameToLayer(aLayerNames[i]));
			}
			return mask;
		}

		/// <summary>
		/// Задает один или несколько слоев для LayerMask с которыми физические объекты НЕ могут взаимодействовать.
		/// LayerMask layer = SetLayerMask("MyLayerName", "Layer2");
		/// В результате будут выделены все доступные слои за исключением указанных.
		/// </summary>
		public static int SetNotLayerMask(params string[] aLayerNames)
		{
			int mask = 0;
			for (int i = 0, n = aLayerNames.Length; i < n; i++)
			{
				mask |= (1 << LayerMask.NameToLayer(aLayerNames[i]));
			}
			mask = ~mask;
			return mask;
		}
		
#if UNITY_EDITOR
		private static readonly List<int> _layerNumbers = new List<int>();
		
		public static LayerMask LayerMaskField(string aLabel, LayerMask aLayerMask)
		{
			var layers = InternalEditorUtility.layers;
		
			_layerNumbers.Clear();
		
			for (int i = 0; i < layers.Length; i++)
			{
				_layerNumbers.Add(LayerMask.NameToLayer(layers[i]));
			}
		
			int maskWithoutEmpty = 0;
			for (int i = 0, n = _layerNumbers.Count; i < n; i++)
			{
				if (((1 << _layerNumbers[i]) & aLayerMask.value) > 0)
				{
					maskWithoutEmpty |= (1 << i);
				}
			}
		
			maskWithoutEmpty = UnityEditor.EditorGUILayout.MaskField(aLabel, maskWithoutEmpty, layers);
		
			int mask = 0;
			for (int i = 0, n = _layerNumbers.Count; i < n; i++)
			{
				if ((maskWithoutEmpty & (1 << i)) > 0)
					mask |= (1 << _layerNumbers[i]);
			}

			aLayerMask.value = mask;
			return aLayerMask;
		}
#endif
	}
}