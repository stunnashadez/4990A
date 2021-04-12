using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Threading;
public class BFS : MonoBehaviour
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
        gui.isRunning = true;                               //Lock gui
        Stopwatch sw = new Stopwatch();                     //create a stopwatch to time the speed of the implementation of the algorithm using heap method 
        sw.Start();                                         //start timer before starting pathfinding 

        //need to convert world positions into node  
        Node startNode = grid.NodeFromWorldPoint(startPos);         //create a start node
        Node targetNode = grid.NodeFromWorldPoint(targetPos);       //create a target node
        HashSet<Node> closeSet = new HashSet<Node>();               //use a hashset for our closed set
        Queue<Node> search = new Queue<Node>();                     //Our seach queue 
        List<Node> path = new List<Node>();                         //create a list of nodes called path that will later be used to return final path
        grid.path = new List<Node>();                               //Initialize grid.path 

        //Initialization
        search.Enqueue(startNode);
        closeSet.Add(startNode);
        grid.path = new List<Node>();
        while (search.Count != 0) //while queue is not empty
        {

            Node currentNode = search.Dequeue();

            //code that controls visulization of pathfinding
            if (gui.isVisualEnabled)
            {
                //Add current node to the grid path (which renders using gizmos)
                grid.path.Add(currentNode);

                //We only delay execution for every 15 nodes traversed, this is to not slow down pathfinding completely.
                if (closeSet.Count % 15 == 0)
                    yield return new WaitForSeconds(gui.visualdelay);
            }

            if (currentNode == targetNode) //corrent path has been found
            {
                sw.Stop(); //end timer when path has been found 

                //Backtrack using parent nodes
                path.Add(currentNode);
                while (currentNode != startNode)
                {
                    path.Add(currentNode.parent);
                    currentNode = currentNode.parent;
                }

                //Update the grid path so that the correct path is rendered with gizmos
                grid.path = path;

                //Only display results if visualization is disabled (Since visualization applies delay, elapsed time would not be accurate)
                if (gui.isVisualEnabled == false)
                    gui.results = "Path found: " + sw.ElapsedMilliseconds + " ms " + " | " + "Distance : " + grid.path.Count;

                //Release lock 
                gui.isRunning = false;
                yield break;
            }

            foreach (Node neighbour in grid.GetNeighbours(currentNode)) //for each neighbour of the current node 
            {
                if (neighbour.walkable && !closeSet.Contains(neighbour)) //if the neighbouring node walkable and not in the closed set
                {
                    closeSet.Add(neighbour);
                    search.Enqueue(neighbour);
                    neighbour.parent = currentNode;
                }

            }
        }
        gui.results = "Target not Reachable!";
        gui.isRunning = false;
    }
}
