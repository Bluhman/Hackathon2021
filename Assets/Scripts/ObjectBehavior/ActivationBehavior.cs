using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
public class ActivationBehavior : MonoBehaviour
{
	Animator animator;
	BoxCollider2D triggerArea;

	// Start is called before the first frame update
	void Start()
	{
		animator = GetComponent<Animator>();
		triggerArea = GetComponent<BoxCollider2D>();
	}

	// Update is called once per frame
	void Update()
	{

	}
}
