using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class AStar : MonoBehaviour
{

    public Transform seeker, target;

    Gridd grid; //reference to our grid 

    void Awake()
    {
        grid = GetComponent<Gridd>();
    }

    void Update()
    {
    }

    public void Run()
    {
        //if visual pathfinding option is enabled, first find path normally, then show visualization
        //else just run pathfinding normally
        //this is so that pathfinding time isn't affected by visualization delay.

        if (gui.isVisualEnabled)
        {
            gui.isVisualEnabled = false;
            StartCoroutine(FindPath(seeker.position, target.position));
            gui.isVisualEnabled = true;
            StartCoroutine(FindPath(seeker.position, target.position));
        }
        else
            StartCoroutine(FindPath(seeker.position, target.position));
    }

    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)  //start position and target position
    {
        gui.isRunning = true;               //algorithm has started, tell the gui we are running so that no other algo's can be started
        Stopwatch sw = new Stopwatch();     //create a stopwatch to time the speed of the implementation of the algorithm using heap method 
        sw.Start();                         //start timer before starting pathfinding 

        //need to convert world positions into node  
        Node startNode = grid.NodeFromWorldPoint(startPos);   //create a start node
        Node targetNode = grid.NodeFromWorldPoint(targetPos); //create a target node

        Heap<Node> openSet = new Heap<Node>(grid.MaxSize); //create a list of nodes for our open set
        HashSet<Node> closeSet = new HashSet<Node>();      //use a hashset for our closed set
        openSet.Add(startNode);                            //add the starting node to the open set
        grid.path = new List<Node>();
        while (openSet.Count > 0)                          //enter a loop, while openSet count is greater than 0
        {
            Node currentNode = openSet.RemoveFirst(); //set current node to the first element in the openset
            closeSet.Add(currentNode);                //add the currentNode to the closedSet

            //This is visualization code
            if (gui.isVisualEnabled)
            {
                grid.path.Add(currentNode);                                 //Add current node to visual representation grid
                if (closeSet.Count % 15 == 0)                               //This makes sure visualization is only updated every 15 nodes (save resources)
                    yield return new WaitForSeconds(gui.visualdelay);       //delay for as much as visualization speed is set in the gui
            }
            if (currentNode == targetNode) //corrent path has been found
            {
                sw.Stop();                           //end timer when path has been found 
                RetracePath(startNode, targetNode);  //call the RetracePath passing in the startNode we want to start at and the targetNode that we want to finish at

                //Only show result if we are running in non-visualization mode
                if (gui.isVisualEnabled == false)
                    gui.results = "Path found: " + sw.ElapsedMilliseconds + " ms " + " | " + "Distance : " + grid.path.Count;

                gui.isRunning = false;  //tell gui we are done
                yield break;
            }
            
            foreach (Node neighbour in grid.GetNeighbours(currentNode)) //for each neighbour of the current node 
            {
                if (!neighbour.walkable || closeSet.Contains(neighbour)) //if the neighbouring node is not walkable or in the closed set
                {
                    continue; //then skip to the next neighbour 
                }

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour); //variable for the cost of the new path to neighbour equal to current node's gCost + the cost of getting from the current node to the neighbour
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) //if cost of the new path to neighbour is less than neighbour's current gCost or if the neighbour node is not in the current open set
                {
                    //set the fCost of the neighbour by calculating the gCost and hCost 
                    neighbour.gCost = newMovementCostToNeighbour; //gCost of the neighbour is newMovementCostToNeighbour 
                    neighbour.hCost = GetDistance(neighbour, targetNode); //hCost is the distance between neighbour node and the target node 

                    neighbour.parent = currentNode; //set the parent of the neighbour to the current node

                    if (!openSet.Contains(neighbour))
                    { //check if the neighbour is in the openSet, if not add it in.
                        openSet.Add(neighbour); //add neighbour to the openSet
                    }
                }
            }
        }
        gui.results = "Target not Reachable!";
        gui.isRunning = false;
    }

    void RetracePath(Node startNode, Node endNode) //method to retrace our path from the parent node at which we will have our optimal path/solution using the retraced path
    {
        List<Node> path = new List<Node>(); //create a list of nodes called path
        Node currentNode = endNode; //create a node called current node which will be equal to the endNode, so that we can use this to trace the path back

        while (currentNode != startNode) //so while this currentNode that is equal to the endNode does not equal the startNode
        {
            path.Add(currentNode); //then add that node to the list of nodes of the path
            currentNode = currentNode.parent; //then the currentNode goes back to the parent to restart the loop so we can continue to retrace steps 
        }
        path.Reverse(); //Since we are retracing our steps back, the path is going backwards, so we need to use the reverse() function to reverse the direction so get the path in the correct direction 
        path.Add(startNode);
        grid.path = path; //use to visual our path
    }

    int GetDistance(Node nodeA, Node nodeB) //Method to find the distance between any given two nodes
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX); //an integer for the distance on the x-axis
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY); //an integer for the distance on the Y-axis

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY); //double check this to confirm 13:58 part3 and part1
        return 14 * dstY + 10 * (dstY - dstX);
    }
}
