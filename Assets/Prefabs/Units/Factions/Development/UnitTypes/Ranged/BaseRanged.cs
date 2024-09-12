using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Melee", menuName = "ScriptableObjects/UnitTypes/Ranged")]
public class BaseRanged : UnitType
{
    [Header("Ranged")]
    [Range(0, 100)]
    [Tooltip("Damage of the missile")]
    public int missileDamage = 50;
    [Range(0, 100)]
    [Tooltip("The maximum range")]
    public int range = 50;
}
