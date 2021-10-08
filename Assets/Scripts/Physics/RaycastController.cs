using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]

public class RaycastController : MonoBehaviour
{
	public LayerMask collisionMask;

	public const float skinWidth = 0.15f;
	const float dstBetweenRays = .25f;
	[HideInInspector]
	public int horizontalRayCount;
	[HideInInspector]
	public int verticalRayCount;

	//Derived vars.
	[HideInInspector]
	public float horizontalRaySpacing;
	[HideInInspector]
	public float verticalRaySpacing;

	[HideInInspector]
	public BoxCollider2D boxCollision;
	[HideInInspector]
	public RaycastOrigins raycastOrigins;

	public virtual void Awake()
	{
		boxCollision = GetComponent<BoxCollider2D>();
	}

	// Start is called before the first frame update
	public virtual void Start()
	{
		CalculateRaySpacing();
	}

	public void UpdateRaycastOrigins()
	{
		Bounds bounds = boxCollision.bounds;
		bounds.Expand(skinWidth * -2);

		raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
	}

	public void CalculateRaySpacing()
	{
		Bounds bounds = boxCollision.bounds;
		bounds.Expand(skinWidth * -2);

		float boundsWidth = bounds.size.x;
		float boundsHeight = bounds.size.y;

		horizontalRayCount = Mathf.RoundToInt(boundsHeight / dstBetweenRays);
		verticalRayCount = Mathf.RoundToInt(boundsWidth / dstBetweenRays);

		horizontalRaySpacing = boundsHeight / (horizontalRayCount - 1);
		verticalRaySpacing = boundsWidth / (verticalRayCount - 1);

		//Debug.Log($"{ horizontalRayCount} horizontal rays");
		//Debug.Log($"{ verticalRayCount} vertical rays");

		//Debug.Log($"{ horizontalRaySpacing} horizontal spacing");
		//Debug.Log($"{ verticalRaySpacing} vertical spacing");
	}

	public struct RaycastOrigins
	{
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}

}
