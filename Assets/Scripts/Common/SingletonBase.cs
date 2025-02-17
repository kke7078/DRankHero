using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace KGY
{
    //SingletonBase : 싱글톤 패턴을 적용하기 위한 기반 클래스
    public class SingletonBase<T> : MonoBehaviour where T : class
    {
        public static T Singleton
        {
            get
            {
                return _instance.Value;
            }
        }

        private static readonly Lazy<T> _instance =
            new Lazy<T>(() =>
            {
                T instance = FindAnyObjectByType(typeof(T)) as T;

                if (instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).ToString());
                    instance = obj.AddComponent(typeof(T)) as T;

#if UNITY_EDITOR
                    if (EditorApplication.isPlaying)
                    {
                        DontDestroyOnLoad(obj);
                    }
#else
                    DontDestroyOnLoad(obj);
#endif
                }

                return instance;
            });

        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
