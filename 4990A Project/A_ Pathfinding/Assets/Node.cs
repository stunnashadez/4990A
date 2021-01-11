using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
  public bool walkable;
  public Vector3 worldPosition;
  public int gridX;
  public int gridY;

  public int gCost;
  public int hCost;
  public Node parent; //parent variable 
  int heapIndex;
  
public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY){
	
	walkable = _walkable;
	worldPosition = _worldPos;
	gridX = _gridX;
	gridY = _gridY;	

	}

public int fCost
    {
        get	{
			return gCost + hCost; //get fCost by calculating g + h costs
		}
    }

public int HeapIndex //implement all the items in the iHeapIndex Interface from Heap.cs
    {
        get
        {
            return heapIndex; //return heapIndex 
        }
        set
        {
            heapIndex = value; //set heapIndex value
        }
    }

public int CompareTo(Node nodeToCompare) //compare the fcosts of the 2 nodes 
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost); //compare the fCosts of the 2 nodes 

        if(compare == 0) //if compare is equal to 0 or in other words the 2 fcosts of the nodes is equal then
        {
            compare = hCost.CompareTo(nodeToCompare.hCost); //then we compare it to the hCost as a tie breaker
        }
        return -compare; //return 1 if the item has a higher priority than the one we are comparing it to, CompareTo returns 1 if the integer is higher, but since the nodes are reversed (since we are finding the path not traveling towards it) then we return negative compare instead of regular compare 
    }


}
