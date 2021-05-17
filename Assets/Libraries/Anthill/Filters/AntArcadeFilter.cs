namespace Anthill.Filters
{
	using UnityEngine;

	[ExecuteInEditMode]
	[AddComponentMenu("Anthill/Filters/Arcade Filter")]
	public class AntArcadeFilter : MonoBehaviour
	{
	#region Public Variables

		public Shader shader;

		[Range(0.0f, 10.0f)]
		public float interferanceSize = 1.0f;

		[Range(0.0f, 10.0f)]
		public float interferanceSpeed = 0.5f;

		[Range(0.0f, 10.0f)]
		public float contrast = 1.0f;

		[Range(0.001f, 10.0f)]
		public float value = 1.1f;

	#endregion
	
	#region Private Variables

		private const string SHADER_NAME = "Anthill/Arcade";
		private float _time = 1.0f;
		private Material _material;
		private bool _isInitialized;

		private readonly int _TimeX = Shader.PropertyToID("_TimeX");
		private readonly int _Size = Shader.PropertyToID("_Size");
		private readonly int _Speed = Shader.PropertyToID("_Speed");
		private readonly int _Value = Shader.PropertyToID("_Value");
		private readonly int _Contrast = Shader.PropertyToID("_Contrast");
		private readonly int _ScreenResolution = Shader.PropertyToID("_ScreenResolution");

	#endregion

	#region Unity Calls

		private void Start()
		{
			shader = Shader.Find(SHADER_NAME);
			if (shader != null)
			{
				_material = new Material(shader);
				_material.hideFlags = HideFlags.HideAndDontSave;
				_isInitialized = true;
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
			if (_isInitialized)
			{
				_time += Time.deltaTime;
				if (_time > 100.0f)
				{
					_time = 0.0f;
				}

				_material.SetFloat(_TimeX, _time);
				_material.SetFloat(_Size, interferanceSize);
				_material.SetFloat(_Speed, interferanceSpeed);
				_material.SetFloat(_Value, value);
				_material.SetFloat(_Contrast, contrast);
				_material.SetVector(_ScreenResolution, new Vector4(aSourceTexture.width, aSourceTexture.height, 0.0f, 0.0f));
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
			if (!Application.isPlaying && shader == null)
			{
				shader = Shader.Find(SHADER_NAME);
			}
		}
#endif

	#endregion
	}
}