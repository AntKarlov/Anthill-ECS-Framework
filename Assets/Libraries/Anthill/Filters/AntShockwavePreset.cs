namespace Anthill.Filters
{
	using UnityEngine;

	[CreateAssetMenuAttribute(fileName="NewShockwavePreset", menuName="Anthill/Shockwave Preset")]
	public class AntShockwavePreset : ScriptableObject
	{
		[Range(-0.1f, 2.0f)]
		public float startRadius; // Value

		[Range(-0.1f, 2.0f)]
		public float endRadius = 1.0f; // toValue

		[Range(0.0f, 10.0f)]
		public float startDepth = 4.0f; // Size

		[Range(0.0f, 10.0f)]
		public float endDepth; // toSize

		public float duration = 1.0f; // time;

		public AnimationCurve animation = new AnimationCurve();
	}
}