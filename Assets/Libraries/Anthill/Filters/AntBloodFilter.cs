namespace Anthill.Filters
{
	using UnityEngine;
	using Anthill.Utils;

	[ExecuteInEditMode]
	[AddComponentMenu("Anthill/Filters/Blood Filter")]
	public class AntBloodFilter : MonoBehaviour
	{
	#region Public Variables

		public Shader shader;
		public Texture2D texture;

		[Range(0.0f, 1.0f)]
		public float hitLeft = 1.0f;
		[Range(0.0f, 1.0f)]
		public float hitUp;
		[Range(0.0f, 1.0f)]
		public float hitRight;
		[Range(0.0f, 1.0f)]
		public float hitDown;

		[Range(0.0f, 1.0f)]
		public float bloodHitLeft;
		[Range(0.0f, 1.0f)]
		public float bloodHitUp;
		[Range(0.0f, 1.0f)]
		public float bloodHitRight;
		[Range(0.0f, 1.0f)]
		public float bloodHitDown;

		[Range(0.0f, 1.0f)]
		public float hitFull;
		[Range(0.0f, 1.0f)]
		public float bloodHitFull1;
		[Range(0.0f, 1.0f)]
		public float bloodHitFull2;
		[Range(0.0f, 1.0f)]
		public float bloodHitFull3;

		[Range(0.0f, 1.0f)]
		public float lightReflect = 0.5f;

	#endregion

	#region Private Variables

		private const string SHADER_NAME = "Anthill/BloodFade";
		private const string TEXTURE_NAME = "BloodFadeTexture";
		private float _time = 1.0f;
		private Material _material;
		private bool _isInitialized;

		private readonly int _TimeX = Shader.PropertyToID("_TimeX");
		private readonly int _Value = Shader.PropertyToID("_Value");
		private readonly int _Value2 = Shader.PropertyToID("_Value2");
		private readonly int _Value3 = Shader.PropertyToID("_Value3");
		private readonly int _Value4 = Shader.PropertyToID("_Value4");
		private readonly int _Value5 = Shader.PropertyToID("_Value5");
		private readonly int _Value6 = Shader.PropertyToID("_Value6");
		private readonly int _Value7 = Shader.PropertyToID("_Value7");
		private readonly int _Value8 = Shader.PropertyToID("_Value8");
		private readonly int _Value9 = Shader.PropertyToID("_Value9");
		private readonly int _Value10 = Shader.PropertyToID("_Value10");
		private readonly int _Value11 = Shader.PropertyToID("_Value11");
		private readonly int _Value12 = Shader.PropertyToID("_Value12");
		private readonly int _Value13 = Shader.PropertyToID("_Value13");
		private readonly int _MainTex2 = Shader.PropertyToID("_MainTex2");

	#endregion

	#region Unity Calls

		private void Start()
		{
			shader = Shader.Find(SHADER_NAME);
			if (shader != null)
			{
				texture = Resources.Load(TEXTURE_NAME) as Texture2D;
				if (texture != null)
				{
					_material = new Material(shader);
					_material.hideFlags = HideFlags.HideAndDontSave;
					_isInitialized = true;
				}
				else
				{
					A.Warning($"AntBloodFilter can't find {TEXTURE_NAME} texture!");
				}
			}
			else
			{
				A.Warning($"AntArcadeFilter can't find the `{SHADER_NAME}` shader!");
				_isInitialized = false;
				enabled = false;
			}
		}

		private void OnRenderImage(RenderTexture aSourceTexture, RenderTexture aTargetTexture)
		{
			if (_isInitialized && IsActive)
			{
				_time += Time.deltaTime;
				if (_time > 100)
				{
					_time = 0.0f;
				}

				_material.SetFloat(_TimeX, _time);
				_material.SetFloat(_Value, lightReflect);
				_material.SetFloat(_Value2, Mathf.Clamp(hitLeft, 0.0f, 1.0f));
				_material.SetFloat(_Value3, Mathf.Clamp(hitUp, 0, 1));
				_material.SetFloat(_Value4, Mathf.Clamp(hitRight, 0, 1));
				_material.SetFloat(_Value5, Mathf.Clamp(hitDown, 0, 1));
				_material.SetFloat(_Value6, Mathf.Clamp(bloodHitLeft, 0, 1));
				_material.SetFloat(_Value7, Mathf.Clamp(bloodHitUp, 0, 1));
				_material.SetFloat(_Value8, Mathf.Clamp(bloodHitRight, 0, 1));
				_material.SetFloat(_Value9, Mathf.Clamp(bloodHitDown, 0, 1));
				_material.SetFloat(_Value10, Mathf.Clamp(hitFull, 0, 1));
				_material.SetFloat(_Value11, Mathf.Clamp(bloodHitFull1, 0, 1));
				_material.SetFloat(_Value12, Mathf.Clamp(bloodHitFull2, 0, 1));
				_material.SetFloat(_Value13,Mathf.Clamp(bloodHitFull3, 0, 1));
				_material.SetTexture(_MainTex2, texture);
				Graphics.Blit(aSourceTexture, aTargetTexture, _material);
			}
			else
			{
				Graphics.Blit(aSourceTexture, aTargetTexture);
			}
		}

#if UNITY_EDITOR
		private void Update()
		{
			if (!Application.isPlaying)
			{
				shader = Shader.Find(SHADER_NAME);
				texture = Resources.Load(TEXTURE_NAME) as Texture2D;
			}
		}
#endif

	#endregion

	#region Getters / Setters

		private bool IsActive
		{
			get => (hitLeft > 0.0f ||
					hitUp > 0.0f ||
					hitRight > 0.0f ||
					hitDown > 0.0f ||
					bloodHitLeft > 0.0f ||
					bloodHitUp > 0.0f ||
					bloodHitRight > 0.0f ||
					bloodHitDown > 0.0f ||
					hitFull > 0.0f ||
					bloodHitFull1 > 0.0f ||
					bloodHitFull2 > 0.0f ||
					bloodHitFull3 > 0.0f);
		}

		private float WarningLevel
		{
			get => hitFull;
			set
			{
				hitFull = (value > 1.0f) ? 1.0f : value;
				hitLeft = hitFull * 0.25f;
				hitUp = hitFull * 0.25f;
				hitRight = hitFull * 0.25f;
				hitDown = hitFull * 0.25f;
			}
		}

	#endregion
	}
}