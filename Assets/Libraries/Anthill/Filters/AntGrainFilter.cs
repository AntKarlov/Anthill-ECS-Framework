using UnityEngine;

namespace Anthill.Filters
{
	[ExecuteInEditMode]
	[AddComponentMenu("Anthill/Filters/Grain")]
	public class AntGrainFilter : MonoBehaviour
	{
	#region Public Variables

		public Shader shader;
		[Range(-64.0f, 64.0f)]
		public float value;

	#endregion

	#region Private Variables

		private const string SHADER_NAME = "Anthill/Grain";
		private float _time = 1.0f;
		private Material _material;
		private bool _isInitialized;

		private readonly int _TimeX = Shader.PropertyToID("_TimeX");
		private readonly int _Value = Shader.PropertyToID("_Value");

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
				A.Warning($"AntGrainFilter can't find `{SHADER_NAME}` shader!");
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
				_material.SetFloat(_Value, value);
				//_material.SetFloat("_Smooth", smooth);
				//_material.SetFloat("_Value3", stretchX);
				//_material.SetFloat("_Value4", stretchY);
				//_material.SetVector("_ScreenResolution",new Vector4(aSourceTexture.width, aSourceTexture.height,0.0f,0.0f));
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