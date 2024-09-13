using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Agent : MonoBehaviour
{
    //PATHFINDING
    //COMBAT
    //...
    [SerializeField]
    private GameObject positionPreview;

    [SerializeField]
    private NavMeshAgent pathFinder;

    private void Awake()
    {
        pathFinder = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        DebugPositionMatch();
    }

    public void AssignPositionPreview(GameObject positionPreview)
    {
        this.positionPreview = positionPreview;
    }

    //Only Debugging
    private void DebugPositionMatch()
    { 
        Debug.DrawLine(transform.position,
            positionPreview.transform.position,
            Color.cyan);
    }

    public void PreviewActive(bool active)
    {
        positionPreview.SetActive(active);
    }

    public void Move()
    {
        var path = new NavMeshPath();
        pathFinder.CalculatePath(positionPreview.transform.position, path);
        pathFinder.SetPath(path);

        //Temporary workaround to keep units moving towards the target position if it is unaccesible
        if(path.status==NavMeshPathStatus.PathInvalid)
        {
            Debug.LogWarning("Invalid path");
            pathFinder.SetDestination(positionPreview.transform.position);
        }
        //TODO: Face forward when target reached
    }

}
