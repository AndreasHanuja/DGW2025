using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingeltonMonoBehaviour<T> : MonoBehaviour where T : SingeltonMonoBehaviour<T>
{
	static T instance = null;
	static public T Instance { get => instance; protected set => instance = value; }


	protected virtual void Awake()
	{
		if (instance != null)
		{
#if UNITY_EDITOR && VERBOSE
			Debug.LogError($"Multiple Instances of {typeof(T).Name}", Instance);
			Debug.LogError($"Multiple Instances of {typeof(T).Name}", this);
#endif
			Destroy(gameObject);
			return;
		}

		instance = (T)this;
	}
}
