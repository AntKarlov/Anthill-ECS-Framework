namespace Anthill.Inject
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;

	public class AntInjectContainer : IInjectContainer, IServiceLocator, IInjector
	{
		private class TypeData
		{
			public object Instance { get; set; }
			public List<KeyValuePair<InjectAttribute, PropertyInfo>> Properties { get; private set; }
			public List<KeyValuePair<InjectAttribute, FieldInfo>> Fields { get; private set; }
			public bool IsSingleton { get; private set; }

			private TypeData()
			{
				Properties = new List<KeyValuePair<InjectAttribute, PropertyInfo>>();
				Fields = new List<KeyValuePair<InjectAttribute, FieldInfo>>();
			}

			public static TypeData Create(Type aType, bool aIsSingleton = false, object aInstance = null)
			{
				var typeData = new TypeData
				{ 
					IsSingleton = aIsSingleton, 
					Instance = aInstance
				};

				foreach (var field in aType.GetFields(BindingFlags.Public | BindingFlags.Instance))
				{
					var inject = (InjectAttribute) field.GetCustomAttributes(typeof(InjectAttribute), true).FirstOrDefault();
					if (inject != null)
					{
						typeData.Fields.Add(new KeyValuePair<InjectAttribute, FieldInfo>(inject, field));
					}
				}

				foreach (var property in aType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
				{
					var inject = (InjectAttribute) property.GetCustomAttributes(typeof(InjectAttribute), true).FirstOrDefault();
					if (inject != null)
					{
						typeData.Properties.Add(new KeyValuePair<InjectAttribute, PropertyInfo>(inject, property));
					}
				}

				return typeData;
			}
		}
	
	#region Private Variables

		//private string singletonGameObjectName;
		private readonly Dictionary<Type, Dictionary<string, Type>> types = new Dictionary<Type, Dictionary<string, Type>>();
		private readonly Dictionary<Type, TypeData> typeDatas = new Dictionary<Type, TypeData>();

	#endregion

	#region Public Methods

		public AntInjectContainer()
		{
			RegisterSingleton<IServiceLocator>(this);
		}

		public void Register<T>() where T : class
		{
			Type type = typeof(T);
			TypeData typeData = TypeData.Create(type);
			Register(null, type, typeData);
		}

		public void Register<TInterface, TClass>(string aKey = null) where TClass : class, TInterface
		{
			Type typeInterface = typeof(TInterface);
			Type type = typeof(TClass);
			TypeData typeData = TypeData.Create(type);
			Register(typeInterface, type, typeData, aKey);
		}

		public void RegisterSingleton<T>() where T : class
		{
			Type type = typeof(T);
			TypeData typeData = TypeData.Create(type, true);
			Register(null, type, typeData);
		}

		public void RegisterSingleton<T>(T aInstance, string aKey = null) where T : class 
		{
			Type type = typeof(T);
			TypeData typeData = TypeData.Create(type, true, aInstance);
			Register(null, type, typeData, aKey);
		}

		public void RegisterSingleton<TInterface, TClass>(string aKey = null) where TClass : class, TInterface
		{
			Type typeInterface = typeof(TInterface);
			Type type = typeof(TClass);
			TypeData typeData = TypeData.Create(type, true);
			Register(typeInterface, type, typeData, aKey);
		}

		public T Resolve<T>(string aKey = null) where T : class
		{
			return (T) Resolve(typeof(T), aKey);
		}

		public T Inject<T>(object aObject)
		{
			return (T) Inject(typeof(T), aObject);
		}

		public object Inject(Type aType, object aObject)
		{
			TypeData typeData = GetTypeData(aType);
			typeData.Fields.ForEach(x => x.Value.SetValue(aObject, Resolve(x.Value.FieldType, x.Key.Key)));
			typeData.Properties.ForEach(x => x.Value.SetValue(aObject, Resolve(x.Value.PropertyType, x.Key.Key), null));
			return aObject;
		}

		private TypeData GetTypeData(Type aType)
		{
			if (!typeDatas.ContainsKey(aType))
			{
				TypeData typeData = TypeData.Create(aType);
				Register(null, aType, typeData);
			}
			return typeDatas[aType];
		}

		public object Resolve(Type aType, string aKey = null)
		{
			if (!types.ContainsKey(aType))
			{
				throw new Exception($"The type `{aType.Name}` is not registered.");
			}

			if (!types[aType].ContainsKey(aKey ?? string.Empty))
			{
				throw new Exception($"There is no implementation registered with the key `{aKey}` for the type `{aType}`.");
			}

			Type foundType = types[aType][aKey ?? string.Empty];
			TypeData typeData = typeDatas[foundType];
			/*if (foundType.IsSubclassOf(typeof(MonoBehaviour)))
			{
				if (singletonGameObjectName == null)
				{
					throw new Exception(string.Format("You have to set a game object name to use for MonoBehaviours with SetSingletonGameObject() first."));
				}

				GameObject gameObject = GameObject.Find(singletonGameObjectName)
					?? new GameObject(singletonGameObjectName);
				
				return gameObject.GetComponent(aType.Name) ?? Inject(foundType, gameObject.AddComponent(foundType));
			}*/

			if (typeData.IsSingleton)
			{
				return typeData.Instance ?? (typeData.Instance = Setup(foundType));
			}

			return Setup(foundType);
		}

	#endregion

	#region Private Methods

		/*public void SetSingletonGameObject(string aName)
		{
			singletonGameObjectName = aName;
		}*/

		private object Setup(Type aType)
		{
			object instace = Activator.CreateInstance(aType);
			Inject(aType, instace);
			return instace;
		}

		private void Register(Type aInterfaceType, Type aType, TypeData aTypeData, string aKey = null)
		{
			try
			{
				if (types.ContainsKey(aInterfaceType ?? aType))
				{
					types[aInterfaceType ?? aType].Add(aKey ?? string.Empty, aType);
				}
				else
				{
					types.Add(aInterfaceType ?? aType, new Dictionary<string, Type> { { aKey ?? string.Empty, aType } });
				}

				typeDatas.Add(aType, aTypeData);
			}
			catch (Exception ex)
			{
				throw new Exception("Register type is failed!", ex);
			}
		}
	}

	#endregion
}