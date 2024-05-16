using UnityEngine;

namespace Anthill.Tweening
{
	[CreateAssetMenu(fileName = "tween_preset 2", menuName = "Anthill/Tween Preset")]
	public class AntTweenPreset : ScriptableObject
	{
		[System.Serializable]
		public struct EaseItem
		{
			public string name;
			public AntEase ease;
			public AnimationCurve curve;
		}

		public EaseItem[] easings = new EaseItem[0];

		public bool TryGetEase(AntEase aEase, out EaseItem aResult)
		{
			int index = System.Array.FindIndex(easings, x => x.ease == aEase);
			if (index >= 0 && index < easings.Length)
			{
				aResult = easings[index];
				return true;
			}

			aResult = default;
			return false;
		}
	}
}