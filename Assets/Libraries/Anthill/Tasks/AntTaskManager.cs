namespace Anthill.Tasks
{
	using UnityEngine;
	using System.Collections.Generic;
	using Anthill.Core;
	using Anthill.Extensions;

	public enum UpdateMode
	{
		Auto,
		Manual
	}

	public class AntTaskManager
	{
	#region Public Variables

		public delegate void TaskManagerDelegate(AntTaskManager aTaskManager);
		public event TaskManagerDelegate EventFinished;

		public delegate void TaskManagerCompleteDelegate();

	#endregion

	#region Private Variables

		private static AntTaskManagerScenario _scenario;
		private static bool _isInitialized;

		protected TaskManagerCompleteDelegate _completeDelegate;
		protected List<ITask> _queue;
		protected ITask _currentTask;
		protected bool _isHasTask;
		protected float _delay;
		protected UpdateMode _updateMode = UpdateMode.Auto;

	#endregion

	#region Getters / Setters

		private static AntTaskManagerScenario Scenario
		{
			get
			{
				if (!_isInitialized)
				{
					_scenario = AntEngine.Add<AntTaskManagerScenario>(-1);
					_isInitialized = true;
				}

				return _scenario;
			}
		}
		
		public bool IsCycled { get; set; }
		public bool IsPaused { get; set; }
		public bool IsStarted { get; protected set; }
		public int TaskCount { get => _queue.Count; }
		
	#endregion

	#region Public Methods

		public static AntTaskManager Do(bool aIsCycled = false)
		{
			return new AntTaskManager(aIsCycled);
		}

		/// <summary>
		/// Creates new instance of the TaskManager.
		/// </summary>
		/// <param name="aIsCycled">If true, then queue of the tasks will be cycled.</param>
		public AntTaskManager(bool aIsCycled = false)
		{
			_queue = new List<ITask>();
			IsCycled = aIsCycled;
			IsStarted = false;
			IsPaused = false;
		}

		/// <summary>
		/// Sets update mode, automatic or manual.
		/// - AUTO mode means that TaskManager will be updating via AntEngine system.
		/// - MANUAL mode means tha TaskManager will be updating by calling Execute() method.
		/// </summary>
		/// <param name="aUpdateMode">Update Mode</param>
		/// <returns>Reference to the task manager.</returns>
		public AntTaskManager SetUpdateMode(UpdateMode aUpdateMode)
		{
			if (_updateMode != aUpdateMode)
			{
				_updateMode = aUpdateMode;
				if (_updateMode == UpdateMode.Auto && IsStarted)
				{
					Scenario.Add(this);
				}
				else if (_updateMode == UpdateMode.Manual && IsStarted)
				{
					Scenario.Remove(this);
				}
			}
			return this;
		}

		/// <summary>
		/// Adds delay to the end of the queue.
		/// </summary>
		/// <param name="aTime">Delay in seconds.</param>
		/// <param name="aIgnoreCycle">If true, then delay will be called once per queue cycle.</param>
		/// <returns>Reference to the task manager.</returns>
		public AntTaskManager Delay(float aTime, bool aIgnoreCycle = false)
		{
			var task = new AntTask<float>(aIgnoreCycle);
			_queue.Add(task.SetProcess(DelayProcess).SetArguments(aTime));

			Start();
			return this;
		}

		/// <summary>
		/// Adds delay to the begining of the queue.
		/// </summary>
		/// <param name="aTime">Delay in seconds.</param>
		/// <param name="aIgnoreCycle">If true, then delay will be called once per queue cycle.</param>
		/// <returns>Reference to the task manager.</returns>
		public AntTaskManager UrgentDelay(float aTime, bool aIgnoreCycle = false)
		{
			var task = new AntTask<float>(aIgnoreCycle);
			_queue.Insert(0, task.SetProcess(DelayProcess).SetArguments(aTime));

			Start();
			return this;
		}

		/// <summary>
		/// Adds new task to the end of the queue. Task will be called until returns true.
		/// </summary>
		/// <param name="aProcess">Reference to the task method.</param>
		/// <param name="aIgnoreCycle">If true, then task will be called once per queue cycle.</param>
		/// <returns>Reference to the task manager.</returns>
		public AntTaskManager Task(AntTask.TaskProcess0 aProcess, bool aIgnoreCycle = false)
		{
			var task = new AntTask(aIgnoreCycle);
			_queue.Add(task.SetProcess(aProcess));

			Start();
			return this;
		}

		/// <summary>
		/// Adds new instant task to the end of the queue. Task will be called once.
		/// </summary>
		/// <param name="aProcess">Reference to the task method.</param>
		/// <param name="aIgnoreCycle">If true, then task will be called once per queue cycle.</param>
		/// <returns>Reference to the task manager.</returns>
		public AntTaskManager InstantTask(AntInstantTask.TaskProcess0 aProcess, bool aIgnoreCycle = false)
		{
			var task = new AntInstantTask(aIgnoreCycle);
			_queue.Add(task.SetProcess(aProcess));

			Start();
			return this;
		}

		/// <summary>
		/// Adds new urgent task to the begining of the queue. Task will be called until returns true.
		/// </summary>
		/// <param name="aProcess">Reference to the task method.</param>
		/// <param name="aIgnoreCycle">If true, then task will be called once per queue cycle.</param>
		/// <returns>Reference to the task manager.</returns>
		public AntTaskManager UrgentTask(AntTask.TaskProcess0 aProcess, bool aIgnoreCycle = false)
		{
			var task = new AntTask(aIgnoreCycle);
			_queue.Insert(0, task.SetProcess(aProcess));

			Start();
			return this;
		}

		/// <summary>
		/// Adds new urgent task to the begining of the queue. Task will be called once.
		/// </summary>
		/// <param name="aProcess">Reference to the task method.</param>
		/// <param name="aIgnoreCycle">f true, then task will be called once per queue cycle.</param>
		/// <returns>Reference to the task manager.</returns>
		public AntTaskManager UrgentInstantTask(AntInstantTask.TaskProcess0 aProcess, bool aIgnoreCycle = false)
		{
			var task = new AntInstantTask(aIgnoreCycle);
			_queue.Insert(0, task.SetProcess(aProcess));

			Start();
			return this;
		}

		/// <summary>
		/// Adds new task to the end of the queue. Task will be called until returns true.
		/// </summary>
		/// <param name="aProcess">Reference to the task method.</param>
		/// <param name="aArg1">First argument.</param>
		/// <param name="aIgnoreCycle">If true, then task will be called once per queue cycle.</param>
		/// <typeparam name="T1">Type of the first argument.</typeparam>
		/// <returns></returns>
		public AntTaskManager Task<T1>(
			AntTask<T1>.TaskProcess1 aProcess, 
			T1 aArg1, 
			bool aIgnoreCycle = false)
		{
			var task = new AntTask<T1>(aIgnoreCycle);
			_queue.Add(task.SetProcess(aProcess).SetArguments(aArg1));

			Start();
			return this;
		}

		/// <summary>
		/// Adds new instant task to the end of the queue. Task will be called once.
		/// </summary>
		/// <param name="aProcess">Reference to the task method.</param>
		/// <param name="aArg1">First argument.</param>
		/// <param name="aIgnoreCycle">If true, then task will be called once per queue cycle.</param>
		/// <typeparam name="T1">Type of the first argument.</typeparam>
		/// <returns></returns>
		public AntTaskManager InstantTask<T1>(
			AntInstantTask<T1>.TaskProcess1 aProcess, 
			T1 aArg1, 
			bool aIgnoreCycle = false
		)
		{
			var task = new AntInstantTask<T1>(aIgnoreCycle);
			_queue.Add(task.SetProcess(aProcess).SetArguments(aArg1));

			Start();
			return this;
		}

		/// <summary>
		/// Adds new urgent task to the begining of the queue. Task will be called until returns true.
		/// </summary>
		/// <param name="aProcess">Reference to the task method.</param>
		/// <param name="aArg1">First argument.</param>
		/// <param name="aIgnoreCycle">If true, then task will be called once per queue cycle.</param>
		/// <typeparam name="T1">Type of the first argument.</typeparam>
		/// <returns>Reference to the task manager.</returns>
		public AntTaskManager UrgentTask<T1>(
			AntTask<T1>.TaskProcess1 aProcess, 
			T1 aArg1, 
			bool aIgnoreCycle = false)
		{
			var task = new AntTask<T1>(aIgnoreCycle);
			_queue.Insert(0, task.SetProcess(aProcess).SetArguments(aArg1));

			Start();
			return this;
		}

		/// <summary>
		/// Adds new urgent task to the begining of the queue. Task will be called once.
		/// </summary>
		/// <param name="aProcess">Reference to the task method.</param>
		/// <param name="aArg1">First argument.</param>
		/// <param name="aIgnoreCycle">If true, then task will be called once per queue cycle.</param>
		/// <typeparam name="T1">Type of the first argument.</typeparam>
		/// <returns>Reference to the task manager.</returns>
		public AntTaskManager UrgentInstantTask<T1>(
			AntInstantTask<T1>.TaskProcess1 aProcess, 
			T1 aArg1, 
			bool aIgnoreCycle = false)
		{
			var task = new AntInstantTask<T1>(aIgnoreCycle);
			_queue.Insert(0, task.SetProcess(aProcess).SetArguments(aArg1));

			Start();
			return this;
		}

		/// <summary>
		/// Adds new task to the end of the queue. Task will be called until returns true.
		/// </summary>
		/// <param name="aProcess">Reference to the task method.</param>
		/// <param name="aArg1">First argument.</param>
		/// <param name="aArg2">Second argument.</param>
		/// <param name="aIgnoreCycle">If true, then task will be called once per queue cycle.</param>
		/// <typeparam name="T1">Type of the first argument.</typeparam>
		/// <typeparam name="T2">Type of the second argument.</typeparam>
		/// <returns>Reference to the task manager.</returns>
		public AntTaskManager Task<T1, T2>(
			AntTask<T1, T2>.TaskProcess2 aProcess, 
			T1 aArg1, 
			T2 aArg2, 
			bool aIgnoreCycle = false)
		{
			var task = new AntTask<T1, T2>(aIgnoreCycle);
			_queue.Add(task.SetProcess(aProcess).SetArguments(aArg1, aArg2));

			Start();
			return this;
		}

		/// <summary>
		/// Adds new instant task to the end of the queue. Task will be called once.
		/// </summary>
		/// <param name="aProcess">Reference to the task method.</param>
		/// <param name="aArg1">First argument.</param>
		/// <param name="aArg2">Second argument.</param>
		/// <param name="aIgnoreCycle">If true, then task will be called once per queue cycle.</param>
		/// <typeparam name="T1">Type of the first argument.</typeparam>
		/// <typeparam name="T2">Type of the second argument.</typeparam>
		/// <returns>Reference to the task manager.</returns>
		public AntTaskManager InstantTask<T1, T2>(
			AntInstantTask<T1, T2>.TaskProcess2 aProcess, 
			T1 aArg1, 
			T2 aArg2, 
			bool aIgnoreCycle = false)
		{
			var task = new AntInstantTask<T1, T2>(aIgnoreCycle);
			_queue.Add(task.SetProcess(aProcess).SetArguments(aArg1, aArg2));

			Start();
			return this;
		}

		/// <summary>
		/// Adds new urgent task to the begining of the queue. Task will be called until returns true.
		/// </summary>
		/// <param name="aProcess">Reference to the task method.</param>
		/// <param name="aArg1">First argument.</param>
		/// <param name="aArg2">Second argument.</param>
		/// <param name="aIgnoreCycle">If true, then task will be called once per queue cycle.</param>
		/// <typeparam name="T1">Type of the first argument.</typeparam>
		/// <typeparam name="T2">Type of the second argument.</typeparam>
		/// <returns>Reference to the task manager.</returns>
		public AntTaskManager UrgentTask<T1, T2>(
			AntTask<T1, T2>.TaskProcess2 aProcess, 
			T1 aArg1, 
			T2 aArg2, 
			bool aIgnoreCycle = false)
		{
			var task = new AntTask<T1, T2>(aIgnoreCycle);
			_queue.Insert(0, task.SetProcess(aProcess).SetArguments(aArg1, aArg2));

			Start();
			return this;
		}

		/// <summary>
		/// Adds new urgent task to the begining of the queue. Task will be called once.
		/// </summary>
		/// <param name="aProcess">Reference to the task method.</param>
		/// <param name="aArg1">First argument.</param>
		/// <param name="aArg2">Second argument.</param>
		/// <param name="aIgnoreCycle">If true, then task will be called once per queue cycle.</param>
		/// <typeparam name="T1">Type of the first argument.</typeparam>
		/// <typeparam name="T2">Type of the second argument.</typeparam>
		/// <returns>Reference to the task manager.</returns>
		public AntTaskManager UrgentInstantTask<T1, T2>(
			AntInstantTask<T1, T2>.TaskProcess2 aProcess, 
			T1 aArg1, 
			T2 aArg2, 
			bool aIgnoreCycle = false)
		{
			var task = new AntInstantTask<T1, T2>(aIgnoreCycle);
			_queue.Insert(0, task.SetProcess(aProcess).SetArguments(aArg1, aArg2));

			Start();
			return this;
		}

		/// <summary>
		/// Adds new task to the end of the queue. Task will be called until returns true.
		/// </summary>
		/// <param name="aProcess">Reference to the task method.</param>
		/// <param name="aArg1">First argument.</param>
		/// <param name="aArg2">Second argument.</param>
		/// <param name="aArg3">Third argument.</param>
		/// <param name="aIgnoreCycle">If true, then task will be called once per queue cycle.</param>
		/// <typeparam name="T1">Type of the first argument.</typeparam>
		/// <typeparam name="T2">Type of the second argument.</typeparam>
		/// <typeparam name="T3">Type of the third argument.</typeparam>
		/// <returns>Reference to the task manager.</returns>
		public AntTaskManager Task<T1, T2, T3>(
			AntTask<T1, T2, T3>.TaskProcess3 aProcess, 
			T1 aArg1, 
			T2 aArg2, 
			T3 aArg3, 
			bool aIgnoreCycle = false)
		{
			var task = new AntTask<T1, T2, T3>(aIgnoreCycle);
			_queue.Add(task.SetProcess(aProcess).SetArguments(aArg1, aArg2, aArg3));

			Start();
			return this;
		}

		/// <summary>
		/// Adds new instant task to the end of the queue. Task will be called once.
		/// </summary>
		/// <param name="aProcess">Reference to the task method.</param>
		/// <param name="aArg1">First argument.</param>
		/// <param name="aArg2">Second argument.</param>
		/// <param name="aArg3">Third argument.</param>
		/// <param name="aIgnoreCycle">If true, then task will be called once per queue cycle.</param>
		/// <typeparam name="T1">Type of the first argument.</typeparam>
		/// <typeparam name="T2">Type of the second argument.</typeparam>
		/// <typeparam name="T3">Type of the third argument.</typeparam>
		/// <returns>Reference to the task manager.</returns>
		public AntTaskManager InstantTask<T1, T2, T3>(
			AntInstantTask<T1, T2, T3>.TaskProcess3 aProcess, 
			T1 aArg1, 
			T2 aArg2, 
			T3 aArg3, 
			bool aIgnoreCycle = false)
		{
			var task = new AntInstantTask<T1, T2, T3>(aIgnoreCycle);
			_queue.Add(task.SetProcess(aProcess).SetArguments(aArg1, aArg2, aArg3));

			Start();
			return this;
		}

		/// <summary>
		/// Adds new urgent task to the begining of the queue. Task will be called until returns true.
		/// </summary>
		/// <param name="aProcess">Reference to the task method.</param>
		/// <param name="aArg1">First argument.</param>
		/// <param name="aArg2">Second argument.</param>
		/// <param name="aArg3">Third argument.</param>
		/// <param name="aIgnoreCycle">If true, then task will be called once per queue cycle.</param>
		/// <typeparam name="T1">Type of the first argument.</typeparam>
		/// <typeparam name="T2">Type of the second argument.</typeparam>
		/// <typeparam name="T3">Type of the third argument.</typeparam>
		/// <returns>Reference to the task manager.</returns>
		public AntTaskManager UrgentTask<T1, T2, T3>(
			AntTask<T1, T2, T3>.TaskProcess3 aProcess, 
			T1 aArg1, 
			T2 aArg2, 
			T3 aArg3, 
			bool aIgnoreCycle = false)
		{
			var task = new AntTask<T1, T2, T3>(aIgnoreCycle);
			_queue.Insert(0, task.SetProcess(aProcess).SetArguments(aArg1, aArg2, aArg3));

			Start();
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="aProcess">Reference to the task method.</param>
		/// <param name="aArg1">First argument.</param>
		/// <param name="aArg2">Second argument.</param>
		/// <param name="aArg3">Third argument.</param>
		/// <param name="aIgnoreCycle">If true, then task will be called once per queue cycle.</param>
		/// <typeparam name="T1">Type of the first argument.</typeparam>
		/// <typeparam name="T2">Type of the second argument.</typeparam>
		/// <typeparam name="T3">Type of the third argument.</typeparam>
		/// <returns>Reference to the task manager.</returns>
		public AntTaskManager UrgentInstantTask<T1, T2, T3>(
			AntInstantTask<T1, T2, T3>.TaskProcess3 aProcess, 
			T1 aArg1, 
			T2 aArg2, 
			T3 aArg3, 
			bool aIgnoreCycle = false)
		{
			var task = new AntInstantTask<T1, T2, T3>(aIgnoreCycle);
			_queue.Insert(0, task.SetProcess(aProcess).SetArguments(aArg1, aArg2, aArg3));

			Start();
			return this;
		}

		/// <summary>
		/// Adds new task to the end of the queue. Task will be called until returns true.
		/// </summary>
		/// <param name="aProcess">Reference to the task method.</param>
		/// <param name="aArg1">First argument.</param>
		/// <param name="aArg2">Second argument.</param>
		/// <param name="aArg3">Third argument.</param>
		/// <param name="aArg4">Four argument.</param>
		/// <param name="aIgnoreCycle">If true, then task will be called once per queue cycle.</param>
		/// <typeparam name="T1">Type of the first argument.</typeparam>
		/// <typeparam name="T2">Type of the second argument.</typeparam>
		/// <typeparam name="T3">Type of the third argument.</typeparam>
		/// <typeparam name="T4">Type of the four argument.</typeparam>
		/// <returns>Reference to the task manager.</returns>
		public AntTaskManager Task<T1, T2, T3, T4>(
			AntTask<T1, T2, T3, T4>.TaskProcess4 aProcess, 
			T1 aArg1, 
			T2 aArg2, 
			T3 aArg3, 
			T4 aArg4, 
			bool aIgnoreCycle = false)
		{
			var task = new AntTask<T1, T2, T3, T4>(aIgnoreCycle);
			_queue.Add(task.SetProcess(aProcess).SetArguments(aArg1, aArg2, aArg3, aArg4));

			Start();
			return this;
		}

		/// <summary>
		/// Adds new instant task to the end of the queue. Task will be called once.
		/// </summary>
		/// <param name="aProcess">Reference to the task method.</param>
		/// <param name="aArg1">First argument.</param>
		/// <param name="aArg2">Second argument.</param>
		/// <param name="aArg3">Third argument.</param>
		/// <param name="aArg4">Four argument.</param>
		/// <param name="aIgnoreCycle">If true, then task will be called once per queue cycle.</param>
		/// <typeparam name="T1">Type of the first argument.</typeparam>
		/// <typeparam name="T2">Type of the second argument.</typeparam>
		/// <typeparam name="T3">Type of the third argument.</typeparam>
		/// <typeparam name="T4">Type of the four argument.</typeparam>
		/// <returns>Reference to the task manager.</returns>
		public AntTaskManager InstantTask<T1, T2, T3, T4>(
			AntInstantTask<T1, T2, T3, T4>.TaskProcess4 aProcess, 
			T1 aArg1, 
			T2 aArg2, 
			T3 aArg3, 
			T4 aArg4, 
			bool aIgnoreCycle = false)
		{
			var task = new AntInstantTask<T1, T2, T3, T4>(aIgnoreCycle);
			_queue.Add(task.SetProcess(aProcess).SetArguments(aArg1, aArg2, aArg3, aArg4));

			Start();
			return this;
		}

		/// <summary>
		/// Adds new urgent task to the begining of the queue. Task will be called until returns true.
		/// </summary>
		/// <param name="aProcess">Reference to the task method.</param>
		/// <param name="aArg1">First argument.</param>
		/// <param name="aArg2">Second argument.</param>
		/// <param name="aArg3">Third argument.</param>
		/// <param name="aArg4">Four argument.</param>
		/// <param name="aIgnoreCycle">If true, then task will be called once per queue cycle.</param>
		/// <typeparam name="T1">Type of the first argument.</typeparam>
		/// <typeparam name="T2">Type of the second argument.</typeparam>
		/// <typeparam name="T3">Type of the third argument.</typeparam>
		/// <typeparam name="T4">Type of the four argument.</typeparam>
		/// <returns>Reference to the task manager.</returns>
		public AntTaskManager UrgentTask<T1, T2, T3, T4>(
			AntTask<T1, T2, T3, T4>.TaskProcess4 aProcess, 
			T1 aArg1, 
			T2 aArg2, 
			T3 aArg3, 
			T4 aArg4, 
			bool aIgnoreCycle = false)
		{
			var task = new AntTask<T1, T2, T3, T4>(aIgnoreCycle);
			_queue.Insert(0, task.SetProcess(aProcess).SetArguments(aArg1, aArg2, aArg3, aArg4));

			Start();
			return this;
		}

		/// <summary>
		/// Adds new urgent task to the begining of the queue. Task will be called once.
		/// </summary>
		/// <param name="aProcess">Reference to the task method.</param>
		/// <param name="aArg1">First argument.</param>
		/// <param name="aArg2">Second argument.</param>
		/// <param name="aArg3">Third argument.</param>
		/// <param name="aArg4">Four argument.</param>
		/// <param name="aIgnoreCycle">If true, then task will be called once per queue cycle.</param>
		/// <typeparam name="T1">Type of the first argument.</typeparam>
		/// <typeparam name="T2">Type of the second argument.</typeparam>
		/// <typeparam name="T3">Type of the third argument.</typeparam>
		/// <typeparam name="T4">Type of the four argument.</typeparam>
		/// <returns>Reference to the task manager.</returns>
		public AntTaskManager UrgentInstantTask<T1, T2, T3, T4>(
			AntInstantTask<T1, T2, T3, T4>.TaskProcess4 aProcess, 
			T1 aArg1, 
			T2 aArg2, 
			T3 aArg3, 
			T4 aArg4, 
			bool aIgnoreCycle = false)
		{
			var task = new AntInstantTask<T1, T2, T3, T4>(aIgnoreCycle);
			_queue.Insert(0, task.SetProcess(aProcess).SetArguments(aArg1, aArg2, aArg3, aArg4));

			Start();
			return this;
		}

		/// <summary>
		/// Adds new task to the end of the queue. Task will be called until returns true.
		/// </summary>
		/// <param name="aProcess">Reference to the task method.</param>
		/// <param name="aArg1">First argument.</param>
		/// <param name="aArg2">Second argument.</param>
		/// <param name="aArg3">Third argument.</param>
		/// <param name="aArg4">Four argument.</param>
		/// <param name="aArg5">Five argument.</param>
		/// <param name="aIgnoreCycle">If true, then task will be called once per queue cycle.</param>
		/// <typeparam name="T1">Type of the first argument.</typeparam>
		/// <typeparam name="T2">Type of the second argument.</typeparam>
		/// <typeparam name="T3">Type of the third argument.</typeparam>
		/// <typeparam name="T4">Type of the four argument.</typeparam>
		/// <typeparam name="T5">Type of the five argument.</typeparam>
		/// <returns>Reference to the task manager.</returns>
		public AntTaskManager Task<T1, T2, T3, T4, T5>(
			AntTask<T1, T2, T3, T4, T5>.TaskProcess5 aProcess, 
			T1 aArg1, 
			T2 aArg2, 
			T3 aArg3, 
			T4 aArg4, 
			T5 aArg5, 
			bool aIgnoreCycle = false)
		{
			var task = new AntTask<T1, T2, T3, T4, T5>(aIgnoreCycle);
			_queue.Add(task.SetProcess(aProcess).SetArguments(aArg1, aArg2, aArg3, aArg4, aArg5));

			Start();
			return this;
		}

		/// <summary>
		/// Adds new instant task to the end of the queue. Task will be called once.
		/// </summary>
		/// <param name="aProcess">Reference to the task method.</param>
		/// <param name="aArg1">First argument.</param>
		/// <param name="aArg2">Second argument.</param>
		/// <param name="aArg3">Third argument.</param>
		/// <param name="aArg4">Four argument.</param>
		/// <param name="aArg5">Five argument.</param>
		/// <param name="aIgnoreCycle">If true, then task will be called once per queue cycle.</param>
		/// <typeparam name="T1">Type of the first argument.</typeparam>
		/// <typeparam name="T2">Type of the second argument.</typeparam>
		/// <typeparam name="T3">Type of the third argument.</typeparam>
		/// <typeparam name="T4">Type of the four argument.</typeparam>
		/// <typeparam name="T5">Type of the five argument.</typeparam>
		/// <returns>Reference to the task manager.</returns>
		public AntTaskManager InstantTask<T1, T2, T3, T4, T5>(
			AntInstantTask<T1, T2, T3, T4, T5>.TaskProcess5 aProcess, 
			T1 aArg1, 
			T2 aArg2, 
			T3 aArg3, 
			T4 aArg4, 
			T5 aArg5, 
			bool aIgnoreCycle = false)
		{
			var task = new AntInstantTask<T1, T2, T3, T4, T5>(aIgnoreCycle);
			_queue.Add(task.SetProcess(aProcess).SetArguments(aArg1, aArg2, aArg3, aArg4, aArg5));

			Start();
			return this;
		}

		/// <summary>
		/// Adds new urgent task to the begining of the queue. Task will be called until returns true.
		/// </summary>
		/// <param name="aProcess">Reference to the task method.</param>
		/// <param name="aArg1">First argument.</param>
		/// <param name="aArg2">Second argument.</param>
		/// <param name="aArg3">Third argument.</param>
		/// <param name="aArg4">Four argument.</param>
		/// <param name="aArg5">Five argument.</param>
		/// <param name="aIgnoreCycle">If true, then task will be called once per queue cycle.</param>
		/// <typeparam name="T1">Type of the first argument.</typeparam>
		/// <typeparam name="T2">Type of the second argument.</typeparam>
		/// <typeparam name="T3">Type of the third argument.</typeparam>
		/// <typeparam name="T4">Type of the four argument.</typeparam>
		/// <typeparam name="T5">Type of the five argument.</typeparam>
		/// <returns>Reference to the task manager.</returns>
		public AntTaskManager UrgentTask<T1, T2, T3, T4, T5>(
			AntTask<T1, T2, T3, T4, T5>.TaskProcess5 aProcess, 
			T1 aArg1, 
			T2 aArg2, 
			T3 aArg3, 
			T4 aArg4, 
			T5 aArg5, 
			bool aIgnoreCycle = false)
		{
			var task = new AntTask<T1, T2, T3, T4, T5>(aIgnoreCycle);
			_queue.Insert(0, task.SetProcess(aProcess).SetArguments(aArg1, aArg2, aArg3, aArg4, aArg5));

			Start();
			return this;
		}

		/// <summary>
		/// Adds new urgent task to the begining of the queue. Task will be called once.
		/// </summary>
		/// <param name="aProcess">Reference to the task method.</param>
		/// <param name="aArg1">First argument.</param>
		/// <param name="aArg2">Second argument.</param>
		/// <param name="aArg3">Third argument.</param>
		/// <param name="aArg4">Four argument.</param>
		/// <param name="aArg5">Five argument.</param>
		/// <param name="aIgnoreCycle">If true, then task will be called once per queue cycle.</param>
		/// <typeparam name="T1">Type of the first argument.</typeparam>
		/// <typeparam name="T2">Type of the second argument.</typeparam>
		/// <typeparam name="T3">Type of the third argument.</typeparam>
		/// <typeparam name="T4">Type of the four argument.</typeparam>
		/// <typeparam name="T5">Type of the five argument.</typeparam>
		/// <returns>Reference to the task manager.</returns>
		public AntTaskManager UrgentInstantTask<T1, T2, T3, T4, T5>(
			AntInstantTask<T1, T2, T3, T4, T5>.TaskProcess5 aProcess, 
			T1 aArg1, 
			T2 aArg2, 
			T3 aArg3, 
			T4 aArg4, 
			T5 aArg5, 
			bool aIgnoreCycle = false)
		{
			var task = new AntInstantTask<T1, T2, T3, T4, T5>(aIgnoreCycle);
			_queue.Insert(0, task.SetProcess(aProcess).SetArguments(aArg1, aArg2, aArg3, aArg4, aArg5));

			Start();
			return this;
		}

		/// <summary>
		/// Sets callback methods on the completion of the queue.
		/// </summary>
		/// <param name="aCallback">Reference on the callback method.</param>
		/// <returns>Reference to the task manager.</returns>
		public AntTaskManager OnComplete(TaskManagerCompleteDelegate aCallback)
		{
			_completeDelegate = aCallback;
			return this;
		}

		/// <summary>
		/// Stops current task and clears queue.
		/// </summary>
		public void Kill()
		{
			Stop();
			for (int i = 0, n = _queue.Count; i < n; i++)
			{
				_queue[i].Release();
				_queue[i] = null;
			}

			_queue.Clear();
		}

		/// <summary>
		/// Breaks processing of the current task and moves to the next one.
		/// </summary>
		public void Break()
		{
			if (_isHasTask)
			{
				NextTask(_currentTask.IgnoreCycle);
			}
		}

		/// <summary>
		/// Processing.
		/// </summary>
		public void Execute()
		{
			if (!IsStarted || IsPaused)
			{
				return;
			}

			if (_isHasTask)
			{
				if (_currentTask.Execute())
				{
					NextTask(_currentTask.IgnoreCycle);
				}
			}
			else
			{
				Stop();
				EventFinished?.Invoke(this);
				_completeDelegate?.Invoke();
				_completeDelegate = null;
			}
		}
		
	#endregion

	#region Private Methods

		private bool DelayProcess(float aTime)
		{
			_delay += Time.deltaTime;
			if (_delay >= aTime)
			{
				_delay = 0.0f;
				return true;
			}

			return false;
		}

		private void NextTask(bool aIgnoreCycle = false)
		{
			if (_isHasTask)
			{
				if (IsCycled && !aIgnoreCycle)
				{
					// If TaskManager works in cycle and last task not ignore it,
					// then add last task to the end of list.
					_queue.Add(_currentTask);
				}
				else
				{
					// Release task if task ignores cycle or task manager not cycled.
					_currentTask.Release();
				}
			}

			// Extract new task from the begining of queue
			_currentTask = _queue.PopFirst();
			_isHasTask = (_currentTask != null);
		}
		
		private void Start()
		{
			if (!IsStarted)
			{
				if (_updateMode == UpdateMode.Auto)
				{
					Scenario.Add(this);
				}

				NextTask();
				IsStarted = true;
				IsPaused = false;
			}
		}

		private void Stop()
		{
			if (IsStarted)
			{
				if (_updateMode == UpdateMode.Manual)
				{
					Scenario.Remove(this);
				}

				IsStarted = false;
			}
		}
		
	#endregion
	}
}