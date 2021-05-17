namespace Anthill.Scene
{
	using UnityEngine;
	using UnityEngine.SceneManagement;
	using System.Collections.Generic;
	using System.Collections;
	using System.Diagnostics;

	public class SceneLoader : MonoBehaviour
	{
		public delegate void SceneLoaderDelegate(string aSceneName);
		public delegate void ProgressSceneLoaderDelegate(string aSceneName, float aProgress);

		/// <summary>
		/// Invokes when loading of the scene is started.
		/// </summary>
		public event SceneLoaderDelegate EventBeginLoading;

		/// <summary>
		/// Invokes during proccess of the loading with current progress value (0-1).
		/// </summary>
		public event ProgressSceneLoaderDelegate EventProcessLoading;

		/// <summary>
		/// Invokes when loading of the scene is finished.
		/// </summary>
		public event SceneLoaderDelegate EventFinishLoading;

		/// <summary>
		/// Invokes when unloading of the scene is started.
		/// </summary>
		public event SceneLoaderDelegate EventBeginUnloading;

		/// <summary>
		/// Invokes during process of the unloading with current progress value (0-1).
		/// </summary>
		public event ProgressSceneLoaderDelegate EventProcessUnloading;

		/// <summary>
		/// Invokes when unloading of the scene is finished.
		/// </summary>
		public event SceneLoaderDelegate EventFinishUnloading;

		[Header("Loading Settings")]
		public ThreadPriority loadThreadPriority = ThreadPriority.Normal;

		[HideInInspector]
		public List<string> scenesHistory = new List<string>();

		private AsyncOperation _operation = new AsyncOperation();
		private Stopwatch _stopwatch = new Stopwatch();
		private string _nextScene;
		private float _progress;

		private SceneLoaderDelegate _beginLoadingCallback;
		private SceneLoaderDelegate _finishLoadingCallback;
		private SceneLoaderDelegate _beginUnloadingCallback;
		private SceneLoaderDelegate _finishUnloadingCallback;

	#region Getters / Setters

		public string CurrentScene { get; private set; }
		public bool HasScene { get; private set; }
		
	#endregion

	#region Public Methods

		/// <summary>
		/// Unloads current scene and loads given scene.
		/// </summary>
		/// <param name="aSceneName">Name of the scene.</param>
		/// <returns>Reference on the SceneLoader.</returns>
		public SceneLoader UnloadCurrentAndLoad(string aSceneName)
		{
			_nextScene = aSceneName;
			if (HasScene)
			{
				EventFinishUnloading += UnloadedHandler;
				UnloadScene(CurrentScene);
			}
			else
			{
				LoadScene(CurrentScene);
			}
			return this;
		}

		/// <summary>
		/// Starts loading scene by name.
		/// </summary>
		/// <param name="aSceneName">Name of the scene.</param>
		/// <returns>Reference on the SceneLoader.</returns>
		public SceneLoader LoadScene(string aSceneName)
		{
			StartCoroutine(AsyncLoading(aSceneName));
			return this;
		}

		/// <summary>
		/// Starts unloading scene by name.
		/// </summary>
		/// <param name="aSceneName">Name of the scene.</param>
		/// <returns>Reference on the SceneLoader.</returns>
		public SceneLoader UnloadScene(string aSceneName)
		{
			StartCoroutine(AsyncUnloading(aSceneName));
			return this;
		}

		/// <summary>
		/// Sets callback on the starting of loading process.
		/// </summary>
		/// <param name="aCallback">Reference on the method.</param>
		/// <returns>Reference on the SceneLoader.</returns>
		public SceneLoader OnBeginLoading(SceneLoaderDelegate aCallback)
		{
			_beginLoadingCallback = aCallback;
			return this;
		}

		/// <summary>
		/// Sets callback on the finish of loading process.
		/// </summary>
		/// <param name="aCallback">Reference on the method.</param>
		/// <returns>Reference on the SceneLoader.</returns>
		public SceneLoader OnFinishLoading(SceneLoaderDelegate aCallback)
		{
			_finishLoadingCallback = aCallback;
			return this;
		}

		/// <summary>
		/// Sets callback on the starting of unloading process.
		/// </summary>
		/// <param name="aCallback">Reference on the method.</param>
		/// <returns>Reference on the SceneLoader.</returns>
		public SceneLoader OnBeginUnloading(SceneLoaderDelegate aCallback)
		{
			_beginUnloadingCallback = aCallback;
			return this;
		}

		/// <summary>
		/// Sets callback on the finish of unloading process.
		/// </summary>
		/// <param name="aCallback">Reference on the method.</param>
		/// <returns>Reference on the SceneLoader.</returns>
		public SceneLoader OnFinishUnloading(SceneLoaderDelegate aCallback)
		{
			_finishUnloadingCallback = aCallback;
			return this;
		}

	#endregion

	#region Private Methods

		private IEnumerator AsyncLoading(string aSceneName)
		{
			A.Verbose($"Start loading `{aSceneName}` scene.", "SceneLoader");

			_stopwatch.Reset();

			yield return null;

			_stopwatch.Start();
			_progress = 0.0f;

			_beginLoadingCallback?.Invoke(aSceneName);
			_beginLoadingCallback = null;

			EventBeginLoading?.Invoke(aSceneName);
			
			Application.backgroundLoadingPriority = loadThreadPriority;
			_operation = SceneManager.LoadSceneAsync(aSceneName, LoadSceneMode.Additive);
			_operation.allowSceneActivation = false;

			// while (!(_operation.progress >= 0.9f))
			while (!_operation.isDone)
			{
				yield return null;
				if (!Mathf.Approximately(_operation.progress, _progress))
				{
					_progress = _operation.progress;
					EventProcessLoading?.Invoke(aSceneName, _progress);
				}
			}

			_operation.allowSceneActivation = true;
			yield return null;

			_progress = 1.0f;
			CurrentScene = aSceneName;
			HasScene = true;

			scenesHistory.Add(aSceneName);
			_stopwatch.Stop();

			A.Verbose($"Scene `{aSceneName}` loaded in {_stopwatch.Elapsed.TotalMilliseconds} ms.", "SceneLoader");

			EventFinishLoading?.Invoke(aSceneName);

			_finishLoadingCallback?.Invoke(aSceneName);
			_finishLoadingCallback = null;
		}

		private IEnumerator AsyncUnloading(string aSceneName)
		{
			A.Verbose($"Start unloading `{aSceneName}` scene.", "SceneLoader");

			_stopwatch.Reset();
			
			yield return null;
			
			_stopwatch.Start();
			_progress = 0.0f;

			_beginUnloadingCallback?.Invoke(aSceneName);
			_beginUnloadingCallback = null;

			EventBeginUnloading?.Invoke(aSceneName);

			Application.backgroundLoadingPriority = loadThreadPriority;
			_operation = SceneManager.UnloadSceneAsync(aSceneName, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
			_operation.allowSceneActivation = false;

			// while (!(_operation.progress >= 0.9f))
			while (!_operation.isDone)
			{
				yield return null;
				if (!Mathf.Approximately(_operation.progress, _progress))
				{
					_progress = _operation.progress;
					EventProcessUnloading?.Invoke(aSceneName, _progress);
				}
			}
			
			_operation.allowSceneActivation = true;
			yield return null;

			_progress = 1.0f;
			HasScene = false;
			CurrentScene = string.Empty;
			_stopwatch.Stop();
			A.Verbose($"Scene `{aSceneName}` unloaded in {_stopwatch.Elapsed.TotalMilliseconds} ms.", "SceneLoader");
			
			EventFinishUnloading?.Invoke(aSceneName);

			_finishUnloadingCallback?.Invoke(aSceneName);
			_finishUnloadingCallback = null;
		}
		
	#endregion

	#region Event Handlers
		
		private void UnloadedHandler(string aSceneName)
		{
			EventFinishUnloading -= UnloadedHandler;
			LoadScene(_nextScene);
		}
		
	#endregion
	}
}