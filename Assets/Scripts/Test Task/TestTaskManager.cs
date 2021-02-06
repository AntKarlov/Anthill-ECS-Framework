namespace Anthill
{
	using UnityEngine;
	using System.Collections;
	using Anthill.Tasks;
	using Anthill.Core;
	
	public class TestTaskManager : MonoBehaviour
	{
		public Transform fromRef;
		public Transform toRef;
		public float time;
		public AnimationCurve animationCurve;

		private Transform _t;
		private float _time;
		private AntTaskManager _tm;
	
		#region Unity Calls
	
		private void Awake()
		{
			A.verbosity = Verbosity.Verbose;
			A.editorVerbosity = Verbosity.Verbose;
			_t = GetComponent<Transform>();

			var engine = new AntEngine();
			AntDelayed.Call(1.0f, () =>
			{
				A.Editor.Verbose("Test");
				AntDelayed.Call(2.0f, () =>
				{
					A.Editor.Verbose("Second delayed call");
					_tm.SetUpdateMode(UpdateMode.Auto);
				});
			});

			_tm = AntTaskManager.Do(true)
				.SetUpdateMode(UpdateMode.Manual)
				.Delay(1.5f, true)
				.Task(MoveTo, fromRef.position, toRef.position)
				.Task(MoveTo, toRef.position, fromRef.position)
				.InstantTask(Track)
				.OnComplete(() => A.Editor.Verbose("Queue completed!", this));
		}

		private void Update()
		{
			AntEngine.ExecuteSystems();
		}

		private void FixedUpdate()
		{
			AntEngine.ExecuteFixedSystems();
		}
	
		#endregion
		#region Private Methods

		private void Track()
		{
			A.Editor.Warning("Track!", this, this);
			_tm.UrgentInstantTask(() => A.Editor.Verbose($"Urgent! {_tm.TaskCount}"), true);
		}
		
		private bool MoveTo(Vector3 aFrom, Vector3 aTo)
		{
			_time += Time.deltaTime;
			if (_time < time)
			{
				_t.position = Vector3.LerpUnclamped(aFrom, aTo, animationCurve.Evaluate(_time / time));
			}
			
			if (_time >= time)
			{
				_time = 0.0f;
				return true;
			}

			return false;
			// var t = 0.0f;
			// while (t < aTime)
			// {
			// 	t += Time.deltaTime;
			// 	_t.position = Vector3.LerpUnclamped(aFrom, aTo, aCurve.Evaluate(t / aTime));
			// 	yield return null;
			// }
		}
		
		#endregion
	}
}