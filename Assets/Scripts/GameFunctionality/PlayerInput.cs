using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerInput : MonoBehaviour
{
	PlayerController player;
	public float doubleTapDashTimingWindow = 0.25f;
	private bool _pressedDirectionRecently; 

	void Start()
	{
		player = GetComponent<PlayerController>();
	}

	void Update()
	{

		Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		player.SetDirectionalInput(directionalInput);

		if (!_pressedDirectionRecently)
		{
			_pressedDirectionRecently = directionalInput.magnitude >= 1;
		}

		if (Input.GetButtonDown("Jump"))
		{
			player.OnJumpInputDown();
		}
		if (Input.GetButtonUp("Jump"))
		{
			player.OnJumpInputUp();
		}

		if (Input.GetButtonDown("Fire1"))
		{
			player.OnAttack();
		}
	}
}