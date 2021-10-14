using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
public class ActivationBehavior : MonoBehaviour
{
	Animator animator;

	[HideInInspector]
	public bool usable;

	public Animator[] affectedAnimators;
	public bool isOn;
	public string switchText;
	public string switchOffText;
	public float cooldownDuration;

	// Start is called before the first frame update
	void Start()
	{
		usable = true;
		animator = GetComponent<Animator>();
		alterOnState(isOn);
	}

	private void BecomeUsable()
	{
		usable = true;
	}

	public void Use()
	{
		if (!usable) return;
		alterOnState(!isOn);
		usable = false;
		if (cooldownDuration > 0)
		{
			Invoke("BecomeUsable", cooldownDuration);
		}

	}

	private void alterOnState(bool newState)
	{
		isOn = newState;
		animator.SetBool("on", isOn);
		foreach (var anim in affectedAnimators)
		{
			anim.SetBool("on", isOn);
		}
	}
}
