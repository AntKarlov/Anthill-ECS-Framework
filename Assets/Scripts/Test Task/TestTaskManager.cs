namespace Anthill.Test
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
			_t = GetComponent<Transform>();
			_tm = AntTaskManager.Do()
				.Delay(1.5f, true)
				.Task<Vector3, Vector3>(MoveTo, fromRef.position, toRef.position)
				.Task<Vector3, Vector3>(MoveTo, toRef.position, fromRef.position)
				.InstantTask(Track)
				.OnComplete(() => A.Log("Queue completed!"));
		}

		private void Update()
		{
			_tm.Execute();
		}
	
		#endregion
		#region Private Methods

		private void Track()
		{
			A.Log("Track!");
			_tm.UrgentInstantTask(() => A.Log($"Urgent! {_tm.TaskCount}"), true);
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