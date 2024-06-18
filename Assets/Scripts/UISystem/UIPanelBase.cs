using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIPanelBase : MonoBehaviour
{
	public string UID = "";
	protected SceneUIControllerBase _controller;
	public virtual UIPanelBase SetUp(SceneUIControllerBase controller)
	{
		_controller = controller;
		return this;
	}
	public abstract void Show(Dictionary<string, object> data = null);
	public abstract void Hide();
}
