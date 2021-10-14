using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public class PlayerActivationInteractor : MonoBehaviour
{
	List<GameObject> targetActivations;
	TextMesh text;
	public Collider2D activationRange;

	private void Start()
	{
		targetActivations = new List<GameObject>();
		text = GetComponent<TextMesh>();
		text.text = "";
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		var otherObject = collision.gameObject;
		var activationGuy = otherObject.GetComponent<ActivationBehavior>();
		if (activationGuy == null || !activationGuy.usable || !collision.CompareTag("Activation"))
		{
			return;
		}

		targetActivations.Add(otherObject);
		OnListChange();
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		var otherObject = collision.gameObject;
		targetActivations.Remove(otherObject);
		OnListChange();
	}

	private void OnListChange()
	{
		if (targetActivations.Count == 0)
		{
			text.text = "";
			return;
		}

		targetActivations.Sort((a,b) => {
			var acol = a.GetComponent<Collider2D>();
			var bcol = b.GetComponent<Collider2D>();
			var aDist = 0.0f;
			var bDist = 0.0f;
			if (acol == null)
			{
				aDist = 9999;
			}
			else
			{
				aDist = activationRange.Distance(acol).distance;
			}
			if (bcol == null)
			{
				bDist = 9999;
			}
			else
			{
				bDist = activationRange.Distance(bcol).distance;
			}
			return bDist.CompareTo(aDist);
		});

		//After a truly rousing sort job, we then take the leading entry (the closest)
		//and utilize its values.
		var closest = targetActivations[0];
		var activationGuy = closest.GetComponent<ActivationBehavior>();
		text.text = "W: " + (activationGuy.isOn && activationGuy.switchOffText.Length > 0
			? activationGuy.switchOffText : activationGuy.switchText);
	}
}
