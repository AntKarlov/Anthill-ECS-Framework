namespace Anthill.Filters
{
	using UnityEngine;

	[ExecuteInEditMode]
	[AddComponentMenu("Anthill/Filters/Vignetting Filter")]
	public class AntVignettingFilter : MonoBehaviour
	{
	#region Public Variables

		public Shader shader;
		public Texture2D texture;

		[Range(0.0f, 1.0f)]
		public float vignetting = 1.0f;
		
		[Range(0.0f, 1.0f)]
		public float vignettingFull;

		[Range(0.0f, 1.0f)]
		public float vignettingDirt;
		public Color vignettingColor = new Color(0.0f, 0.0f, 0.0f);

	#endregion

	#region Private Variables

		private const string SHADER_NAME = "Anthill/Vignetting";
		private const string TEXTURE_NAME = "VignettingTexture";
		private float _time = 1.0f;
		private Material _material;
		private bool _isInitialized;

		private readonly int Vignette = Shader.PropertyToID("Vignette");
		private readonly int _Vignetting = Shader.PropertyToID("_Vignetting");
		private readonly int _Vignetting2 = Shader.PropertyToID("_Vignetting2");
		private readonly int _VignettingColor = Shader.PropertyToID("_VignettingColor");
		private readonly int _VignettingDirt = Shader.PropertyToID("_VignettingDirt");

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
					A.Warning($"AntVignettingFilter can't `{TEXTURE_NAME}` not found!");
				}
			}
			else
			{
				A.Warning($"AntVignettingFilter can't `{SHADER_NAME}` not found!");
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

				_material.SetTexture(Vignette, texture);
				_material.SetFloat(_Vignetting, vignetting);
				_material.SetFloat(_Vignetting2, vignettingFull);
				_material.SetColor(_VignettingColor, vignettingColor);
				_material.SetFloat(_VignettingDirt, vignettingDirt);
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
			}
		}
#endif

	#endregion
	}
}