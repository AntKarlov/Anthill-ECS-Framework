namespace Anthill.Filters
{
	using UnityEngine;

	[ExecuteInEditMode]
	[AddComponentMenu("Anthill/Filters/Wide Screen")]
	public class AntWideScreenFilter : MonoBehaviour
	{
	#region Public Variables

		public Shader shader;

		[Range(0.0f, 0.8f)]
		public float size = 0.55f;

		[Range(0.001f, 0.4f)]
		public float smooth = 0.01f;
		public Color color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

	#endregion

	#region Private Variables

		private const string SHADER_NAME = "Anthill/WideScreenHorizontal";
		private float _time = 1.0f;
		private bool _unscaledTime;
		private Material _material;
		private bool _isInitialized;

		private readonly int _TimeX = Shader.PropertyToID("_TimeX");
		private readonly int _Size = Shader.PropertyToID("_Size");
		private readonly int _Smooth = Shader.PropertyToID("_Smooth");
		private readonly int _Color = Shader.PropertyToID("_Color");

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
				A.Warning($"AntWideScreenFilter can't find `{SHADER_NAME}` shader!");
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
				_time += (_unscaledTime)
					? Time.unscaledDeltaTime
					: Time.deltaTime;
				
				if (_time > 100.0f) 
				{
					_time = 0.0f;
				}

				_material.SetFloat(_TimeX, _time);
				_material.SetFloat(_Size, size);
				_material.SetFloat(_Smooth, smooth);
				_material.SetColor(_Color, color);
				Graphics.Blit(aSourceTexture, aTargetTexture, _material);
			}
			else
			{
				Graphics.Blit(aSourceTexture, aTargetTexture);
			}
		}

		public void SetUpdate(bool aUnscaledTime)
		{
			_unscaledTime = aUnscaledTime;
		}
		
	#endregion
	}
}