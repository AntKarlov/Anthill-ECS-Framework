namespace Anthill.Core
{
	using System;
	
	public class AntSystemInfo
	{
	#region Public Variables

		public bool isActive;

		public ISystem System { get; private set; }
		public string Name { get; private set; }
		public bool IsInitializeSystem { get; private set; }
		public bool IsExecuteSystem { get; private set; }

	#endregion

	#region Private Variables

		private double _accumulatedExecutionDuration;
		private double _minExecutionDuration;
		private double _maxExecutionDuration;
		private int _durationCount;

		private const string SYSTEM_SUFFIX = "System";

	#endregion

	#region Getters / Setters

		public double AverageExecutionDuration 
		{
			get => (_durationCount == 0) ? 0 : _accumulatedExecutionDuration / _durationCount;
		}

		public double MinExecutionDuration { get => _minExecutionDuration; }
		public double MaxExecutionDuration { get => _maxExecutionDuration; }

	#endregion

	#region Public Methods

		public AntSystemInfo(ISystem aSystem)
		{
			System = aSystem;
			isActive = true;

			var initializeSystem = aSystem as IInitializeSystem;
			if (initializeSystem != null)
			{
				IsInitializeSystem = true;
			}

			var executeSystem = aSystem as IExecuteSystem;
			if (executeSystem != null)
			{
				IsExecuteSystem = true;
			}

			var debugScenario = aSystem as AntBaseScenario;
			if (debugScenario != null)
			{
				Name = debugScenario.Name;
			}
			else
			{
				Type systemType = aSystem.GetType();
				Name = systemType.Name.EndsWith(SYSTEM_SUFFIX, StringComparison.Ordinal)
                    ? systemType.Name.Substring(0, systemType.Name.Length - SYSTEM_SUFFIX.Length)
                    : systemType.Name;
			}
		}

		public void AddExecutionDuration(double aExecutionDuration)
		{
			if (aExecutionDuration < _minExecutionDuration || _minExecutionDuration == 0.0f)
			{
				_minExecutionDuration = aExecutionDuration;
			}
			else if (aExecutionDuration > _maxExecutionDuration)
			{
				_maxExecutionDuration = aExecutionDuration;
			}

			_accumulatedExecutionDuration += aExecutionDuration;
			_durationCount++;
		}

		public void ResetDurations()
		{
			_accumulatedExecutionDuration = 0.0f;
			_durationCount = 0;

			var debugScenario = (AntDebugScenario) System;
			debugScenario?.ResetDurations();
		}

	#endregion
	}
}