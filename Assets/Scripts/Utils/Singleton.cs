using UnityEngine;

//Reference: http://wiki.unity3d.com/index.php/Singleton
/// <summary>
/// Be aware this will not prevent a non singleton constructor
///   such as `T myT = new T();`
/// To prevent that, add `protected T () {}` to your singleton class.
/// </summary>
public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    /// <summary>
    /// A method that should be overridden to Initialize.
    ///             The default implementation does nothing.
    /// 
    /// </summary>
    protected virtual void OnInit()
    {
      
    }

    private static T _instance;

    private static readonly object _lock = new object();

    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        return _instance;
                    }

                    if (_instance == null)
                    {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = "(singleton) " + typeof(T).ToString();

                        DontDestroyOnLoad(singleton);
                    }
                    else
                    {

                    }
                    _instance.OnInit();
                }

                return _instance;
            }
        }
    }

    private static bool applicationIsQuitting = false;

    void OnApplicationQuit()
    {
        applicationIsQuitting = true;
    }
}