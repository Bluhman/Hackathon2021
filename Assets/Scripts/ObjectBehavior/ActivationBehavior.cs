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

	public bool isOn;
	public string switchText;
	public string switchOffText;
	public float cooldownDuration;

	// Start is called before the first frame update
	void Start()
	{
		usable = true;
		animator = GetComponent<Animator>();
		animator.SetBool("on", isOn);
	}

	private void BecomeUsable()
	{
		usable = true;
	}

	public void Use(bool? state)
	{
		if (!usable) return;
		isOn = state ?? !isOn;
		usable = false;
		if (cooldownDuration > 0)
		{
			Invoke("BecomeUsable", cooldownDuration);
		}
	}
}
