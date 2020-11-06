namespace Anthill.Utils
{
	public class AntFpsCounter
	{
		private float _deltaTime;
		private float _msec;
		private float _fps;

		public void Update(float aDeltaTime)
		{
			_deltaTime += (aDeltaTime - _deltaTime) * 0.1f;
			_msec = _deltaTime * 1000.0f;
			_fps = 1.0f / _deltaTime;
		}
		
		public float Fps { get => _fps; }
		public float Ms { get => _msec; }
		public string FpsStr { get => _fps.ToString("0.0"); }
		public string MsStr { get => _msec.ToString("0.0"); }
	}
}