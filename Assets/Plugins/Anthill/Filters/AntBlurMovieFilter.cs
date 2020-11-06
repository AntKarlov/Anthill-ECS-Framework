namespace Anthill.Filters
{
	using UnityEngine;

	[ExecuteInEditMode]
	[AddComponentMenu("Anthill/Filters/Blur Movie Filter")]
	public class AntBlurMovieFilter : MonoBehaviour
	{
		public Shader shader;
		public bool fixedResolution;
		public Vector2 resolution = new Vector2(1024.0f, 768.0f);
		[Range(0, 1000)]
		public float radius = 150.0f;
		[Range(0, 1000)]
		public float factor = 200.0f;
		[RangeAttribute(1, 8)]
		public int fastFilter = 2;

		private const string SHADER_NAME = "Anthill/BlurMovie";
		private float _time = 1.0f;
		private Material _material;
		private bool _isInitialized;

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
				A.Warning($"AntBlurMovieFilter can't find {SHADER_NAME} shader!");
				enabled = false;
				_isInitialized = false;
			}
		}

		private void OnRenderImage(RenderTexture aSourceTexture, RenderTexture aTargetTexture)
		{
			if (_isInitialized)
			{
				_time += Time.deltaTime;
				if (_time > 100)
				{
					_time = 0.0f;
				}

				int downScale = fastFilter;
				var res = new Vector4(resolution.x, resolution.y, 0.0f, 0.0f);
				if (!fixedResolution)
				{
					res.x = aSourceTexture.width;
					res.y = aSourceTexture.height;
				}
				
				_material.SetFloat("_TimeX", _time);
				_material.SetFloat("_Radius", radius / downScale);
				_material.SetFloat("_Factor", factor);
				_material.SetVector("_ScreenResolution", res);
				
				if (fastFilter > 1)
				{
					int rtW = aSourceTexture.width / downScale;
					int rtH = aSourceTexture.height / downScale;
					RenderTexture buffer = RenderTexture.GetTemporary(rtW, rtH, 0);
					Graphics.Blit(aSourceTexture, buffer, _material);
					Graphics.Blit(buffer, aTargetTexture);
					RenderTexture.ReleaseTemporary(buffer);
				}
				else
				{
					Graphics.Blit(aSourceTexture, aTargetTexture, _material);
				}
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