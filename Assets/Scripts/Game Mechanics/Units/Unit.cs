using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

    [SerializeField]
    [Header("Team")]
    public int teamId = 0;

    [Header("Unit Settings")]
    [SerializeField]
    private UnitType unitType;
    [SerializeField]
    private GameObject agentPrefab;
    [SerializeField]
    private GameObject positionPreviewPrefab;

    [Header("Formation")]
    [SerializeField]
    private int unitSize = 64;
    [SerializeField]
    private int formationRows = 8;
    [SerializeField]
    private float offset = 2.2f;

    [Header("Setup")]
    [SerializeField]
    private List<Agent> agents;
    [SerializeField]
    private List<GameObject> posPreviews;
    public List<Agent> Agents { get { return agents; } }
    public List<GameObject> PosPreview {  get { return posPreviews; } }

    [Header("Combat")]
    private Unit target;

    void Awake()
    {
        SetAgents();
        SetFormation();
    }

    private void SetAgents()
    {
        agents = new List<Agent>();
        posPreviews = new List<GameObject>();

        for (int i = 0; i < unitSize; i++)
        {
            agents.Add(Instantiate(agentPrefab, transform).GetComponent<Agent>());
            posPreviews.Add(Instantiate(positionPreviewPrefab, transform));
            agents[i].GetComponent<Agent>().AssignPositionPreview(posPreviews[i]);
        }

    }

    //For Initialization 
    private void SetFormation()
    {
        //TODO: Improve positioning
        //See SetFormation(V3, V3, V3) 
        for(int i = 0; i < formationRows; i++)
        {
            for(int j = 0; j < unitSize/formationRows; j++)
            {
                Vector3 pos = transform.position + 
                    new Vector3(-(unitSize / formationRows * offset / 2) + offset * j + 1,
                    0,
                    (formationRows * offset / 2) - offset * i - 1);
                agents[i*formationRows + j].transform.position = pos;
                posPreviews[i * formationRows + j].transform.position = pos;
            }
        }
    }

    private Vector3 SetFormation(Vector3 startPos, Vector3 endPos, Vector3 right)
    {
        float distance = Vector3.Distance(startPos, endPos); //Max width of formation
        int agentCount = Agents.Count;

        int rowAgentCount = Mathf.Clamp
            (Mathf.FloorToInt(distance / offset), 
            MinRow(agentCount), 
            MaxRow(agentCount));
        
        int depth = 0;
        for(int j = 0; j < agentCount; depth++)
        {
            //Place last row in center
            if (agentCount - j < rowAgentCount)
            {
                int agentsLeft = agentCount - j;
                float centerOffset = (float)(rowAgentCount - agentsLeft) / 2;
                for (int i = 0; i < agentsLeft; i++, j++)
                {
                    posPreviews[j].transform.rotation = Quaternion.Euler(0, -Vector3.Angle(right, Vector3.right), 0);
                    posPreviews[j].transform.position = new Vector3(startPos.x, 1, startPos.z)
                        + right * offset * (i+centerOffset)
                        + new Vector3(right.z, 0, -right.x) * offset * depth;
                }
                continue;
            }
            for (int i = 0; i < rowAgentCount; i++, j++)
            {
                if (j >= agentCount) break;
                //TODO: Set y-coordinate to ground height
                posPreviews[j].transform.rotation = Quaternion.Euler(0, -Vector3.Angle(right, Vector3.right), 0);
                posPreviews[j].transform.position = new Vector3(startPos.x, 1, startPos.z)
                    + right * offset * i
                    + new Vector3(right.z, 0, -right.x) * offset * depth;
            }
        }
        return new Vector3(startPos.x, 1, startPos.z)
                    + right * offset * (rowAgentCount+1); 
    }

    //startPos is the position of the leftmost agent in first row
    public Vector3 SetFormationPreview(Vector3 startPos, Vector3 endPos, Vector3 right, Vector3 currentRight, bool setTarget = false)
    {
        bool reverse = false;
        //TODO: Fix reverse
        if((Vector2.SignedAngle(right, currentRight) > 90 
            || Vector2.SignedAngle(right, currentRight) < -90) 
            && setTarget)
        {
            Debug.Log("reversed");
            posPreviews.Reverse();
            reverse = true;
        }

        if(setTarget)
        {
            if (reverse)
            {
                for (int i = 0; i < unitSize; i++)
                {
                    agents[i].AssignPositionPreview(posPreviews[unitSize - 1 - i]);
                }
            }
            else
            {
                for (int i = 0; i < unitSize; i++)
                {
                    agents[i].AssignPositionPreview(posPreviews[i]);
                }
            }
            
        }
        Vector3 nextStartPos = SetFormation(startPos, endPos, right);
        if (setTarget)
        {
            agents.ForEach(x => x.Move());
        }
        return nextStartPos;
    }

    private void ReverseFormation()
    {

    }

    //Returns the min number of Agents in the first row of a formation
    //Ensures formation is not deeper than it is wide
    private int MinRow(int agentCount)
    {
        return agentCount > 8 ? 
            Mathf.FloorToInt(Mathf.Sqrt(agentCount)) : 
            Mathf.CeilToInt(Mathf.Sqrt(agentCount));
    }

    //Returns the max number of Agents in the first row of a formation
    //Ensures formation is at max 3 times as wide as its minimal width
    private int MaxRow(int agentCount)
    {
        return MinRow(agentCount) * 3;

    }

    #region Combat
    public void Attack(Unit unit)
    {
        target = unit;
        Debug.Log("Attacking " + unit);
        //TODO: Move towards target, fight
    }
    #endregion

}
