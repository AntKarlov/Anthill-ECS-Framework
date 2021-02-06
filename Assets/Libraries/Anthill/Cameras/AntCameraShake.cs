namespace Anthill.Cameras
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[ExecuteInEditMode]
	public class AntCameraShake : MonoBehaviour
	{
		private const float MIN_SHAKE_VALUE = 0.001f;
		private const float MIN_ROTATION_VALUE = 0.001f;

		public enum ShakeType
		{
			CameraMatrix,
			LocalPosition
		}

		public static AntCameraShake Current;
		public delegate void ShakeDelegate(AntCameraShake aCameraShake);
		public delegate void ShakeOffsetDelegate(Vector3 aOffsetPosition, Quaternion aOffsetRotation);
		public delegate void ShakeRumbleDelegate(int aMotorId, float aMotorForce, float aMotorDuration);
		public ShakeDelegate EventStartShaking;
		public ShakeDelegate EventEndShaking;
		public ShakeDelegate EventPreShake;
		public ShakeDelegate EventPostShake;
		public ShakeRumbleDelegate EventRumble;
		public ShakeOffsetDelegate EventShakeOffset;

		[HideInInspector] public List<Camera> cameras = new List<Camera>();
		[HideInInspector] public List<AntCameraShakePreset> presets = new List<AntCameraShakePreset>();
		[HideInInspector] public ShakeType shakeType = ShakeType.CameraMatrix;
		[HideInInspector] public int numShakes = 2;
		[HideInInspector] public Vector3 shakeAmount = Vector3.one;
		[HideInInspector] public Vector3 rotationAmount = Vector3.one;
		[HideInInspector] public float distance = 0.1f;
		[HideInInspector] public float speed = 50.0f;
		[HideInInspector] public float decay = 0.20f;
		[HideInInspector] public bool multiplyByTimeScale = true;

		private bool _shaking;
		private bool _cancelling;

		private readonly List<Vector3> _matrixOffsetCache = new List<Vector3>(10);
		private readonly List<Quaternion> _matrixRotationCache = new List<Quaternion>(10);
		private readonly List<Vector3> _offsetCache = new List<Vector3>(10);
		private readonly List<Quaternion> _rotationCache = new List<Quaternion>(10);
		private readonly List<bool> _ignoreMatrixCache = new List<bool>(10);
		private readonly List<bool> _ignorePositionCache = new List<bool>(10);

		internal class ShakeState
		{
			internal readonly ShakeType _shakeType;
			internal readonly Vector3 _startPosition;
			internal readonly Quaternion _startRotation;
			internal Vector3 _shakePosition;
			internal Quaternion _shakeRotation;

			internal ShakeState(ShakeType aShakeType, Vector3 aPosition, Quaternion aRotation)
			{
				_shakeType = aShakeType;
				_startPosition = aPosition;
				_startRotation = aRotation;
				_shakePosition = aPosition;
				_shakeRotation = aRotation;
			}
		}

		readonly Dictionary<Camera, List<ShakeState>> _states = new Dictionary<Camera, List<ShakeState>>();
		readonly Dictionary<Camera, int> _shakeCount = new Dictionary<Camera, int>();

		readonly static List<AntCameraShake> _components = new List<AntCameraShake>();

		#region Getters / Setters

		public bool IsShaking { get => _shaking; }
		public bool IsCancelling { get => _cancelling; }

		#endregion
		#region Static Methods

		public static AntCameraShake[] GetComponents()
		{
			return _components.ToArray();
		}

		public static void ShakeAll()
		{
			for (int i = 0, n = _components.Count; i < n; i++)
			{
				_components[i].Shake();
			}
		}

		public static void ShakeAll(ShakeDelegate aCallback)
		{
			AntCameraShake component;
			for (int i = 0, n = _components.Count; i < n; i++)
			{
				component = _components[i];
				component.EventEndShaking += aCallback;
				component.Shake();
			}
		}

		public static void ShakeAll(ShakeType aShakeType, int aNumShakes, Vector3 aShakeAmout,
			Vector3 aRotationAmount, float aDistance, float aSpeed, float aDecay, bool aMultiplyByTimeScale)
		{
			for (int i = 0, n = _components.Count; i < n; i++)
			{
				_components[i].Shake(aShakeType, aNumShakes, aShakeAmout, aRotationAmount, aDistance, aSpeed, aDecay, aMultiplyByTimeScale);
			}
		}

		public static void ShakeAll(ShakeType aShakeType, int aNumShakes, Vector3 aShakeAmout,
			Vector3 aRotationAmount, float aDistance, float aSpeed, float aDecay, bool aMultiplyByTimeScale, ShakeDelegate aCallback)
		{
			AntCameraShake component;
			for (int i = 0, n = _components.Count; i < n; i++)
			{
				component = _components[i];
				component.EventEndShaking += aCallback;
				component.Shake(aShakeType, aNumShakes, aShakeAmout, aRotationAmount, aDistance, aSpeed, aDecay, aMultiplyByTimeScale);
			}
		}

		public static void CancelAllShakes()
		{
			for (int i = 0, n = _components.Count; i < n; i++)
			{
				_components[i].CancelShake();
			}
		}

		public static void CancelAllShakes(float aTime)
		{
			for (int i = 0, n = _components.Count; i < n; i++)
			{
				_components[i].CancelShake(aTime);
			}
		}

		#endregion
		#region Unity Calls

		private void OnEnable()
		{
			Current = this;
			if (Application.isPlaying)
			{
				if (cameras.Count < 1)
				{
					var cam = GetComponent<Camera>();
					if (cam != null)
					{
						cameras.Add(GetComponent<Camera>());
					}
				}

				if (cameras.Count < 1)
				{
					if (Camera.main)
					{
						cameras.Add(Camera.main);
					}
				}

				if (cameras.Count < 1)
				{
					A.Editor.Warning("Can't find the Cameras!", this);
				}

				_components.Add(this);
			}
		}

		// private void Update()
		// {
		// 	if (cameraMotion)
		// 	{
		// 		Camera.main.transform.localPosition = new Vector3(
		// 			Mathf.Sin(Time.time) * 0.1f,
		// 			Mathf.Cos(Time.time) * 0.05f,
		// 			Camera.main.transform.localPosition.z);
		// 	}
		// }

		private void OnDisable()
		{
			_components.Remove(this);
		}

		#endregion
		#region Public Methods

		public void Shake(string aPresetName)
		{
			int index = presets.FindIndex(x => x.name.Equals(aPresetName));
			if (index >= 0 && index < presets.Count)
			{
				Shake(presets[index]);
			}
			else 
			{
				A.Warning($"Can't find `{aPresetName}` preset!", this);
			}
		}

		public void Shake(AntCameraShakePreset aPreset)
		{
			shakeType = aPreset.shakeType;
			numShakes = aPreset.numShakes;
			shakeAmount = aPreset.shakeAmount;
			rotationAmount = aPreset.rotationAmount;
			distance = aPreset.distance;
			speed = aPreset.speed;
			decay = aPreset.decay;
			multiplyByTimeScale = !aPreset.ignoreTimeScale;

			if (aPreset.rumble && EventRumble != null)
			{
				EventRumble(aPreset.motorId, aPreset.motorForce, aPreset.motorDuration);
			}

			Shake();
		}

		public void Shake()
		{
			Vector3 seed = Random.insideUnitSphere;
			for (int i = 0, n = cameras.Count; i < n; i++)
			{
				StartCoroutine(DoShake(cameras[i], seed, shakeType, numShakes, shakeAmount, rotationAmount,
					distance, speed, decay, multiplyByTimeScale));
			}
		}

		public void Shake(ShakeType aShakeType, int aNumShakes, Vector3 aShakeAmount, Vector3 aRotationAmount, 
			float aDistance, float aSpeed, float aDecay, bool aMultiplyByTimeScale)
		{
			Vector3 seed = Random.insideUnitSphere;
			for (int i = 0, n = cameras.Count; i < n; i++)
			{
				StartCoroutine(DoShake(cameras[i], seed, aShakeType, aNumShakes, aShakeAmount, aRotationAmount,
					aDistance, aSpeed, aDecay, aMultiplyByTimeScale));
			}
		}

		public void CancelShake()
		{
			if (_shaking && !_cancelling)
			{
				_shaking = false;
				StopAllCoroutines();
				Camera cam;
				for (int i = 0, n = cameras.Count; i < n; i++)
				{
					cam = cameras[i];
					if (_shakeCount.ContainsKey(cam))
					{
						_shakeCount[cam] = 0;
					}
					ResetState(cam);
				}
			}
		}

		public void CancelShake(float aTime)
		{
			if (_shaking && !_cancelling)
			{
				StopAllCoroutines();
				StartCoroutine(DoResetState(cameras, _shakeCount, aTime));
			}
		}

		#endregion
		#region Private Methods

		private IEnumerator DoResetState(IList<Camera> aCameras, IDictionary<Camera, int> aShakeCount, float aTime)
		{
			_matrixOffsetCache.Clear();
			_matrixRotationCache.Clear();
			_offsetCache.Clear();
			_rotationCache.Clear();
			_ignoreMatrixCache.Clear();
			_ignorePositionCache.Clear();

			Camera cam;
			Transform cachedTransform;
			for (int i = 0, n = cameras.Count; i < n; i++)
			{
				cam = cameras[i];
				cachedTransform = cam.transform;
				_matrixOffsetCache.Add((Vector3)((cam.worldToCameraMatrix * cachedTransform.worldToLocalMatrix.inverse).GetColumn(3)));
				_matrixRotationCache.Add(
					QuaternionFromMatrix((cam.worldToCameraMatrix * cachedTransform.worldToLocalMatrix.inverse).inverse * Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1.0f, 1.0f, -1.0f)))
				);
				
				_offsetCache.Add(cachedTransform.localPosition);
				_rotationCache.Add(cachedTransform.localRotation);

				if (aShakeCount.ContainsKey(cam))
				{
					aShakeCount[cam] = 0;
				}

				bool ignoreMatrix = true;
				bool ignorePosition = true;
				var states = _states[cam];
				ShakeState state;
				for (int j = 0, n1 = states.Count; j < n1; j++)
				{
					state = states[j];
					if (state._shakeType == ShakeType.CameraMatrix)
					{
						ignoreMatrix = false;
					}
					else
					{
						ignorePosition = false;
					}

					if (!(ignoreMatrix || ignorePosition))
					{
						break;
					}
				}

				_ignoreMatrixCache.Add(ignoreMatrix);
				_ignorePositionCache.Add(ignorePosition);
				_states[cam].Clear();
			}

			float t = 0.0f;
			_cancelling = true;
			while (t < aTime)
			{
				int i = 0;
				for (int j = 0, n = cameras.Count; j < n; j++)
				{
					cam = cameras[j];
					cachedTransform = cam.transform;
					if (_ignoreMatrixCache[j])
					{
						var matrixPos = Vector3.Lerp(_matrixOffsetCache[i], Vector3.zero, t / aTime);
						var matrixRot = Quaternion.Slerp(_matrixRotationCache[i], cachedTransform.rotation, t / aTime);
						var m = Matrix4x4.TRS(matrixPos, matrixRot, new Vector3(1.0f, 1.0f, -1.0f));
						cam.worldToCameraMatrix = m * cachedTransform.worldToLocalMatrix;
					}

					if (_ignorePositionCache[j])
					{
						var pos = Vector3.Lerp(_offsetCache[i], Vector3.zero, t / aTime);
						var rot = Quaternion.Slerp(_rotationCache[i], Quaternion.identity, t / aTime);
						cachedTransform.localPosition = pos;
						cachedTransform.localRotation = rot;
					}
					i++;
				}
				t += Time.deltaTime;
				yield return null;
			}

			for (int i = 0, n = cameras.Count; i < n; i++)
			{
				cam = cameras[i];
				if (!_ignoreMatrixCache[i])
				{
					cam.ResetWorldToCameraMatrix();
				}

				if (!_ignorePositionCache[i])
				{
					cam.transform.localPosition = Vector3.zero;
					cam.transform.localRotation = Quaternion.identity;
				}
			}

			_shaking = false;
			_cancelling = false;
		}

		private IEnumerator DoShake(Camera aCamera, Vector3 aSeed, ShakeType aShakeType, int aNumShakes,
			Vector3 aShakeAmount, Vector3 aRotationAmount, float aDistance, float aSpeed, float aDecay, bool aMultByTimeScale)
		{
			if (_cancelling)
			{
				yield return null;
			}

			// Random values
			float mod1 = aSeed.x > 0.5f ? 1.0f : -1.0f;
			float mod2 = aSeed.y > 0.5f ? 1.0f : -1.0f;
			float mod3 = aSeed.z > 0.5f ? 1.0f : -1.0f;

			// First Shake
			if (!_shaking)
			{
				_shaking = true;
				EventStartShaking?.Invoke(this);
			}

			if (_shakeCount.ContainsKey(aCamera))
			{
				_shakeCount[aCamera]++;
			}
			else
			{
				_shakeCount.Add(aCamera, 1);
			}

			EventPreShake?.Invoke(this);

			Transform cachedTransform = aCamera.transform;
			Vector3 cameraOffset;
			Quaternion cameraRoation;

			int currentShakes = aNumShakes;
			float shakeDistance = aDistance;
			float rotationStrength = 1.0f;

			float startTime = Time.time;
			float scale = aMultByTimeScale ? Time.timeScale : 1.0f;
			Vector3 start1 = Vector2.zero;
			Quaternion startR = Quaternion.identity;
			Vector2 start2 = Vector2.zero;

			var state = new ShakeState(aShakeType, cachedTransform.position, cachedTransform.rotation);
			List<ShakeState> stateList;

			if (_states.TryGetValue(aCamera, out stateList))
			{
				stateList.Add(state);
			}
			else
			{
				stateList = new List<ShakeState>();
				stateList.Add(state);
				_states.Add(aCamera, stateList);
			}

			// Main loop
			while (currentShakes > 0)
			{
				if (Mathf.Abs(aRotationAmount.sqrMagnitude) > Vector3.kEpsilon && 
					rotationStrength <= MIN_ROTATION_VALUE)
				{
					break;
				}

				if (Mathf.Abs(aShakeAmount.sqrMagnitude) > Vector3.kEpsilon && 
					Mathf.Abs(aDistance) > Vector3.kEpsilon && 
					aDistance <= MIN_SHAKE_VALUE)
				{
					break;
				}

				float timer = (Time.time - startTime) * aSpeed;
				state._shakePosition = start1 + new Vector3(
					mod1 * Mathf.Sin(timer) * (aShakeAmount.x * shakeDistance * scale),
					mod2 * Mathf.Cos(timer) * (aShakeAmount.y * shakeDistance * scale),
					mod3 * Mathf.Sin(timer) * (aShakeAmount.z * shakeDistance * scale)
				);

				state._shakeRotation = startR * Quaternion.Euler(
					mod1 * Mathf.Cos(timer) * (aRotationAmount.x * rotationStrength * scale),
					mod2 * Mathf.Sin(timer) * (aRotationAmount.y * rotationStrength * scale),
					mod3 * Mathf.Cos(timer) * (aRotationAmount.z * rotationStrength * scale)	
				);

				cameraOffset = GetGeometricAvg(stateList);
				cameraRoation = GetRotationAvg(stateList);
				NormalizeQuaternion(ref cameraRoation);

				EventShakeOffset?.Invoke(cameraOffset, cameraRoation);

				switch (state._shakeType)
				{
					case ShakeType.CameraMatrix :
						var m = Matrix4x4.TRS(cameraOffset, cameraRoation, new Vector3(1.0f, 1.0f, -1.0f));
						aCamera.worldToCameraMatrix = m * cachedTransform.worldToLocalMatrix;
						break;

					case ShakeType.LocalPosition :
						cachedTransform.localPosition = cameraOffset;
						cachedTransform.localRotation = cameraRoation;
						break;
				}

				if (timer > Mathf.PI * 2.0f)
				{
					startTime = Time.time;
					shakeDistance *= (1.0f - Mathf.Clamp01(aDecay));
					rotationStrength *= (1.0f - Mathf.Clamp01(aDecay));
					currentShakes--;
				}
				yield return null;
			}

			// End conditions
			_shakeCount[aCamera]--;

			EventPostShake?.Invoke(this);

			// Last shake
			if (_shakeCount[aCamera] == 0)
			{
				_shaking = false;
				ResetState(aCamera);
				EventEndShaking?.Invoke(this);
			}
			else
			{
				stateList.Remove(state);
			}
		}

		private void ResetState(Camera aCamera)
		{
			aCamera.ResetWorldToCameraMatrix();
			_states[aCamera].Clear();
		}

		private static Vector3 GetGeometricAvg(IList<ShakeState> aStates)
		{
			float x = 0.0f;
			float y = 0.0f;
			float z = 0.0f;
			float l = aStates.Count;

			ShakeState state;
			for (int i = 0, n = aStates.Count; i < n; i++)
			{
				state = aStates[i];
				x -= state._shakePosition.x;
				y -= state._shakePosition.y;
				z -= state._shakePosition.z;
			}

			return new Vector3(x / l, y / l, z / l);
		}

		private static Quaternion GetRotationAvg(IList<ShakeState> aStates)
		{
			var avg = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);

			ShakeState state;
			for (int i = 0, n = aStates.Count; i < n; i++)
			{
				state = aStates[i];
				if (Quaternion.Dot(state._shakeRotation, avg) > 0.0f)
				{
					avg.x += state._shakeRotation.x;
					avg.y += state._shakeRotation.y;
					avg.z += state._shakeRotation.z;
					avg.w += state._shakeRotation.w;
				}
				else
				{
					avg.x += -state._shakeRotation.x;
					avg.y += -state._shakeRotation.y;
					avg.z += -state._shakeRotation.z;
					avg.w += -state._shakeRotation.w;
				}
			}

			float mag = Mathf.Sqrt(avg.x * avg.x + avg.y * avg.y + avg.z * avg.z + avg.w * avg.w);
			if (mag > 0.0001f)
			{
				avg.x /= mag;
				avg.y /= mag;
				avg.z /= mag;
				avg.w /= mag;
			}
			else
			{
				avg = aStates[0]._shakeRotation;
			}

			return avg;
		}

		// private static float GetPixelWidth(Transform aTransform, Camera aCamera)
		// {
		// 	Vector3 position = aTransform.position;
		// 	Vector3 screenPos = aCamera.WorldToScreenPoint(position - aTransform.forward * 0.01f);
		// 	Vector3 offset = screenPos;

		// 	offset += (screenPos.x > 0) ? -Vector3.right : Vector3.right;
		// 	offset += (screenPos.y > 0) ? -Vector3.up : Vector3.up;

		// 	offset = aCamera.ScreenToWorldPoint(offset);
		// 	return 1.0f / (aTransform.InverseTransformPoint(position) - aTransform.InverseTransformPoint(offset)).magnitude;
		// }

		private static void NormalizeQuaternion(ref Quaternion aQ)
		{
			float sum = 0.0f;
			for (int i = 0; i < 4; ++i)
			{
				sum += aQ[i] * aQ[i];
			}

			float magnitudeInverse = 1.0f / Mathf.Sqrt(sum);
			for (int i = 0; i < 4; ++i)
			{
				aQ[i] *= magnitudeInverse;
			}
		}

		private Quaternion QuaternionFromMatrix(Matrix4x4 aMatrix)
		{
			return Quaternion.LookRotation(aMatrix.GetColumn(2), aMatrix.GetColumn(1));
		}

		#endregion
	}
}