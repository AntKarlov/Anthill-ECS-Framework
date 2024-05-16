using UnityEngine;

namespace Anthill.Tweening
{
	public static class TweenTransformExtensions
	{
		public static AntTween ToPosition(this Transform aTarget, Vector3 aTo, float aDuration)
		{
			return AntTweening.To(() => aTarget.position, x => aTarget.position = x, aTo, aDuration)
				.SetGameObject(aTarget.gameObject);
		}

		public static AntTween ToPosition(this Transform aTarget, Vector3 aFrom, Vector3 aTo, float aDuration)
		{
			aTarget.position = aFrom;
			return AntTweening.To(() => aTarget.position, x => aTarget.position = x, aTo, aDuration)
				.SetGameObject(aTarget.gameObject);
		}

		public static AntTween ToLocalPosition(this Transform aTarget, Vector3 aTo, float aDuration)
		{
			return AntTweening.To(() => aTarget.localPosition, x => aTarget.localPosition = x, aTo, aDuration)
				.SetGameObject(aTarget.gameObject);
		}

		public static AntTween ToLocalPosition(this Transform aTarget, Vector3 aFrom, Vector3 aTo, float aDuration)
		{
			aTarget.localPosition = aFrom;
			return AntTweening.To(() => aTarget.localPosition, x => aTarget.localPosition = x, aTo, aDuration)
				.SetGameObject(aTarget.gameObject);
		}

		public static AntTween ToRotation(this Transform aTarget, Quaternion aTo, float aDuration)
		{
			return AntTweening.To(() => aTarget.localRotation, x => aTarget.localRotation = x, aTo, aDuration)
				.SetGameObject(aTarget.gameObject);
		}

		public static AntTween ToRotation(this Transform aTarget, Quaternion aFrom, Quaternion aTo, float aDuration)
		{
			aTarget.localRotation = aFrom;
			return AntTweening.To(() => aTarget.localRotation, x => aTarget.localRotation = x, aTo, aDuration)
				.SetGameObject(aTarget.gameObject);
		}

		public static AntTween ToScale(this Transform aTarget, Vector3 aTo, float aDuration)
		{
			return AntTweening.To(() => aTarget.localScale, x => aTarget.localScale = x, aTo, aDuration)
				.SetGameObject(aTarget.gameObject);
		}

		public static AntTween ToScale(this Transform aTarget, Vector3 aFrom, Vector3 aTo, float aDuration)
		{
			aTarget.localScale = aFrom;
			return AntTweening.To(() => aTarget.localScale, x => aTarget.localScale = x, aTo, aDuration)
				.SetGameObject(aTarget.gameObject);
		}
	}
}