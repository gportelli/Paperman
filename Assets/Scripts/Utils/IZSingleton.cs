using UnityEngine;
using System;

/// <summary>
/// Prefab attribute. Use this on child classes
/// to define if they have a prefab associated or not
/// By default will attempt to load a prefab
/// that has the same name as the class,
/// otherwise [Prefab("path/to/prefab")] to define it specifically. 
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public class PrefabAttribute : Attribute
{
	string _name;
	public string Name { get { return this._name; } }
	public PrefabAttribute() { this._name = ""; }
	public PrefabAttribute(string name) { this._name = name; }
}

public abstract class IZSingleton<T> : MonoBehaviour where T : IZSingleton<T>
{
	// Block the "new" instance with a protected constructor
	protected IZSingleton () { } 
	
	public bool IsPersistent;
	private static T _instance = null;
	private static bool isApplicationQuit = false;
	
	//Avoid concurrent access with a lock object, try to be thread safe...
	private static object _lock = new object();
	
	public static T Instance
	{
		get
		{
			lock(_lock)
			{
				// Instance required for the first time, we look for it
				if( _instance == null && !isApplicationQuit)
				{
					_instance = GameObject.FindObjectOfType(typeof(T)) as T;
					// Object not found, we create a temporary one
					if( _instance == null )
					{
						Type mytype = typeof(T);
						//Debug.LogWarning("No instance of " + typeof(T).ToString() + ", a temporary one is created.");
						bool hasPrefab = Attribute.IsDefined(mytype, typeof(PrefabAttribute));
						// checks if the [Prefab] attribute is set and pulls that if it can
						if (hasPrefab)
						{
							PrefabAttribute attr = (PrefabAttribute)Attribute.GetCustomAttribute(mytype,typeof(PrefabAttribute));
							//Debug.LogWarning(goName + " not found attempting to instantiate prefab... either: " + goName + " or: " + prefabname);
							try
							{
								if (attr.Name != "")
								{
									_instance = Instantiate(Resources.Load(attr.Name, typeof(T))) as T;
								}
								else
								{
									_instance = Instantiate(Resources.Load(mytype.ToString(), typeof(T))) as T;
								}
							} catch (Exception e)
							{
								Debug.LogError("could not instantiate prefab even though prefab attribute was set: " + e.Message + "\n" + e.StackTrace);
							}
						}
						else _instance = new GameObject("Temp " + typeof(T).ToString(), typeof(T) ).GetComponent<T>();
						
						// Problem during the creation, this should not happen
						if( _instance == null )
						{
							Debug.LogError("Problem during the creation of " + typeof(T).ToString());
						}
					} else _instance.Init();
				} 
				return _instance;
			}
		}
	}
	
	private void Awake()
	{
		isApplicationQuit = false;
		if( _instance == null )
		{
			_instance = this as T;
			_instance.Init();
			
			if (IsPersistent)
			{
				DontDestroyOnLoad(gameObject);
			}
		}
		else //if (_instance != this)
		{
			Destroy (gameObject);
		}
	}
	
	// This function is called when the instance is used the first time
	// Put all the initializations you need here, as you would do in Awake
	public virtual void Init(){ }
	
	//OnDestroy commented because is causing errors on change level without persistent checked. 
	//Seems that the isApplicationQuit was always true and prevent instantiation of a new singleton instance.
	private void OnDestroy(){
		isApplicationQuit = true;
		//		Debug.LogWarning ("OnDestroy()");
	}
	
	//    private void OnApplicationQuit()
	//    {
	//		isApplicationQuit = true;
	//    }
}
