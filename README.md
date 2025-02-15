# Anthill Entity-Component-System

Плюсы использования:

* Подходит для разработки любых игр на Unity;
* В качестве компонентов использует простые классы и `MonoBehaviour`;
* Возможность использования в качестве компонентов любые стандартные компоненты Unity;
* Совместим с любыми фреймворками и ассетами;
* Простой синтаксис и архитектура;
* Не требует кода-генерации и C# Reflection в рантайме;
* Работает в одном потоке;
* Полный исходный код.

---

## Содержание

* 📦 [Установка](#-установка)
* 🔖 [Введение](#-введение)
    * 🎯 [Концепция](#-концепция)
    * 🐌 [Производительность](#-производительность)
    * 🚀 [Быстрый старт](#-быстрый-старт)
    * 🌿 [Доступ к системам и сценариям](#-доступ-к-системам-и-сценариям)
    * 🌼 [Типы систем](#-типы-систем)
    * 🗿 [Отладка](#-отладка)
    * ⏱️ [Отложенные вызовы](#-отложенные-вызовы)
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

**Anthill** — это коллекция решений для разных задач, где каждый класс это муравей (Ant) способный делать что-то полезное.

В состав Anthill входит много интересных инструментов разработанных исходя из собственных нужд за время работы с Unity. К сожалению, данная инструкция не рассказывает о всех особенностях и возможностях данного набора и содержит базовую информацию об Entity-Component-System.

---

### 🎯 Концепция

**Entity Component System** — это архитектура построенная на **сущностях**, логику которых определяет наличие **компонентов**, а обработка сущностей осуществляется через **системы**. Где сущность — это любой игровой объект, например персонаж, который имеет компоненты: здоровье, движение, инвентарь и пользовательский ввод. Исходя из определенного набора компонентов, сущность автоматически распределяется в системы которые обрабатывают игровую логику работая с компонентами сущности.


**Entity** 

Сущность — это обезличенный игровой объект. Сущность является связующим звеном для игровых объектов и логикой их работы. Обращаясь к сущности вы можете создавать, удалять компоненты и изменять данные компонентов. Например, изменять значение здоровья в компоненте здоровья если сущность получила урон.

Чтобы создать любую игровую сущность в Unity, достаточно на любой игровой объект добавить компонент `AntEntity` через меню `Add Component` в инспекторе объекта.


**Component** 

Компонент в Anthill — это `MonoBehaviour` или простой класс, который может содержать любые данные об игровом объекте, например запас здоровья или содержимое инвентаря. Компоненты созданные на базе `MonoBehaviour` вы можете добавлять к сущностям в режиме редактора и настраивать их параметры, в то время как простые классы рекомендуется использовать для быстрой передачи информации между системами - «посыльные с данными».

Чтобы создать компонент, создайте любой класс и унаследуйте его от `MonoBehaviour`:

```c#
using Anthill.Core;

public class Health : MonoBehaviour
{
    public float health = 1.0f;
}
```

Если следовать регламенту Entity-Component-System архитектуры, то компоненты должны содержать только данные и не должны иметь какой-либо игровой логики. Совокупность компонентов добавленных к игровому объекту (сущности) должна определять логику поведения объекта. Например, наличие компонента Здоровья на любом игровом объекте подразумевает, что объект может получать урон и погибнуть если здоровье достигнет критического уровня.

> ☝🏻 **Важно:** при использовании данного подхода не рекомендуется реализовывать обмен данными через события. Если вам необходимо реализовать обмен данными между игровыми объектами, то вам следует использовать подход семафоров и передавать данные через переменные или структуры внутри компонентов.

> 🍀 **Совет:** вы можете реализовывать логику внутри компонентов, но только для удобства работы с данными компонента, например, при изменении компонента здоровья, вы можете реализовать геттер/сеттер для получения и установки значения здоровья который не позволит сделать здоровье больше указанного максимального значения.


**Node**

Узел — используется чтобы распределять сущности по системам. То есть, узел — это фильтр который позволяет передавать сущности в системы если они соотвествуют правилам наличия или отсутствия компонентов. Через узел мы можем задать набор компонентов который должен быть у сущности чтобы она стала частью этого узла.

Пример узла:

```c#
using Anthill.Core;

public class HealthNode : AntNode
{
    public Health Health { get; set; }
}
```

Обычно из узлов формируется список сущностей которые соотвествуют правилу узла. Например, через данный узел можно получить список всех сущностей у которых есть компонент `Health` и выполнять какие-либо операции с ними. Как пример использование данного узла, вы можете написать систему, которая следит за состоянием здоровья объектов, проверяет его запас и если объект исчерпал здоровье - уничтожит его (или вернет в пул объектов).

Так же мы можем указать, каких компонентов не должно быть у сущности, чтобы она попала в определнный узел. Например, изменив узел следующим образом мы можем исключить попадания сущности в систему следящую за здоровьем если у нее будет компонент `Immortal` (бессмертие):

```c#
using Anthill.Core;

[Exclude(typeof(Immortal))]
public class HealthNode : AntNode
{
    public Health Health { get; set; }
}
```

Аттрибут `Exclude` позволяет задать один или несколько (максимум шесть) компонентов, которых при этом не должно быть у сущности чтобы она соответствовала правилам попадания в данный узел. Таким образом, добавив на лету любой сущности компонент `Immortal` мы сделаем её неуязвимой, не удаляя при этом компонент `Health`, чтобы сохранить данные о текущем здоровье.


**System**

Системы содержат логику и являются основными процессорами для обработки игровых сущностей и их компонентов. Системы не имеют прямого отношения к движку Unity и их функционал определяется через интерфейсы. Рассмотрим пару: `ISystem` и `IExecuteSystem`.

```c#
using Anthill.Core;

public class HealthSystem : ISystem, IExecuteSystem
{
    // Список узлов для сущностей у которых есть компонент здоровья.
    private AntNodeList<HealthNode> _healthNodes;

    public void AddedToEngine()
    {
        // Получаем список узлов при добавлении системы.
        _healthNodes = AntEngine.GetNodes<HealthNode>();
    }

    public void RemovedFromEngine()
    {
        // Освобождаем указатель на список узлов при удалении системы.
        _healthNodes = null;
    }

    public void Execute()
    {
        // Каждый Update() перебираем все компоненты здоровья и проверяем его.
        HealthNode node;
        for (int i = _healthNodes.Count; i >= 0; i--)
        {
            node = _healthNodes;
            if (node.Health.value <= 0.0f)
            {
                // Если здоровье кончилось, то сущность удаляем.
                AntEngine.RemoveEntity(node.Entity);

                // Отдельно удаляем геймобжект со сцены.
                GameObject.Destroy(node.Entity.gameObject);
            }
        }
    }
}
```

---

### 🐌 Производительность

Большинство известных **Entity-Component-System** фреймворков ориентированны на высокую производительность при оперировании большим количеством сущностей. Высокая производительность — это всегда плюс и бонус к запасу прочности и надежности игрового проекта. Но, как правило, в подавляющем большинстве игровых проектов нам не требуется десятки тысяч одновременно обновляемых игровых объектов (сущностей) и все игровые процессы чаще всего ограничиваются оперированием не более чем одной или двумя сотнями игровых сущностей одновременно. Но, ставя упор на производительность, большинство Entity-Component-System фреймворков жертвуют удобством их использования: сложнее собирать и настраивать игровые объекты, а в некоторых случаях и вовсе богатые возможности редактора Unity приходится использовать исключительно в качестве компилятора и упаковщика ресурсов.

При разработке **Anthill Entity-Component-System** упор ставился в первую очередь на простой и понятный API, а так же на удобство использование редактора Unity. Задача была сделать так, чтобы можно было использовать все возможности ECS архитектуры и совмещать эту архитектуру со стандартными подходами в разработке Unity. Да, фреймворк не может похвастаться быстрым оперированием десятками тысяч сущностей одновременно, но нам это и не нужно. Производительность **Anthill Entity-Component-System** точно такая же как и обычный подход при разработке проектов на Unity. То есть, фреймворк не дает рост производительности при большом количестве игровых объектов, но дает все архитектурные возможности ECS и удобство использования редактора Unity при разработке проектов.

Вы можете свободно создавать и настраивать игровые объекты, сохранять их в префабы и добавлять их в игровой движок привычным вам способом без необходимости создавать дополнительные «фасады» чтобы подружить стандартный подход разработки в Unity с ECS подходом при использовании большинства известных ECS фреймворков.

Но, важно помнить, что добавление и удаление компонентов унаследованных от `MonoBehaviour` требует чуть больше ресурсов чем обычных компонентов на базе простого `class`. Поэтому, если вам не требуется настраивать данные компонентов в `Unity Inspector` и вы хотите их активно создавать и удалять во время игрового процесса, то старайтесь их делать обычными классами.

Наши тесты показали, что на 1000 добавлений и одновременного удаления компонентов:

* Для `MonoBehaviour` компонентов потребуется примерно ~36ms времени;
* Для `class` компонентов потребуется примерно ~6ms времени;

В случае с обработкой систем и сущностей, вся производительность будет точно такая же как и для обновления игровых объектов стандартными спобосами Unity. То есть, __прироста производительности нет__, а потери производительности будут зависить только от того какой код вы напишите внутри своих систем — медленный или быстрый.

Из собственного опыта можно сделать вывод, что фремворк работает быстро и никаких проблем с производительностью не возникает. На практике, чаще всего проблемы производительности при использовании **Anthill Entity-Component-System** — возникали непосредственно в коде игровой логики или не оптимизированном рендере.

> ☝🏻 **Важно:** данный фреймворк не дает никаких бонусов производительности для ваших проектов и то как быстро будет работать ваш проект — зависит только от вашего кода и оптимизации графики и ресурсов, как при стандартном подходе разработки игры в Unity.

---

### 🚀 Быстрый старт

Рассмотрим простой пример интеграции **Anthill Entity-Component-System** в Unity.


**Иерархия проекта**

Рекомендуется придерживаться следующей иерархии проекта в рамках использования Anthill:

* **Assets**
    * **Scripts**
        * **Core** — содержит различные игровые менеджеры
        * **Components** — содержит компоненты
        * **Nodes** — содержит узлы
        * **Systems** — содержит системы

Вы можете свободно использовать подпапки, если у вас много разнообразных систем, компонентов и узлов.


**Создание сущности**

1️⃣ Создайте скрипт в папке `Scripts/Components` и назовите его **Health.cs**
2️⃣ Вставьте в скрипт следующий код:

```c#
using UnityEngine;

public class Health : MonoBehaviour
{
    public float health = 1.0f;
}
```

3️⃣ Создайте на сцене новый GameObject и назовите его `world_root`.
4️⃣ Внутри `world_root` создайте еще один пустой GameObject и назовите его `player`.
5️⃣ Прикрепите к объекту `player` компоненты: `AntEntity` и `Health`.
✅ Сущность готова!


**Создание узла**

1️⃣ Создайте новый скрипт в папке `Scripts/Nodes` и назовите его **HealthNode.cs**
2️⃣ Вставьте в скрипт следующий код:

```c#
using Anthill.Core;

public class HealthNode : AntNode
{
    public Health Health { get; set; }
}
```

✅ Узел готов!

Теперь все сущности имеющие компонент `AntEntity` и `Health` будут представлены через узел `HealthNode`.


**Создание системы**

1️⃣ Создайте новый скрипт в папке `Scripts/Systems` и назовите его **HealthSystem.cs**
2️⃣ Вставьте в скрипт следующий код:

```c#
using UnityEngine;
using Anthill.Core;

public class HealthSystem : ISystem, IExecuteSystem
{
    // Список всех сущностей отфлитрованных через правило HealthNode у которых есть компонент здоровья.
    private AntNodeList<HealthNode> _healthNodes;

    public void AddedToEngine()
    {
        // Получаем список узлов при добавлении системы.
        _healthNodes = AntEngine.GetNodes<HealthNode>();

        // Так мы можем узнать когда в список узлов добавилась или удалилась сущность после его инициализации.
        _healthNodes.EventNodeAdded += HealthNodeAddedHandler;
        _healthNodes.EventNodeRemoved += HealthNodeRemovedHandler;
    }

    public void RemovedFromEngine()
    {
        // Освобождаем указатель на список узлов если система была удалена.
        _healthNodes = null;
    }

    public void Execute()
    {
        // Перебираем список узлов обрабатывая все сущности у которых есть компонент здоровья.
        HealthNode node;
        for (int i = _healthNodes.Count - 1; i >= 0; i--)
        {
            node = _healthNodes[i];
            // Убавляем здоровье.
            node.Health.health -= 0.1f * Time.deltaTime;

            if (node.Health.health <= 0.0f)
            {
                // Если здоровье кончилось, удаляем сущность.
                AntEngine.RemoveEntity(node.Entity);

                // Отдельно нужно уничтожить игровой объект (или вернуть его в пул объекто ;).
                GameObject.Destroy(node.Entity.gameObject);
            }
        }
    }

    public void HealthNodeAddedHandler(HealthNode aNode)
    {
        // Выполнится когда новая сущность (игровой объект) был добавлен в движок.
        Debug.Log($"Added `{aNode.Entity.gameObject.name}`!");
    }

    public void HealthNodeRemovedHandler(HealthNode aNode)
    {
        // Выполнится когда сущность (игровой объект) был удален из движка.
        Debug.Log($"Removed `{aNode.Entity.gameObject.name}`!");
    }
}
```

✅ Система обрабатывающая все сущности с компонентами `AntEntity` и `Health` готова!


**Scenario**

**Сценарий** — это это группа систем. Сценарии удобно использовать при разработки больших и сложных проектов группируя разные системы в сценарии по какому-либо признаку (на ваше усмотрение). Такой подход упращает развитие и поддержку игровых проектов в долгосрочной перспективе.

Например, если вы разрабатываете игру в жанре GTA - где игрок управляет персонажем, предполагается, что герой может бегать по локации, заходить в здания, сесть в авто, мотоцикл, лодку или даже самолет — реализация всех этих возможностей большая головная боль для разработчика, так как каждый игровой объект который мы можем активировать и управлять — имеет свою уникальную логику. Чтобы отключить одну логику и включить другую — может быть большой проблемой. Но в случае с Entity-Component-System нам достаточно будет просто удалить один сценарий и добавить другой сценарий, чтобы изменить игровое поведение мира игры.

Если мы заранее сгруппируем все системы по какому-либо принципу, например, системы отвечающие за управление персонажем добавим в `InfantryScenario`, а системы отвечающие за управление автомобилем поместим в `VehicleScenario`, то в будущем, при активации автомобиля нам достаточно будет удалить `InfantryScenario` и добавить `VehicleScenario`. Кроме этого, при дальнейшем развитии игры, если мы добавили или удалили новые системы в рамках сценария, нам не нужно будет переписывать код переключения логики игры при смене активностей.

```c#
// Пример метода активации автомобиля.
public void GetInCar()
{
    AntEngine.Remove<InfantryScenario>();
    AntEngine.Add<VehicleScenario>();
}
```

Давайте создадим первый сценарий:

1️⃣ Создайте в папке `Scripts/Systems` новый скрипт и назовите его **Gameplay.cs** — это будет `GameplayScenario`.
2️⃣ Добавьте в него следующий код:

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
        // .. не забывайте удалять здесь системы из сценария когда он удаляется
        base.RemovedFromEngine();
    }
}
```

✅ Сценарий готов!

Вы можете создавать любое количество сценариев и систем в рамках одного сценария. Помните, сценарии — это способ держать вещи в порядке.


**AntEngine**

Кажется мы создали много интересных штук, но до сих пор не увидили результата. Теперь настало время все это собрать вместе.

Доступ к Anthill Entity-Component-System осуществляется через статический класс **AntEngine**, так же через этот класс происходит обработка всех систем. Чтобы запустить движок в работу, нам нужна некоторая точка входа в игру.

1️⃣ Создайте в папке `Scripts/Core` новый скрипт и назовите его **Game.cs**
2️⃣ Добавьте в него следующий код:

```c#
using UnityEngine;
using Anthill.Core;

public class Game : MonoBehaviour
{     
    private void Start()
    {
        // Добавляем основной сценарий.
        AntEngine.Add<Gameplay>();
        
        // Добавляем все сущности в игровой движок.
        AntEngine.AddEntitiesFromHierarchy("world_root");
    }

    private void Update()
    {
        AntEngine.Execute();
    }

    private void FixedUpdate()
    {
        AntEngine.ExecuteFixed();
    }

    private void LateUpdate()
    {
        AntEngine.ExecuteLate();
    }
}
```

3️⃣ Создайте на сцене новый GameObject и назовите его `game`. 
4️⃣ Прикрепите к объекту `game` скрипт **Game.cs**
✅ Теперь можно запустить проект и проверить его работу.

Если все сделано правильно, то в консоли вы увидите сообщения:

```
Added `player` node.
Removed `player` node.
```

> 🍀 **Совет:** создание и удаление игровых объектов — это дорогая операция, поэтому рекомендуется переиспользовать все что только можно. Например, помещайте удаленные объекты в пул объектов когда они не нужны и извлекайте их из пула когда они потребуются вновь, вместо того чтобы их создавать.


**Добавление сущностей**

Чтобы добавить новые сущности в игровой движок, например, после создания монстра из префаба, вам достаточно лишь получить компонент `AntEntity` с созданного игрового объекта и добавить его в `AntEngine` следующим образом:

```c#
public GameObject monsterPrefab;

// ..

// Классическое создание объекта из префаба.
var go = GameObject.Instantiate(monsterPrefab);
if (go.TryGetComponent<AntEntity>(out var entity))
{
    // Если удалось достать сущность с игрового объекта, то помещаем её в движок.
    AntEngine.AddEntity(entity);
}
```

После добавления сущности, она будет автоматическ распределена по спискам узлов согласно наличию компонентов. Обновятся даже те списки узлов, которые вы уже получили в рамках игровых систем, так как при извлечении списка нод через `AntEngine.GetNodes<T>()` — вы получаете лишь указатель на список узлов, а не его копию.

Если вам нужно добавить сразу много сущностей, например, после загрузки сцены, то вы можете воспользоваться следующим способом:

```c#
// Пытается найти все дочерние сущности у игрового объекта NameOfParentObject.
AntEngine.AddEntitiesFromHierarchy("NameOfParentObject");

// Или более быстрый способ, указать ссылку на transform геймобжекта.
AntEngine.AddEntitiesFromHierarchy(parentTransform);
```

При таком способе добавления будет произведен поиск сущностей внутри указанного `Transform` и каждый найденный `AntEntity` будет добавлен в `AntEngine`.


**Добавление и удаление компонентов**

Работая с сущностями, вы можете добавлять или удалять компоненты для них, чтобы изменить логику поведения сущности. Например, если вы хотите сделать персонажа бессметрным, чтобы он был исключен из системы обработки здоровья, удалите компонент `Health`:

```c#
node.Entity.Remove<Health>();
```

И верните его обратно когда персонаж должен стать вновь уязвимым:

```c#
node.Entity.Add<Health>();
```

Кроме этого, вы можете подписаться на события добавления или удаления компонентов для сущностей используя события:

```c#
node.Entity.EventComponentAdded += AddedComponentHandler;
node.Entity.EventComponentRemoved += RemovedComponentHandler;

// ..

private void AddedComponentHandler(AntEntity entity, Type componentType)
{
    Debug.Log("Component added!");
}

private void RemovedComponentHandler(AntEntity entity, Type componentType)
{
    Debug.Log("Component removed!");
}
```

При удалении или добавлении компонентов через методы `AntEntity`, распределение сущностей внутри списков узлов изменяется.

> ☝🏻 **Важно:** если вы удалите компонент через стандартные методы Unity, то произойдет ошибка, так как движок не умеет обрабатывать такую ситуацию, поэтому важно удалять компоненты только через `AntEntity`!

Пример как делать __не стоит__:

```c#
// Если добавить компонент стандартным образом.
node.Entity.gameObject.AddComponent<Health>();

// Если удалить компонент стандартным образом.
node.Entity.gameObject.RemoveComponent<Health>();
```

В обоих приведенных случаях, логика объекта не изменится. А при удалении компонента через стандартный `gameObject` — вовсе может привести к ошибкам, если сущность ранее была добавлена в движок.

---

### 🌿 Доступ к системам и сценариям

В **Entity-Component-System** архитектуре **категорически нельзя** обращаться из одной системы в другую и передать какие-либо данные. Каждая система должна быть изолирована и не может знать что-либо о других системах. Все общение между системами должно быть реализовано через компоненты. То есть, если вы хотите передать какую-либо информацию о сущности для обработки её повдения в другой системе, то вы должны делать это исключительно через компоненты — просто записывайте в них информацию о том что произошло.

Например, если ваш персонаж получил урон и вам нужно как-то передать информацию из системы обработки пуль, о точке попадания пули, её скорости и другой технической информации, то вам следует сделать и добавить компонент `HitBulletInfo`, записав в него все данные, чтобы передать их, например в систему создания эффектов:

```c#
public class HitBulletInfo
{
    // Этот компонент - это обычный класс, не обязательно создавать MonoBehaviour для быстрых компонентов,
    // которые не нужно настраивать в испекторе! :)
    public Vector2 hitPoint;
    public Vector2 normal;
    public float force;
}
```

Пример системы обрабатывающей пули:

```c#
public class BulletHitSystem : ISystem, IExecuteSystem
{
    // Список всех узлов которые попадают под правило пули.
    private AntNodeList<BulletNode> _bulletNodes;

    public void AddedToEngine() { ... }
    public void RemovedFromEngine() { ... }

    public void Execute()
    {
        BulletNode node;
        for (int i = _bulletNodes.Count - 1; i >= 0; i--)
        {
            node = _bulletNodes[i];
            // Если пуля врезалась и есть информация о том что она попала в другую сущность.
            if (node.Bullet.hasHit && node.Bullet.hitEntity != null)
            {
                // Добавляем сущности информацию о том что в нее попала пуля.
                node.Bullet.hitEntity.Add(new HitBulletInfo { 
                    hitPoint: node.Bullet.hitPoint, 
                    normal: node.Bullet.normal,
                    force: node.Bullet.force
                });
            }
        }
    }
}
```

После того как мы добавили в сущность компонент с информацией о том, что в нее попала пуля, она непременно попадет в список узлов которым требуется этот компонент и соответствующие системы смогут это обработать, например система создания эффектов.

```c#
public class HitEffectSystem : ISystem, IExecuteSystem
{
    // Список всех узлов которые содержат информацию о попадании пуль.
    private AntNodeList<BulletHitNode> _hitNodes;

    public void AddedToEngine() { ... }
    public void RemovedFromEngine() { ... }

    public void Execute()
    {
        BulletHitNode node;
        for (int i = _hitNodes.Count - 1; i >= 0; i--)
        {
            node = _hitNodes[i];

            // Создаем эффект!
            MakeEffect(node.HitBulletInfo.hitPoint);

            // И удаляем компонент из сущности чтобы исключить её из списка узлов для обработки эффекто.
            // Если конечно эта информация не нужна в других системах ^__^
            node.Entity.Remove<HitBulletInfo>();
        }
    }

    private void MakeEffect(Vector3 position) { ... }
}
```

Это хороший пример того как вы можете распределять логику поведения объектов по разным системам и реализовать коммуникацию между ними. Но все же, иногда может потребоваться прямой доступ к системам откуда-либо еще, например, чтобы удалить сценарий или систему из сценария на лету, когда нужно изменить поведение игры. Для этого используйте следующие методы `AntEngine`:

```c#
// Получение уже добавленного сценария или системы.
AntEngine.Get<Scenario>(); // - Где Scenario, это имя класса сценария который вам нужно извлечь.
AntEngine.Get<Scenario>().Get<BulletSystem>(); // - Где мы обращаемся к сценарию и достаем из него нужную систему.

// Добавление нового сценария или системы.
AntEngine.Add<Scenario>(); // - Так добавляем любой новый сценарий.
AntEngine.Add<Scenario>().Add<HealthSystem>(); // - Так добавляем систему в сценарий.

// Удаление сценария или системы.
AntEngine.Remove<Scenario>(); // - Так удаляем любой новый сценарий.
AntEngine.Remove<Scenario>().Remove<HealthSystem>(); // - Так удаляем систему в сценарий.

// Проверка наличия сценария или системы.
AntEngine.Has<Scenario>(); // Вернет true если сценарий существует в движке.
AntEngine.Get<Scenario>().Has<HealthSystem>(); // Вернет true если система добавлена в указанный сценарий.
```

Это безопасный способ изменить логику работы игры налету, при условии если вы не связывали между собой разные сценарии и системы. То есть из одной системы явно не ссылаетесь на другую, работая с ними по ссылке. В противном случае такой подход может вызвать массу проблем и ошибок.

> ☝🏻 **Важно:** держите системы изолированными друг от друга и не позволяйте одной системе, обращаться к другой напрямую!


---

### 🌼 Типы систем

**Anthill Entity-Component-System** поддерживает несколько разновидностей систем. Каждая разновидность системы определяется наличием интерфейсов. Любая система может поддерживать сразу все, несколько или только один тип.

* `ICleanupSystem` — система которая освобождает/удаляет какие-либо используемые ресурсы, например после выгрузки уровня (например, возвращает не выгружаемые объекты в пул);
* `IDeinitializeSystem` — система которая деинициализирует игровой мир или другие вещи (например, перед тем как выгрузить игровой уровень вам нужно отвязать игру от его объектов);
* `IDisableSystem` — система которая может быть по какой-то причине заблокирована и не обрабатывать что-то (например, система которая реализует пользовательский ввод и вам нужно его включить);
* `IEnableSystem` — система которая может быть разблокирована (обычно используется в паре с `IDisableSystem`, чтобы возвращать контроль);
* `IExecuteSystem` — большинство систем которые должны получать обновление `Update()`;
* `IExecuteFixedSystem` — системы должны получать обновление через `FixedUpdate()`;
* `IExecuteLateSystem` — системы которые должны получать обновление через `LateUpdate()`;
* `IInitializeSystem` — система которая инициализирует игровой мир или другие вещи (например, после того как загрузился игровой уровень и вам нужно привязать его объекты к игре);
* `IResetSystem` — система которая может сбрасывать состояние (например, система пользовательского ввода, чтобы обнулить данные о последних нажатиях кнопок и сбросить движение персонажа).

Возможно вы обратили внимание в базовом примере, что в `Update()` для класса Game вызывается `AntEngine.Execute()`, в методе `FixedUpdate()` вызывается `AntEngine.ExecuteFixed()` и в методе `LateUpdate()` вызывается `AntEngine.ExecuteLate()`. Точно так же вы можете вызывать и любые другие типы систем, когда вы считаете это нужным.

Например, перед тем как текущий уровень будет выгружен, мы можем вызывать метод `Cleanup()` для всех систем которые имплементируют интерфейс `ICleanupSystem`, чтобы освободить используемые для этого уровня ресурсы:

```c#
// Например, после того как уровень был выгружен, но новый еще не загружен.
AntEngine.Cleanup();
```

Когда нам нужно что-то инициализировать или деинициализировать, например, после загрузки или перед выгрузкой уровня, то мы можем вызвать соотвествующие системы:

```c#
// Уровень загрузился.
AntEngine.Initialize();

// Или сейчас уровень будет выгружен, деинициализация.
AntEngine.Deinitialize();
```

Аналогично и с системами `IDisableSystem` и `IEnableSystem` - вы можете использовать эти интерфейсы чтобы временно выключать или включать работу систем, но логику их временного выключения вам следует реализовать самостоятельно, так как они по прежнему продолжат получать все необходимые вызовы реализованные через другие интерфейсы:

```c#
// Вызвать метод Enable() для всех IEnableSystem.
AntEngine.Enable();

// Вызвать метод Disable() для всех IDisableSystem.
AntEngine.Disable();
```

Пример `IEnableSystem` и `IDisableSystem`:

```c#
public class SomeSystem : ISystem, IExecuteSystem, IEnableSystem, IDisableSystem
{
    private bool _isEnabled;

    public void AddedToEngine() { ... }
    public void RemovedFromEngine() { ... }

    public void Execute()
    {
        if (!_isEnabled)
        {
            return;
        }

        // Тут код который выполняется когда система включена.
    }

    public void Enable()
    {
        _isEnabled = true;
    }

    public void Disable()
    {
        _isEnabled = false;
    }
}
```

> ☝🏻 **Важно:** если вы выполните метод `Disable()` для сценария, то все добавленные в него системы прекратят свое обновление до тех пор пока вы не вызовите метод `Enable()`. Таким образом вы можете блокировать на некоторе выполнение отдельных сценариев не удаляя их из движка.

Метод сбросить — обычно используется для обнуления состояния каких-либо систем, например для системы пользовательского ввода когда нужно сбросить состояние ввода:

```c#
AntEngine.Reset();
```

Внутри `AntEngine` нет никакой логики для вызова этих методов систем — это лишь заготовка для вашего удобства, вы сами определяете что и когда будет работать.

Любая система может реализовывать сразу несколько интерфейсов если вам нужен этот функционал в рамках этой системы, например:

```c#
public class SomeSystem : ISystem, IResetSystem, IExecuteSystem, ICleanupSystem
{
    public void AddedToEngine() { .. }
    public void RemovedFromEngine() { .. }
    public void Reset() { .. }
    public void Execute() { .. }
    public void Cleanup() { .. }
}
```


---

### 🗿 Отладка 

При запуске проекта в Unity вы можете видеть динамически создаваемый объект `Systems` в корне активной сцены. Данный объект является отладочным инструментом демонстрирующий какие системы существуют в данный момент времени и что они делают, а так же сколько времени и ресурсов потребляют. Обращайте внимание на то, сколько требуется времени на обработку каждой системы и соотвествует ли это вашим ожиданиям.

Объект `Systems` создается только при запуске проекта в редакторе и отсуствует в финальных билдах. Вы можете отключить отладочный режим для Anthill перейдя в Player Settings и добавив символ `FINAL_BUILD` в список `Scripting Define Symbols` конкретной платформы.


---

### ⏱️ Отложенные вызовы 

Часто возникает необходимость выполнить какое-то действие с определенной задержкой. Для реализации этой задачи в Anthill существует специальный статический класс `AntDelayed` который позволяет создавать отложенные вызовы на базе **Entity-Component-System**.

Чтобы создать отложенный вызов, достаточно использовать следующий код:

```c#
AntDelayed.Call(0.5f, () => Debug.Log("Call!"));
```

При создании отложенного вызова, вы можете сохранить на него ссылку, например, чтобы его уничтожить до того как он будет
вызван, если в этом возникнет необходимость:

```c#
var call = AntDelayed.Call(1.5f, () => Debug.Log("Call!"));
call.Kill();
```

Если вам необходимо, чтобы отложенные вызовы работали при `timeScale == 0.0f`, используйте следующий пример:

```c#
AntDelayed.Call(1.5f, () => Debug.Log("Call!")).SetUpdate(true);
```

Для реализации отложенных вызовов используется встроенная система в **Anthill Entity-Component-System** - при создании первого отложенного вызова в отладочном режиме вы увидете как будет создана и добавлена система в движок для обработки отложенных вызовов.

---

### 🍀 Внедрение зависимостей 

**Anthill** имеет встроенную реализацию паттерна **Dependency Injection** - данный паттерн является заменой **Singleton** и устраняет всего его недостатки. **Dependency Injection** представляет собой «облачное хранилище» классов или объектов к которым необходимо получать доступ из любой точки игры. 

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

В данном примере Game — это `MonoBehaviour` который прикрепляется на основной игровой объект, в нем регистрируются все «условные» синглтоны. Такой подход позволяет решить несколько известных проблем паттерна **Singleton**:

* Явно контролируется порядок инициализации синглтонов;
* Логика синглтона вынесена из классов, что упрощает их переиспользование и дает возможность создавать их дубликаты, например при авто-тестах.

Чтобы получить доступ к любому классу из DI контейнера, необходимо сделать иньекцию:

```c#
using Anthill.Inject;

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

Атрибут `[Inject]` — помечает сеттеры в которые будут помещены указатели на ранее зарегистрированные экземпляры классов соотвествующего типа из глобального хранилища. Когда вызывается метод `Inject()` — то для переданного класса проверяются все сеттеры с атрибуттом `[Inject]` и в каждый из них будет помещена ссылка на экземпляр из глобального хранилища DI.


---

## 📝 Лицензия

[MIT License](https://github.com/AntKarlov/Anthill-ECS-Framework/blob/main/license.md)

Предоставляется разрешение любому лицу, получающему копию этого программного обеспечения и связанных с ним файлов документации («Программное обеспечение»), безвозмездно использовать Программное обеспечение без ограничений, включая права на использование, копирование, изменение, слияние, публикацию, распространение, сублицензирование и/или продажу копий Программного обеспечения, а также разрешать лицам, которым предоставляется Программное обеспечение, делать это при соблюдении следующих условий:

Вышеуказанное уведомление об авторских правах и это уведомление о разрешении должны быть включены во все копии или существенные части Программного обеспечения.

ПРОГРАММНОЕ ОБЕСПЕЧЕНИЕ ПРЕДОСТАВЛЯЕТСЯ «КАК ЕСТЬ», БЕЗ КАКИХ-ЛИБО ГАРАНТИЙ, ЯВНЫХ ИЛИ ПОДРАЗУМЕВАЕМЫХ, ВКЛЮЧАЯ, НО НЕ ОГРАНИЧИВАЯСЬ ​​ГАРАНТИЯМИ ТОВАРНОЙ ПРИГОДНОСТИ, ПРИГОДНОСТИ ДЛЯ КОНКРЕТНОЙ ЦЕЛИ И НЕНАРУШЕНИЯ ПРАВ. НИ ПРИ КАКИХ ОБСТОЯТЕЛЬСТВАХ АВТОРЫ ИЛИ ОБЛАДАТЕЛИ АВТОРСКИХ ПРАВ НЕ НЕСУТ ОТВЕТСТВЕННОСТИ ЗА ЛЮБЫЕ ПРЕТЕНЗИИ, УБЫТКИ ИЛИ ДРУГУЮ ОТВЕТСТВЕННОСТЬ, БУДЬ ТО В ДЕЙСТВИИ КОНТРАКТА, ДЕЛИКТА ИЛИ ИНЫМ ОБРАЗОМ, ВОЗНИКАЮЩИЕ ИЗ, ИЗ ИЛИ В СВЯЗИ С ПРОГРАММНЫМ ОБЕСПЕЧЕНИЕМ ИЛИ ИСПОЛЬЗОВАНИЕМ ИЛИ ДРУГИМИ ДЕЛАМИ В ПРОГРАММНОМ ОБЕСПЕЧЕНИИ.

---

## 💬 Контакты

Спасибо за помощь в развитии Александру Храмушену! 🙂

Контакты для связи:
Telegram: [AntKarlov](https://t.me/AntKarlov)