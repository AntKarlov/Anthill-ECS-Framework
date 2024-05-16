// using System.Collections.Generic;
using UnityEngine;
using Anthill.Core;

namespace Anthill.Tweening
{
	public static class AntTweening
	{
	#region Private Variables

		private static bool _isInitialized = false;
		private static AntTweeningScenario _scenario = null;
		private static AntTweenPreset _tweenPreset = null;

		// private static readonly List<AntTween> _floatPool = new();
		// private static readonly List<AntTweenVector2> _vector2Pool = new();
		// private static readonly List<AntTweenVector3> _vector3Pool = new();
		// private static readonly List<AntTweenQuaternion> _quaternionPool = new();
		// private static readonly List<AntTweenColor> _colorPool = new();

	#endregion

	#region Getters / Setters
		
		public static AntTweeningScenario Scenario
		{
			get
			{
				Initialize();
				return _scenario;
			}
		}

	#endregion

	#region Public Methods

		public static AnimationCurve GetCurve(AntEase aEase)
		{
			Initialize();
			return _tweenPreset.TryGetEase(aEase, out AntTweenPreset.EaseItem result) 
				? result.curve 
				: AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
		}

		public static AntTween To(GetFloatDelegate aFrom, SetFloatDelegate aSetTo, float aTo, float aDuration)
		{
			AntTween tween = new();
			tween.SetFrom(aFrom())
				.SetSetter(aSetTo)
				.SetTo(aTo)
				.SetDuration(aDuration);

			Scenario.Add(tween);
			return tween;
		}

		public static AntTween To(GetVector2Delegate aFrom, SetVector2Delegate aSetTo, Vector2 aTo, float aDuration)
		{
			AntTweenVector2 tween = new();
			tween.SetFrom(aFrom())
				.SetSetter(aSetTo)
				.SetTo(aTo)
				.SetDuration(aDuration);

			Scenario.Add(tween);
			return tween;
		}

		public static AntTween To(GetVector3Delegate aFrom, SetVector3Delegate aSetTo, Vector3 aTo, float aDuration)
		{
			AntTweenVector3 tween = new();
			tween.SetFrom(aFrom())
				.SetSetter(aSetTo)
				.SetTo(aTo)
				.SetDuration(aDuration);

			Scenario.Add(tween);
			return tween;
		}

		public static AntTween To(GetQuaternionDelegate aFrom, SetQuaternionDelegate aSetTo, Quaternion aTo, float aDuration)
		{
			AntTweenQuaternion tween = new();
			tween.SetFrom(aFrom())
				.SetSetter(aSetTo)
				.SetTo(aTo)
				.SetDuration(aDuration);

			Scenario.Add(tween);
			return tween;
		}

		public static AntTween To(GetColorDelegate aFrom, SetColorDelegate aSetTo, Color aTo, float aDuration)
		{
			AntTweenColor tween = new();
			tween.SetFrom(aFrom())
				.SetSetter(aSetTo)
				.SetTo(aTo)
				.SetDuration(aDuration);

			Scenario.Add(tween);
			return tween;
		}

	#endregion

	#region Private Methods

		private static void Initialize()
		{
			if (_isInitialized) return;
			_isInitialized = true;
			
			_scenario = AntEngine.Add<AntTweeningScenario>(aPriority: -2);

			try
			{
				_tweenPreset = (AntTweenPreset) Resources.Load("tween_preset", typeof(AntTweenPreset));
			}
			catch (System.Exception aException)
			{
				A.Error($"Error getting AntTweenPreset: {aException}", "AntTweening");
			}
		}

		// private static AntTween GetFloatTween()
		// {
		// 	AntTween tween;
		// 	for (int i = _floatPool.Count - 1; i >= 0; i--)
		// 	{
		// 		tween = _floatPool[i];
		// 		if (tween.IsAvailable)
		// 		{
		// 			tween.IsAvailable = false;
		// 			return tween;
		// 		}
		// 	}

		// 	tween = new AntTween();
		// 	_floatPool.Add(tween);
		// 	ConsoleProDebug.Watch("Float", $"{_floatPool.Count}");

		// 	return tween;
		// }

		// private static AntTweenVector2 GetVector2Tween()
		// {
		// 	AntTweenVector2 tween;
		// 	for (int i = _vector2Pool.Count - 1; i >= 0; i--)
		// 	{
		// 		tween = _vector2Pool[i];
		// 		if (tween.IsAvailable)
		// 		{
		// 			tween.IsAvailable = false;
		// 			return tween;
		// 		}
		// 	}

		// 	tween = new AntTweenVector2();
		// 	_vector2Pool.Add(tween);
		// 	ConsoleProDebug.Watch("Vector2", $"{_vector2Pool.Count}");

		// 	return tween;
		// }

		// private static AntTweenVector3 GetVector3Tween()
		// {
		// 	AntTweenVector3 tween;
		// 	for (int i = _vector3Pool.Count - 1; i >= 0; i--)
		// 	{
		// 		tween = _vector3Pool[i];
		// 		if (tween.IsAvailable)
		// 		{
		// 			tween.IsAvailable = false;
		// 			return tween;
		// 		}
		// 	}

		// 	tween = new AntTweenVector3();
		// 	_vector3Pool.Add(tween);
		// 	ConsoleProDebug.Watch("Vector3", $"{_vector3Pool.Count}");

		// 	return tween;
		// }

		// private static AntTweenQuaternion GetQuaternionTween()
		// {
		// 	AntTweenQuaternion tween;
		// 	for (int i = _quaternionPool.Count - 1; i >= 0; i--)
		// 	{
		// 		tween = _quaternionPool[i];
		// 		if (tween.IsAvailable)
		// 		{
		// 			tween.IsAvailable = false;
		// 			return tween;
		// 		}
		// 	}

		// 	tween = new AntTweenQuaternion();
		// 	_quaternionPool.Add(tween);
		// 	ConsoleProDebug.Watch("Quaternion", $"{_quaternionPool.Count}");

		// 	return tween;
		// }

		// private static AntTweenColor GetColorTween()
		// {
		// 	AntTweenColor tween;
		// 	for (int i = _colorPool.Count - 1; i >= 0; i--)
		// 	{
		// 		tween = _colorPool[i];
		// 		if (tween.IsAvailable)
		// 		{
		// 			tween.IsAvailable = false;
		// 			return tween;
		// 		}
		// 	}

		// 	tween = new AntTweenColor();
		// 	_colorPool.Add(tween);
		// 	ConsoleProDebug.Watch("Color", $"{_colorPool.Count}");

		// 	return tween;
		// }

	#endregion

	#region Event Handlers

		// ..

	#endregion
	}
}