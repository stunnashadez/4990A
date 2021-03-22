using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class Gridd : MonoBehaviour
{

	public bool onlyDisplayPathGizmos;
	public LayerMask unwalkableMask;
	public Vector2 gridWorldSize;
	public float nodeRadius;
	Node[,] grid;

	float nodeDiameter;
	int gridSizeX, gridSizeY;

	void Start() {

	nodeDiameter = nodeRadius * 2;
	gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
	gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
	CreateGrid();

	}

	public int MaxSize //method to get the max size of the heap
    {
        get
        {
			return gridSizeX * gridSizeY; //get the max size of the heap
        }
    }
	public void CreateGrid()
    {
		grid = new Node[gridSizeX, gridSizeY];
		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y/2; //get left edge of world and bottom left corner 

		for(int x = 0; x < gridSizeX; x++)
        {
			for (int y = 0; y < gridSizeY; y++)
            {	
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);  //as x increases we will go through the node diameter around the world. 
				bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask)); //collision check, checksphere returns true if there is a collision, so if there is a collision return set walkable to opposite to that. 
				grid[x, y] = new Node(walkable, worldPoint, x, y); //create a new node, start to populate our grid with new nodes. 
            }
		}
    }
 
	public List<Node> GetNeighbours(Node node) //method to return a list of neighbouring nodes 
    {
		List<Node> neighbours = new List<Node>(); //create a list of nodes 

		//create a loop that will search around the node in a 3x3 block for neighbouring nodes  
		for (int x = -1; x <= 1; x++) 
        {
			for(int y = -1; y <= 1; y++)
            {
				if (x == 0 && y == 0) //when x, y is equal to 0 its in the center of that block, so we skip in iteration
					continue;

				//check if its inside of the grid
				int checkX = node.gridX + x; 
				int checkY = node.gridY + y;

				if(checkX >= 0 && checkX < gridSizeX && checkY >=0 && checkY < gridSizeY) //if its within the grid
                {
					neighbours.Add(grid[checkX, checkY]); //add the node as a neighbour
                }
			}
        }
		return neighbours; //now return this list of neighbouring nodes
    }


	public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
		float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
		float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
		percentX = Mathf.Clamp01(percentX); //clamp the position so its within the grid, in other words if the worldposition is outside the grid it clamps it within it so you do not get an invalid index or error
		percentY = Mathf.Clamp01(percentY); //clamp the position so its within the grid, in other words if the worldposition is outside the grid it clamps it within it so you do not get an invalid index or error

		int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
		return grid[x, y];

	}

	public List<Node> path; //create a list of nodes called path

	void OnDrawGizmos() {
		Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x,1,gridWorldSize.y));
		if (onlyDisplayPathGizmos)
		{
			if (path != null)
			{
				foreach (Node n in path)
				{
					Gizmos.color = Color.black; //then we set the color of that node n to black to distinguish it from the rest so we can see the pathfinding line 
					Gizmos.DrawWireCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
				}
			}
		}
		else
		{
			if (grid != null)
			{
				foreach (Node n in grid)
				{
					Gizmos.color = (n.walkable) ? Color.white : Color.red;
					if (path != null) //if the path is not null or there is a path available
						if (path.Contains(n)) //and if the path contains the node we are looking at currently called n
							Gizmos.color = Color.black; //then we set the color of that node n to black to distinguish it from the rest so we can see the pathfinding line 
					Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
				}
			}
		}
	}
}
