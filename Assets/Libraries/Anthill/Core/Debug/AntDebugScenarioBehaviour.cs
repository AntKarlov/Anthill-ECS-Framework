namespace Anthill.Core
{
	using UnityEngine;
	
	public class AntDebugScenarioBehaviour : MonoBehaviour
	{
		public AntDebugScenario Scenario { get; private set; }

		public void Init(AntDebugScenario aScenario)
		{
			Scenario = aScenario;
		}
	}
}