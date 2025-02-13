using System;

namespace Anthill.Core
{
	public static class AntDelayed
	{
	#region Private Variables

		private static bool _isInitialized = false;
		private static AntDelayedCallScenario _scenario = null;

	#endregion

	#region Getters / Setters
		
		public static AntDelayedCallScenario Scenario
		{
			get
			{
				if (!_isInitialized)
				{
					_scenario = AntEngine.Add<AntDelayedCallScenario>(priority: -10);
					_isInitialized = true;
				}

				return _scenario;
			}
		}

	#endregion

	#region Public Methods

		/// <summary>
		/// Sets delayed call of specified action without arguments.
		/// </summary>
		/// <param name="delay">Delay in secodns before calling.</param>
		/// <param name="func">Reference to the method to call.</param>
		public static DelayedCall Call(float delay, Action func)
		{
			var call = new DelayedCall();
			call.SetProcess(func);
			call.delay = delay;
			Scenario.Add(call);
			return call;
		}
		
		/// <summary>
		/// Sets delayed call of specified action with one argument.
		/// </summary>
		/// <param name="delay">Delay in secodns before calling.</param>
		/// <param name="func">Reference to the method to call.</param>
		/// <param name="arg1">Argument for method.</param>
		/// <typeparam name="T1">Type of the argument.</typeparam>
		public static DelayedCall Call<T1>(float delay, Action<T1> func, T1 arg1)
		{
			var call = new DelayedCall<T1>();
			call.SetProcess(func);
			call.SetArgumens(arg1);
			call.delay = delay;
			Scenario.Add(call);
			return call;
		}
		
		/// <summary>
		/// Sets delayed call of specified action with two arguments.
		/// </summary>
		/// <param name="delay">Delay in secodns before calling.</param>
		/// <param name="func">Reference to the method to call.</param>
		/// <param name="arg1">Argument one.</param>
		/// <param name="arg2">Argument two.</param>
		/// <typeparam name="T1">Type of the first argument.</typeparam>
		/// <typeparam name="T2">Type of the second argument.</typeparam>
		public static DelayedCall Call<T1, T2>(float delay, Action<T1, T2> func, T1 arg1, T2 arg2)
		{
			var call = new DelayedCall<T1, T2>();
			call.SetProcess(func);
			call.SetArgumens(arg1, arg2);
			call.delay = delay;
			Scenario.Add(call);
			return call;
		}

		/// <summary>
		/// Sets delayed call of specified action with three arguments.
		/// </summary>
		/// <param name="delay">Delay in secodns before calling.</param>
		/// <param name="func">Reference to the method to call.</param>
		/// <param name="arg1">Argument one.</param>
		/// <param name="arg2">Argument two.</param>
		/// <param name="arg3"></param>
		/// <typeparam name="T1">Type of the first argument.</typeparam>
		/// <typeparam name="T2">Type of the second argument.</typeparam>
		/// <typeparam name="T3">Type of the third argument.</typeparam>
		/// <returns></returns>
		public static DelayedCall Call<T1, T2, T3>(float delay, Action<T1, T2, T3> func, T1 arg1, T2 arg2, T3 arg3)
		{
			var call = new DelayedCall<T1, T2, T3>();
			call.SetProcess(func);
			call.SetArgumens(arg1, arg2, arg3);
			call.delay = delay;
			Scenario.Add(call);
			return call;
		}

		/// <summary>
		/// Sets delayed call of specified action with four arguments.
		/// </summary>
		/// <param name="delay">Delay in secodns before calling.</param>
		/// <param name="func">Reference to the method to call.</param>
		/// <param name="arg1">Argument one.</param>
		/// <param name="arg2">Argument two.</param>
		/// <param name="arg3">Argument three.</param>
		/// <param name="arg4">Argument four.</param>
		/// <typeparam name="T1">Type of the first argument.</typeparam>
		/// <typeparam name="T2">Type of the second argument.</typeparam>
		/// <typeparam name="T3">Type of the third argument.</typeparam>
		/// <typeparam name="T4">Type of the fourth argument.</typeparam>
		/// <returns></returns>
		public static DelayedCall Call<T1, T2, T3, T4>(float delay, Action<T1, T2, T3, T4> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			var call = new DelayedCall<T1, T2, T3, T4>();
			call.SetProcess(func);
			call.SetArgumens(arg1, arg2, arg3, arg4);
			call.delay = delay;
			Scenario.Add(call);
			return call;
		}

	#endregion
	}

	/// <summary>
	/// DelayedCall helper classes.
	/// </summary>
	public class DelayedCall
	{
		public float delay;
		private Action _process;

		public bool IsUnscaledTime { get; private set; }
		public bool IsKillOnDeinitialize { get; private set; }

		public virtual bool Update(float deltaTime)
		{
			delay -= deltaTime;
			if (delay <= 0.0f)
			{
				_process();
				_process = null;
				return true;
			}
			return false;
		}

		public void SetProcess(Action process)
		{
			_process = process;
		}

		public void Kill()
		{
			AntDelayed.Scenario.Remove(this);
		}

		public DelayedCall SetUpdate(bool useUnscaledTime)
		{
			IsUnscaledTime = useUnscaledTime;
			return this;
		}

		public DelayedCall SetKillOnDeinitialize(bool kill)
		{
			IsKillOnDeinitialize = kill;
			return this;
		}
	}

	public class DelayedCall<T1> : DelayedCall
	{
		private Action<T1> _process;
		protected T1 _arg1;

		public override bool Update(float deltaTime)
		{
			delay -= deltaTime;
			if (delay <= 0.0f)
			{
				_process(_arg1);
				_process = null;
				return true;
			}
			return false;
		}

		public void SetProcess(Action<T1> process)
		{
			_process = process;
		}

		public void SetArgumens(T1 arg1)
		{
			_arg1 = arg1;
		}
	}

	public class DelayedCall<T1, T2> : DelayedCall<T1>
	{
		private Action<T1, T2> _process;
		protected T2 _arg2;

		public override bool Update(float deltaTime)
		{
			delay -= deltaTime;
			if (delay <= 0.0f)
			{
				_process(_arg1, _arg2);
				_process = null;
				return true;
			}
			return false;
		}

		public void SetProcess(Action<T1, T2> process)
		{
			_process = process;
		}

		public void SetArgumens(T1 arg1, T2 arg2)
		{
			_arg1 = arg1;
			_arg2 = arg2;
		}
	}

	public class DelayedCall<T1, T2, T3> : DelayedCall<T1, T2>
	{
		private Action<T1, T2, T3> _process;
		protected T3 _arg3;

		public override bool Update(float deltaTime)
		{
			delay -= deltaTime;
			if (delay <= 0.0f)
			{
				_process(_arg1, _arg2, _arg3);
				_process = null;
				return true;
			}
			return false;
		}

		public void SetProcess(Action<T1, T2, T3> process)
		{
			_process = process;
		}

		public void SetArgumens(T1 arg1, T2 arg2, T3 arg3)
		{
			_arg1 = arg1;
			_arg2 = arg2;
			_arg3 = arg3;
		}
	}

	public class DelayedCall<T1, T2, T3, T4> : DelayedCall<T1, T2, T3>
	{
		private Action<T1, T2, T3, T4> _process;
		protected T4 _arg4;

		public override bool Update(float deltaTime)
		{
			delay -= deltaTime;
			if (delay <= 0.0f)
			{
				_process(_arg1, _arg2, _arg3, _arg4);
				_process = null;
				return true;
			}
			return false;
		}

		public void SetProcess(Action<T1, T2, T3, T4> process)
		{
			_process = process;
		}

		public void SetArgumens(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			_arg1 = arg1;
			_arg2 = arg2;
			_arg3 = arg3;
			_arg4 = arg4;
		}
	}
}