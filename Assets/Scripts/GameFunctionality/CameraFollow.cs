using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public Controller2D target;
	public Vector2 focusAreaSize;
	public Vector2 lookAreaSize;

	public float verticalOffset;
	public float lookAheadDistX;
	public float smoothTimeX;
	public float smoothTimeY;
	public float camDistance;
	public float lookMultiplier;

	FocusArea focusArea;

	float currentLookAheadX;
	float targetLookAheadX;
	float lookDirectionX;
	float smoothLookVelocityX;
	float smoothLookVelocityY;

	bool lookAheadStopped;
	bool freeLook;

	private void Start()
	{
		focusArea = new FocusArea(target.boxCollision.bounds, focusAreaSize);
	}

	private void LateUpdate()
	{

		if (Input.GetButtonDown("Look"))
		{
			focusArea.ResizeBounds(lookAreaSize);
		}
		if (Input.GetButtonUp("Look"))
		{
			focusArea.ResizeBounds(focusAreaSize);
		}

		freeLook = Input.GetButton("Look");
		if (freeLook)
		{
			var lookingTarget = Input.mousePosition;
			//Center:
			lookingTarget.x -= Screen.width/2;
			lookingTarget.y -= Screen.height/2;

			//Normalize
			lookingTarget.x /= Screen.width;
			lookingTarget.y /= Screen.height;
			lookingTarget *= lookMultiplier;

			//center on player.
			lookingTarget.x += target.boxCollision.bounds.center.x;
			lookingTarget.y += target.boxCollision.bounds.center.y;
			Debug.Log(lookingTarget);
			focusArea.Update(new Bounds(lookingTarget, new Vector2(0.5f, 0.5f)));
		} 
		else
		{

		}

		focusArea.Update(target.boxCollision.bounds);

		

		Vector2 focusPosition = focusArea.center + Vector2.up * verticalOffset;

		if (focusArea.velocity.x != 0)
		{
			lookDirectionX = Mathf.Sign(focusArea.velocity.x);
			if (Mathf.Sign(target.playerInput.x) == Mathf.Sign(focusArea.velocity.x) && target.playerInput.x != 0)
			{
				lookAheadStopped = false;
				targetLookAheadX = lookDirectionX * lookAheadDistX;
			}
			else
			{
				if (!lookAheadStopped)
				{
					targetLookAheadX = currentLookAheadX + (lookDirectionX * lookAheadDistX - currentLookAheadX) / 4f;
					lookAheadStopped = true;
				}
			}
		}

		
		currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, smoothTimeX);

		focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothLookVelocityY, smoothTimeY);

		focusPosition += Vector2.right * currentLookAheadX;

		transform.position = (Vector3)focusPosition + Vector3.forward * -camDistance;
	}

	private void OnDrawGizmos()
	{
		{
			Gizmos.color = new Color(0.8f, 0.8f, 1.0f, 0.4f);
			Gizmos.DrawCube(focusArea.center, focusAreaSize);
		}
	}

	struct FocusArea
	{
		public Vector2 velocity;
		public Vector2 center;
		float left, right;
		float top, bottom;

		public FocusArea(Bounds targetBoundaries, Vector2 size)
		{
			left = targetBoundaries.center.x - size.x / 2;
			right = targetBoundaries.center.x + size.x / 2;
			bottom = targetBoundaries.min.y;
			top = targetBoundaries.min.y + size.y;

			velocity = Vector2.zero;
			center = new Vector2((left + right) / 2, (top + bottom) / 2);
		}

		public void ResizeBounds(Vector2 newSize)
		{
			left = center.x - newSize.x / 2;
			right = center.x + newSize.x / 2;
			bottom = center.y - newSize.y / 2;
			top = center.y + newSize.y / 2;
		}

		public void Update(Bounds targetBounds)
		{
			float shiftX = 0;
			if (targetBounds.min.x < left)
			{
				shiftX = targetBounds.min.x - left;
			}
			else if (targetBounds.max.x > right)
			{
				shiftX = targetBounds.max.x - right;
			}
			left += shiftX;
			right += shiftX;

			float shiftY = 0;
			if (targetBounds.min.y < bottom)
			{
				shiftY = targetBounds.min.y - bottom;
			}
			else if (targetBounds.max.y > top)
			{
				shiftY = targetBounds.max.y - top;
			}
			bottom += shiftY;
			top += shiftY;

			center = new Vector2((left + right) / 2, (top + bottom) / 2);
			velocity = new Vector2(shiftX, shiftY);
		}
	}
}
