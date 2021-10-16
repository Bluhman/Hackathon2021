using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class CanidEnemyStat : CharacterStat
{
	Animator animator;
	WalkingController wc;
	public GameObject bodySpriteObject;
	Vector2 initialSpriteLocalPos;
	SpriteRenderer bodySprite;

	public override void Start()
	{
		base.Start();

		animator = GetComponent<Animator>();
		wc = GetComponent<WalkingController>();
		bodySprite = bodySpriteObject.GetComponent<SpriteRenderer>();
		initialSpriteLocalPos = bodySpriteObject.transform.localPosition;
		Debug.Log(wc);
	}

	protected override void OnHit()
	{
		//CancelInvoke();
		InvokeRepeating("randomShakePerFrame", 0, Time.deltaTime);
		Invoke("randomShakeStop", 0.3f);
	}

	void randomShakePerFrame()
	{
		var randomizedLocalPos = initialSpriteLocalPos;
		randomizedLocalPos.x += Random.Range(-0.05f, 0.05f);
		randomizedLocalPos.y += Random.Range(-0.05f, 0.05f);
		bodySpriteObject.transform.localPosition = randomizedLocalPos;
		Debug.Log(bodySpriteObject.transform.localPosition);
	}

	void randomShakeStop()
	{
		bodySpriteObject.transform.localPosition = initialSpriteLocalPos;
		CancelInvoke();
	}

	protected override void OnStagger(int amount, Vector2 direction)
	{
		if (amount <= 0)
		{
			//Probably shouldn't be zero... but just in case.
			return;
		}

		float staggerTime = (amount / 1.5f) / charMetrics.balance;
		animator.SetFloat("staggerAnimSpeed", 1 / staggerTime);
		animator.SetTrigger("stagger");
		//wc.directionalInput.x = Mathf.Sign(-direction.x);
		wc.directionalInput.x = 0;
		wc.velocity = Vector3.zero;
		Debug.Log(direction);
		wc.velocity = direction * Mathf.Abs(charMetrics.currentFooting);
		Debug.Log(wc.velocity);

		if (direction.x != 0)
		{
			bodySprite.flipX = -direction.x > 0;
		}

		base.OnStagger(amount, direction);
		//Debug.Break();

	}
}


