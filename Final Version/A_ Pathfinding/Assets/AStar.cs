using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;
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
            gui.grid.CreateGrid();
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

        //need to convert world positions into node  
        Node startNode = grid.NodeFromWorldPoint(startPos);     //create a start node
        Node targetNode = grid.NodeFromWorldPoint(targetPos);   //create a target node
        HashSet<Node> closeSet = new HashSet<Node>();           //use a hashset for our closed set

        Heap<Node> openSet = new Heap<Node>();                  //Min Heap is used to keep track of Nodes with minimum fCost
        HashSet<Node> toExplore = new HashSet<Node>();          //Hashset is used to check if a Node has been explored
        List<Node> path = new List<Node>();                     //create a list of nodes called path that will later be used to return final path
        grid.path = new List<Node>();                           //Initialize grid.path


        //Initilization 
        openSet.Enqueue(startNode);
        toExplore.Add(startNode);
        startNode.gCost=0;
        startNode.fCost=GetDistance(startNode, targetNode);
        
        while (openSet.Count != 0) //while priority queue is not empty
        {
            Node currentNode = openSet.Dequeue();
            toExplore.Remove(currentNode);

            //code that controls visulization of pathfinding
            if(gui.isVisualEnabled){
                //Add current node to the grid path (which renders using gizmos)
                grid.path.Add(currentNode);

                //We only delay execution for every 15 nodes traversed, this is to not slow down pathfinding completely.
                if (openSet.Count % 15 == 0)
                    yield return new WaitForSeconds(gui.visualdelay);
            }
           
            if(currentNode == targetNode) //corrent path has been found
            {
                sw.Stop(); //end timer when path has been found 

                //Backtrack using parent nodes
                path.Add(currentNode);
                while(currentNode != startNode){
                    path.Add(currentNode.parent);
                    currentNode = currentNode.parent;
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

            foreach (Node neighbour in grid.GetNeighbours(currentNode)) //for each neighbour of the current node 
            {
                if(neighbour.walkable) //if the neighbouring node is walkable
                {
                    int alt = currentNode.gCost+1;          //check tentative gcost
                    if(alt<neighbour.gCost){                
                        neighbour.parent = currentNode;     //update neighbour gcost and fcost
                        neighbour.gCost = alt;
                        neighbour.fCost = alt+ GetDistance(neighbour, targetNode);
                        if(!toExplore.Contains(neighbour)){ //check hashset in O(1) to see if node has already been found
                            openSet.Enqueue(neighbour);
                            toExplore.Add(neighbour);
                        }
                    }
                    
                }

            }
        }
        gui.results = "Target not Reachable!";
        gui.isRunning = false;
    }

    Node getMinElement(List<Node> elements){
        Node min = elements[0];
        foreach(Node element in elements){
            if (element.fCost<min.fCost)
                min = element;
        }
        return min;
    }
    int GetDistance(Node nodeA, Node nodeB) //Method to find the distance between any given two nodes
    {
        //This is Chebyshev distance formula
        return Math.Max(Math.Abs(nodeB.gridX-nodeA.gridX),Math.Abs(nodeB.gridY-nodeA.gridY));
    }
}
