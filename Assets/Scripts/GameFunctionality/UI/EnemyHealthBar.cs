using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
	public GameObject HPBarSliderObj;
	Slider HPBarSlider;
	public Text damageText;
	public CharacterStat targetStats;
	public float damageTextDuration;

	private void Start()
	{
		HPBarSlider = HPBarSliderObj.GetComponent<Slider>();
		HPBarSlider.maxValue = targetStats.charMetrics.health;
		currentHealth = currentHealth;

		damageValue = 0;
		damageText.enabled = false;
	}

	int currentHealth
	{
		get => targetStats.charMetrics.currentHealth;
		set
		{
			targetStats.charMetrics.currentHealth = value;
			//Hide the HP Bar Slider if the target's at full HP.
			HPBarSliderObj.SetActive(value == targetStats.charMetrics.health);
		}
	}

	int damageValue
	{
		get => Convert.ToInt32(damageText.text);
		set
		{
			damageText.text = "" + value;
			damageText.enabled = true;
		}
	}

	private void Update()
	{
		HPBarSlider.value = Mathf.Lerp(HPBarSlider.value, currentHealth, 0.1f);
	}

	public void DisplayDamage(int amount)
	{
		CancelInvoke();
		HPBarSliderObj.SetActive(true);
		//Adds the current damage dealt to the previous damage displayed in case it hasn't
		//gone away yet.
		damageValue = damageValue += amount;
		Invoke("ResetDamage", damageTextDuration);
	}

	private void ResetDamage()
	{
		damageValue = 0;
		damageText.enabled = false;

		HPBarSliderObj.SetActive(!targetStats.charMetrics.isDead);
	}
}
