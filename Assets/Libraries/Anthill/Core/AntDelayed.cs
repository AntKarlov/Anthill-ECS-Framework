namespace Anthill.Core
{
	using System;
	
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
					_scenario = AntEngine.Add<AntDelayedCallScenario>(-1);
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
		/// <param name="aDelay">Delay in secodns before calling.</param>
		/// <param name="aFunc">Reference to the method to call.</param>
		public static DelayedCall Call(float aDelay, Action aFunc)
		{
			var call = new DelayedCall();
			call.SetProcess(aFunc);
			call.delay = aDelay;
			Scenario.Add(call);
			return call;
		}
		
		/// <summary>
		/// Sets delayed call of specified action with one argument.
		/// </summary>
		/// <param name="aDelay">Delay in secodns before calling.</param>
		/// <param name="aFunc">Reference to the method to call.</param>
		/// <param name="aArg1">Argument for method.</param>
		/// <typeparam name="T1">Type of the argument.</typeparam>
		public static DelayedCall Call<T1>(float aDelay, Action<T1> aFunc, T1 aArg1)
		{
			var call = new DelayedCall<T1>();
			call.SetProcess(aFunc);
			call.SetArgumens(aArg1);
			call.delay = aDelay;
			Scenario.Add(call);
			return call;
		}
		
		/// <summary>
		/// Sets delayed call of specified action with two arguments.
		/// </summary>
		/// <param name="aDelay">Delay in secodns before calling.</param>
		/// <param name="aFunc">Reference to the method to call.</param>
		/// <param name="aArg1">Argument one.</param>
		/// <param name="aArg2">Argument two.</param>
		/// <typeparam name="T1">Type of the first argument.</typeparam>
		/// <typeparam name="T2">Type of the second argument.</typeparam>
		public static DelayedCall Call<T1, T2>(float aDelay, Action<T1, T2> aFunc, T1 aArg1, T2 aArg2)
		{
			var call = new DelayedCall<T1, T2>();
			call.SetProcess(aFunc);
			call.SetArgumens(aArg1, aArg2);
			call.delay = aDelay;
			Scenario.Add(call);
			return call;
		}

		/// <summary>
		/// Sets delayed call of specified action with three arguments.
		/// </summary>
		/// <param name="aDelay">Delay in secodns before calling.</param>
		/// <param name="aFunc">Reference to the method to call.</param>
		/// <param name="aArg1">Argument one.</param>
		/// <param name="aArg2">Argument two.</param>
		/// <param name="aArg3"></param>
		/// <typeparam name="T1">Type of the first argument.</typeparam>
		/// <typeparam name="T2">Type of the second argument.</typeparam>
		/// <typeparam name="T3">Type of the third argument.</typeparam>
		/// <returns></returns>
		public static DelayedCall Call<T1, T2, T3>(float aDelay, Action<T1, T2, T3> aFunc, T1 aArg1, T2 aArg2, T3 aArg3)
		{
			var call = new DelayedCall<T1, T2, T3>();
			call.SetProcess(aFunc);
			call.SetArgumens(aArg1, aArg2, aArg3);
			call.delay = aDelay;
			Scenario.Add(call);
			return call;
		}

		/// <summary>
		/// Sets delayed call of specified action with four arguments.
		/// </summary>
		/// <param name="aDelay">Delay in secodns before calling.</param>
		/// <param name="aFunc">Reference to the method to call.</param>
		/// <param name="aArg1">Argument one.</param>
		/// <param name="aArg2">Argument two.</param>
		/// <param name="aArg3">Argument three.</param>
		/// <param name="aArg4">Argument four.</param>
		/// <typeparam name="T1">Type of the first argument.</typeparam>
		/// <typeparam name="T2">Type of the second argument.</typeparam>
		/// <typeparam name="T3">Type of the third argument.</typeparam>
		/// <typeparam name="T4">Type of the fourth argument.</typeparam>
		/// <returns></returns>
		public static DelayedCall Call<T1, T2, T3, T4>(float aDelay, Action<T1, T2, T3, T4> aFunc, T1 aArg1, T2 aArg2, T3 aArg3, T4 aArg4)
		{
			var call = new DelayedCall<T1, T2, T3, T4>();
			call.SetProcess(aFunc);
			call.SetArgumens(aArg1, aArg2, aArg3, aArg4);
			call.delay = aDelay;
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
		public bool isUnscaledTime;
		private Action _process;

		public virtual bool Update(float aDeltaTime)
		{
			delay -= aDeltaTime;
			if (delay <= 0.0f)
			{
				_process();
				_process = null;
				return true;
			}
			return false;
		}

		public void SetProcess(Action aProcess)
		{
			_process = aProcess;
		}

		public void Kill()
		{
			AntDelayed.Scenario.Remove(this);
		}

		public void SetUpdate(bool aIsUnscaledTime)
		{
			isUnscaledTime = aIsUnscaledTime;
		}
	}

	public class DelayedCall<T1> : DelayedCall
	{
		private Action<T1> _process;
		protected T1 _arg1;

		public override bool Update(float aDeltaTime)
		{
			delay -= aDeltaTime;
			if (delay <= 0.0f)
			{
				_process(_arg1);
				_process = null;
				return true;
			}
			return false;
		}

		public void SetProcess(Action<T1> aProcess)
		{
			_process = aProcess;
		}

		public void SetArgumens(T1 aArg1)
		{
			_arg1 = aArg1;
		}
	}

	public class DelayedCall<T1, T2> : DelayedCall<T1>
	{
		private Action<T1, T2> _process;
		protected T2 _arg2;

		public override bool Update(float aDeltaTime)
		{
			delay -= aDeltaTime;
			if (delay <= 0.0f)
			{
				_process(_arg1, _arg2);
				_process = null;
				return true;
			}
			return false;
		}

		public void SetProcess(Action<T1, T2> aProcess)
		{
			_process = aProcess;
		}

		public void SetArgumens(T1 aArg1, T2 aArg2)
		{
			_arg1 = aArg1;
			_arg2 = aArg2;
		}
	}

	public class DelayedCall<T1, T2, T3> : DelayedCall<T1, T2>
	{
		private Action<T1, T2, T3> _process;
		protected T3 _arg3;

		public override bool Update(float aDeltaTime)
		{
			delay -= aDeltaTime;
			if (delay <= 0.0f)
			{
				_process(_arg1, _arg2, _arg3);
				_process = null;
				return true;
			}
			return false;
		}

		public void SetProcess(Action<T1, T2, T3> aProcess)
		{
			_process = aProcess;
		}

		public void SetArgumens(T1 aArg1, T2 aArg2, T3 aArg3)
		{
			_arg1 = aArg1;
			_arg2 = aArg2;
			_arg3 = aArg3;
		}
	}

	public class DelayedCall<T1, T2, T3, T4> : DelayedCall<T1, T2, T3>
	{
		private Action<T1, T2, T3, T4> _process;
		protected T4 _arg4;

		public override bool Update(float aDeltaTime)
		{
			delay -= aDeltaTime;
			if (delay <= 0.0f)
			{
				_process(_arg1, _arg2, _arg3, _arg4);
				_process = null;
				return true;
			}
			return false;
		}

		public void SetProcess(Action<T1, T2, T3, T4> aProcess)
		{
			_process = aProcess;
		}

		public void SetArgumens(T1 aArg1, T2 aArg2, T3 aArg3, T4 aArg4)
		{
			_arg1 = aArg1;
			_arg2 = aArg2;
			_arg3 = aArg3;
			_arg4 = aArg4;
		}
	}
}