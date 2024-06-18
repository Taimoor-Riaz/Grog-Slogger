using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public abstract class AnimatableUIPanel : UIPanelBase
{
	[SerializeField] protected float showHideTime = 0.25f;

	protected CanvasGroup canvasGroup;

	public override UIPanelBase SetUp(SceneUIControllerBase controller)
	{
		base.SetUp(controller);
		canvasGroup = GetComponent<CanvasGroup>();
		return this;
	}
	protected void AnimateShow()
	{
		var desc = LeanTween.value(0, 1, showHideTime);
		desc.setOnStart(() =>
		{
			canvasGroup.interactable = false;
		})
		.setOnComplete(() =>
		{
			canvasGroup.interactable = true;
		})
		.setOnUpdate((float value) =>
		{
			transform.localScale = Vector3.Lerp(new Vector3(0.9f, 0.9f, 0.9f), Vector3.one, value);
			canvasGroup.alpha = value;
		})
		.setIgnoreTimeScale(true);
	}
	protected void AnimateHide()
	{
		var desc = LeanTween.value(1, 0, showHideTime);
		desc.setOnStart(() =>
		{
			canvasGroup.interactable = false;
		})
		.setOnComplete(() =>
		{
			Destroy(gameObject);
		})
		.setOnUpdate((float value) =>
		{
			transform.localScale = Vector3.Lerp(new Vector3(0.8f, 0.9f, 0.9f), Vector3.one, value);
			canvasGroup.alpha = value;
		})
		.setIgnoreTimeScale(true);
	}
}
