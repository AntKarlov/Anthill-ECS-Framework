namespace Anthill.Tasks
{
	public interface ITask
	{
		bool IgnoreCycle { get; set; }
		bool Execute();
		void Release();
	}
}