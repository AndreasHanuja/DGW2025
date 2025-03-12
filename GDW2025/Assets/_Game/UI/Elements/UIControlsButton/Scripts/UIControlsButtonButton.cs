using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControlsButtonButton : Button
{
	protected override void DoStateTransition(SelectionState state, bool instant)
	{
		//prevent beeing selected or highlighted
		if (state == SelectionState.Highlighted || state == SelectionState.Selected)
		{
			state = SelectionState.Normal;
		}

		base.DoStateTransition(state, instant);
	}
}
