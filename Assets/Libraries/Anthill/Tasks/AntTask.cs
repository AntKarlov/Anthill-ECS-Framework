namespace Anthill.Tasks
{
	public class AntTask : ITask
	{
		public delegate bool TaskProcess0();
		private TaskProcess0 _process;
		
		public bool IgnoreCycle { get; set; }

		public AntTask(bool aIgnoreCycle)
		{
			IgnoreCycle = aIgnoreCycle;
		}
		
		public virtual bool Execute()
		{
			return _process();
		}

		public AntTask SetProcess(TaskProcess0 aProcess)
		{
			_process = aProcess;
			return this;
		}

		public virtual void Release()
		{
			_process = null;
		}
	}

	public class AntTask<T1> : AntTask
	{
		public delegate bool TaskProcess1(T1 aArg1);

		private TaskProcess1 _process;
		protected T1 _arg1;

		public AntTask(bool aIgnoreCycle) : base(aIgnoreCycle)
		{
			// ..
		}

		public override bool Execute()
		{
			return _process(_arg1);
		}

		public AntTask<T1> SetProcess(TaskProcess1 aProcess)
		{
			_process = aProcess;
			return this;
		}

		public AntTask<T1> SetArguments(T1 aArg1)
		{
			_arg1 = aArg1;
			return this;
		}

		public override void Release()
		{
			base.Release();
			_arg1 = default(T1);
		}
	}

	public class AntTask<T1, T2> : AntTask<T1>
	{
		public delegate bool TaskProcess2(T1 aArg1, T2 aArg2);

		private TaskProcess2 _process;
		protected T2 _arg2;

		public AntTask(bool aIgnoreCycle) : base(aIgnoreCycle)
		{
			// ..
		}

		public override bool Execute()
		{
			return _process(_arg1, _arg2);
		}

		public AntTask<T1, T2> SetProcess(TaskProcess2 aProcess)
		{
			_process = aProcess;
			return this;
		}

		public AntTask<T1, T2> SetArguments(T1 aArg1, T2 aArg2)
		{
			SetArguments(aArg1);
			_arg2 = aArg2;
			return this;
		}

		public override void Release()
		{
			_process = null;
			_arg1 = default(T1);
			_arg2 = default(T2);
		}
	}

	public class AntTask<T1, T2, T3> : AntTask<T1, T2>
	{
		public delegate bool TaskProcess3(T1 aArg1, T2 aArg2, T3 aArg3);

		private TaskProcess3 _process;
		protected T3 _arg3;

		public AntTask(bool aIgnoreCycle) : base(aIgnoreCycle)
		{
			// ..
		}

		public override bool Execute()
		{
			return _process(_arg1, _arg2, _arg3);
		}

		public AntTask<T1, T2, T3> SetProcess(TaskProcess3 aProcess)
		{
			_process = aProcess;
			return this;
		}

		public AntTask<T1, T2, T3> SetArguments(T1 aArg1, T2 aArg2, T3 aArg3)
		{
			SetArguments(aArg1, aArg2);
			_arg3 = aArg3;
			return this;
		}

		public override void Release()
		{
			base.Release();
			_arg3 = default(T3);
		}
	}

	public class AntTask<T1, T2, T3, T4> : AntTask<T1, T2, T3>
	{
		public delegate bool TaskProcess4(T1 aArg1, T2 aArg2, T3 aArg3, T4 aArg4);

		private TaskProcess4 _process;
		protected T4 _arg4;

		public AntTask(bool aIgnoreCycle) : base(aIgnoreCycle)
		{
			// ..
		}

		public override bool Execute()
		{
			return _process(_arg1, _arg2, _arg3, _arg4);
		}

		public AntTask<T1, T2, T3, T4> SetProcess(TaskProcess4 aProcess)
		{
			_process = aProcess;
			return this;
		}

		public AntTask<T1, T2, T3, T4> SetArguments(T1 aArg1, T2 aArg2, T3 aArg3, T4 aArg4)
		{
			SetArguments(aArg1, aArg2, aArg3);
			_arg4 = aArg4;
			return this;
		}

		public override void Release()
		{
			base.Release();
			_arg4 = default(T4);
		}
	}

	public class AntTask<T1, T2, T3, T4, T5> : AntTask<T1, T2, T3, T4>
	{
		public delegate bool TaskProcess5(T1 aArg1, T2 aArg2, T3 aArg3, T4 aArg4, T5 aArg5);

		private TaskProcess5 _process;
		protected T5 _arg5;

		public AntTask(bool aIgnoreCycle) : base(aIgnoreCycle)
		{
			// ..
		}

		public override bool Execute()
		{
			return _process(_arg1, _arg2, _arg3, _arg4, _arg5);
		}

		public AntTask<T1, T2, T3, T4, T5> SetProcess(TaskProcess5 aProcess)
		{
			_process = aProcess;
			return this;
		}

		public AntTask<T1, T2, T3, T4, T5> SetArguments(T1 aArg1, T2 aArg2, T3 aArg3, T4 aArg4, T5 aArg5)
		{
			SetArguments(aArg1, aArg2, aArg3, aArg4);
			_arg5 = aArg5;
			return this;
		}

		public override void Release()
		{
			base.Release();
			_arg5 = default(T5);
		}
	}

	public class AntInstantTask : ITask
	{
		public delegate void TaskProcess0();
		private TaskProcess0 _process;
		
		public bool IgnoreCycle { get; set; }

		public AntInstantTask(bool aIgnoreCycle)
		{
			IgnoreCycle = aIgnoreCycle;
		}
		
		public virtual bool Execute()
		{
			_process();
			return true;
		}

		public AntInstantTask SetProcess(TaskProcess0 aProcess)
		{
			_process = aProcess;
			return this;
		}

		public virtual void Release()
		{
			_process = null;
		}
	}

	public class AntInstantTask<T1> : AntInstantTask
	{
		public delegate void TaskProcess1(T1 aArg1);
		private TaskProcess1 _process;
		protected T1 _arg1;

		public AntInstantTask(bool aIgnoreCycle) : base(aIgnoreCycle)
		{
			// ..
		}
		
		public override bool Execute()
		{
			_process(_arg1);
			return true;
		}

		public AntInstantTask<T1> SetProcess(TaskProcess1 aProcess)
		{
			_process = aProcess;
			return this;
		}

		public AntInstantTask<T1> SetArguments(T1 aArg1)
		{
			_arg1 = aArg1;
			return this;
		}

		public override void Release()
		{
			base.Release();
			_arg1 = default(T1);
		}
	}

	public class AntInstantTask<T1, T2> : AntInstantTask<T1>
	{
		public delegate void TaskProcess2(T1 aArg1, T2 aArg2);
		private TaskProcess2 _process;
		protected T2 _arg2;

		public AntInstantTask(bool aIgnoreCycle) : base(aIgnoreCycle)
		{
			// ..
		}
		
		public override bool Execute()
		{
			_process(_arg1, _arg2);
			return true;
		}

		public AntInstantTask<T1, T2> SetProcess(TaskProcess2 aProcess)
		{
			_process = aProcess;
			return this;
		}

		public AntInstantTask<T1, T2> SetArguments(T1 aArg1, T2 aArg2)
		{
			SetArguments(aArg1);
			_arg2 = aArg2;
			return this;
		}

		public override void Release()
		{
			base.Release();
			_arg2 = default(T2);
		}
	}

	public class AntInstantTask<T1, T2, T3> : AntInstantTask<T1, T2>
	{
		public delegate void TaskProcess3(T1 aArg1, T2 aArg2, T3 aArg3);
		private TaskProcess3 _process;
		protected T3 _arg3;

		public AntInstantTask(bool aIgnoreCycle) : base(aIgnoreCycle)
		{
			// ..
		}
		
		public override bool Execute()
		{
			_process(_arg1, _arg2, _arg3);
			return true;
		}

		public AntInstantTask<T1, T2, T3> SetProcess(TaskProcess3 aProcess)
		{
			_process = aProcess;
			return this;
		}

		public AntInstantTask<T1, T2, T3> SetArguments(T1 aArg1, T2 aArg2, T3 aArg3)
		{
			SetArguments(aArg1, aArg2);
			_arg3 = aArg3;
			return this;
		}

		public override void Release()
		{
			base.Release();
			_arg3 = default(T3);
		}
	}

	public class AntInstantTask<T1, T2, T3, T4> : AntInstantTask<T1, T2, T3>
	{
		public delegate void TaskProcess4(T1 aArg1, T2 aArg2, T3 aArg3, T4 aArg4);
		private TaskProcess4 _process;
		protected T4 _arg4;

		public AntInstantTask(bool aIgnoreCycle) : base(aIgnoreCycle)
		{
			// ..
		}
		
		public override bool Execute()
		{
			_process(_arg1, _arg2, _arg3, _arg4);
			return true;
		}

		public AntInstantTask<T1, T2, T3, T4> SetProcess(TaskProcess4 aProcess)
		{
			_process = aProcess;
			return this;
		}

		public AntInstantTask<T1, T2, T3, T4> SetArguments(T1 aArg1, T2 aArg2, T3 aArg3, T4 aArg4)
		{
			SetArguments(aArg1, aArg2, aArg3);
			_arg4 = aArg4;
			return this;
		}

		public override void Release()
		{
			base.Release();
			_arg4 = default(T4);
		}
	}

	public class AntInstantTask<T1, T2, T3, T4, T5> : AntInstantTask<T1, T2, T3, T4>
	{
		public delegate void TaskProcess5(T1 aArg1, T2 aArg2, T3 aArg3, T4 aArg4, T5 aArg5);
		private TaskProcess5 _process;
		protected T5 _arg5;

		public AntInstantTask(bool aIgnoreCycle) : base(aIgnoreCycle)
		{
			// ..
		}
		
		public override bool Execute()
		{
			_process(_arg1, _arg2, _arg3, _arg4, _arg5);
			return true;
		}

		public AntInstantTask<T1, T2, T3, T4, T5> SetProcess(TaskProcess5 aProcess)
		{
			_process = aProcess;
			return this;
		}

		public AntInstantTask<T1, T2, T3, T4, T5> SetArguments(T1 aArg1, T2 aArg2, T3 aArg3, T4 aArg4, T5 aArg5)
		{
			SetArguments(aArg1, aArg2, aArg3, aArg4);
			_arg5 = aArg5;
			return this;
		}

		public override void Release()
		{
			base.Release();
			_arg5 = default(T5);
		}
	}
}