using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class UIVersionNumber : MonoBehaviour
{
    //Settings
    [SerializeField]
    private string versionTextTemplate = "{0}";


    //Functions
    private void Start()
    {
        GetComponent<TMP_Text>().text = string.Format(versionTextTemplate, Application.version);
    }
}
