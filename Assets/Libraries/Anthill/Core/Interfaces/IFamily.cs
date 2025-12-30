using System;

namespace Anthill.Core
{
	public interface IFamily
	{
		void ComponentAdded(IEntity entity, Type componentType);
		void ComponentRemoved(IEntity entity, Type componentType);
		void EntityAdded(IEntity entity);
		void EntityRemoved(IEntity entity);
	}

	public interface IFamily<T> : IFamily
	{
		AntNodeList<T> Nodes { get; }
	}
}