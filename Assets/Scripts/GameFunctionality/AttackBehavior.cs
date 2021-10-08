using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]

public class AttackBehavior : MonoBehaviour
{
   public int crushingDmg;
   public int slashingDmg;
   public int piercingDmg;
   public int mysticalDmg;
   public int fireDmg;
   public int electricDmg;
   public int frostDmg;
   public int darkDmg;
   public int staggerStrength;
   public string[] hittableTargetTags;

   public float repeatHitTime = 0;

   public bool melee;

   public float dmgAnimationScalar = 1;

   public float attackLifetime;
   float timer;

   List<GameObject> hitTargets = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
      timer = attackLifetime;
    }

	private void OnTriggerEnter2D(Collider2D collision)
	{

      if (hitTargets.Contains(collision.gameObject))
      {
         return;
      }

      foreach (string candidateTag in hittableTargetTags)
		{

         if (collision.gameObject.CompareTag(candidateTag))
			{
            var stats = collision.gameObject.GetComponent<CharacterStat>();
            if (stats != null)
				{
               var drs = stats.charMetrics.damageResistances;
               //Calculate damage output in response to the defenses and status on the target.
               int mitigatedCrushingDmg = Mathf.RoundToInt(crushingDmg * (1f - drs.crushing) * dmgAnimationScalar);
               int mitigatedSlashingDmg = Mathf.RoundToInt(slashingDmg * (1f - drs.slashing) * dmgAnimationScalar);
               int mitigatedPiercingDmg = Mathf.RoundToInt(piercingDmg * (1f - drs.piercing) * dmgAnimationScalar);
               int mitigatedMysticalDmg = Mathf.RoundToInt(mysticalDmg * (1f - drs.mystical) * dmgAnimationScalar);
               int mitigatedFireDamage = Mathf.RoundToInt(fireDmg * (1f - drs.fire) * dmgAnimationScalar);
               int mitigatedElectricDmg = Mathf.RoundToInt(electricDmg * (1f - drs.electric) * dmgAnimationScalar);
               int mitigatedFrostDamage = Mathf.RoundToInt(frostDmg * (1f - drs.frost) * dmgAnimationScalar);
               int mitigatedDarkDamage = Mathf.RoundToInt(darkDmg * (1f - drs.dark) * dmgAnimationScalar);
               //Addd them AAAALLLL up...
               int totalDmg = mitigatedCrushingDmg
                  + mitigatedDarkDamage
                  + mitigatedElectricDmg
                  + mitigatedFireDamage
                  + mitigatedFrostDamage
                  + mitigatedMysticalDmg
                  + mitigatedPiercingDmg
                  + mitigatedSlashingDmg;
               //Deal the hit.
               stats.charMetrics.currentHealth -= totalDmg;
               int adjustedStagger = Mathf.RoundToInt(staggerStrength * dmgAnimationScalar);
               stats.DrainFooting(adjustedStagger, collision.transform.position - gameObject.transform.position);

               //Add the target to the hitTargets so that it doesn't become a victim of chain hits.
               hitTargets.Add(collision.gameObject);

               if (repeatHitTime > 0)
					{
                  StartCoroutine(PrepareRepeatHit(collision.gameObject, repeatHitTime));
					}
            }
            break;
			}
		}
	}

   IEnumerator PrepareRepeatHit(GameObject target, float delay)
	{
      yield return new WaitForSeconds(delay);
      hitTargets.Remove(target);
	}

	// Update is called once per frame
	void Update()
    {
      if (timer == -1)
		{
         //Timer of exactly -1 is a permanent effect. Environment hazard.
         return;
		}

      timer -= Time.deltaTime;
      if (timer <= 0)
		{
         Destroy(gameObject);
		}
    }
}
