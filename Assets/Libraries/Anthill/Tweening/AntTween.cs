using System;
using UnityEngine;

namespace Anthill.Tweening
{
	// Float
	public delegate float GetFloatDelegate();
	public delegate void SetFloatDelegate(float aValue);

	// Vector2
	public delegate Vector2 GetVector2Delegate();
	public delegate void SetVector2Delegate(Vector2 aValue);
	
	// Vector3
	public delegate Vector3 GetVector3Delegate();
	public delegate void SetVector3Delegate(Vector3 aValue);

	// Quaternion
	public delegate Quaternion GetQuaternionDelegate();
	public delegate void SetQuaternionDelegate(Quaternion aValue);

	// Color
	public delegate Color GetColorDelegate();
	public delegate void SetColorDelegate(Color aValue);

	public class AntTween : ITween
	{
	#region Public Variables

		public float delay;
		public float duration;

	#endregion
		
	#region Private Variables

		protected Action _startCallback;
		protected Action _completeCallback;
		protected Action _killCallback;
		protected AnimationCurve _curve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
		protected bool _isStarted = false;
		protected bool _isFinished = false;
		protected float _duration;

		private SetFloatDelegate _setter;
		protected string _gameObjectName;
		protected GameObject _gameObject;
		protected bool _safeCheck;

		private float _from;
		private float _to;

	#endregion

	#region Getters / Setters

		public bool IsUnscaledTime { get; private set; }
		public bool IsKillOnDeinitialize { get; private set; }
		// public bool IsAvailable { get; set; }

	#endregion

	#region ITween Implementation

		public virtual void Execute(float aDeltaTime)
		{
			if (!_isStarted)
			{
				Start();
			}

			delay -= aDeltaTime;
			if (delay <= 0.0f)
			{
				_duration += aDeltaTime;
				if (_duration >= duration)
				{
					_isFinished = true;
					_duration = duration;
				}

				if (!_safeCheck)
				{
					ApplyValues();
				}
				else
				{
					if (_gameObject == null)
					{
						A.Warning($"Object `{_gameObjectName}` has been destroyed but tween trying to access!", "AntTween");
						_isFinished = true;
						_duration = duration;
					}
					else
					{
						ApplyValues();
					}
				}

				if (_isFinished)
				{
					Kill(true);
				}
			}
		}

		public virtual void Release()
		{
			delay = 0.0f;
			duration = 0.0f;

			_startCallback = null;
			_completeCallback = null;
			_killCallback = null;
			_curve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
			_isStarted = false;
			_isFinished = false;
			_duration = 0.0f;

			_setter = null;

			IsUnscaledTime = false;
			IsKillOnDeinitialize = false;

			AntTweening.Scenario.Remove(this);
			// IsAvailable = true;
		}

	#endregion

	#region Public Methods

		public AntTween SetPause(float aValue)
		{
			delay = aValue;
			return this;
		}

		public AntTween SetFrom(float aValue)
		{
			_from = aValue;
			return this;
		}

		public AntTween SetTo(float aValue)
		{
			_to = aValue;
			return this;
		}

		public AntTween SetDuration(float aValue)
		{
			_duration = 0.0f;
			duration = aValue;
			return this;
		}

		public AntTween SetSetter(SetFloatDelegate aSetter)
		{
			_setter = aSetter;
			return this;
		}

		public AntTween SetEase(AnimationCurve aCurve)
		{
			_curve = aCurve;
			return this;
		}

		public AntTween SetEase(AntEase aEase)
		{
			_curve = AntTweening.GetCurve(aEase);
			return this;
		}

		public AntTween SetGameObject(GameObject aGameObject)
		{
			_safeCheck = false;
			_gameObject = aGameObject;
			if (_gameObject != null)
			{
				_gameObjectName = _gameObject.name;
				_safeCheck = true;
			}
			return this;
		}

		public AntTween SetKillOnDeinitialize(bool aValue)
		{
			IsKillOnDeinitialize = aValue;
			return this;
		}

		public AntTween OnStart(Action aCallback)
		{
			_startCallback = aCallback;
			return this;
		}

		public AntTween OnComplete(Action aCallback)
		{
			_completeCallback = aCallback;
			return this;
		}

		public AntTween OnKill(Action aCallback)
		{
			_killCallback = aCallback;
			return this;
		}

		public void Kill(bool aCallEvents = false)
		{
			// if (IsAvailable) return;
			
			if (aCallEvents)
			{
				_completeCallback?.Invoke();
				_killCallback?.Invoke();
			}
			
			Release();
		}

		public AntTween SetUpdate(bool aIsUnscaledTime)
		{
			IsUnscaledTime = aIsUnscaledTime;
			return this;
		}

	#endregion

	#region Private Methods

		protected void Start()
		{
			_isStarted = true;
			_startCallback?.Invoke();
			_startCallback = null;
		}

		private void ApplyValues()
		{
			_setter(Mathf.LerpUnclamped(_from, _to, _curve.Evaluate(_duration / duration)));
		}

	#endregion
	}

	// -----------------------------------------------------------------------------------------------------------------


	public class AntTweenVector2 : AntTween
	{
	#region Public Variables

		// ..

	#endregion
		
	#region Private Variables

		private SetVector2Delegate _setter;

		private Vector2 _from;
		private Vector2 _to;

	#endregion

	#region Getters / Setters

		// ..

	#endregion

	#region ITween Implementation

		public override void Execute(float aDeltaTime)
		{
			if (!_isStarted)
			{
				Start();
			}

			delay -= aDeltaTime;
			if (delay <= 0.0f)
			{
				_duration += aDeltaTime;
				if (_duration >= duration)
				{
					_isFinished = true;
					_duration = duration;
				}

				if (!_safeCheck)
				{
					ApplyValues();
				}
				else
				{
					if (_gameObject == null)
					{
						A.Warning($"Object `{_gameObjectName}` has been destroyed but tween trying to access!", "AntTweenVector2");
						_isFinished = true;
						_duration = duration;
					}
					else
					{
						ApplyValues();
					}
				}

				if (_isFinished)
				{
					Kill(true);
				}
			}
		}

		public override void Release()
		{
			base.Release();
			_setter = null;
		}

	#endregion

	#region Public Methods

		public AntTweenVector2 SetFrom(Vector2 aValue)
		{
			_from = aValue;
			return this;
		}

		public AntTweenVector2 SetTo(Vector2 aValue)
		{
			_to = aValue;
			return this;
		}

		public AntTweenVector2 SetSetter(SetVector2Delegate aSetter)
		{
			_setter = aSetter;
			return this;
		}

	#endregion

	#region Private Methods

		private void ApplyValues()
		{
			_setter(Vector2.LerpUnclamped(_from, _to, _curve.Evaluate(_duration / duration)));
		}

	#endregion
	}


	// -----------------------------------------------------------------------------------------------------------------


	public class AntTweenVector3 : AntTween
	{
	#region Public Variables

		// ..

	#endregion
		
	#region Private Variables

		private SetVector3Delegate _setter;

		private Vector3 _from;
		private Vector3 _to;

	#endregion

	#region Getters / Setters

		// ..

	#endregion

	#region ITween Implementation

		public override void Execute(float aDeltaTime)
		{
			if (!_isStarted)
			{
				Start();
			}

			delay -= aDeltaTime;
			if (delay <= 0.0f)
			{
				_duration += aDeltaTime;
				if (_duration >= duration)
				{
					_isFinished = true;
					_duration = duration;
				}

				if (!_safeCheck)
				{
					ApplyValues();
				}
				else
				{
					if (_gameObject == null)
					{
						A.Warning($"Object `{_gameObjectName}` has been destroyed but tween trying to access!", "AntTweenVector3");
						_isFinished = true;
						_duration = duration;
					}
					else
					{
						ApplyValues();
					}
				}

				if (_isFinished)
				{
					Kill(true);
				}
			}
		}

		public override void Release()
		{
			base.Release();
			_setter = null;
		}

	#endregion

	#region Public Methods

		public AntTweenVector3 SetFrom(Vector3 aValue)
		{
			_from = aValue;
			return this;
		}

		public AntTweenVector3 SetTo(Vector3 aValue)
		{
			_to = aValue;
			return this;
		}

		public AntTweenVector3 SetSetter(SetVector3Delegate aSetter)
		{
			_setter = aSetter;
			return this;
		}

	#endregion

	#region Private Methods

		private void ApplyValues()
		{
			_setter(Vector3.LerpUnclamped(_from, _to, _curve.Evaluate(_duration / duration)));
		}

	#endregion
	}


	// -----------------------------------------------------------------------------------------------------------------


	public class AntTweenQuaternion : AntTween
	{
	#region Public Variables

		// ..

	#endregion
		
	#region Private Variables

		private SetQuaternionDelegate _setter;

		private Quaternion _from;
		private Quaternion _to;

	#endregion

	#region Getters / Setters

		// ..

	#endregion

	#region ITween Implementation

		public override void Execute(float aDeltaTime)
		{
			if (!_isStarted)
			{
				Start();
			}

			delay -= aDeltaTime;
			if (delay <= 0.0f)
			{
				_duration += aDeltaTime;
				if (_duration >= duration)
				{
					_isFinished = true;
					_duration = duration;
				}

				if (!_safeCheck)
				{
					ApplyValues();
				}
				else
				{
					if (_gameObject == null)
					{
						A.Warning($"Object `{_gameObjectName}` has been destroyed but tween trying to access!", "AntTweenQuaternion");
						_isFinished = true;
						_duration = duration;
					}
					else
					{
						ApplyValues();
					}
				}

				if (_isFinished)
				{
					Kill(true);
				}
			}
		}

		public override void Release()
		{
			base.Release();
			_setter = null;
		}

	#endregion

	#region Public Methods

		public AntTweenQuaternion SetFrom(Quaternion aValue)
		{
			_from = aValue;
			return this;
		}

		public AntTweenQuaternion SetTo(Quaternion aValue)
		{
			_to = aValue;
			return this;
		}

		public AntTweenQuaternion SetSetter(SetQuaternionDelegate aSetter)
		{
			_setter = aSetter;
			return this;
		}

	#endregion

	#region Private Methods

		private void ApplyValues()
		{
			_setter(Quaternion.LerpUnclamped(_from, _to, _curve.Evaluate(_duration / duration)));
		}

	#endregion
	}


	// -----------------------------------------------------------------------------------------------------------------


	public class AntTweenColor : AntTween
	{
	#region Public Variables

		// ..

	#endregion
		
	#region Private Variables

		private SetColorDelegate _setter;

		private Color _from;
		private Color _to;

	#endregion

	#region Getters / Setters

		// ..

	#endregion

	#region ITween Implementation

		public override void Execute(float aDeltaTime)
		{
			if (!_isStarted)
			{
				Start();
			}
			
			delay -= aDeltaTime;
			if (delay <= 0.0f)
			{
				_duration += aDeltaTime;
				if (_duration >= duration)
				{
					_isFinished = true;
					_duration = duration;
				}

				if (!_safeCheck)
				{
					ApplyValues();
				}
				else
				{
					if (_gameObject == null)
					{
						A.Warning($"Object `{_gameObjectName}` has been destroyed but tween trying to access!", "AntTweenColor");
						_isFinished = true;
						_duration = duration;
					}
					else
					{
						ApplyValues();
					}
				}

				if (_isFinished)
				{
					Kill(true);
				}
			}
		}

		public override void Release()
		{
			base.Release();
			_setter = null;
		}

	#endregion

	#region Public Methods

		public AntTweenColor SetFrom(Color aValue)
		{
			_from = aValue;
			return this;
		}

		public AntTweenColor SetTo(Color aValue)
		{
			_to = aValue;
			return this;
		}

		public AntTweenColor SetSetter(SetColorDelegate aSetter)
		{
			_setter = aSetter;
			return this;
		}

	#endregion

	#region Private Methods

		private void ApplyValues()
		{
			_setter(Color.LerpUnclamped(_from, _to, _curve.Evaluate(_duration / duration)));
		}

	#endregion
	}
}