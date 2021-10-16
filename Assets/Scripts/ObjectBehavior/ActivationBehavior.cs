using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
public class ActivationBehavior : MonoBehaviour
{
	Animator animator;
	public AudioSource soundOnActivation;
	public GameObject[] thingsToActivate;

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
		if (soundOnActivation != null)
		{
			soundOnActivation.Play();
		}
		usable = false;
		if (cooldownDuration > 0)
		{
			Invoke("BecomeUsable", cooldownDuration);
		}

		foreach(GameObject obj in thingsToActivate)
		{
			obj.SetActive(true);
		}
		//Empty out this array after used.
		thingsToActivate = new GameObject[] { };

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
