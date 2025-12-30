using System;

namespace Anthill.Core
{
	public class AntSystemInfo
	{
		// -----------------------------------------------------
		// Public Variables
		// -----------------------------------------------------

		public bool isActive;

		public ISystem System { get; private set; }
		public string Name { get; private set; }
		public bool IsInitializeSystem { get; private set; }
		public bool IsExecuteSystem { get; private set; }

		// -----------------------------------------------------
		// Private Variables
		// -----------------------------------------------------

		private double _accumulatedExecutionDuration;
		private double _minExecutionDuration;
		private double _maxExecutionDuration;
		private int _durationCount;

		private const string SYSTEM_SUFFIX = "System";

		// -----------------------------------------------------
		// Getters / Setters
		// -----------------------------------------------------

		public double AverageExecutionDuration => (_durationCount == 0) ? 0 : _accumulatedExecutionDuration / _durationCount;
		public double MinExecutionDuration => _minExecutionDuration;
		public double MaxExecutionDuration => _maxExecutionDuration;

		// -----------------------------------------------------
		// Public Methods
		// -----------------------------------------------------

		public AntSystemInfo(ISystem system)
		{
			System = system;
			isActive = true;

			if (system is IInitializeSystem)
			{
				IsInitializeSystem = true;
			}

			if (system is IExecuteSystem)
			{
				IsExecuteSystem = true;
			}

			if (system is AntBaseScenario debugScenario)
			{
				Name = debugScenario.Name;
			}
			else
			{
				Type systemType = system.GetType();
				Name = systemType.Name.EndsWith(SYSTEM_SUFFIX, StringComparison.Ordinal)
					? systemType.Name.Substring(0, systemType.Name.Length - SYSTEM_SUFFIX.Length)
					: systemType.Name;
			}
		}

		public void AddExecutionDuration(double executionDuration)
		{
			if (executionDuration < _minExecutionDuration || _minExecutionDuration == 0.0f)
			{
				_minExecutionDuration = executionDuration;
			}
			else if (executionDuration > _maxExecutionDuration)
			{
				_maxExecutionDuration = executionDuration;
			}

			_accumulatedExecutionDuration += executionDuration;
			_durationCount++;
		}

		public void ResetDurations()
		{
			_accumulatedExecutionDuration = 0.0f;
			_durationCount = 0;

			var debugScenario = (AntDebugScenario)System;
			debugScenario?.ResetDurations();
		}
	}
}