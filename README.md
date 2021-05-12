# Anthill ECS Framework

Возможности:

* Подходит для разработки любого рода игр на Unity;
* Совместим с Unity, работает с MonoBehaviour;
* Совместим с любыми фреймворками;
* Простой синтаксис и архитектура;
* Не требует кода-генерации и какого либо C# Reflection в рантайме;
* Работает в одном потоке;
* Полный исходный код.

---

## Содержание

* 📦 [Установка](#-установка)
* 🔖 [Введение](#-введение)
	* 🎯 [Базовая концепция ECS](#-базовая-концепция-ecs)
	* 🚀 [Быстрый старт](#-быстрый-старт)
	* 🌼 [Типы систем](#-типы-систем)
	* 🗿 [Отладка](#-отладка)
	* ⏲ [Отложенные вызовы](#-отложенные-вызовы)
	* 🍀 [Внедрение зависимостей](#-внедрение-зависимостей)
* 📝 [Лицензия](#лицензия)
* 💬 [Контакты](#контакты) 

---

## 📦 Установка

Откройте Package Manager и добавьте URL:

* https://github.com/AntKarlov/Anthill-ECS-Framework.git

Либо скачайте актуальный Package и импортируйте его в свой проект:

* https://github.com/AntKarlov/Anthill-ECS-Framework/tree/main/Assets/Packages

---

## 🔖 Введение

Anthill — это коллекция различных решений для разных задач, где каждый класс это муравей (Ant) способный делать что-то 
полезное.

В состав Anthill входит много интересных вещей разработанных по ходу создания игр с Unity. К сожалению, данная 
инструкция не рассказывает о всех возможностях и содержит лишь базовую информацию об ECS.

---

### 🎯 Базовая концепция ECS

Entity Component System — это сущности состоящих из набора компонентов логика которых определяется системами. Где под 
каждой сущностью скрывается любой игровой объект, например персонаж, монстр или монеты с которыми может 
взаимодействовать игрок. 


**Entity** 

Сущность является связующим звеном для всех компонентов. Обращаясь к сущности вы можете создавать, удалять или 
проверять наличие тех или иных компонентов.

Чтобы создать любую игровую сущность в Anthill, вам достаточно на любой игровой объект добавить компонент AntEntity 
через меню `Add Component` в инспекторе объекта.


**Component** 

Компонент в Anthill — это `MonoBehaviour` который может содержать любые данные об игровом объекте, например запас 
здоровья или содержимое инвентаря.

Чтобы создать компонент, создайте любой класс и унаследуйте его от `MonoBehaviour`:

```c#
using Anthill.Core;

public class Health : MonoBehaviour
{
	public float health = 1.0f;
}
```

Обычно компоненты содержат только данные и не должны иметь какой-либо игровой логики. Но совокупность компонентов может 
определять логику поведения объекта исходя из ваших задач.

При использовании ECS не рекомендуется реализовывать обмен данными через события. Если вам необходимо реализовать обмен
данными между компонентами, используйте семафоры, например, через логические переменные.


**Node**

Чтобы распределить сущности по системам, то есть определить какая сущность в какой системе должна обрабатываться, 
следует использовать Node (узел или их еще называют фильтрами). Узел позволяет указать какие компоненты должны быть у 
сущности, чтобы эта сущность попала в определенный узел.

Пример простого узла:

```c#
using Anthill.Core;

public class HealthNode : AntNode
{
	public HealthNode Health { get; set; }
}
```

Пример узла приведенный выше, будет представлять все сущности которые имеют компонент AntEntity и Health. Узел — это 
безразличное представление игрового объекта в рамках одной или нескольких систем. То есть, нам не важно, что это за 
объект и что он делает, нам важно какие компоненты у него есть чтобы обработать его в рамках одной системы.


**System**

Системы обычно содержат логику и являются основными процессорами для всего что присходит в игре. Системы не имеют 
прямого отношения к движку Unity и их функционал определяется через интерфейсы.

```c#
using Anthill.Core;

public class HealthSystem : ISystem, IExecuteSystem
{
	private AntNodeList<HealthNode> _healthNodes;

#region ISystem Implementation

	public void AddedToEngine()
	{
		_healthNodes = AntEngine.GetNodes<HealthNode>();
	}

	public void RemovedFromEngine()
	{
		_healthNodes = null;
	}

#endregion

#region IExecuteSystem Implementation

	public void Execute()
	{
		HealthNode node;
		for (i = _healthNodes.Count - 1; i >= 0; i--)
		{
			node = _healthNodes[i];
			node.Health.health -= 0.1f * Time.deltaTime;
			if (node.Health.health <= 0.0f)
			{
				AntEngine.RemoveEntity(node.Entity);
			}
		}
	}

#endregion
}
```

---

### 🚀 Быстрый старт

Рассмотрим простой пример интеграции Anthill ECS в Unity. Но прежде рассмотрим возможную иерархию папок для скриптов


**Иерархия проекта**

Рекомендуется придерживаться следующей иерархии проекта в рамках использования Anthill:

* Assets
	* Scripts
		* Core — содержит различные игровые менеджеры
		* Components — содержит все компоненты
		* Nodes — содержит все ноды
		* Systems — содержит все системы.

Вы можете свободно использовать подпапки, если у вас много систем, компонентов или узлов.


**Создание сущности**

1. Создайте новый файл в папке `Scripts/Components` и назовите его **Health.cs**
2. Вставьте в новый файл следующий код:

```c#
using UnityEngine;

public class Health : MonoBehaviour
{
	public float health = 1.0f;
}
```

3. Создайте на сцене новый GameObject и назовите его `world_root`.
4. Внутри `world_root` создайте еще один пустой GameObject и назовите его `player`.
5. Прикрепите к объекту `player` компоненты: AntEntity и Health.
6. Сущность готова.


**Создание ноды**

1. Создайте новый файл в папке `Scripts/Nodes` и назовите его **HealthNode.cs**
2. Вставьте в него код приведенный ниже:

```c#
using Anthill.Core;

public class HealthNode : AntNode
{
	public Health Health { get; set; }
}
```

3. Нода готова. Все сущности имеющие компонент AntEntity и Health будут представлены через HealthNode.


**Создание системы**

1. Создайте новый файл в папке `Scripts/Systems` и назовите его **HealthSystem.cs**
2. Вставьте в него код приведенный ниже:

```c#
using Anthill.Core;

public class HealthSystem : ISystem, IExecuteSystem
{
	private AntNodeList<HealthNode> _healthNodes;

	public void AddedToEngine()
	{
		_healthNodes = AntEngine.GetNodes<HealthNode>();
		_healthNodes.EventNodeAdded += HealthNodeAddedHandler;
		_healthNodes.EventNodeRemoved += HealthNodeRemovedHandler;
	}

	public void RemovedFromEngine()
	{
		_healthNodes = null;
	}

	public void Execute()
	{
		HealthNode node;
		for (i = _healthNodes.Count - 1; i >= 0; i--)
		{
			node = _healthNodes[i];
			node.Health.health -= 0.1f * Time.deltaTime;
			if (node.Health.health <= 0.0f)
			{
				AntEngine.RemoveEntity(node.Entity);
			}
		}
	}

	public void HealthNodeAddedHandler(HealthNode aNode)
	{
		Debug.Log($"Added `{aNode.Entity.gameObject.name}` node!");
	}

	public void HealthNodeRemovedHandler(HealthNode aNode)
	{
		Debug.Log($"Rmoved `{aNode.Entity.gameObject.name}` node!");
	}
}
```

3. Система обрабатывающая все сущности с компонентами AntEntity и Health готова.


**Scenario**

Anthill ECS имеет реализацию сценариев. Сценарий — это что-то вроде папки для разных систем в рамках общей логики. 
Технически, работа сценариев никак не влияет на ECS паттерн. Но при разработке сложных проектов, объединение разных 
систем по какому-либо признаку (на ваше усмотрение) в общие сценарии — упрощает развитие и поддержку игровых проектов.

Например, если вы разрабатываете игру в жанре GTA где игрок управляет персонажем, но может сесть в авто, мотоцикл, 
лодку или самолет — все это приводит к тому, что при смене транспортного средства вам необходимо будет выключить один 
десяток систем и включить другой десяток систем чтобы изменить логику поведения игрока в игре. А если мы заранее 
объединим все системы отвечающие за управление персонажем, например в InfantryScenario, а все системы отвечающие за 
управление автомобилем в VehicleScenario, то в будущем, при активации автомобиля нам достаточно будет удалить 
InfantryScenario и добавить VehicleScenario. Кроме этого, при дальнейшем развитии игры, если мы добавили или удалили 
новые системы в рамках сценария, нам не нужно будет переписывать код переключения логики игры, так как она оперирует 
сценариями.

Давайте создадим сценарий:

1. Создайте в папке `Scripts/Systems` новый файл и назовите его **Gameplay.cs** — это будет GameplayScenario.
2. Добавьте в него следующий код:

```c#
using Anthill.Core;

public class Gameplay : AntScenario
{
	public Gameplay() : base("Gameplay")
	{
		// ...
	}

	public override void AddedToEngine()
	{
		base.AddedToEngine();
		Add<HealthSystem>();
		// .. добавляйте здесь любые системы в рамках геймплея
	}

	public override void RemovedFromEngine()
	{
		Remove<HealthSystem>();
		base.RemovedFromEngine();
	}
}
```

3. Сценарий готов! Вы можете создавать любое количество сценариев и систем в рамках одного сценария. Помните, 
сценарии — это лишь способ держать вещи в порядке.


**AntEngine**

Кажется мы создали много интересных вещей и теперь настало время все это собрать воедино. Доступ к Anthill ECS 
осуществляется через статический класс **AntEngine**, так же через этот класс происходит и процессинг всех систем. 
Но чтобы запустить движок в работу, нам нужна некоторая точка входа в игру.

1. Создайте в папке `Scripts/Core` новый файл и назовите его **Game.cs**
2. Добавьте в него следующий код:

```c#
using UnityEngine;
using Anthill.Core;

public enum Priority
{
	Gameplay = 0
}

public class Game : AntAbstractBootstrapper
{
#region AntAbstractBootstrapper Implementation
		
	public override void Configure(IInjectContainer aContainer)
	{
		aContainer.RegisterSingleton<Game>(this);
		// .. конфигурация DI Container.
	}
		
#endregion

#region Unity Calls
		
	private void Start()
	{
		InitializeSystems();
		
		// Добавляем все сущности в игровой движок.
		AntEngine.AddEntitiesFromHierarchy("WorldRoot");
	}

	private void Update()
	{
		AntEngine.Execute();
	}

	private void FixedUpdate()
	{
		AntEngine.ExecuteFixed();
	}
		
#endregion

#region Private Methods
		
	private void InitializeSystems()
	{
		Engine.Add<Gameplay>(Priority.Gameplay);
		// .. инициализация других систем
	}

#endregion
}
```

3. Создайте на сцене новый GameObject и назовите его `game`. 
4. Прикрепите к объекту `game` скрипт **Game.cs**
5. Теперь можно запустить проект и проверить его работу.

Если все сделано правильно, то в консоли вы увидите сообщения:

```
Added `player` node.
Removed `player` node.
```

Обратите внимание, что после удаления `player` ноды, объект `player` останется в иерархии сцены. Это означает лишь то, 
что мы удалили игровую сущность только из движка когда у нее закончилось здоровье, но сам игровой объект (если в этом 
есть необходимость) вы должны удалить классическим образом, например так:

```c#
GameObject.Destroy(node.Entity.gameObject); 
```

Важно помнить: создание и удаление игровых объектов — это самая дорогая операция, поэтому рекомендуется переиспользовать
все что только можно. Например, помещайте объекты в пул объектов когда они не нужны и извлекайте их от туда когда они 
необходимы вновь.


**Добавление сущностей**

Чтобы добавить новые сущности в игровой движок, например, после создания монстра из префаба, вам достаточно лишь 
получить компонент AntEntity с созданного игрового объекта и добавить его в AntEngine следующим образом:

```c#
var go = Instantiate(prefabRef);
var entity = go.GetComponent<AntEntity>();
if (entity != null)
{
	AntEngine.AddEntity(entity);
}
```

После добавления сущности, она будет распределена по спискам нод автоматически согласно наличию компонентов. Обновятся
даже те списки нод которые вы уже получили, так как при извлечении списка нод через AntEngine.GetNodes<T>() — вы 
получаете лишь указатель на список нод.

Если вам нужно добавить сразу много сущностей, например, после аддитивной загрузки сцены, то вы можете воспользоваться 
следующим способом:

```c#
AntEngine.AddEntitiesFromHierarchy("NameOfParentObject");
```

или

```c#
AntEngine.AddEntitiesFromHierarch(parentTransform);
```

При таком способе добавления будет произведен поиск сущностей внутри указанного Transform и каждый найденный AntEntity 
будет добавлен в AntEngine.


**Добавление и удаление компонентов**

Работая с сущностями, вы можете добавлять или удалять компоненты для них, тем самым меняя логику поведения сущности. 
Например, если вы хотите сделать персонажа бессметрным, чтобы он был исключен из системы обработки здоровья, то 
достаточно просто удалить компонент Health:

```c#
node.Entity.Remove<Health>();
```

И вернуть его обратно когда персонаж должен стать вновь уязвимым:

```c#
node.Entity.Add<Health>();
```

Кроме этого, вы можете подписаться на события добавления или удаления компонентов для сущностей используя события:

```c#
node.Entity.EventComponentAdded += AddedComponentHandler;
node.Entity.EventComponentRemoved += RemovedComponentHandler;

private void AddedComponentHandler(AntEntity aEntity, Type aComponentType)
{
	Debug.Log("Component added!");
}

private void RemovedComponentHandler(AntEntity aEntity, Type aComponentType)
{
	Debug.Log("Component removed!");
}
```

При удалении или добавлении компонентов через методы AntEntity, распределение сущностей внутри списков нод изменяется.

---

### 🌼 Типы систем

Anthill ECS поддерживает несколько разновидностей систем. Каждая разновидность системы определяется наличием 
интерфейсов. Любая система может поддерживать сразу все, несколько или только один тип.

* `ICleanupSystem` — должна реализовать Cleanup() метод.
* `IDeinitializeSystem` — должна реализовать Deinitialize() метод.
* `IDisableSystem` — должна реализовать Disable() метод.
* `IEnableSystem` — должна реализовать Enable() метод.
* `IExecuteFixedSystem` — должна реализовать ExecuteFixed() метод.
* `IExecuteSystem` — должна реализовать Execute() метод.
* `IInitializeSystem` — должна реализовать Initialize() метод.
* `IResetSystem` — должна реализовать Reset() метод.

Когда мы рассматривали базовый пример, то вы должны были обратить внимание на то что в `Update()` для класса Game мы 
вызываем `AntEngine.Execute()` и в методе `FixedUpdate()` для класса Game мы вызываем `AntEngine.ExecuteFixed()`. Точно 
так же вы можете вызывать и любые другие типы систем, тогда когда вы считаете это нужным.

Например, перед тем как текущий уровень будет выгружен, мы можем вызывать метод `Cleanup()` для всех систем которые 
имплементируют интерфейс `ICleanupSystem`:

```c#
AntEngine.Cleanup();
```

Когда нам нужно что-то инициализировать или деинициализировать, то мы можем вызвать соотвествующие системы:

```c#
// Уровень загрузился.
AntEngine.Initialize();

// Или сейчас уровень будет выгружен, деинициализация.
AntEngine.Deinitialize();
```

Аналогично и с системами Disable и Enable:

```c#
// Вызвать метод Enable() для всех IEnableSystem.
AntEngine.Enable();

// Вызвать метод Disable() для всех IDisableSystem.
AntEngine.Disable();
```

Или метод сбросить:

```c#
AntEngine.Reset();
```

Внутри AntEngine нет никакой логики для вызова этих методов систем — это лишь заготовка для вашего удобства, вы сами 
определяете что и когда будет работать.

Любая система может поддерживать сразу несколько типов, если это необходимо:

```c#
public class SomeSystem : ISystem, IResetSystem, IExecuteSystem, ICleanupSystem
{
#region ISystem Implementation

	public void AddedToEngine() { .. }
	public void RemovedFromEngine() { .. }

#endregion

#region IResetSystem Implementation

	public void Reset() { .. }

#endregion

#region IExecuteSystem Implementation

	public void Execute() { .. }

#endregion

#region ICleanupSystem Implementation

	public void Cleanup() { .. }

#endregion
}
```

---

### 🗿 Отладка 

При запуске проекта в Unity вы можете видеть динамически создаваемый объект `Systems` в корне активной сцены. Данный
объект является отладочным инструментом демонстрирующий какие системы существуют в данный момент времени и что они 
делают, а так же сколько времени и ресурсов потребляют. Обращайте внимание на то сколько требуется времени на обработку
каждой системы и соотвествует ли это вашим ожиданиям.

Объект `Systems` создается только при запуске проекта в редакторе и отсуствует в финальных билдах. Вы можете отключить
отладочный режим для Anthill перейдя в Player Settings и добавив символ `ANTHILL_DISABLE_DEBUG` в Scripting Define 
Symbols конкретной платформы.


---

### ⏲ Отложенные вызовы 

При разработке игр очень часто возникает необходимость выполнить какое-то действие с определенной задержкой. Для
реализации этой задачи в Anthill существует специальный класс AntDelayed который позволяет создавать отложенные вызовы
на базе ECS.

Чтобы создать отложенный вызов, достаточно использовать следующий код:

```c#
AntDelayed.Call(0.5f, () => Debug.Log("Call!"));
```

При создании отложенного вызова, вы можете сохранить на него ссылку, например, чтобы его уничтожить до того как он будет
вызван, если в этом будет необходимость:

```c#
var call = AntDelayed.Call(1.5f, () => Debug.Log("Call!"));
call.Kill();
```

Если вам необходимо чтобы отложенные вызовы работали при timeScale == 0.0f, используйте следующий пример:

```c#
AntDelayed.Call(1.5f, () => Debug.Log("Call!")).SetUpdate(true);
```

Для реализации отложенных вызовов используется встроенная система в ECS, при создании первого отложенного вызова в
отладочном режиме вы увидете как будет создана новая система которая будет следить за всеми отложенными вызовами.

---

### 🍀 Внедрение зависимостей 

Anthill имеет встроенную реализацию паттерна Dependency Injection - данный паттерн является заменой Singleton и решает 
всего его недостатки. Dependency Injection представляет собой «облачное хранилище» некоторых классов или объектов 
к которым необходимо получать доступ из любой точки игры. 

Пример инициализации DI:

```c#
public class Game : AntAbstractBootstrapper
{
	public override void Configure(IInjectContainer aContainer)
	{
		aContainer.RegisterSingleton(new MyGameEngine());
		aContainer.RegisterSingleton(GetComponent<TextLoader>());
		aContainer.RegisterSingleton(new LevelManager());
	}

	private void Start()
	{
		// ...
	}
}
```

Game — это скрипт который прикрепляется на основной игровой объект, в нем регистрируются все «условные» синглтоны. 
Такой подход сразу позволяет решить несколько известных проблем паттерна Singleton:

* Явно контролируется порядок инициализации синглтонов;
* Лгику синглтона вынесена из классов, что упрощает их переиспользование и дает возможность создавать 
  их дубликаты, например при Unit-тестировании.

Чтобы получить доступ к любому классу из DI контейнера, немобходимо сделать иньекцию:

```c#
public class SomeClass : MonoBehaviour
{
	[Inject] public LevelManager LevelManager { get; set; }

	private void Start()
	{
		AntInject.Inject<SomeClass>(this);
		LevelManager.LoadLevel("Level1");
	}
}
```

Атрибут `[Inject]` — отмечает сеттеры в которые необходимо поместить какие-либо референсы из глобального хранилища. 
Когда вызывается метод `Inject()` — то для переданного класса проверяются все сеттеры с атрибуттом `[Inject]`, 
и в каждый из них будет помещена ссылка на класс из глобального хранилища DI.


---

## 📝 Лицензия

[MIT License](license)

---

## 💬 Контакты

Telegram: [AntKarlov](https://t.me/AntKarlov)
E-mail: [ant.anthill@gmail.com](ant.anthill@gmail.com)