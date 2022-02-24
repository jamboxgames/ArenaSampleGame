using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Jambox.Common.Utility
{
    /// <summary>
    /// Mono singleton Class. Extend this class to make singleton component.
    /// Example: 
    /// <code>
    /// public class Foo : MonoSingleton<Foo>
    /// </code>. To get the instance of Foo class, use <code>Foo.instance</code>
    /// Override <code>Init()</code> method instead of using <code>Awake()</code>
    /// from this class.
    /// </summary>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T m_Instance = null;

        public static T Instance
        {
            get
            {
                // Instance requiered for the first time, we look for it
                if (m_Instance == null)
                {
                    m_Instance = new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();
                }
                return m_Instance;
            }
        }

        // If no other monobehaviour request the instance in an awake function
        // executing before this one, no need to search the object.
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        /// Make sure the instance isn't referenced anymore when the user quit, just in case.
        private void OnApplicationQuit()
        {
            m_Instance = null;
        }
    }
}
