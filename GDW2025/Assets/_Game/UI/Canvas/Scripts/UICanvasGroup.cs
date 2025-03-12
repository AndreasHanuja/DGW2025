using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Action = System.Action;

public class UICanvasGroup : MonoBehaviour, IUICanvasGroupMember
{
	//Settings
	[SerializeField]
	private bool active = true;
	[SerializeField]
	private bool disableOnGroupDisable = true;


	//Globals
	private UICanvasGroup parentGroup;
	public UICanvasGroup ParentGroup { get => parentGroup; }
	private List<IUICanvasGroupMember> groupMembers = new List<IUICanvasGroupMember>();
	public bool Active
	{
		get => active && ParentGroupActive;
		set { if (active != value) { active = value; OnActiveUpdate(); } }
	}
	private bool ParentGroupActive { get => parentGroup != null ? parentGroup.Active : true; }
	public event Action OnActivate;
	public event Action OnDeactivate;


	//Functions
	private void Awake()
	{
		RegisterParentCanvasGroup();
	}


	/// <summary>
	/// Change the visibility of the own canvas.
	/// </summary>
	/// <param name="active"></param>
	public void SetActive(bool active)
	{
		Active = active;
	}


	/// <summary>
	/// Enable the visibility of the own canvas and all parent canvas groups.
	/// </summary>
	/// <param name="active"></param>
	public void SetActiveRecoursively()
	{
		Active = true;
		if (parentGroup != null)
		{
			parentGroup.SetActiveRecoursively();
		}
	}


	public void RegisterGroupMember(IUICanvasGroupMember childCanvas)
	{
		groupMembers.Add(childCanvas);
		childCanvas.OnParentGroupActiveUpdate();
	}


	public void OnParentGroupActiveUpdate()
	{
		if (!ParentGroupActive && disableOnGroupDisable)
		{
			active = false;
		}

		OnActiveUpdate();
	}


	private void OnActiveUpdate()
	{
		bool active = Active;

		foreach (IUICanvasGroupMember groupMember in groupMembers)
		{
			groupMember.OnParentGroupActiveUpdate();
		}

		if (active)
		{
			OnActivate?.Invoke();
		}
		else
		{
			OnDeactivate?.Invoke();
		}
	}


	private void RegisterParentCanvasGroup()
	{
		if(transform.parent == null)
		{ return; }

		//GetComponentInParent searches the specified object too, so start searching at transform.parent
		UICanvasGroup parentGroup = transform.parent.GetComponentInParent<UICanvasGroup>(true);
		if (parentGroup == null)
		{ return; }

		this.parentGroup = parentGroup;
		parentGroup.RegisterGroupMember(this);
	}
}
