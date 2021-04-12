using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Threading;
public class DFS : MonoBehaviour
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

    public void Run(){
        if(gui.isVisualEnabled){
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
        gui.isRunning = true;           //Lock gui
        Stopwatch sw = new Stopwatch(); //create a stopwatch to time the speed of the implementation of the algorithm using heap method 
        sw.Start();                     //start timer before starting pathfinding 

       
        bool foundWalkable = false;     //variable is used to detect when to backtrack

        //need to convert world positions into node  
        Node startNode = grid.NodeFromWorldPoint(startPos);     //create a start node
        Node targetNode = grid.NodeFromWorldPoint(targetPos);   //create a target node
        HashSet<Node> closeSet = new HashSet<Node>();           //use a hashset for our closed set
        Stack<Node> current_path = new Stack<Node>();           //stack that keeps track of path
        List<Node> path = new List<Node>();                     //create a list of nodes called path that will later be used to return final path
        grid.path = new List<Node>();                           //initialize grid.path

        //intilization
        current_path.Push(startNode);
        
        while (current_path.Count != 0) //while stack is not empty
        {
            Node currentNode = current_path.Peek(); //set current node to the first element in the stack

            //code that controls visulization of pathfinding
            if(gui.isVisualEnabled){

                //Add current node to the grid path (which renders using gizmos)
                grid.path.Add(currentNode);

                //We only delay execution for every 15 nodes traversed, this is to not slow down pathfinding completely.
                if (closeSet.Count % 15 == 0)
                    yield return new WaitForSeconds(gui.visualdelay);
            }

            closeSet.Add(currentNode); //add the currentNode to the closedSet

            if(currentNode == targetNode) //corrent path has been found
            {
                sw.Stop(); //end timer when path has been found 

                //return entire stack
                while(current_path.Count != 0){
                    path.Add(current_path.Pop());
                }

                //Update the grid path so that the correct path is rendered with gizmos
                grid.path = path;

                //Only display results if visualization is disabled (Since visualization applies delay, elapsed time would not be accurate)
                if(gui.isVisualEnabled == false)
                    gui.results = "Path found: " + sw.ElapsedMilliseconds + " ms " + " | " + "Distance : " + grid.path.Count;


                //Release lock 
                gui.isRunning = false;
                yield break;
            }

            foundWalkable= false; 
            foreach (Node neighbour in grid.GetNeighbours(currentNode)) //for each neighbour of the current node 
            {
                if(neighbour.walkable && !closeSet.Contains(neighbour)) //if the neighbouring node is not walkable or in the closed set
                {
                    current_path.Push(neighbour);   //push to stack
                    foundWalkable = true;           //we found a node, therefore no need to backtrack
                    break;
                }

            }
            //If we didn't find a walkable node, backtrack.
            if(!foundWalkable){

                //handles visualizatio for backtracking
                if(gui.isVisualEnabled){
                    List<Node> tmp = grid.path;
                    tmp.Remove(current_path.Peek());
                    tmp.RemoveAt(tmp.Count -1);
                    grid.path = tmp;
                }
                
                current_path.Pop();
                }
        }
        gui.results = "Target not Reachable!";
        gui.isRunning = false;
    }
    
}
