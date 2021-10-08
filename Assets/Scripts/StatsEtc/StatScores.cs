using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StatScores
{
   public int vigor;
   public int poise;
   public int endurance;
   public int strength;
   public int agility;
   public int intelligence;
   public int willpower;
   public int luck;
   public int level
   {
      get => vigor + poise + endurance + strength + agility + intelligence + willpower + luck - 79;
   }
}
