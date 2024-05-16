using UnityEngine;

namespace Anthill.Tweening
{
	public static class TweenSpriteExtensions
	{
		public static AntTween ToAlpha(this SpriteRenderer aTarget, float aTo, float aDuration)
		{
			var to = new Color(1.0f, 1.0f, 1.0f, aTo);
			return AntTweening.To(() => aTarget.color, x => aTarget.color = x, to, aDuration)
				.SetGameObject(aTarget.gameObject);
		}

		public static AntTween ToAlpha(this SpriteRenderer aTarget, float aFrom, float aTo, float aDuration)
		{
			aTarget.color = new Color(1.0f, 1.0f, 1.0f, aFrom);
			var to = new Color(1.0f, 1.0f, 1.0f, aTo);
			return AntTweening.To(() => aTarget.color, x => aTarget.color = x, to, aDuration)
				.SetGameObject(aTarget.gameObject);
		}

		public static AntTween ToColor(this SpriteRenderer aTarget, Color aTo, float aDuration)
		{
			return AntTweening.To(() => aTarget.color, x => aTarget.color = x, aTo, aDuration)
				.SetGameObject(aTarget.gameObject);
		}

		public static AntTween ToColor(this SpriteRenderer aTarget, Color aFrom, Color aTo, float aDuration)
		{
			aTarget.color = aFrom;
			return AntTweening.To(() => aTarget.color, x => aTarget.color = x, aTo, aDuration)
				.SetGameObject(aTarget.gameObject);
		}
	}
}