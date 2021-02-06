namespace Anthill
{
	using UnityEngine;
	using System.Collections;
	using Anthill.Task;
	
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
			_tm = AntTaskManager.Do(true)
				.Delay(1.5f, true)
				.Task<Vector3, Vector3>(MoveTo, fromRef.position, toRef.position)
				.Task<Vector3, Vector3>(MoveTo, toRef.position, fromRef.position)
				.InstantTask(Track)
				.OnComplete(() => A.Editor.Verbose("Queue completed!", this));
		}

		private void Update()
		{
			_tm.Execute();
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