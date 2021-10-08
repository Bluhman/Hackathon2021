using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDatabase : MonoBehaviour
{
   public List<Effect> effects = new List<Effect>();
	// Start is called before the first frame update

	private void Awake()
	{
		BuildDB();
	}

	void BuildDB()
	{

	}
}
