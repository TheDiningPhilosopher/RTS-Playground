using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Melee", menuName = "ScriptableObjects/UnitTypes/Melee")]
public class BaseMelee : UnitType
{
    [Header("Melee")]
    [Range(0, 100)]
    [Tooltip("Additional damage dealt when charging into an enemy.")] //In percent of damage?
    public int chargeBonus = 50;
    [Range(0, 100)]
    [Tooltip("Bonus damage against cavalry")] //In percent of damage?
    public int bonusAgainstCavalry = 50;
}
