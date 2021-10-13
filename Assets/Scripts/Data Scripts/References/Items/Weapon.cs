using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackAnimationType
{
   thrust,
   swing,
   bow,
   shoot
}

[Serializable]
public class Weapon : Item
{
   public StatScores statRequirements;
   public string attackEffectPrefabName;
   public AttackAnimationType animationCategory;
   public float strScaling;
   public float agiScaling;
   public float intScaling;
   public float wilScaling;
   public float weight;
   public int staminaCost;
   //All effects should be normalized around the standing position's animation.
   public Vector2 spawnPositionCrouching;
   
   [NonSerialized]
   GameObject cachedPrefab;

   public GameObject prefab
	{
      get
      {
         if (cachedPrefab == null)
         {
            cachedPrefab = Resources.Load($"Prefabs/AttackFX/{attackEffectPrefabName}") as GameObject;
         }
         return cachedPrefab;
      }
	}

   public float attackSpeed { get {
         var behavior = prefab.GetComponent<AttackBehavior>();
         if (behavior != null)
			{
            return 1 / behavior.attackLifetime;
			}
         else
			{
            //Just play default speed..
            return 1;
			}
      }
   }
}
