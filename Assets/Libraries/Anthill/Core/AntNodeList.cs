using System;
using System.Collections.Generic;

namespace Anthill.Core
{
	public class AntNodeList<T>
	{
	#region Public Variables

		public delegate void NodeChangeDelegate(T node);

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

		internal enum PendingChange
		{
			Add,
			Remove
		}
		
		private readonly List<T> _nodes;
		private int _count;
		private readonly List<KeyValuePair<T, PendingChange>> _pending;
		private int _lockCount;

	#endregion

	#region Getters / Setters

		/// <summary>
		/// Returns first element of the list or null if list is empty.
		/// </summary>
		public T FirstOrNull => _count > 0 ? _nodes[0] : default;

		/// <summary>
		/// Returns last element of the list or null if list is empty.
		/// </summary>
		public T LastOrNull => _count > 0 ? _nodes[_count - 1] : default;

		/// <summary>
		/// Returns element of the list by index.
		/// </summary>
		public T this[int aIndex] => _nodes[aIndex];

		/// <summary>
		/// Returns count of nodes in the list.
		/// </summary>
		public int Count => _count;

		/// <summary>
		/// Returns true is list locked.
		/// </summary>
		public bool IsLocked => _lockCount > 0;

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
		/// <param name="node">Node.</param>
		public void Add(T node)
		{
			if (IsLocked)
			{
				_pending.Add(new KeyValuePair<T, PendingChange>(node, PendingChange.Add));
				return;
			}

			EventNodeAdded?.Invoke(node);
			_nodes.Add(node);
			_count++;
		}

		/// <summary>
		/// Removes node from the list.
		/// </summary>
		/// <param name="node"></param>
		public void Remove(T node)
		{
			if (IsLocked)
			{
				_pending.Add(new KeyValuePair<T, PendingChange>(node, PendingChange.Remove));
				return;
			}
				
			EventNodeRemoved?.Invoke(node);
			_nodes.Remove(node);
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

		/// <summary>
		/// Calls specified method for all nodes.
		/// </summary>
		/// <param name="func">Method to apply each node from list.</param>
		public void ForEach(Action<T> func)
		{
			for (int i = _nodes.Count - 1; i >= 0; i--)
			{
				func(_nodes[i]);
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