using System;
using System.Collections.Generic;

public class Singleton<T> where T : class, new()
{
    private static T _instance = default(T);

    public static T GetInstance()
    {
        if (_instance == null)
        {
            _instance = new T();
        }
        return _instance;
    }

    public static T Instance
    {
        get
        {
            if (Singleton<T>._instance == null)
            {
                Singleton<T>._instance = Activator.CreateInstance<T>();
            }
            return Singleton<T>._instance;
        }
    }

    public virtual void Initialize()
    {

    }

    public virtual void UnInitialize()
    {
        Destory();
    }

    public static void Destory()
    {
        if (Singleton<T>._instance != null)
        {
            Singleton<T>._instance = null;
        }
    }

}


