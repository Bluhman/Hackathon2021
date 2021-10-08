using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateWithinRange : MonoBehaviour
{
	public enum deactivateNature
	{
		dontDeactivate,
		freeze,
		resetPosition,
		vanish
	}

	public Bounds activationDistance;
	public Bounds deactivationDistance;
	public MonoBehaviour targetScript;
	public deactivateNature onLeaveRange;
	public float offscreenPadding;

	GameObject playerObject;
	Vector3 originalPosition;
	Camera gameCamera;
	CameraFollow gameCameraLogic;

	[ExecuteInEditMode]
	private void Awake()
	{
		originalPosition = transform.position;
	}

	private void OnDrawGizmosSelected()
	{
		if (!Application.isPlaying)
		{
			Gizmos.color = new Color(1, 0.6f, 0.6f, 0.3f);
			Gizmos.DrawCube(activationDistance.center + transform.position, activationDistance.size);
			Gizmos.color = new Color(0.8f, 0.6f, 0.2f, 0.3f);
			Gizmos.DrawWireCube(deactivationDistance.center + transform.position, deactivationDistance.size);
		}
		else
		{
			Gizmos.color = new Color(1, 0.6f, 0.6f, 0.3f);
			Gizmos.DrawCube(activationDistance.center, activationDistance.size);
			Gizmos.color = new Color(0.8f, 0.6f, 0.2f, 0.3f);
			Gizmos.DrawWireCube(deactivationDistance.center, deactivationDistance.size);
		}

	}

	private void Start()
	{
		//Center these on the script user, just like as it's shown in the gizmos.
		activationDistance.center += originalPosition;
		deactivationDistance.center += originalPosition;

		playerObject = GameObject.FindGameObjectWithTag("Player");
		var cameraInstance = GameObject.FindGameObjectWithTag("MainCamera");
		gameCamera = cameraInstance.GetComponent<Camera>();
		gameCameraLogic = cameraInstance.GetComponent<CameraFollow>();
		if (playerObject == null)
		{
			Debug.LogError("Unable to find player?");
			//Destroy the activateWithinRange script...
			Destroy(this);
		}


	}

	private void LateUpdate()
	{

		if (activationDistance.Contains(gameCamera.transform.position + new Vector3(0, 0, gameCameraLogic.camDistance)))
		{
			targetScript.enabled = true;
			//Activate all children:
			for (int i = 0; i < transform.childCount; i++)
			{
				transform.GetChild(i).gameObject.SetActive(true);
			}
		}

		else if (targetScript.enabled && !deactivationDistance.Contains(playerObject.transform.position))
		{
			Plane[] frustrumPlanes = GeometryUtility.CalculateFrustumPlanes(gameCamera);
			if (GeometryUtility.TestPlanesAABB(frustrumPlanes,
					new Bounds(transform.position, new Vector3(offscreenPadding, offscreenPadding, 2))
					)
				)
			{
				return;
			}

			switch (onLeaveRange)
			{
				case deactivateNature.resetPosition:
					//Ensure camera is not within spawn location of the origin spot.
					if (!GeometryUtility.TestPlanesAABB(frustrumPlanes, new Bounds(originalPosition, Vector3.zero)))
					{
						Debug.Log(originalPosition);
						transform.position = originalPosition;
					}
					else
					{
						Debug.Log(frustrumPlanes);
					}
					goto case deactivateNature.freeze;
				case deactivateNature.freeze:
					targetScript.enabled = false;
					//Deactivate all children:
					for (int i = 0; i < transform.childCount; i++)
					{
						transform.GetChild(i).gameObject.SetActive(false);
					}
					break;
				case deactivateNature.vanish:
					Destroy(gameObject);
					break;
				default:
					//Do not deactivate:
					return;
			}
		}
	}
}
