using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Action = System.Action;

public class UICanvas : MonoBehaviour, IUICanvasGroupMember
{
	//Settings
	[SerializeField]
	private bool active;
	[SerializeField]
	private bool disableOnGroupDisable = true;


	//Globals
	private Canvas ownCanvas;
	private UICanvasGroup parentGroup;
	public UICanvasGroup ParentGroup { get => parentGroup; }
	private CanvasGroup ownUnityCanvasGroup;
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
		ownCanvas = GetComponent<Canvas>();
		if (TryGetComponent(out CanvasGroup canvasGroup))
		{
			ownUnityCanvasGroup = canvasGroup;
		}
		RegisterParentCanvasGroup();
		OnActiveUpdate();
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


	/// <summary>
	/// Change visibility focus from own canvas to another canvas.
	/// </summary>
	/// <param name="other">The canvas to change focus to.</param>
	public void ChangeActive(UICanvas other)
	{
		if (other == null)
		{
			throw new System.Exception("Missing UICanvas to change to.");
		}

		Active = false;
		other.Active = true;
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
		
		if (ownUnityCanvasGroup != null)
		{
			ownUnityCanvasGroup.interactable = active;
		}

		if (active == ownCanvas.enabled)
		{
			return;
		}

		ownCanvas.enabled = active;

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
		if (transform.parent == null)
		{
			return;
		}

		//GetComponentInParent searches the specified object too, so start searching at transform.parent
		UICanvasGroup parentGroup = transform.parent.GetComponentInParent<UICanvasGroup>(true);
		if (parentGroup == null)
		{ return; }

		this.parentGroup = parentGroup;
		parentGroup.RegisterGroupMember(this);
	}
}
