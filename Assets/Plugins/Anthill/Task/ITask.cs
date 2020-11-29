namespace Anthill.Task
{
	public interface ITask
	{
		bool IgnoreCycle { get; set; }
		bool Execute();
		void Release();
	}
}