namespace Anthill.Core
{
	using System.Collections.Generic;
	
	public class AntNodeList<T>
	{
		internal enum PendingChange
		{
			Add,
			Remove
		}

	#region Public Variables

		public delegate void NodeChangeDelegate(T aNode);

		/// <summary>
		/// Called when new node added to the list.
		/// </summary>
		public event NodeChangeDelegate EventNodeAdded;

		/// <summary>
		/// Called when node removed from the list.
		/// </summary>
		public event NodeChangeDelegate EventNodeRemoved;

	#endregion

	#region Private Variables

		private List<T> _nodes;
		private int _count;
		private List<KeyValuePair<T, PendingChange>> _pending;
		private int _lockCount;

	#endregion

	#region Getters / Setters

		public T this[int aIndex] { get => _nodes[aIndex]; }

		/// <summary>
		/// Returns count of nodes in the list.
		/// </summary>
		public int Count { get => _count; }

		/// <summary>
		/// Returns true is list locked.
		/// </summary>
		public bool IsLocked { get => (_lockCount > 0); }

	#endregion

	#region Public Methods

		public AntNodeList()
		{
			_nodes = new List<T>();
			_pending = new List<KeyValuePair<T, PendingChange>>();
		}

		/// <summary>
		/// Adds new node to the list.
		/// </summary>
		/// <param name="aNode">Node.</param>
		public void Add(T aNode)
		{
			if (IsLocked)
			{
				_pending.Add(new KeyValuePair<T, PendingChange>(aNode, PendingChange.Add));
				return;
			}

			EventNodeAdded?.Invoke(aNode);
			_nodes.Add(aNode);
			_count++;
		}

		/// <summary>
		/// Removes node from the list.
		/// </summary>
		/// <param name="aNode"></param>
		public void Remove(T aNode)
		{
			if (IsLocked)
			{
				_pending.Add(new KeyValuePair<T, PendingChange>(aNode, PendingChange.Remove));
				return;
			}
				
			EventNodeRemoved?.Invoke(aNode);
			_nodes.Remove(aNode);
			_count--;
		}

		/// <summary>
		/// Locks list for the changes.
		/// </summary>
		public void Lock()
		{
			_lockCount++;
		}

		/// <summary>
		/// Unlocks list for the changes.
		/// </summary>
		public void Unlock()
		{
			_lockCount--;
			if (_lockCount <= 0)
			{
				ApplyPending();
				_lockCount = 0;
			}
		}

	#endregion

	#region Private Methods

		private void ApplyPending()
		{
			KeyValuePair<T, PendingChange> pair;
			for (int i = 0, n = _pending.Count; i < n; i++)
			{
				pair = _pending[i];
				if (pair.Value == PendingChange.Add)
				{
					Add(pair.Key);
				}
				else
				{
					Remove(pair.Key);
				}
			}
			_pending.Clear();
		}

	#endregion
	}
}