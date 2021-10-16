using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatTracker : CharacterStat
{
   /// <summary>
   /// Player's character metrics before stats have been applied.
   /// </summary>
   public BaseMetrics level0Metrics;
   public BaseMetrics level100Metrics;
   public StatScores stats;

   public AnimationCurve vigorHealthCurve;
   public AnimationCurve poiseLoadCurve;
   public AnimationCurve enduranceStaminaCurve;
   public AnimationCurve strengthFootingCurve;
   public AnimationCurve agilityBalanceCurve;
   public AnimationCurve intelligenceSkillSpdCurve;
   public AnimationCurve luckDiscoveryCurve;

   public AnimationCurve vigorColdCurve;
   public AnimationCurve enduranceElectricCurve;
   public AnimationCurve strengthFireCurve;
   public AnimationCurve agilityPhysicalCurve;
   public AnimationCurve intelligenceMagicCurve;
   public AnimationCurve willDarkCurve;

   public AnimationCurve vigorPoisonCurve;
   public AnimationCurve enduranceInjuryCurve;
   public AnimationCurve willCurseCurve;

   public Slider HPBarSlider;
   public Slider StaminaBarSlider;

   //This reference asset ties into the charMetrics.
   public IntReference playerCurrentHealthReference;

   int itemDiscovery;
   PlayerController pc;
   Inventory inv;

    // Start is called before the first frame update
    public override void Start()
    {
      pc = GetComponent<PlayerController>();
      inv = GetComponent<Inventory>();

      CalculateCurrentMetrics();
      base.Start();

      //UI Inits: Stat Bars.
      RectTransform hp = HPBarSlider.GetComponent<RectTransform>();
      float percentMaxSize = (float)charMetrics.health / 3000f;

      hp.anchorMax = new Vector2( percentMaxSize * 0.8f, hp.anchorMax.y);
      hp.offsetMax = new Vector2(0, hp.offsetMax.y);
      HPBarSlider.maxValue = charMetrics.health;
      HPBarSlider.value = charMetrics.currentHealth;

      RectTransform sta = StaminaBarSlider.GetComponent<RectTransform>();
      percentMaxSize = (float)charMetrics.stamina / 700f;
      sta.anchorMax = new Vector2(percentMaxSize * 0.8f, sta.anchorMax.y);
      sta.offsetMax = new Vector2(0, sta.offsetMax.y);
      StaminaBarSlider.maxValue = charMetrics.stamina;
      StaminaBarSlider.value = charMetrics.currentStamina;
    }

   public override void Update()
	{
      base.Update();

      playerCurrentHealthReference.value = charMetrics.currentHealth;

      StaminaBarSlider.value = Mathf.Lerp(StaminaBarSlider.value, charMetrics.currentStamina, 0.1f);
      HPBarSlider.value = Mathf.Lerp(HPBarSlider.value, charMetrics.currentHealth, 0.1f);
   }

	void CalculateCurrentMetrics()
	{
      //Start at pure baseline:
      charMetrics = level0Metrics;

      Debug.Log(charMetrics);

      //From stats:
      charMetrics.health = scaleStat(vigorHealthCurve, stats.vigor, "health");
      charMetrics.stamina = scaleStat(enduranceStaminaCurve, stats.endurance, "stamina");
      charMetrics.equipLoad = scaleStat(poiseLoadCurve, stats.poise, "equipLoad");
      charMetrics.footing = scaleStat(strengthFootingCurve, stats.strength, "footing");
      charMetrics.balance = scaleStat(agilityBalanceCurve, stats.agility, "balance");
      //Luck discovery determination:
      itemDiscovery = 100 * Mathf.RoundToInt(luckDiscoveryCurve.Evaluate((float)(stats.luck + 1) / 100));

      //I sure hope this is a reference.
      var drs = charMetrics.damageResistances;
      var physicalScale = scaleResist(agilityPhysicalCurve, stats.agility, "crushing");
      drs.crushing = physicalScale;
      drs.slashing = physicalScale;
      drs.piercing = physicalScale;
      drs.mystical = scaleResist(intelligenceMagicCurve, stats.intelligence, "mystical");
      drs.fire = scaleResist(strengthFireCurve, stats.strength, "fire");
      drs.electric = scaleResist(enduranceElectricCurve, stats.endurance, "electric");
      drs.frost = scaleResist(vigorColdCurve, stats.vigor, "frost");
      drs.dark = scaleResist(willDarkCurve, stats.willpower, "dark");
      //It is not!
      charMetrics.damageResistances = drs;

      var ars = charMetrics.statusResistances;
      ars.injury = scaleAil(enduranceInjuryCurve, stats.endurance, "injury");
      ars.poison = scaleAil(vigorPoisonCurve, stats.vigor, "poison");
      ars.venom = scaleAil(vigorPoisonCurve, stats.vigor, "venom");
      ars.curse = scaleAil(willCurseCurve, stats.willpower, "curse");
      charMetrics.statusResistances = ars;

      //[TODO - Do more tabulation for layers of equipment and accessories.]
      charMetrics.weight += inv.equippedWeapon != null ? inv.equippedWeapon.weight : 0;
   }

	protected override void OnStagger(int amount, Vector2 direction)
	{
      if (amount <= 0)
		{
         //Probably shouldn't be zero... but just in case.
         return;
		}

      float staggerTime = (amount / 1.5f) / charMetrics.balance;
      pc.animator.SetFloat("staggerAnimSpeed", 1 / staggerTime);
      pc.animator.SetTrigger("stagger");
      pc.velocity.x = 0;


      var myKids = pc.transform.GetComponentsInChildren<AttackBehavior>();
      if (myKids.Length > 0)
		{
         var myMeleeAttack = myKids.Where(atk => atk.melee).First().gameObject;
         if (myMeleeAttack != null)
         {
            Destroy(myMeleeAttack);
         }
      }
      
      base.OnStagger(amount, direction);
   }

	protected override void OnHit(bool blocked, Vector2 dir)
	{
      base.OnHit(blocked, dir);
      pc.bodySprite.color = new Color(255, 0, 0);
      //CancelInvoke();
      InvokeRepeating("fadeBodyBack", 0, 0.01f);
	}

   private void fadeBodyBack()
	{
      if (pc.bodySprite.color == pc.skinColor)
		{
         CancelInvoke();
		}
      //There's no lerp for this so uh...
      //transform the color into a vector 3.
      Vector3 currentColor = new Vector3(pc.bodySprite.color.r, pc.bodySprite.color.g, pc.bodySprite.color.b);
      Vector3 targetColor = new Vector3(pc.skinColor.r, pc.skinColor.g, pc.skinColor.b);
      Vector3 nextColor = Vector3.Lerp(currentColor, targetColor, 0.33f);
      Debug.Log(nextColor);
      //Set the actual damn color
      pc.bodySprite.color = new Color(nextColor.x, nextColor.y, nextColor.z);
   }

	int scaleStat(AnimationCurve curve, int statLevel, string statTarget)
	{
      //Start from the level 0 baseline.
      int value = level0Metrics.ReflectByName<int>(statTarget);
      //Extra is the max-level additional on top of the baseline.
      int extra = level100Metrics.ReflectByName<int>(statTarget) - value;
      extra = Mathf.RoundToInt(extra * curve.Evaluate((float)(statLevel + 1) / 100));
      return value + extra;
   }

   float scaleResist(AnimationCurve curve, int statLevel, string resistTarget)
   {
      //Start from the level 0 baseline.
      float value = level0Metrics.damageResistances.ReflectByName<float>(resistTarget);
      //Extra is the max-level additional on top of the baseline.
      float extra = level100Metrics.damageResistances.ReflectByName<float>(resistTarget) - value;
      extra *= curve.Evaluate((float)(statLevel + 1) / 100);
      return value + extra;
   }

   int scaleAil(AnimationCurve curve, int statLevel, string statTarget)
   {
      //Start from the level 0 baseline.
      int value = level0Metrics.statusResistances.ReflectByName<int>(statTarget);
      //Extra is the max-level additional on top of the baseline.
      int extra = level100Metrics.statusResistances.ReflectByName<int>(statTarget) - value;
      extra = Mathf.RoundToInt(extra * curve.Evaluate((float)(statLevel + 1) / 100));
      return value + extra;
   }
}
