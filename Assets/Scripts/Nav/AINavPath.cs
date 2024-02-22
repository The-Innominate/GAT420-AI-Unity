using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AINavAgent))]
public class AINavPath : MonoBehaviour
{
	public enum ePathType
	{
		WAYPOINT,
        DIJKSTRA,
        ASTAR
    }

	[SerializeField] ePathType pathType;
	[SerializeField] AINavAgent agent;
	List<AINavNode> path = new List<AINavNode>();


	public AINavNode targetNode { get; set; } = null;
	public Vector3 destination 
	{ 
		get 
		{ 
			return (targetNode != null) ? targetNode.transform.position : Vector3.zero;
		}
		set
		{

			if (pathType == ePathType.WAYPOINT) { targetNode = agent.GetNearestAINavNode(value); }
			else if (pathType == ePathType.DIJKSTRA || pathType == ePathType.ASTAR) 
			{
				AINavNode startNode = agent.GetNearestAINavNode();
				AINavNode endNode = agent.GetNearestAINavNode(value);

				GeneratePath(startNode, endNode);
				targetNode = startNode;
			}
			
		}
	}

	public bool HasTarget()
	{
		return targetNode != null;
	}

	public AINavNode GetNextAINavNode(AINavNode node)
	{
        if (pathType == ePathType.WAYPOINT)
        {
			return node.GetRandomNeighbor();
        }
		if (pathType == ePathType.DIJKSTRA || pathType == ePathType.ASTAR)
		{
            return GetNextPathAINavNode(node);
        }
		return null;
	}

	private void GeneratePath(AINavNode startNode, AINavNode endNode)
	{
		AINavNode.ResetNodes();
		if (pathType == ePathType.DIJKSTRA)
		{
            AINavDijkstra.Generate(startNode, endNode, ref path);
        }
        else if (pathType == ePathType.ASTAR)
		{
            AINavAStar.Generate(startNode, endNode, ref path);
        }
		//AINavDijkstra.Generate(startNode, endNode, ref path);
	}

	private AINavNode GetNextPathAINavNode(AINavNode node)
	{
		if (path.Count == 0) { return null; }
		int index = path.FindIndex(x => x == node);
		
		if(index + 1 == path.Count || index == -1) { return null; }
		AINavNode nextNode = path[index + 1];

		return nextNode;
	}

    private void OnDrawGizmosSelected()
    {
        if (path.Count == 0) return;

        var pathArray = path.ToArray();

        for (int i = 1; i < path.Count - 1; i++)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(pathArray[i].transform.position + Vector3.up, 1);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(pathArray[0].transform.position + Vector3.up, 1);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(pathArray[pathArray.Length - 1].transform.position + Vector3.up, 1);
    }
}
