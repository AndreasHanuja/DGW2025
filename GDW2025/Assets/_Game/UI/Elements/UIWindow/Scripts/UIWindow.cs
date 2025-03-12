using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWindow : MonoBehaviour
{
	//Functions
	public virtual void SetActive(bool active)
	{
		gameObject.SetActive(active);
	}


	public virtual void ChangeActive(UIWindow changeTo)
	{
		if (changeTo == null)
		{
			throw new System.Exception("Missing UIWindow to change to.");
		}

		SetActive(false);
		changeTo.SetActive(true);
	}
}
