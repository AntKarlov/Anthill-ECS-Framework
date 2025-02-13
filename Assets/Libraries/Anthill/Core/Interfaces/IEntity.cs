using System;

namespace Anthill.Core
{
	public interface IEntity
	{
		event ComponentDelegate EventComponentAdded;
		event ComponentDelegate EventComponentRemoved;

		event EntityDelegate EventAddedToEngine;
		event EntityDelegate EventRemovedFromEngine;

		bool AllowToAddFromHierachy { get; }
		bool IsAddedToEngine { get; }

		bool Has(Type componentType);
		bool Has<T>() where T : class;

		object Get(Type componentType);
		bool TryGet(Type componentType, out object component);

		T Get<T>() where T : class;
		bool TryGet<T>(out T component) where T : class;

		object Add(object component);
		T Add<T>() where T : class;

		object Remove(object component);
		T Remove<T>() where T : class;
		bool TryRemove<T>(out object component) where T : class;

		void OnAddedToEngine();
		void OnRemovedFromEngine();
	}
}