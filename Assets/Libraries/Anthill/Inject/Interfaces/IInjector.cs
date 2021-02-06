namespace Anthill.Inject
{
	using System;
	
	public interface IInjector
	{
		object Inject(Type aType, object aObject);
		T Inject<T>(object aObject);
	}	
}