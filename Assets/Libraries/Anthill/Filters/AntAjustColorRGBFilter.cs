namespace Anthill.Filters
{
	using UnityEngine;

	[ExecuteInEditMode]
	[AddComponentMenu("Anthill/Filters/Ajust Color RGB Filter")]
	public class AntAjustColorRGBFilter : MonoBehaviour
	{
	#region Public Variables

		public Shader shader;

		[Range(-200.0f, 200.0f)]
		public float red;

		[Range(-200.0f, 200.0f)]
		public float green;

		[Range(-200.0f, 200.0f)]
		public float blue;
		
		[Range(-100.0f, 100.0f)]
		public float brightness;

	#endregion

	#region Private Variables

		private const string SHADER_NAME = "Anthill/AdjustColorRGB";
		private float _time = 1.0f;
		private Material _material;
		private bool _isInitialized;

		private readonly int _TimeX = Shader.PropertyToID("_TimeX");
		private readonly int _Red = Shader.PropertyToID("_Red");
		private readonly int _Green = Shader.PropertyToID("_Green");
		private readonly int _Blue = Shader.PropertyToID("_Blue");
		private readonly int _Bright = Shader.PropertyToID("_Bright");

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
				A.Warning($"AntAjustColorRGBFilter can't find the `{SHADER_NAME}` Shader!");
				enabled = false;
				_isInitialized = false;
			}
		}

#if UNITY_EDITOR
		private void Update()
		{
			if (!Application.isPlaying)
			{
				shader = Shader.Find(SHADER_NAME);
			}
		}
#endif

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
				_material.SetFloat(_Red, red / 100.0f);
				_material.SetFloat(_Green, green / 100.0f);
				_material.SetFloat(_Blue, blue / 100.0f);
				_material.SetFloat(_Bright, brightness / 100.0f);
				Graphics.Blit(aSourceTexture, aTargetTexture, _material);
			}
			else
			{
				Graphics.Blit(aSourceTexture, aTargetTexture);
			}
		}
		
	#endregion
	}
}