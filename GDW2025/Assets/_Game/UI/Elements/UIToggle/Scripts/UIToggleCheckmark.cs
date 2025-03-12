using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIToggleCheckmark : MonoBehaviour
{
    //Settings
    [SerializeField]
    private bool ShowWhenOn;
	[SerializeField]
	private Toggle onwToggle;


	//Globals
	private Image ownImage;
	

	//Functions
	private void Awake()
	{
		ownImage = GetComponent<Image>();
		onwToggle.onValueChanged.AddListener(OnToggleValueChanged);
	}


	private void OnToggleValueChanged(bool isOn)
	{
		ownImage.enabled = isOn == ShowWhenOn;
	}
}
