using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Node : IComparable<Node>
{
  public bool walkable;
  public Vector3 worldPosition;
  public int gridX;
  public int gridY;
  public int fCost=2000000;
  public int gCost=2000000;
  public Node parent; //parent variable 
  int heapIndex;
  
public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY){
	
	walkable = _walkable;
	worldPosition = _worldPos;
	gridX = _gridX;
	gridY = _gridY;	

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

 public int CompareTo(Node other)
        {
            //use gcost as tie breaker
            if(fCost.CompareTo(other.fCost) == 0)
                return gCost.CompareTo(other.gCost);
            else
                return fCost.CompareTo(other.fCost);
        }
}
