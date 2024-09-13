using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitType : ScriptableObject
{
    [Header("Vitals")]
    [Range(0,100)]
    [Tooltip("The unit's health. If this reaches 0, the unit dies.")]
    public int health = 50;
    [Range(0, 100)]
    [Tooltip("The maximum speed at which the unit can move.")]
    public int speed = 50;
    [Range(0, 100)]
    [Tooltip("How fast the unit can turn and accelerate")]
    public int agility = 50;
    [Range(0, 100)]
    [Tooltip("The unit's stamina, which influences performance. Depletes over time in battle and regenerates while resting.")]
    public int stamina = 50;

    [Header("Attack")]
    [Range(0, 100)]
    [Tooltip("The maximum damage the unit can inflict in melee combat.")]
    public int meleeDamage = 50;
    [Range(0, 100)]
    [Tooltip("The speed at which the unit attacks in melee combat.")] //In seconds?
    public int attackSpeed = 50;

    //Show to user "meleeAttack" composed of meleeDamage and attackSpeed

    [Header("Defense")]
    [Range(0, 100)]
    [Tooltip("The unit's chance to block or reduce damage from attacks.")] // In percent?
    public int meleeDefense = 50;
    [Range(0, 100)]
    [Tooltip("The effectiveness of the unit's armor in reducing incoming damage.")] //In percent?
    public int armour = 50;

}
