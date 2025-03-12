using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMainMenu : UICanvasGeneric
{
	//Functions
	public void PlayClicked()
	{
		GameManager.Instance.FireTrigger(GameManager.Trigger.EnterLevel);
	}

	public void ExitGame()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}
}
