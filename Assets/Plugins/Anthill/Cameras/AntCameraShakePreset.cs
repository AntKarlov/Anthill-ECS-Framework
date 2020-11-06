namespace Anthill.Cameras
{
	using UnityEngine;
	
	[CreateAssetMenuAttribute(fileName="NewShakePreset", menuName="Anthill/Shake Preset")]
	public class AntCameraShakePreset : ScriptableObject
	{
		public AntCameraShake.ShakeType shakeType = AntCameraShake.ShakeType.CameraMatrix;
		public int numShakes = 2;
		public Vector3 shakeAmount = Vector3.one;
		public Vector3 rotationAmount = Vector3.one;
		public float distance = 0.1f;
		public float speed = 50.0f;
		public float decay = 0.2f;
		public bool ignoreTimeScale;
		public bool rumble;
		public int motorId;
		public float motorForce;
		public float motorDuration;
	}
}