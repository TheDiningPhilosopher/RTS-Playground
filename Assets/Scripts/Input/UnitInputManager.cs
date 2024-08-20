using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;

public class UnitInputManager : MonoBehaviour
{

    [Header("UI")]
    [SerializeField]
    private Image selectionBox;

    //Unit Selection
    private Vector2 selectionStartPos;
    private bool selectionStartPosSet;
    private bool groupKeyDown;
    private bool addKeyDown;

    //Unit Positioning
    private bool previewKeyDown;
    private Vector3 positioningStartPos;
    private Vector3 positioningStartDirection;
    private bool positioningStartPosSet;
    private bool positionKeyDown;


    void Awake()
    {

    }

    void Update()
    {
        StartBoxSelection();
        UnitPositionPreview();
    }

    #region PositionPreviewInputs

    public void GetPreview(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>() != 0)
        {
            previewKeyDown = true;
        }
        else
        {
            previewKeyDown = false;
        }
        UnitPreviewActive(previewKeyDown);
    }

    public void GetPositioningDown(InputAction.CallbackContext ctx)
    {
        positionKeyDown = ctx.action.WasPressedThisFrame();

        if (ctx.action.WasReleasedThisFrame())
        {
            ReleasePositioning();
        }
    }

    #endregion

    #region PositionPreview

    private void UnitPreviewActive(bool active)
    {
        Array.ForEach(GameManager.Instance.Units, u => u.Agents.ForEach(a =>
            a.PreviewActive(active)));
    }

    private void UnitPositionPreview()
    {
        if (GameManager.Instance.GetSelectedUnits().Count == 0) return;
        if (positionKeyDown && !positioningStartPosSet)
        {
            positioningStartDirection = GameManager.Instance.GetSelectedUnits()[0].transform.right.normalized;
            
            Ray ray = GameManager.Instance.MainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hitInfo;
            if(Physics.Raycast(ray, out hitInfo))
            {
                positioningStartPos = hitInfo.point;
                positioningStartPosSet = true;
                PositionUnit(positioningStartPos, true);
            }
        }
        if (positioningStartPosSet)
        {
            Ray ray = GameManager.Instance.MainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                Vector3 currentPos = hitInfo.point;
                PositionUnit(currentPos, true);
            }
        }
    }

    private void ReleasePositioning()
    {
        Ray ray = GameManager.Instance.MainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
            Vector3 currentPos = hitInfo.point;
            PositionUnit(currentPos, false || previewKeyDown, true);
        }
        else
        {
            PositionUnit(positioningStartPos, false || previewKeyDown, true);
        }
        
        positioningStartDirection = Vector2.zero;
        positioningStartPosSet = false;


    }

    private void PositionUnit(Vector3 currentPosition, bool previewActive, bool setTarget = false)
    {
        //TODO: Edge case currentPosition = positioningStartPos => return agentDirecton.right?
        Vector3 directionVector = (currentPosition - positioningStartPos).normalized;

        var selectedUnits = GameManager.Instance.GetSelectedUnits();
        Vector3 startPos = positioningStartPos;
        for (int i = 0; i < selectedUnits.Count; i++)
        {
            if (setTarget)
            {
                Debug.Log("startPos" + startPos);
                Debug.Log("endPos´" + new Vector3(currentPosition.x / selectedUnits.Count, currentPosition.y, currentPosition.z));
            }
            selectedUnits[i].Agents.ForEach(y => y.PreviewActive(previewActive));
            Vector3 nextStartPos = selectedUnits[i].SetFormationPreview(
                startPos,
                new Vector3(currentPosition.x, currentPosition.y, currentPosition.z),
                directionVector,
                positioningStartDirection,
                setTarget);
            startPos = nextStartPos;
        }
    }

    #endregion  

    #region SelectionInputs

    public void GetSelect(InputAction.CallbackContext ctx)
    {
        if (ctx.action.WasPressedThisFrame())
        {
            SelectUnit(addKeyDown);
        }
    }

    public void GetGroupSelectUnit(InputAction.CallbackContext ctx)
    {
        groupKeyDown = ctx.action.WasPressedThisFrame();

        if(ctx.action.WasReleasedThisFrame() && selectionStartPosSet)
        {
            ReleaseBoxSelection();
        }
    }

    public void GetDeselectAll(InputAction.CallbackContext ctx)
    {
        if (ctx.action.WasPressedThisFrame())
        {
            GameManager.Instance.DeselectAll();
        }
    }

    public void GetAddSelect(InputAction.CallbackContext ctx)
    {
        addKeyDown = ctx.action.WasPressedThisFrame();
    }

    #endregion

    #region Selection

    private void StartBoxSelection()
    {
        if (groupKeyDown && !selectionStartPosSet)
        {
            selectionStartPos = Mouse.current.position.ReadValue();
            selectionStartPosSet = true;
            selectionBox.enabled = true;
        }
        if (selectionStartPosSet)
        {
            Vector2 currentPos = Mouse.current.position.ReadValue();
            float width = currentPos.x - selectionStartPos.x;
            float height = currentPos.y - selectionStartPos.y;

            selectionBox.rectTransform.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
            selectionBox.rectTransform.anchoredPosition = selectionStartPos + new Vector2(width, height) / 2;
        }
    }

    private void ReleaseBoxSelection()
    {
        selectionStartPosSet = false;
        selectionBox.enabled = false;
        BoxSelectUnits();
    }

    private void BoxSelectUnits()
    {
        RectTransform rect = selectionBox.rectTransform;
        Vector2 min = rect.anchoredPosition - (rect.sizeDelta / 2);
        Vector2 max = rect.anchoredPosition + (rect.sizeDelta / 2);

        foreach(Unit u in GameManager.Instance.Units)
        {
            foreach (Agent agent in u.Agents)
            {
                Vector3 screenPos = GameManager.Instance.MainCamera.WorldToScreenPoint(agent.transform.position);
                if(screenPos.x > min.x && screenPos.x < max.x && screenPos.y > min.y && screenPos.y < max.y)
                {
                    GameManager.Instance.SelectUnit(u);
                    break;
                }
                else
                {
                    if (!addKeyDown)
                    {
                        GameManager.Instance.DeselectUnit(u);
                    }
                }
            }
        }

    }

    //Does not work because box select deselects units. 
    //TODO: Fix (click on UI Symbol over unit instead of agent?)
    private void SelectUnit(bool add)
    {
        Ray ray = GameManager.Instance.MainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit))
        {
            Agent agent;
            
            if(hit.transform.TryGetComponent<Agent>(out agent))
            {
                Unit unit = agent.transform.GetComponentInParent<Unit>();

                if (!GameManager.Instance.GetSelectedUnits().Contains(unit))
                {
                    if (!add)
                    {
                        GameManager.Instance.DeselectAll();
                    }
                    GameManager.Instance.SelectUnit(unit);
                }
               
            }
        }

    }

    #endregion

}
