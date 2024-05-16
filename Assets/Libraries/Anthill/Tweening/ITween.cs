namespace Anthill.Tweening
{
	public interface ITween
	{
		void Execute(float aDeltaTime);
		void Release();
		void Kill(bool aCallEvents = false);
		bool IsUnscaledTime { get; }
		bool IsKillOnDeinitialize { get; }
		// bool IsAvailable { get; set; }
	}
}