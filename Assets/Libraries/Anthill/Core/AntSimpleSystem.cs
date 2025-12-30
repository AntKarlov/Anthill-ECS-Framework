namespace Anthill.Core
{
	public class AntSimpleSystem : ISystem, IExecuteSystem
	{
		// -----------------------------------------------------
		// ISystem Implementation
		// -----------------------------------------------------

		public virtual void AddedToEngine()
		{
			// ..
		}

		public virtual void RemovedFromEngine()
		{
			// ..
		}

		// -----------------------------------------------------
		// IExecuteSystem Implementation
		// -----------------------------------------------------

		public virtual void Execute()
		{
			// ..
		}
	}

	public class AntSimpleSystem<T1> : AntSimpleSystem
	{
		private AntNodeList<AntNode<T1>> _nodes;

		public override void AddedToEngine()
		{
			_nodes = AntEngine.GetNodes<AntNode<T1>>();
		}

		public override void RemovedFromEngine()
		{
			AntEngine.ReleaseNodes<AntNode<T1>>();
		}

		public override void Execute()
		{
			for (int i = _nodes.Count - 1; i >= 0; i--)
			{
				UpdateNode(_nodes[i].Component1);
			}
		}

		protected virtual void UpdateNode(T1 comp1)
		{
			// ..
		}
	}

	public class AntSimpleSystem<T1, T2> : AntSimpleSystem
	{
		private AntNodeList<AntNode<T1, T2>> _nodes;

		public override void AddedToEngine()
		{
			_nodes = AntEngine.GetNodes<AntNode<T1, T2>>();
		}

		public override void RemovedFromEngine()
		{
			AntEngine.ReleaseNodes<AntNode<T1, T2>>();
		}

		public override void Execute()
		{
			AntNode<T1, T2> node;
			for (int i = _nodes.Count - 1; i >= 0; i--)
			{
				node = _nodes[i];
				UpdateNode(node.Component1, node.Component2);
			}
		}

		protected virtual void UpdateNode(T1 comp1, T2 comp2)
		{
			// ..
		}
	}

	public class AntSimpleSystem<T1, T2, T3> : AntSimpleSystem
	{
		private AntNodeList<AntNode<T1, T2, T3>> _nodes;

		public override void AddedToEngine()
		{
			_nodes = AntEngine.GetNodes<AntNode<T1, T2, T3>>();
		}

		public override void RemovedFromEngine()
		{
			AntEngine.ReleaseNodes<AntNode<T1, T2, T3>>();
		}

		public override void Execute()
		{
			AntNode<T1, T2, T3> node;
			for (int i = _nodes.Count - 1; i >= 0; i--)
			{
				node = _nodes[i];
				UpdateNode(node.Component1, node.Component2, node.Component3);
			}
		}

		protected virtual void UpdateNode(T1 comp1, T2 comp2, T3 comp3)
		{
			// ..
		}
	}

	public class AntSimpleSystem<T1, T2, T3, T4> : AntSimpleSystem
	{
		private AntNodeList<AntNode<T1, T2, T3, T4>> _nodes;

		public override void AddedToEngine()
		{
			_nodes = AntEngine.GetNodes<AntNode<T1, T2, T3, T4>>();
		}

		public override void RemovedFromEngine()
		{
			AntEngine.ReleaseNodes<AntNode<T1, T2, T3, T4>>();
		}

		public override void Execute()
		{
			AntNode<T1, T2, T3, T4> node;
			for (int i = _nodes.Count - 1; i >= 0; i--)
			{
				node = _nodes[i];
				UpdateNode(node.Component1, node.Component2, node.Component3, node.Component4);
			}
		}

		protected virtual void UpdateNode(T1 comp1, T2 comp2, T3 comp3, T4 comp4)
		{
			// ..
		}
	}

	public class AntSimpleSystem<T1, T2, T3, T4, T5> : AntSimpleSystem
	{
		private AntNodeList<AntNode<T1, T2, T3, T4, T5>> _nodes;

		public override void AddedToEngine()
		{
			_nodes = AntEngine.GetNodes<AntNode<T1, T2, T3, T4, T5>>();
		}

		public override void RemovedFromEngine()
		{
			AntEngine.ReleaseNodes<AntNode<T1, T2, T3, T4, T5>>();
		}

		public override void Execute()
		{
			AntNode<T1, T2, T3, T4, T5> node;
			for (int i = _nodes.Count - 1; i >= 0; i--)
			{
				node = _nodes[i];
				UpdateNode(node.Component1, node.Component2, node.Component3, node.Component4, node.Component5);
			}
		}

		protected virtual void UpdateNode(T1 comp1, T2 comp2, T3 comp3, T4 comp4, T5 comp5)
		{
			// ..
		}
	}

	public class AntSimpleSystem<T1, T2, T3, T4, T5, T6> : AntSimpleSystem
	{
		private AntNodeList<AntNode<T1, T2, T3, T4, T5, T6>> _nodes;

		public override void AddedToEngine()
		{
			_nodes = AntEngine.GetNodes<AntNode<T1, T2, T3, T4, T5, T6>>();
		}

		public override void RemovedFromEngine()
		{
			AntEngine.ReleaseNodes<AntNode<T1, T2, T3, T4, T5, T6>>();
		}

		public override void Execute()
		{
			AntNode<T1, T2, T3, T4, T5, T6> node;
			for (int i = _nodes.Count - 1; i >= 0; i--)
			{
				node = _nodes[i];
				UpdateNode(node.Component1, node.Component2, node.Component3, node.Component4, node.Component5, node.Component6);
			}
		}

		protected virtual void UpdateNode(T1 comp1, T2 comp2, T3 comp3, T4 comp4, T5 comp5, T6 comp6)
		{
			// ..
		}
	}
}