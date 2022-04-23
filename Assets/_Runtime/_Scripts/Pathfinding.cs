using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

public class Pathfinding : MonoBehaviour
{
    public bool debug;
    [SerializeField] private GridGraph graph;

    public delegate float Heuristic(Transform start, Transform end);

    public GridGraphNode startNode;
    public GridGraphNode goalNode;
    public GameObject openPointPrefab;
    public GameObject closedPointPrefab;
    public GameObject pathPointPrefab;
    public PathfindingManager pf;
    public bool Cluster = false;
    public GridGraphNode[] topNodes;
    public GridGraphNode[] botLeftNodes;
    public GridGraphNode[] botRightNodes;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Node")))
            {
                if (startNode != null && goalNode != null)
                {
                    startNode = null;
                    goalNode = null;
                    ClearPoints();
                }

                if (startNode == null)
                {
                    startNode = hit.collider.gameObject.GetComponent<GridGraphNode>();
                }
                else if (goalNode == null)
                {
                    goalNode = hit.collider.gameObject.GetComponent<GridGraphNode>();

                    // Decides which heuristic to use
                    List<GridGraphNode> path = Cluster ? FindClusterPath(startNode, goalNode) : FindManhattanPath(startNode, goalNode);
                }
            }
        }
    }

    
    
    public List<GridGraphNode> FindManhattanPath(GridGraphNode start, GridGraphNode goal, Heuristic heuristic = null, bool isAdmissible = true)
    {
        if (graph == null) return new List<GridGraphNode>();

        // if no heuristic is provided then set heuristic = 0
        if (heuristic == null) heuristic = (Transform s, Transform e) => 0;
        
       
        
        List<GridGraphNode> path = null;
        bool solutionFound = false;


        // dictionary to keep track of our path (came_from)
        Dictionary<GridGraphNode, GridGraphNode> pathDict = new Dictionary<GridGraphNode, GridGraphNode>();
        pathDict.Add(start, null);
        
        //list for open and closed nodes
        List<GridGraphNode> openSet = new List<GridGraphNode>();
        HashSet<GridGraphNode> closedSet = new HashSet<GridGraphNode>();
        openSet.Add(startNode);

        while (openSet.Count > 0) {
            GridGraphNode node = openSet[0];
            
            // gets the lowest value node
            for (int i = 1; i < openSet.Count; i ++) {
                if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost) {
                    if (openSet[i].hCost < node.hCost)
                        node = openSet[i];
                }
            }
            
            //add node to closed set and removes it from the open set 
            openSet.Remove(node);
            closedSet.Add(node);

            if (node == goal && isAdmissible)
            {
                solutionFound = true;
                break;
            }
            else if (closedSet.Contains(goal))
            {
                // early exit strategy if heuristic is not admissible (try to avoid this if possible)
                float gGoal = goal.gCost;
                bool pathIsTheShortest = true;

                foreach (GridGraphNode entry in openSet)
                {
                    if (gGoal > entry.gCost)
                    {
                        pathIsTheShortest = false;
                        break;
                    }
                }

                if (pathIsTheShortest) break;
            }

            // calculates the cost for the g and h (the cost to get to the node  from the start and the amount for the node to get to the goal respectively) 
            foreach (GridGraphNode neighbour in graph.GetNeighbors(node)) {
                if (closedSet.Contains(neighbour)) {
                    continue;
                }

                float newCostToNeighbour = node.gCost + calcDist(node, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = calcDist(neighbour, goalNode);
                    
                    //remove node from open set and add the and the path to the dictionary so each node knows where it cam from
                    pathDict[neighbour] = node;
                    
                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        // if the closed list contains the goal node then we have found a solution
        if (!solutionFound && closedSet.Contains(goal))
            solutionFound = true;

        //retraces path from the goal to the start
        if (solutionFound)
        {
            path = new List<GridGraphNode>();
            path.Add(goalNode);
            GridGraphNode currentNode = goalNode;
            while (path[path.Count - 1] != startNode)
            {
                currentNode = pathDict[currentNode];
                path.Add(currentNode);
            }

            // reverse the path since we started adding nodes from the goal 
            path.Reverse();
        }

        if (debug)
        {
            ClearPoints();

            List<Transform> openListPoints = new List<Transform>();
            foreach (GridGraphNode node in openSet)
            {
                openListPoints.Add(node.transform);
            }
            SpawnPoints(openListPoints, openPointPrefab, Color.magenta);

            List<Transform> closedListPoints = new List<Transform>();
            foreach (GridGraphNode entry in closedSet)
            {
                //GridGraphNode node = (GridGraphNode) entry.Key;
                //if (solutionFound && !path.Contains(node))
                    closedListPoints.Add(entry.transform);
            }
            SpawnPoints(closedListPoints, closedPointPrefab, Color.red);

            if (solutionFound)
            {
                List<Transform> pathPoints = new List<Transform>();
                foreach (GridGraphNode node in path)
                {
                    pathPoints.Add(node.transform);
                }
                SpawnPoints(pathPoints, pathPointPrefab, Color.green);
            }
        }
        pf.WalkPath(path);
        return path;
    }

    
      public List<GridGraphNode> FindClusterPath(GridGraphNode start, GridGraphNode goal, Heuristic heuristic = null, bool isAdmissible = true)
    {
        if (graph == null) return new List<GridGraphNode>();

        // if no heuristic is provided then set heuristic = 0
        if (heuristic == null) heuristic = (Transform s, Transform e) => 0;
        
        List<GridGraphNode> path = null;
        bool solutionFound = false;


        // dictionary to keep track of our path (came_from)
        Dictionary<GridGraphNode, GridGraphNode> pathDict = new Dictionary<GridGraphNode, GridGraphNode>();
        pathDict.Add(start, null);
        
        List<GridGraphNode> openSet = new List<GridGraphNode>();
        HashSet<GridGraphNode> closedSet = new HashSet<GridGraphNode>();
        openSet.Add(startNode);

        /*
         * 1
         * top right room
         * z > 5.2
         *
         * 2
         * bottome left
         * z < -1.26
         * x < -.1
         *
         * 3
         * bottom right
         * z < .4
         * x > 3.4
         */
        
        //set up for the clusters in each room
        float xlimit = 0;
        float zlimit = 0;
        int quadrant = 0;
        bool inRegion = false;
        if (goalNode.transform.position.z > 5)
        {
            xlimit = 0;
            zlimit = 5;
            quadrant = 1;
            Debug.Log("quadrant " + quadrant);
        }
        if (goalNode.transform.position.x < -1.4 && goalNode.transform.position.z < -4.1)
        {
            zlimit = -4.1f;
            xlimit = -1.4f;
            quadrant = 2;
            Debug.Log("quadrant " + quadrant);
        }
        if (goalNode.transform.position.x > 3.4 && goalNode.transform.position.z < .4)
        {
            xlimit = 3.4f;
            zlimit = .4f;
            quadrant = 3;
            Debug.Log("quadrant " + quadrant);
        }

        float xPos = 0; 
        float zPos = 0; 
        
        while (openSet.Count > 0) {
            GridGraphNode node = openSet[0];
            
            zPos = node.transform.position.z;
            xPos = node.transform.position.x;
            
            //Debug.Log(node.name + "     " + xPos + "    " + zPos);
            
            if (inRegion)
            {
                zPos = node.transform.position.z;
                xPos = node.transform.position.x;
            
                //Debug.Log(node.name + "     " + xPos + "    " + zPos);
                
                openSet.Remove(node);
                closedSet.Add(node);
                // early exit
                if (node == goal && isAdmissible)
                {
                    solutionFound = true;
                    break;
                }
                else if (closedSet.Contains(goal))
                {
                    // early exit strategy if heuristic is not admissible (try to avoid this if possible)
                    float gGoal = goal.gCost;
                    bool pathIsTheShortest = true;

                    foreach (GridGraphNode entry in openSet)
                    {
                        if (gGoal > entry.gCost)
                        {
                            pathIsTheShortest = false;
                            break;
                        }
                    }

                    if (pathIsTheShortest) break;
                }
                
                //this part essentially handles the djikstra's algorithm in the cluster
                List<GridGraphNode> neighbors = graph.GetNeighbors(node);
                foreach (GridGraphNode n in neighbors)
                {
                    float movement_cost = 1;
                    if(closedSet.Contains(n))
                        continue;

                    n.gCost = node.gCost + calcDist(node, n);

                    if(!closedSet.Contains(n))
                    {
                        openSet.Add(n);
                        pathDict[n] = node;
                        closedSet.Add(n);
                    }
                    
                    
                }
            }
            else
            {
                zPos = node.transform.position.z;
                xPos = node.transform.position.x;
            
                //Debug.Log(node.name + "     " + xPos + "    " + zPos);
                
                //check if the current node being checked is in the cluster, if so switch to cluster 
                if (quadrant == 1)
                {
                    if (zPos > zlimit)
                    {
                        inRegion = true;
                        Debug.Log("top");
                        openSet.Clear();
                        openSet.Add(node);
                    }
                }
                else if (quadrant == 2)
                {
                    if (xPos < xlimit && zPos < zlimit)
                    {
                        inRegion = true;
                        Debug.Log("bottom left");
                        openSet.Clear();
                        openSet.Add(node);
                    }
                }
                else if (quadrant == 3)
                {
                    if (xPos > xlimit && zPos < zlimit)
                    {
                        inRegion = true;
                        Debug.Log("bottom right");
                        openSet.Clear();
                        openSet.Add(node);
                    }
                }
                
                
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost)
                    {
                        if (openSet[i].hCost < node.hCost)
                            node = openSet[i];
                    }
                }

                openSet.Remove(node);
                closedSet.Add(node);

                if (node == goal && isAdmissible)
                {
                    solutionFound = true;
                    break;
                }
                else if (closedSet.Contains(goal))
                {
                    // early exit strategy if heuristic is not admissible (try to avoid this if possible)
                    float gGoal = goal.gCost;
                    bool pathIsTheShortest = true;

                    foreach (GridGraphNode entry in openSet)
                    {
                        if (gGoal > entry.gCost)
                        {
                            pathIsTheShortest = false;
                            break;
                        }
                    }

                    if (pathIsTheShortest) break;
                }


                foreach (GridGraphNode neighbour in graph.GetNeighbors(node))
                {
                    if (closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    float newCostToNeighbour = node.gCost + calcDist(node, neighbour);
                    if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newCostToNeighbour;
                        neighbour.hCost = calcDist(neighbour, goalNode);
                        pathDict[neighbour] = node;
                        //neighbour.parent = node;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }
        }

        // if the closed list contains the goal node then we have found a solution
        if (!solutionFound && closedSet.Contains(goal))
            solutionFound = true;

        if (solutionFound)
        {
            path = new List<GridGraphNode>();
            path.Add(goalNode);
            GridGraphNode currentNode = goalNode;
            while (path[path.Count - 1] != startNode)
            {
                currentNode = pathDict[currentNode];
                path.Add(currentNode);
            }
            path.Reverse();
        }

        if (debug)
        {
            ClearPoints();

            List<Transform> openListPoints = new List<Transform>();
            foreach (GridGraphNode node in openSet)
            {
                openListPoints.Add(node.transform);
            }
            SpawnPoints(openListPoints, openPointPrefab, Color.magenta);

            List<Transform> closedListPoints = new List<Transform>();
            foreach (GridGraphNode entry in closedSet)
            {
                //GridGraphNode node = (GridGraphNode) entry.Key;
                //if (solutionFound && !path.Contains(node))
                    closedListPoints.Add(entry.transform);
            }
            SpawnPoints(closedListPoints, closedPointPrefab, Color.red);

            if (solutionFound)
            {
                List<Transform> pathPoints = new List<Transform>();
                foreach (GridGraphNode node in path)
                {
                    pathPoints.Add(node.transform);
                }
                SpawnPoints(pathPoints, pathPointPrefab, Color.green);
            }
        }
        pf.WalkPath(path);
        return path;
    }

    
    private void SpawnPoints(List<Transform> points, GameObject prefab, Color color)
    {
        for (int i = 0; i < points.Count; ++i)
        {
#if UNITY_EDITOR
            // Scene view visuals
            points[i].GetComponent<GridGraphNode>()._nodeGizmoColor = color;
#endif

            // Game view visuals
            GameObject obj = Instantiate(prefab, points[i].position, Quaternion.identity, points[i]);
            obj.name = "DEBUG_POINT";
            obj.transform.localPosition += Vector3.up * 0.5f;
        }
    }
    
    float calcDist(GridGraphNode a, GridGraphNode b)
    {
        float distX = Mathf.Abs(a.transform.position.x - b.transform.position.x);
        float distZ = Mathf.Abs(a.transform.position.z - b.transform.position.z);
        return distX + distZ;
    }
    
    private void ClearPoints()
    {
        foreach (GridGraphNode node in graph.nodes)
        {
            for (int c = 0; c < node.transform.childCount; ++c)
            {
                if (node.transform.GetChild(c).name == "DEBUG_POINT")
                {
                    Destroy(node.transform.GetChild(c).gameObject);
                }
            }
        }
    }

}
