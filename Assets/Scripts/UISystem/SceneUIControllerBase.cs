using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SceneUIControllerBase : MonoBehaviour
{
	[Header("Scene UI Controller")]
	[SerializeField]
	protected List<UIPanelBase> registeredPanels = new List<UIPanelBase>();

	[SerializeField]
	protected Canvas rootCanvas;

	[SerializeField]
	protected string rootPanel;

	private void Start()
	{
		if (rootCanvas == null)
		{
			Debug.LogError("No Root Canvas Defined");
			return;
		}
		ShowPanel(rootPanel);
	}

	public virtual void ShowPanel(string panelUID, Dictionary<string, object> data = null)
	{
		UIPanelBase panel = GetPanel(panelUID);
		if (panel != null)
		{
			Instantiate(panel, rootCanvas.transform).SetUp(this).Show(data);
		}
		else
		{
			Debug.LogError("No Panel found with ID: " + panelUID);
		}
	}

	protected UIPanelBase GetPanel(string panelUID)
	{
		// Find panel from available panels
		for (int i = 0; i < registeredPanels.Count; i++)
		{
			if (registeredPanels[i].UID == panelUID)
			{
				return registeredPanels[i];
			}
		}
		return null;
	}
}
