namespace Anthill.Inject
{
	using UnityEngine;
	
	public abstract class AntAbstractBootstrapper : MonoBehaviour
	{
		private void Awake()
		{
			var container = new AntInjectContainer();
			//container.SetSingletonGameObject(gameObject.name);
			AntInject.SetInjector(container);
			Configure(container);
		}

		public abstract void Configure(IInjectContainer aContainer);
	}
}