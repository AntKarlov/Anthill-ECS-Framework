namespace Anthill.Filters
{
	using UnityEngine;

	[ExecuteInEditMode]
	[AddComponentMenu("Anthill/Filters/Ajust Color Filter")]
	public class AntAjustColorFilter : MonoBehaviour
	{
	#region Public Variables

		public Shader shader;

		[Range(-200.0f, 200.0f)]
		public float redR = 100.0f;

		[Range(-200.0f, 200.0f)]
		public float redG;

		[Range(-200.0f, 200.0f)]
		public float redB;

		[Range(-200.0f, 200.0f)]
		public float redContrast;

		[Range(-200.0f, 200.0f)]
		public float greenR;

		[Range(-200.0f, 200.0f)]
		public float greenG = 100.0f;

		[Range(-200.0f, 200.0f)]
		public float greenB;

		[Range(-200.0f, 200.0f)]
		public float greenContrast;

		[Range(-200.0f, 200.0f)]
		public float blueR;

		[Range(-200.0f, 200.0f)]
		public float blueG;

		[Range(-200.0f, 200.0f)]
		public float blueB = 100.0f;

		[Range(-200.0f, 200.0f)]
		public float blueContrast;

	#endregion

	#region Private Variables

		private const string SHADER_NAME = "Anthill/AjustColorFull";
		private float _time = 1.0f;
		private Material _material;
		private bool _isInitialized;

		private readonly int _TimeX = Shader.PropertyToID("_TimeX");
		private readonly int _Red_R = Shader.PropertyToID("_Red_R");
		private readonly int _Red_G = Shader.PropertyToID("_Red_G");
		private readonly int _Red_B = Shader.PropertyToID("_Red_B");
		private readonly int _Green_R = Shader.PropertyToID("_Green_R");
		private readonly int _Green_G = Shader.PropertyToID("_Green_G");
		private readonly int _Green_B = Shader.PropertyToID("_Green_B");
		private readonly int _Blue_R = Shader.PropertyToID("_Blue_R");
		private readonly int _Blue_G = Shader.PropertyToID("_Blue_G");
		private readonly int _Blue_B = Shader.PropertyToID("_Blue_B");
		private readonly int _Red_C = Shader.PropertyToID("_Red_C");
		private readonly int _Green_C = Shader.PropertyToID("_Green_C");
		private readonly int _Blue_C = Shader.PropertyToID("_Blue_C");

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
				A.Warning($"AntAjustColorFilter can't find the `{SHADER_NAME}` shader!");
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
				_material.SetFloat(_Red_R, redR / 100.0f);
				_material.SetFloat(_Red_G, redG / 100.0f);
				_material.SetFloat(_Red_B, redB / 100.0f);
				_material.SetFloat(_Green_R, greenR / 100.0f);
				_material.SetFloat(_Green_G, greenG / 100.0f);
				_material.SetFloat(_Green_B, greenB / 100.0f);
				_material.SetFloat(_Blue_R, blueR / 100.0f);
				_material.SetFloat(_Blue_G, blueG / 100.0f);
				_material.SetFloat(_Blue_B, blueB / 100.0f);
				_material.SetFloat(_Red_C, redContrast / 100.0f);
				_material.SetFloat(_Green_C, greenContrast / 100.0f);
				_material.SetFloat(_Blue_C, blueContrast / 100.0f);
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