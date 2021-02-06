namespace Anthill.Inject
{
	using UnityEngine;

	/// <summary>
	/// Injection helper.
	/// </summary>
	public static class AntInject
	{
		private static IInjector _injector;
		
		public static IInjectContainer Container { get => (IInjectContainer) _injector; }
		public static IInjector Injector { get => _injector; }

		public static void SetInjector(IInjector aInjector)
		{
			_injector = aInjector;
		}

		/// <summary>
		/// Injection for any class.
		/// </summary>
		/// <param name="aObject">Class for injection.</param>
		/// <typeparam name="T">Type of the class.</typeparam>
		public static void Inject<T>(T aObject)
		{
			_injector.Inject<T>(aObject);
		}

		/// <summary>
		/// Injection for the MonoBehaviour.
		/// </summary>
		/// <param name="aMonoBehaviour">MonoBehaviour for injection.</param>
		/// <typeparam name="T">Type of the MonoBehaviour.</typeparam>
		public static void InjectMono<T>(this T aMonoBehaviour) where T : MonoBehaviour
		{
			_injector.Inject<T>(aMonoBehaviour);
		}
	}
}