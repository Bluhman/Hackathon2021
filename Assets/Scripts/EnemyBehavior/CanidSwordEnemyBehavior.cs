using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(WalkingController))]
[RequireComponent(typeof(Animator))]
public class CanidSwordEnemyBehavior : CharacterStat
{
	Animator animator;
	WalkingController wc;
	public GameObject attackBox;

	// Start is called before the first frame update
	void Start()
	{
		base.Start();

		animator = GetComponent<Animator>();
		wc = GetComponent<WalkingController>();
	}

	// Update is called once per frame
	void Update()
	{
		base.Update();

		DetermineState();
	}

	private void DetermineState()
	{
		animator.SetBool("isDead", charMetrics.isDead);
		animator.SetFloat("walkingSpeed", Math.Abs(wc.velocity.x));
		animator.SetBool("inAir", wc.getIsAirborne());
	}

	protected override void OnStagger(int amount, Vector2 direction)
	{
		base.OnStagger(amount, direction);

		animator.SetTrigger("stagger");
	}
}
