using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerInput : MonoBehaviour
{
	PlayerController player;
	public PlayerActivationInteractor playerInteract;
	public float doubleTapDashTimingWindow = 0.25f;

	void Start()
	{
		player = GetComponent<PlayerController>();
	}

	void Update()
	{

		Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		player.SetDirectionalInput(directionalInput);


		if (Input.GetButtonDown("Jump"))
		{
			player.OnJumpInputDown();
		}
		if (Input.GetButtonUp("Jump"))
		{
			player.OnJumpInputUp();
		}

		if (Input.GetButtonDown("Dash"))
		{
			player.OnDash(true);
		}
		if (Input.GetButtonUp("Dash"))
		{
			player.OnDash(false);
		}

		if (Input.GetButtonDown("Fire1"))
		{
			player.OnAttack();
		}

		if (Input.GetButtonDown("Use"))
		{
			playerInteract.OnActivate();
		}
	}
}