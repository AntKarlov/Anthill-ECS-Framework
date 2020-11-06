namespace Anthill.Inject
{
	using System;
	
	public interface IServiceLocator
	{
		T Resolve<T>(string aKey = null) where T : class;
		object Resolve(Type aType, string aKey = null);
	}
}