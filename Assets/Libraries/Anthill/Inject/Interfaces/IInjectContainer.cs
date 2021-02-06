namespace Anthill.Inject
{
	public interface IInjectContainer
	{
		void Register<T>() where T : class;
		void Register<TInterface, TClass>(string aKey = null) where TClass : class, TInterface;
		void RegisterSingleton<T>() where T : class;
		void RegisterSingleton<T>(T aInstance, string aKey = null) where T : class;
		void RegisterSingleton<TInterface, TClass>(string aKey = null) where TClass : class, TInterface;
	}
}