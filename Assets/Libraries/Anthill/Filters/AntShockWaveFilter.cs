namespace Anthill.Filters
{
	using System.Collections.Generic;
	using UnityEngine;
	using Anthill.Utils;

	[ExecuteInEditMode]
	[AddComponentMenu("Anthill/Filters/Shock Wave Filter")]
	public class AntShockWaveFilter : MonoBehaviour
	{
		internal class ShockWave
		{
		#region Private Variables

			internal Material _material;
			internal Vector3 _worldPosition; // Range -1.5f, 1.5f
			internal float _value;           // Range -0.1f, 2.0f
			internal float _size;            // Range 0.0f, 10.0f
			internal float _time;

			private AnimationCurve _curve;
			private Camera _camera;
			private bool _isAnimation;

			private float _toSize;
			private float _toValue;
			private float _currentTime;
			private float _targetTime;

			private readonly int _TimeX = Shader.PropertyToID("_TimeX");
			private readonly int _PosX = Shader.PropertyToID("_PosX");
			private readonly int _PosY = Shader.PropertyToID("_PosY");
			private readonly int _Value = Shader.PropertyToID("_Value");
			private readonly int _Size = Shader.PropertyToID("_Size");
			private readonly int _ScreenResolution = Shader.PropertyToID("_ScreenResolution");
			private readonly int _Scale = Shader.PropertyToID("_Scale");

		#endregion

		#region Getters / Setters

			public bool IsActive { get => _isAnimation; }

		#endregion

		#region Public Methods

			public ShockWave(Shader aShader)
			{
				_material = new Material(aShader);
				_material.hideFlags = HideFlags.HideAndDontSave;
				_camera = Camera.main;
				_isAnimation = false;
			}

			public void Reset(Vector3 aPosition, float aSize, float aValue)
			{
				aPosition.z = 0.0f;
				_worldPosition = aPosition;
				_value = aValue;
				_size = aSize;
			}

			public void Start(float aToSize, float aToValue, float aTime, AnimationCurve aCurve)
			{
				_curve = aCurve;
				_toSize = aToSize;
				_toValue = aToValue;
				_currentTime = 0.0f;
				_targetTime = aTime;
				_isAnimation = true;
			}

			public void	Stop()
			{
				_isAnimation = false;
			}

			public void Update(Vector4 aResolution, float aScale)
			{
				if (_isAnimation)
				{
					_currentTime += Time.deltaTime;
					float d = _curve.Evaluate(_currentTime / _targetTime);
					_size = AntMath.Lerp(_size, _toSize, d);
					_value = AntMath.Lerp(_value, _toValue, d);

					if (_currentTime >= _targetTime)
					{
						_isAnimation = false;
					}
				}

				_time += Time.deltaTime;
				if (_time > 100.0f)
				{
					_time = 0.0f;
				}

				if (_value > -0.1f || _size > 0.0f)
				{
					Vector2 screenPos = _camera.WorldToViewportPoint(_worldPosition);
					_material.SetFloat(_TimeX, _time);
					_material.SetFloat(_PosX, screenPos.x);
					_material.SetFloat(_PosY, screenPos.y);
					_material.SetFloat(_Value, _value);
					_material.SetFloat(_Size, _size);
					_material.SetVector(_ScreenResolution, aResolution);
					_material.SetFloat(_Scale, aScale);
				}
			}

		#endregion
		}

	#region Public Variables

		public Shader shader;
		public float unitsSize = 1.0f;

		public static AntShockWaveFilter Current { get; private set; }

		[HideInInspector]
		public List<AntShockwavePreset> presets = new List<AntShockwavePreset>();

		[HideInInspector]
		public int selectedPreset;

		[HideInInspector]
		public bool createByMouseClick;

	#endregion

	#region Private Variables

		private const string SHADER_NAME = "Anthill/ShockWave";
		private readonly List<ShockWave> _waves = new List<ShockWave>();
		private bool _isInitialized;
		private Camera _camera;

	#endregion

	#region Unity Calls

		private void Start()
		{
			shader = Shader.Find(SHADER_NAME);
			if (shader != null)
			{
				_camera = Camera.main;
				_isInitialized = true;
			}
			else
			{
				A.Warning($"AntShockWaveFilter can't find `{SHADER_NAME}` shader!");
				_isInitialized = false;
				enabled = false;
			}

			Current = this;
		}

		private void OnRenderImage(RenderTexture aSourceTexture, RenderTexture aTargetTexture)
		{
			if (_isInitialized && IsActiveAny())
			{
				ShockWave wave;
				float scale = 1.0f / _camera.aspect * unitsSize / 2.0f;
				int rtW = aSourceTexture.width;
				int rtH = aSourceTexture.height;
				var res = new Vector4(aSourceTexture.width, aSourceTexture.height, 0.0f, 0.0f);
				RenderTexture prev = RenderTexture.GetTemporary(rtW, rtH, 0, aSourceTexture.format);
				Graphics.Blit(aSourceTexture, prev);

				for (int i = 0, n = _waves.Count; i < n; i++)
				{
					wave = _waves[i];
					if (wave.IsActive)
					{
						wave.Update(res, scale);
						RenderTexture tex = RenderTexture.GetTemporary(rtW, rtH, 0, prev.format);
						Graphics.Blit(prev, tex, wave._material);
						Graphics.Blit(tex, prev);
						RenderTexture.ReleaseTemporary(tex);
					}
				}

				Graphics.Blit(prev, aTargetTexture);
				RenderTexture.ReleaseTemporary(prev);
			}
			else
			{
				Graphics.Blit(aSourceTexture, aTargetTexture);
			}
		}

		private bool IsActiveAny()
		{
			for (int i = 0, n = _waves.Count; i < n; i++)
			{
				if (_waves[i].IsActive)
				{
					return true;
				}
			}
			return false;
		}

#if UNITY_EDITOR
		private void Update()
		{
			if (!Application.isPlaying)
			{
				shader = Shader.Find(SHADER_NAME);
			}
			else if (createByMouseClick)
			{
				if (Input.GetButtonDown("Fire1"))
				{
					var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					if (selectedPreset >= 0 && selectedPreset < presets.Count)
					{
						// Animate(mousePos.x, // PosX
						// 	mousePos.y, // PosY
						// 	size, // Size
						// 	value, // Value
						// 	toSize, // toSize
						// 	toValue, // toValue
						// 	animTime);
						Animate(presets[selectedPreset], mousePos);
					}
					else
					{
						A.Warning("Preset not selected!");
					}
				}
			}
		}
#endif

	#endregion

	#region Public Methods

		public void Animate(string aPresetName, Vector3 aPosition)
		{
			int index = presets.FindIndex(x => x.name.Equals(aPresetName));
			if (index >= 0 && index < presets.Count)
			{
				Animate(presets[index], aPosition);
			}
			else 
			{
				A.Warning($"Preset `{aPresetName}` not found!");
			}
		}

		public void Animate(AntShockwavePreset aPreset, Vector3 aPosition)
		{
			ShockWave wave = GetAvail();
			if (wave == null)
			{
				wave = new ShockWave(shader);
				_waves.Add(wave);
			}

			wave.Reset(aPosition, aPreset.startDepth, aPreset.startRadius);
			wave.Start(aPreset.endDepth, aPreset.endRadius, aPreset.duration, aPreset.animation);
		}

		private ShockWave GetAvail()
		{
			int index = _waves.FindIndex(x => !x.IsActive);
			return (index >= 0 && index < _waves.Count) ? _waves[index] : null;
		}

	#endregion
	}
}