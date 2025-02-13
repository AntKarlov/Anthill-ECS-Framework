using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Anthill.Core
{
	public enum AvgResetDuration
	{
		Always = 1,
		VeryFast = 30,
		Fast = 60,
		Normal = 120,
		Slow = 300,
		Never = int.MaxValue
	}

	public class AntDebugScenario : AntBaseScenario
	{
	#region Public Variables

		public static AvgResetDuration avgResetDuration = AvgResetDuration.Never;
		public bool isPaused;

	#endregion

	#region Private Variables

		private readonly GameObject _container;
		private double _totalDuration;
		private readonly Stopwatch _stopwatch;
		private readonly List<AntSystemInfo> _initializeSystemsInfos;
		private readonly List<AntSystemInfo> _executeSystemsInfos;

		private readonly StringBuilder _stringBuilder = new();
		private readonly static string[] _numbers = new string[] 
		{
			"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", 
			"19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32", "33", "34", "35", 
			"36", "37", "38", "39", "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "50", "51", "52", 
			"53", "54", "55", "56", "57", "58", "59", "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", 
			"70", "71", "72", "73", "74", "75", "76", "77", "78", "79", "80", "81", "82", "83", "84", "85", "86", 
			"87", "88", "89", "90", "91", "92", "93", "94", "95", "96", "97", "98", "99", "100"
		};

		private float _lastUpdateTime = 0.0f;
		private readonly float _updateInterval = 0.25f;
		private readonly bool _isInit;

	#endregion

	#region Getters / Setters

		public GameObject Container => _container;
		public int InitializeSystemsCount => _initializeSystems.Count;
		public int ExecuteSystemsCount => _executeSystems.Count;
		public int SystemsCount => _systems.Count;

		public int TotalInitializeSystemsCount
		{
			get
			{
				int count = 0;
				for (int i = 0, n = _initializeSystems.Count; i < n; i++)
				{
					if (_initializeSystems[i].System is AntDebugScenario debugScenario)
					{
						count += debugScenario.TotalInitializeSystemsCount;
					}
					else
					{
						count++;
					}
				}
				return count;
			}
		}

		public int TotalExecuteSystemsCount
		{
			get
			{
				int count = 0;
				for (int i = 0, n = _executeSystems.Count; i < n; i++)
				{
					if (_executeSystems[i].System is AntDebugScenario debugScenario)
					{
						count += debugScenario.TotalExecuteSystemsCount;
					}
					else
					{
						count++;
					}
				}
				return count;
			}
		}

		public double TotalDuration => _totalDuration;
		public List<AntSystemInfo> InitializeSystemsInfos => _initializeSystemsInfos;
		public List<AntSystemInfo> ExecuteSystemInfos => _executeSystemsInfos;

	#endregion

	#region Public Methods

		public AntDebugScenario(string name) : base(name)
		{
			_container = new GameObject();
			_container.AddComponent<AntDebugScenarioBehaviour>().Init(this);
			_totalDuration = 0;

			_stopwatch = new Stopwatch();
			_initializeSystemsInfos = new List<AntSystemInfo>();
			_executeSystemsInfos = new List<AntSystemInfo>();
			_isInit = true;

			UpdateName();
		}

		public override ISystem Add(ISystem system, int priority = -1)
		{
			if (system is AntDebugScenario debugScenario)
			{
				debugScenario.Container.transform.SetParent(_container.transform, false);
			}

			var systemInfo = new AntSystemInfo(system);
			if (systemInfo.IsInitializeSystem)
			{
				_initializeSystemsInfos.Add(systemInfo);
			}

			if (systemInfo.IsExecuteSystem)
			{
				_executeSystemsInfos.Add(systemInfo);
			}

			return base.Add(system, priority);
		}

		public override ISystem Remove(ISystem system)
		{
			if (system is AntDebugScenario debugScenario)
			{
				GameObject.Destroy(debugScenario.Container);
			}

			_initializeSystemsInfos.RemoveAll(x => System.Object.ReferenceEquals(x.System, system));
			_executeSystemsInfos.RemoveAll(x => System.Object.ReferenceEquals(x.System, system));

			return base.Remove(system);
		}

		public override void Initialize()
		{
			_totalDuration = 0;
			IInitializeSystem system;
			AntSystemInfo systemInfo;
			double duration;
			for (int i = _initializeSystems.Count - 1; i >= 0; i--)
			{
				system = _initializeSystems[i].System;
				systemInfo = _initializeSystemsInfos[i];
				if (systemInfo.isActive)
				{
					duration = MonitorInitializeSystemDuration(system);
					_totalDuration += duration;
					systemInfo.AddExecutionDuration(duration);
				}
			}

			UpdateName();
		}

		public override void Execute()
		{
			if (!isPaused && _enabled)
			{
				Step();
			}
		}

		public void Step()
		{
			_totalDuration = 0;
			if (Time.frameCount % (int) avgResetDuration == 0)
			{
				ResetDurations();
			}

			IExecuteSystem system;
			AntSystemInfo systemInfo;
			double duration;
			for (int i = _executeSystems.Count - 1; i >= 0; i--)
			{
				system = _executeSystems[i].System;
				systemInfo = _executeSystemsInfos[i];
				if (systemInfo.isActive)
				{
					duration = MonitorExecuteSystemDuration(system);
					_totalDuration += duration;
					systemInfo.AddExecutionDuration(duration);
				}
			}

			if (Time.time - _lastUpdateTime >= _updateInterval)
			{
				_lastUpdateTime = Time.time;
				UpdateName();
			}
		}

		public void ResetDurations()
		{
			for (int i = 0, n = _initializeSystemsInfos.Count; i < n; i++)
			{
				_initializeSystemsInfos[i].ResetDurations();
			}

			for (int i = 0, n = _executeSystemsInfos.Count; i < n; i++)
			{
				_executeSystemsInfos[i].ResetDurations();
			}
		}

	#endregion

	#region Private Methods

		private void UpdateName()
		{
			if (!_isInit) return;
			
			_stringBuilder.Append(Name)
				.Append(" (")
				.Append((_initializeSystems.Count <= 100) ? _numbers[_initializeSystems.Count] : ">100")
				.Append(" ini, ")
				.Append((_executeSystems.Count <= 100) ? _numbers[_executeSystems.Count] : ">100")
				.Append(" exe, ")
				.Append(_totalDuration.ToString("0.###"))
				.Append(" ms)");

			_container.name = _stringBuilder.ToString();
			_stringBuilder.Length = 0;

			// _container.name = $"{Name} ({_initializeSystems.Count} ini, {_executeSystems.Count} exe, {_totalDuration.ToString("0.###")} ms)";
			// _container.name = string.Format(
			// 	"{0} ({1} ini, {2} exe, {3:0.###} ms)", 
			// 	Name,
			// 	_initializeSystems.Count, 
			// 	_executeSystems.Count, 
			// 	_totalDuration
			// );
		}

		private double MonitorInitializeSystemDuration(IInitializeSystem system)
		{
			_stopwatch.Reset();
			_stopwatch.Start();
			system.Initialize();
			_stopwatch.Stop();
			return _stopwatch.Elapsed.TotalMilliseconds;
		}

		private double MonitorExecuteSystemDuration(IExecuteSystem system)
		{
			_stopwatch.Reset();
			_stopwatch.Start();
			system.Execute();
			_stopwatch.Stop();
			return _stopwatch.Elapsed.TotalMilliseconds;
		}

	#endregion
	}
}