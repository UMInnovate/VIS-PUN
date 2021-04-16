using System.Collections.Generic;
using UnityEngine;

namespace TexDrawLib
{
#if UNITY_EDITOR

    public static class ObjectPoolShared
    {
        public static List<IObjectPool> objectPools = new List<IObjectPool>();
    }

#endif

    public class ObjectPool<T> : IObjectPool where T : new()
    {
        private readonly Stack<T> m_Stack = new Stack<T>();

#if UNITY_EDITOR
        public int countAll { get; set; }

        public int countActive { get { return countAll - countInactive; } }

        public int countInactive { get { return m_Stack.Count; } }

        public bool hasRegistered = false;
#endif

        public T Get()
        {
            T element;
            if (m_Stack.Count == 0)
            {
                element = new T();
#if UNITY_EDITOR
                countAll++;
                // Debug.LogFormat( "Pop New {0}, Total {1}", typeof(T).Name, countAll);
                if (!hasRegistered)
                {
                    ObjectPoolShared.objectPools.Add(this);
                    hasRegistered = true;
                }
#endif
            }
            else
            {
                element = m_Stack.Pop();
            }
            return element;
        }

        public void Release(T element)
        {
#if UNITY_EDITOR
            if (element.GetType().FullName != typeof(T).FullName)
                Debug.LogError("Non-top level Object pool release!");
            if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
                Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");
#endif
            m_Stack.Push(element);
        }
    }

    public interface IObjectPool
    {
#if UNITY_EDITOR
        int countAll { get; set; }

        int countActive { get; }

        int countInactive { get; }
#endif
    }
}
