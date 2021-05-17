namespace Anthill.Filters
{
	using UnityEngine;

	[ExecuteInEditMode]
	[System.Serializable]
	public class AntDesintegrationFX : MonoBehaviour
	{
	#region Public Variables

		public Material forceMaterial;
		public bool activeChange = true;
		private string shader = "Anthill/DesintegrationFX";

		[Range(0, 1)]
		public float _alpha = 1f;

		public Color _color = new Color (0f, 1f, 1f, 1f); 

		[Range(0, 1)]
		public float seed = 1.0f;

		[Range(0, 1)]
		public float desintegration = 0.5f;

	#endregion

	#region Private Variables

		private int _shaderChange = 0;
		private Material _tempMaterial;
		private Material _defaultMaterial;
		private SpriteRenderer _spriteRenderer;
		private bool _isHasSprite;

		private readonly int _Alpha = Shader.PropertyToID("_Alpha");
		private readonly int _Distortion = Shader.PropertyToID("_Distortion");
		private readonly int _ColorX = Shader.PropertyToID("_ColorX");
		private readonly int _Size = Shader.PropertyToID("_Size");

	#endregion

	#region Unity Calls

		private void Start()
		{  
			_shaderChange = 0;
		}

	 	public void CallUpdate()
		{
			Update();
		}

		private void Update()
		{	
			if (_shaderChange == 0 && forceMaterial != null)
			{
				_shaderChange = 1;

				if (_tempMaterial != null)
				{
					DestroyImmediate(_tempMaterial);
				}

				if(_isHasSprite)
				{
					_spriteRenderer.sharedMaterial = forceMaterial;
				}
				forceMaterial.hideFlags = HideFlags.None;
				forceMaterial.shader = Shader.Find(shader);
			}

			if (forceMaterial == null && _shaderChange == 1)
			{
				if (_tempMaterial != null)
				{
					DestroyImmediate(_tempMaterial);
				}

				_tempMaterial = new Material(Shader.Find(shader));
				_tempMaterial.hideFlags = HideFlags.None;

				if (_isHasSprite)
				{
					_spriteRenderer.sharedMaterial = _tempMaterial;
				}

				_shaderChange = 0;
			}
			
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				string dfname = "";
				if (this.gameObject.GetComponent<SpriteRenderer>() != null)
				{
					dfname = this.GetComponent<Renderer>().sharedMaterial.shader.name;
				}

				if (dfname.Equals("Sprites/Default"))
				{
					forceMaterial.shader = Shader.Find(shader);
					forceMaterial.hideFlags = HideFlags.None;

					if (this.gameObject.GetComponent<SpriteRenderer>() != null)
					{
						this.GetComponent<Renderer>().sharedMaterial = forceMaterial;
					}
				}
			}
#endif

			if (activeChange)
			{
				if (_isHasSprite)
				{
					_spriteRenderer.sharedMaterial.SetFloat(_Alpha, 1 - _alpha);
					_spriteRenderer.sharedMaterial.SetFloat(_Distortion, desintegration);
					_spriteRenderer.sharedMaterial.SetColor(_ColorX, _color);
					_spriteRenderer.sharedMaterial.SetFloat(_Size, seed);
				}
			}
		}
		
		private void OnDestroy()
		{
			if (Application.isPlaying == false && Application.isEditor == true)
			{
				if (_tempMaterial != null)
				{
					DestroyImmediate(_tempMaterial);
				}
				
				if (gameObject.activeSelf && _defaultMaterial != null)
				{
					if (_isHasSprite)
					{
						_spriteRenderer.sharedMaterial = _defaultMaterial;
						_spriteRenderer.sharedMaterial.hideFlags = HideFlags.None;
					}
				}	
			}
		}

		private void OnDisable()
		{
			if (gameObject.activeSelf && _defaultMaterial != null)
			{
				if (_isHasSprite)
				{
					_spriteRenderer.sharedMaterial = _defaultMaterial;
					_spriteRenderer.sharedMaterial.hideFlags = HideFlags.None;
				}
			}		
		}
		
		private void OnEnable()
		{
			_spriteRenderer = GetComponent<SpriteRenderer>();
			_isHasSprite = (_spriteRenderer != null);

			if (_defaultMaterial == null)
			{
				_defaultMaterial = new Material(Shader.Find("Sprites/Default"));
			}

			if (forceMaterial == null)
			{
				activeChange = true;
				_tempMaterial = new Material(Shader.Find(shader));
				_tempMaterial.hideFlags = HideFlags.None;
				if (_isHasSprite)
				{
					_spriteRenderer.sharedMaterial = _tempMaterial;
				}
			}
			else
			{
				forceMaterial.shader = Shader.Find(shader);
				forceMaterial.hideFlags = HideFlags.None;
				if (_isHasSprite)
				{
					_spriteRenderer.sharedMaterial = forceMaterial;
				}
			}
		}

	#endregion

	#region Getters / Setters

		public float Desintegration
		{
			get => desintegration;
			set
			{
				desintegration = value;
				if (_isHasSprite)
				{
					_spriteRenderer.sharedMaterial.SetFloat("_Distortion", desintegration);
				}
			}
		}

	#endregion
	}
}