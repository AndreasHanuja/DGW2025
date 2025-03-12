using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUICanvasGroupMember
{
    public bool Active { get; set; }
	public UICanvasGroup ParentGroup { get; }
	public void OnParentGroupActiveUpdate();
}
