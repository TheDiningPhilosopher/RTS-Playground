using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Singleton")]
    private static GameManager instance;
    public static GameManager Instance 
    { get 
        { 
            if(instance == null)
            {
                Debug.LogError("GameManager Instance is NULL");
            }
            return instance; 
        } 
    }

    [Header("Camera")]
    [SerializeField]
    private Camera mainCamera;
    public Camera MainCamera 
    { 
        get 
        {
            if (mainCamera == null)
            {
                Debug.LogError("GameManager Instance is NULL");
            }
            return mainCamera;
        }
    }

    [Header("Selected Units")]
    private Unit[] units;
    public Unit[] Units { get { return units; } }
    [SerializeField]
    private List<Unit> selectedUnits = new List<Unit>();
    [SerializeField]
    private Material selectedUnitMaterial;
    [SerializeField]
    private Material deselectedUnitMaterial;


    void Awake()
    {
        instance = this;   
        if(MainCamera == null)
        {
            Debug.LogError("Camera not set!");
        }

        units = FindObjectsOfType<Unit>().Where(x => x.teamId == 0).ToArray();
        SetUnitMaterial(deselectedUnitMaterial);
    }

    void Update()
    {
        
    }

    private void SetUnitMaterial(Material material)
    {
        Array.ForEach(units, x => SetUnitMaterial(x, material));
    }
    private void SetUnitMaterial(Unit unit, Material material)
    {
        unit.Agents.ForEach(x => x.GetComponent<Renderer>().material = material);
    }
    public void SelectUnit(Unit unit)
    {
        if (!selectedUnits.Contains(unit) && units.Contains(unit))
        {
            selectedUnits.Add(unit);
            SetUnitMaterial(unit, selectedUnitMaterial);
        }
    }
    public bool DeselectUnit(Unit unit)
    {
        if (selectedUnits.Remove(unit))
        {
            SetUnitMaterial(unit, deselectedUnitMaterial);
            return true;
        }
        return false;
    } 

    public void DeselectAll()
    {
        selectedUnits.ForEach(unit => SetUnitMaterial(unit, deselectedUnitMaterial));
        selectedUnits = new List<Unit>();
    }
    public List<Unit> GetSelectedUnits() => selectedUnits;
}
