namespace Anthill.Core
{
#if UNITY_EDITOR && !FINAL_BUILD
	public class AntScenario : AntDebugScenario
	{
#else
	public class AntScenario : AntBaseScenario
	{
#endif
		public AntScenario(string name = "Systems") : base(name)
		{
			//..
		}
	}
}