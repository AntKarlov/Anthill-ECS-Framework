using UnityEngine;
using NUnit.Framework;

namespace Anthill.Core.Tests
{
	public class AntEngineAutoTest
	{
		public class TestScenario : AntScenario
		{
			public TestScenario() : base("TestScenario")
			{
				// ..
			}

			public override void AddedToEngine()
			{
				base.AddedToEngine();
				Add<SystemA>();
				Add<SystemB>();
				Add<HealthSystem>();
			}

			public override void RemovedFromEngine()
			{
				Remove<SystemA>();
				Remove<SystemB>();
				Remove<HealthSystem>();
				base.RemovedFromEngine();
			}
		}

		public class SystemA : ISystem, IExecuteSystem, IExecuteFixedSystem, IExecuteLateSystem, IInitializeSystem,
			IDeinitializeSystem, ICleanupSystem, IResetSystem
		{
			public int executeCount;
			public int executeFixedCount;
			public int executeLateCount;
			public int initializedCount;
			public int deinitializedCount;
			public int cleanupCount;
			public int addedToEngineCount;

			public void AddedToEngine()
			{
				addedToEngineCount++;
			}

			public void RemovedFromEngine()
			{
				addedToEngineCount--;
			}

			public void Execute()
			{
				executeCount++;
			}

			public void ExecuteFixed()
			{
				executeFixedCount++;
			}

			public void ExecuteLate()
			{
				executeLateCount++;
			}

			public void Initialize()
			{
				initializedCount++;
			}

			public void Deinitialize()
			{
				deinitializedCount++;
			}

			public void Cleanup()
			{
				cleanupCount++;
			}

			public void Reset()
			{
				executeCount = 0;
				executeFixedCount = 0;
				executeLateCount = 0;
				initializedCount = 0;
				deinitializedCount = 0;
				cleanupCount = 0;
				addedToEngineCount = 0;
			}
		}

		public class SystemB : ISystem, IExecuteSystem
		{
			public bool isExecuted;
			public bool isAddedToEngine;

			public void AddedToEngine()
			{
				isAddedToEngine = true;
			}

			public void RemovedFromEngine()
			{
				isAddedToEngine = false;
			}

			public void Execute()
			{
				isExecuted = true;
			}
		}

		public class TestGameplay : AntScenario
		{
			public TestGameplay() : base("TestGameplay")
			{
				// ..
			}

			public override void AddedToEngine()
			{
				base.AddedToEngine();
				Add<MoveSystem>();
				Add<MoveMonoSystem>();
			}

			public override void RemovedFromEngine()
			{
				Remove<MoveSystem>();
				Remove<MoveMonoSystem>();
				base.RemovedFromEngine();
			}
		}

		public class MoveSystem : ISystem, IExecuteSystem
		{
			public AntNodeList<MoveNode> moveNodes;
			public int addEventCount;
			public int removeEventCount;

			public void AddedToEngine()
			{
				A.Log("Add MoveSystem");
				moveNodes = AntEngine.GetNodes<MoveNode>();
				moveNodes.EventNodeAdded += AddMoveNodeHandler;
				moveNodes.EventNodeRemoved += RemoveMoveNodeHandler;
			}

			public void RemovedFromEngine()
			{
				A.Log("Remove MoveSystem");
				moveNodes.EventNodeAdded -= AddMoveNodeHandler;
				moveNodes.EventNodeRemoved -= RemoveMoveNodeHandler;
				moveNodes = null;
			}

			public void Execute()
			{

			}

			private void AddMoveNodeHandler(MoveNode node)
			{
				addEventCount++;
				A.Log($"Added MoveNode. MoveNodes.Count: {moveNodes.Count + 1}");
			}

			private void RemoveMoveNodeHandler(MoveNode node)
			{
				removeEventCount++;
				A.Log($"Remove MoveNode. MoveNodes.Count: {moveNodes.Count - 1}");
			}
		}

		public class MoveMonoSystem : ISystem
		{
			public AntNodeList<AntNode<Transform, Move>> moveNodes;
			public int addEventCount;
			public int removeEventCount;

			public void AddedToEngine()
			{
				A.Log("Add MoveMonoSystem");
				moveNodes = AntEngine.GetNodes<AntNode<Transform, Move>>();
				moveNodes.EventNodeAdded += AddMoveNodeHandler;
				moveNodes.EventNodeRemoved += RemoveMoveNodeHandler;
			}

			public void RemovedFromEngine()
			{
				A.Log("Remove MoveMonoSystem");
				moveNodes.EventNodeAdded -= AddMoveNodeHandler;
				moveNodes.EventNodeRemoved -= RemoveMoveNodeHandler;
				moveNodes = null;
			}

			private void AddMoveNodeHandler(AntNode<Transform> node)
			{
				addEventCount++;
				A.Log($"Added MoveNode. MoveNodes.Count: {moveNodes.Count + 1}");
			}

			private void RemoveMoveNodeHandler(AntNode<Transform> node)
			{
				removeEventCount++;
				A.Log($"Remove MoveNode. MoveNodes.Count: {moveNodes.Count - 1}");
			}
		}

		public class HealthSystem : ISystem
		{
			public AntNodeList<HealthNode> healthNodes;
			public AntNodeList<EnemyNode> enemyNodes;
			public AntNodeList<PlayerNode> playerNodes;

			public void AddedToEngine()
			{
				healthNodes = AntEngine.GetNodes<HealthNode>();
				enemyNodes = AntEngine.GetNodes<EnemyNode>();
				playerNodes = AntEngine.GetNodes<PlayerNode>();
			}

			public void RemovedFromEngine()
			{
				healthNodes = null;
				enemyNodes = null;
				playerNodes = null;
			}
		}

		public class MoveNode : AntNode
		{
			public Move Move { get; set; }
		}

		public class InputNode : AntNode
		{
			public Move Move { get; set; }
			public Controller Controller { get; set; }
		}

		public class PlayerNode : AntNode
		{
			public Health Health { get; set; }
			public UserControl UserControl { get; set; }
		}

		[Exclude(typeof(UserControl), typeof(BoxContainer))]
		public class EnemyNode : AntNode
		{
			public Health Health { get; set; }
		}

		public class HealthNode : AntNode
		{
			public Health Health { get; set; }
		}

		public class BoxContainer
		{
			// ..
		}

		public class AiControl
		{
			// ..
		}

		public class Move
		{
			public Vector2 position;
			public Vector2 velocity;

			public Move()
			{
				position = Vector2.zero;
				velocity = Vector2.zero;
			}

			public Move(Vector2 position)
			{
				this.position = position;
				velocity = Vector2.zero;
			}
		}

		public class Controller
		{
			public bool isLeft;
			public bool isRight;
		}

		public class Health
		{
			public float current;
			public float max;

			public Health()
			{
				current = 1.0f;
				max = 1.0f;
			}

			public Health(float current, float max = 1.0f)
			{
				this.current = current;
				this.max = max;
			}
		}

		public class HealthMono : MonoBehaviour
		{
			public float value;
		}

		public class BuffMono : MonoBehaviour
		{
			// ..
		}

		public class UserControl
		{
			// ..	
		}

		// [SetUp]
		// public void SomeSetupBeforeTesting()
		// {
		// }

		[Test]
		public void AntScenario_WorksCorrectly()
		{
			// Создание сценария.
			AntEngine.Add<TestScenario>();

			Assert.IsNotNull(AntEngine.Get<TestScenario>(), "TestScenario is not exists.");

			// Проверка доступа к системам из сценария.
			Assert.IsNotNull(AntEngine.Get<TestScenario>().Get<SystemA>(), "SystemA is not exists.");
			Assert.IsNotNull(AntEngine.Get<TestScenario>().Get<SystemB>(), "SystemB is not exists.");

			// Проверка корректности добавления систем.
			Assert.AreEqual(1, AntEngine.Get<TestScenario>().Get<SystemA>().addedToEngineCount, "SystemA.AddedToEngine() not called.");
			Assert.IsTrue(AntEngine.Get<TestScenario>().Get<SystemB>().isAddedToEngine, "SystemB.AddedToEngine() not called.");

			// Проверка инициализации систем.
			AntEngine.Initialize();
			Assert.AreEqual(1, AntEngine.Get<TestScenario>().Get<SystemA>().initializedCount, "SystemA.Initiazlied() not called.");

			// Проверка выполнения систем;
			AntEngine.Execute();
			Assert.AreEqual(1, AntEngine.Get<TestScenario>().Get<SystemA>().executeCount, "SystemA.Execute() not called!");
			Assert.IsTrue(AntEngine.Get<TestScenario>().Get<SystemB>().isExecuted, "SystemB.Execute() not called!");

			// Проверка фиксированного выполнения систем.
			AntEngine.ExecuteFixed();
			Assert.IsTrue(AntEngine.Get<TestScenario>().Get<SystemA>().executeFixedCount == 1, "SystemA.ExecuteFixed() not called.");

			// Проверка позднего выполнения систем.
			AntEngine.ExecuteLate();
			Assert.IsTrue(AntEngine.Get<TestScenario>().Get<SystemA>().executeLateCount == 1, "SystemA.ExecuteLate() not called.");
			
			// Проверка многочиесленных вызовов.
			for (int i = 0; i < 3; i++)
			{
				AntEngine.Execute();
				AntEngine.ExecuteFixed();
				AntEngine.ExecuteLate();
			}

			Assert.AreEqual(4, AntEngine.Get<TestScenario>().Get<SystemA>().executeCount, "SystemA.Execute() not called correctly.");
			Assert.AreEqual(4, AntEngine.Get<TestScenario>().Get<SystemA>().executeFixedCount, "SystemA.ExecuteFixed() not called correctly.");
			Assert.AreEqual(4, AntEngine.Get<TestScenario>().Get<SystemA>().executeLateCount, "SystemA.ExecuteLate() not called correctly.");

			// Проверка деинициализации систем.
			AntEngine.Deinitialize();
			Assert.IsTrue(AntEngine.Get<TestScenario>().Get<SystemA>().initializedCount == 1, "SystemA.Deinitiazlied() not called.");

			// Проверка очистки систем.
			AntEngine.Cleanup();
			Assert.IsTrue(AntEngine.Get<TestScenario>().Get<SystemA>().cleanupCount == 1, "SystemA.Cleanup() not called.");

			// Проверка удаления системы.
			AntEngine.Get<TestScenario>().Remove<SystemB>();
			Assert.IsNull(AntEngine.Get<TestScenario>().Get<SystemB>(), "SystemB is not removed.");
			Assert.IsFalse(AntEngine.Get<TestScenario>().TryGet<SystemB>(out _), "SystemB is not removed.");
			
			// Проверка добавления системы.
			AntEngine.Get<TestScenario>().Add<SystemB>();
			Assert.IsNotNull(AntEngine.Get<TestScenario>().Get<SystemB>(), "SystemB is not exists.");
			Assert.IsTrue(AntEngine.Get<TestScenario>().TryGet<SystemB>(out var sys), "SystemB is not exists.");
			Assert.IsNotNull(sys, "TryGet<SystemB>() return the null system.");
			
			// Проверка удаления сценария.
	#if FINAL_BUILD
			AntEngine.Remove<TestScenario>();
			Assert.IsNull(AntEngine.Get<TestScenario>(), "TestScenario is not removed.");
	#endif
		}

		[Test]
		public void Exclude_WorksCorrectly()
		{
			AntEngine.Add<TestScenario>();

			var player = new AntEntityBasic();
			player.Add(new Health(1.0f));
			player.Add(new UserControl());
			AntEngine.AddEntity(player);

			var enemy = new AntEntityBasic();
			enemy.Add(new Health(1.0f));
			enemy.Add(new AiControl());
			AntEngine.AddEntity(enemy);

			var box = new AntEntityBasic();
			box.Add(new Health(0.1f));
			box.Add(new BoxContainer());
			AntEngine.AddEntity(box);

			Assert.AreEqual(1, AntEngine.Get<TestScenario>().Get<HealthSystem>().playerNodes.Count);
			Assert.AreEqual(1, AntEngine.Get<TestScenario>().Get<HealthSystem>().enemyNodes.Count);
			Assert.AreEqual(3, AntEngine.Get<TestScenario>().Get<HealthSystem>().healthNodes.Count);

			box.Remove<BoxContainer>();
			box.Add<AiControl>();

			Assert.AreEqual(1, AntEngine.Get<TestScenario>().Get<HealthSystem>().playerNodes.Count);
			Assert.AreEqual(2, AntEngine.Get<TestScenario>().Get<HealthSystem>().enemyNodes.Count);
			Assert.AreEqual(3, AntEngine.Get<TestScenario>().Get<HealthSystem>().healthNodes.Count);

			player.Remove<UserControl>();
			box.Remove<AiControl>();
			box.Add<BoxContainer>();

			Assert.AreEqual(0, AntEngine.Get<TestScenario>().Get<HealthSystem>().playerNodes.Count);
			Assert.AreEqual(2, AntEngine.Get<TestScenario>().Get<HealthSystem>().enemyNodes.Count);
			Assert.AreEqual(3, AntEngine.Get<TestScenario>().Get<HealthSystem>().healthNodes.Count);

			player.Remove<Health>();
			box.Remove<Health>();
			enemy.Remove<Health>();

			Assert.AreEqual(0, AntEngine.Get<TestScenario>().Get<HealthSystem>().playerNodes.Count);
			Assert.AreEqual(0, AntEngine.Get<TestScenario>().Get<HealthSystem>().enemyNodes.Count);
			Assert.AreEqual(0, AntEngine.Get<TestScenario>().Get<HealthSystem>().healthNodes.Count);
		}

		[Test]
		public void AntEntityBasic_WorksCorrectly()
		{
			AntEngine.Add<TestGameplay>();

			// Создание сущности.
			var entity = new AntEntityBasic();
			entity.Add(new Move(Vector2.zero));
			entity.Add(new Health(1.0f));
			entity.Add(new UserControl());

			// Проверка наличия компонента.
			Assert.IsTrue(entity.Has(typeof(Health)), "Incorrect result for Entity.Has(type).");

			// Проверка отсуствия компонента.
			Assert.IsFalse(entity.Has(typeof(Controller)), "Incorrect result for Entity.Has(type).");

			// Проверка наличия компонента.
			Assert.IsTrue(entity.Has<UserControl>(), "Incorrect result for Entity.Has<T>().");

			// Проверка отсутствия компонента.
			Assert.IsFalse(entity.Has<Controller>(), "Incorrect result for Entity.Has<T>().");

			// Проверка извлечения существующего компонента.
			Assert.IsNotNull(entity.Get(typeof(Health)), "Incorrect result for Entity.Get(type).");

			// Проверка извлечения отсутствующего компонента.
			Assert.IsNull(entity.Get(typeof(Controller)), "Incorrect result for Entity.Get(type).");

			// Проверка попытки извлечь существующий компонент.
			Assert.IsTrue(entity.TryGet(typeof(UserControl), out var result1), "Incorrect result for Entity.TryGet(type).");
			Assert.IsNotNull(result1, "Incorrect result for Entity.TryGet(type).");

			// Проверка попытки извлечь отсутствующий компонент.
			Assert.IsFalse(entity.TryGet(typeof(Controller), out var result3), "Incorrect result for Entity.TryGet(type).");
			Assert.IsNull(result3, "Incorrect result for Entity.TryGet(type).");

			// Проверка добавления компонента.
			var ctrl = new Controller();
			Assert.IsNotNull(entity.Add(ctrl), "Incorrect result for Entity.Add(object).");
			Assert.IsTrue(entity.Has<Controller>(), "Incorrect result for Entity.Add(object).");

			// Проверка удаления компонента.
			Assert.IsNotNull(entity.Remove(ctrl), "Incorrect result for Entity.Remove(object).");
			Assert.IsFalse(entity.Has<Controller>(), "Incorrect result for Entity.Remove(object).");

			// Проверка добавления компонента.
			Assert.IsNotNull(entity.Add<Controller>(), "Incorrect result for Entity.Add(object).");
			Assert.IsTrue(entity.Has<Controller>(), "Incorrect result for Entity.Add(object).");

			// Проверка удаления компонента.
			Assert.IsNotNull(entity.Remove<Controller>(), "Incorrect result for Entity.Remove(object).");
			Assert.IsFalse(entity.Has<Controller>(), "Incorrect result for Entity.Remove(object).");

			// Проверка добавления сущности в движок.
			AntEngine.AddEntity(entity);
			Assert.AreEqual(1, AntEngine.Get<TestGameplay>().Get<MoveSystem>().moveNodes.Count, "Incorrect count of move nodes.");
			Assert.AreEqual(1, AntEngine.Get<TestGameplay>().Get<MoveSystem>().addEventCount, "No AddEvent for new node.");
			Assert.AreEqual(0, AntEngine.Get<TestGameplay>().Get<MoveSystem>().removeEventCount, "Got RemoveEvent for new node!?");

			// Проверка удаления компонента у сущности и изменения её положения в системе.
			entity.Remove<Move>();
			Assert.AreEqual(0, AntEngine.Get<TestGameplay>().Get<MoveSystem>().moveNodes.Count, "Incorrect count of move nodes.");
			Assert.AreEqual(1, AntEngine.Get<TestGameplay>().Get<MoveSystem>().removeEventCount, "No AddEvent for new node when component is removed.");

			// Проверка добавления компонента у сущности и изменения её положения в системе.
			entity.Add<Move>();
			Assert.AreEqual(1, AntEngine.Get<TestGameplay>().Get<MoveSystem>().moveNodes.Count, "Incorrect count of move nodes.");
			Assert.AreEqual(2, AntEngine.Get<TestGameplay>().Get<MoveSystem>().addEventCount, "No AddEvent when component for entity is added.");

			// Проверка удаления сущности.
			AntEngine.RemoveEntity(entity);
			Assert.AreEqual(0, AntEngine.Get<TestGameplay>().Get<MoveSystem>().moveNodes.Count, "Incorrect count of move nodes.");
			Assert.AreEqual(2, AntEngine.Get<TestGameplay>().Get<MoveSystem>().removeEventCount, "No RemoveEvent when component for entity is added.");

			AntEngine.Remove<TestGameplay>();
		}

		[Test]
		public void AntEntity_WorksCorrectly()
		{
			AntEngine.Add<TestGameplay>();

			// Создание сущности.
			var go = new GameObject("TestObject");
			var entity = go.AddComponent<AntEntity>();
			entity.Add(new Move(Vector2.zero));
			entity.Add(new Health(1.0f));
			entity.Add(new UserControl());
			entity.Add<HealthMono>();

			// Проверка наличия компонента.
			Assert.IsTrue(entity.Has(typeof(Health)), "Incorrect result for Entity.Has(type).");
			Assert.IsTrue(entity.Has(typeof(HealthMono)), "Incorrect result for Entity.Has(type).");

			// Проверка отсуствия компонента.
			Assert.IsFalse(entity.Has(typeof(Controller)), "Incorrect result for Entity.Has(type).");
			Assert.IsFalse(entity.Has(typeof(BuffMono)), "Incorrect result for Entity.Has(type).");

			// Проверка наличия компонента.
			Assert.IsTrue(entity.Has<UserControl>(), "Incorrect result for Entity.Has<T>().");
			Assert.IsTrue(entity.Has<HealthMono>(), "Incorrect result for Entity.Has<T>().");

			// Проверка отсутствия компонента.
			Assert.IsFalse(entity.Has<Controller>(), "Incorrect result for Entity.Has<T>().");
			Assert.IsFalse(entity.Has<BuffMono>(), "Incorrect result for Entity.Has<T>().");

			// Проверка извлечения существующего компонента.
			Assert.IsNotNull(entity.Get(typeof(Health)), "Incorrect result for Entity.Get(type).");
			Assert.IsNotNull(entity.Get(typeof(HealthMono)), "Incorrect result for Entity.Get(type).");

			// Проверка извлечения отсутствующего компонента.
			Assert.IsNull(entity.Get(typeof(Controller)), "Incorrect result for Entity.Get(type).");
			Assert.IsNull(entity.Get(typeof(BuffMono)), "Incorrect result for Entity.Get(type).");

			// Проверка попытки извлечь существующий компонент.
			Assert.IsTrue(entity.TryGet(typeof(UserControl), out var result1), "Incorrect result for Entity.TryGet(type).");
			Assert.IsNotNull(result1, "Incorrect result for Entity.TryGet(type).");

			Assert.IsTrue(entity.TryGet(typeof(HealthMono), out var result2), "Incorrect result for Entity.TryGet(type).");
			Assert.IsNotNull(result2, "Incorrect result for Entity.TryGet(type).");

			// Проверка попытки извлечь отсутствующий компонент.
			Assert.IsFalse(entity.TryGet(typeof(Controller), out var result3), "Incorrect result for Entity.TryGet(type).");
			Assert.IsNull(result3, "Incorrect result for Entity.TryGet(type).");

			Assert.IsFalse(entity.TryGet(typeof(BuffMono), out var result4), "Incorrect result for Entity.TryGet(type).");
			Assert.IsNull(result4, "Incorrect result for Entity.TryGet(type).");

			// Проверка добавления компонента.
			var ctrl = new Controller();
			Assert.IsNotNull(entity.Add(ctrl), "Incorrect result for Entity.Add(object).");
			Assert.IsTrue(entity.Has<Controller>(), "Incorrect result for Entity.Add(object).");

			// Проверка удаления компонента.
			Assert.IsNotNull(entity.Remove(ctrl), "Incorrect result for Entity.Remove(object).");
			Assert.IsFalse(entity.Has<Controller>(), "Incorrect result for Entity.Remove(object).");

			// Проверка добавления компонента.
			Assert.IsNotNull(entity.Add<Controller>(), "Incorrect result for Entity.Add(object).");
			Assert.IsTrue(entity.Has<Controller>(), "Incorrect result for Entity.Add(object).");

			// Проверка удаления компонента.
			Assert.IsNotNull(entity.Remove<Controller>(), "Incorrect result for Entity.Remove(object).");
			Assert.IsFalse(entity.Has<Controller>(), "Incorrect result for Entity.Remove(object).");

			// Проверка добавления сущности в движок.
			AntEngine.AddEntity(entity);
			Assert.AreEqual(1, AntEngine.Get<TestGameplay>().Get<MoveSystem>().moveNodes.Count, "Incorrect count of move nodes.");
			Assert.AreEqual(1, AntEngine.Get<TestGameplay>().Get<MoveSystem>().addEventCount, "No AddEvent for new node.");
			Assert.AreEqual(0, AntEngine.Get<TestGameplay>().Get<MoveSystem>().removeEventCount, "Got RemoveEvent for new node!?");

			// Проверка удаления компонента у сущности и изменения её положения в системе.
			entity.Remove<Move>();
			Assert.AreEqual(0, AntEngine.Get<TestGameplay>().Get<MoveSystem>().moveNodes.Count, "Incorrect count of move nodes.");
			Assert.AreEqual(1, AntEngine.Get<TestGameplay>().Get<MoveSystem>().removeEventCount, "No AddEvent for new node when component is removed.");

			// Проверка добавления компонента у сущности и изменения её положения в системе.
			entity.Add<Move>();
			Assert.AreEqual(1, AntEngine.Get<TestGameplay>().Get<MoveSystem>().moveNodes.Count, "Incorrect count of move nodes.");
			Assert.AreEqual(2, AntEngine.Get<TestGameplay>().Get<MoveSystem>().addEventCount, "No AddEvent when component for entity is added.");

			// Проверка удаления сущности.
			AntEngine.RemoveEntity(entity);
			Assert.AreEqual(0, AntEngine.Get<TestGameplay>().Get<MoveSystem>().moveNodes.Count, "Incorrect count of move nodes.");
			Assert.AreEqual(2, AntEngine.Get<TestGameplay>().Get<MoveSystem>().removeEventCount, "No RemoveEvent when component for entity is added.");

			GameObject.DestroyImmediate(entity.gameObject);
			AntEngine.Remove<TestGameplay>();
		}

		// [Test]
		// public void StressTest()
		// {
		// 	// Add remove AntEntityBasic.
		// 	var sw = new Stopwatch();
		// 	sw.Reset();
		// 	sw.Start();

		// 	var basicEntity = new AntEntityBasic();
		// 	for (int i = 0; i < 1000; i++)
		// 	{
		// 		basicEntity.Add<Health>();
		// 		basicEntity.Remove<Health>();
		// 	}
		// 	sw.Stop();
		// 	A.Log($"AntEntityBasic Add/Remove component 1000 times: {sw.Elapsed.TotalMilliseconds}ms");
			
		// 	// Add remove AntEntity.
		// 	sw.Reset();
		// 	sw.Start();

		// 	var go = new GameObject("TestObject");
		// 	var monoEntity = go.AddComponent<AntEntity>();
		// 	for (int i = 0; i < 1000; i++)
		// 	{
		// 		monoEntity.Add<HealthMono>();
		// 		monoEntity.Remove<HealthMono>();
		// 	}
		// 	sw.Stop();
		// 	A.Log($"AntEntity Add/Remove component 1000 times: {sw.Elapsed.TotalMilliseconds}ms");

		// 	GameObject.DestroyImmediate(go);
		// }
	}
}