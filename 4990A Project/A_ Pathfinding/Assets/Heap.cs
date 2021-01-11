using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Heap <T> where T : IHeapItem<T> //takes type T
{
    T[] items; //create an array of type T called items 
    int currentItemCount; //create a variable to keep track of current item count
    public Heap(int MaxHeapSize) //create a constructor that takes an int called the Max Heap Size
    {
        items = new T[MaxHeapSize]; //items is equal to new array of items with a size of MaxHeapSize 
    }

    public void Add(T item) //method to add items
    {
        item.HeapIndex = currentItemCount; //add item to the count
        items[currentItemCount] = item; //add to the end of items array
        SortUp(item); //call the SortUp method passing in the new item
        currentItemCount++; //increment the current item count

    }

    public T RemoveFirst() //method to remove the first item/node from the heap
    {
        T firstItem = items[0]; //set the first item equal to index 0 of the item array. 
        currentItemCount--; //decrease current item count
        items[0] = items[currentItemCount]; //put the item at the end of the heap to the front.
        items[0].HeapIndex = 0; //set the heapindex of that item to 0
        SortDown(items[0]); //call the SortDown function passing in items[0]
        return firstItem; //return the first item
    }

    public void UpdateItem(T item) //method to change the priority of an item or child nodes, this is when the pathfinding needs to adjust the nodes when a new path is found in the openset and so need to update its fCost 
    {
        SortUp(item); //call SortUp method passing in item/node to increase its priority, you never need to decrease priority in a heap
    }

    public int Count //method to get current heap count 
    {
        get
        {
            return currentItemCount; //return current item count 
        }
    }


    public bool Contains(T item) //method to check if the heap contains a specific item/node
    {
        return Equals(items[item.HeapIndex], item); //use equal function passing in the item in our array and compare that with actual item being passed in to check if they are equal then return a bool value
    }

    void SortDown (T item)
    {
        while (true) //create a loop
        {
            int childIndexLeft = item.HeapIndex * 2 + 1; //the index of the left child nodes for the heap, the mathematical formula is x2+1
            int childIndexRight = item.HeapIndex * 2 + 2; //the index of the right child nodes for the heap, the mathematical formula is x2+2
            int swapIndex = 0; //create integer for swapping index left and right

            if(childIndexLeft < currentItemCount) //if the left child node is less than the current item count then we swap
            {
                swapIndex = childIndexLeft; //set swap to left child index 

                if(childIndexRight < currentItemCount) //if right child index is less than current item count
                {
                    if(items[childIndexLeft].CompareTo(items[childIndexRight]) < 0) //check if the left or right child index has a higher priority and then swap with the one with the higher priority child, if left child has a lower priority swap with right
                    {
                        swapIndex = childIndexRight; //swap with right child, if the left child has lower priority
                    }
                }
                if(item.CompareTo(items[swapIndex]) < 0) //check if the parent node has a lower priority than its highest priority child, then we will swap them.
                {
                    Swap(item, items[swapIndex]); //call swap fn to swap child node with the parent
                }
                else //otherwise the parent is in its correct position and we just exit out of the loop
                {
                    return; //return
                }
            }
            else //if the parent node does not have any child nodes to swap with then its in correct position, then just exit loop
            {
                return;
            }
        }
    }

    void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2; //create an integer for the parent index, this represents the parent node of a heap, mathematically this is x-1/2

        while (true) //create a loop while true
        {
            T parentItem = items[parentIndex]; //the parent item is equal to the items in the items array with an index of parentIndex
            if (item.CompareTo(parentItem) > 0) //if the item compared to the parentItem or parent node is greater than 0 then
            {
                Swap(item, parentItem); //swap item with parent item, aka swap the parent node with the child node.
            }
            else //else
            {
                break; //break out of loop
            }

            parentIndex = (item.HeapIndex - 1) / 2;

        }
    }

    void Swap(T itemA, T itemB) //method to swap items
    {
        items[itemA.HeapIndex] = itemB; //itemA is equal to itemB
        items[itemB.HeapIndex] = itemA; //itemB is equal to itemA
        int itemAIndex = itemA.HeapIndex; //temp variable called itemAIndex is equal to itemA.HeapIndex
        itemA.HeapIndex = itemB.HeapIndex; //set itemA HeapIndex equal to ItemB's 
        itemB.HeapIndex = itemAIndex; //Set itemB to the temp variable


    }

}

public interface IHeapItem<T> : IComparable<T> //create a public interface for the generic T type we have above, also uses an interface called IComparable of type T 
{
    int HeapIndex { //Heap Index 

        get; //get an integer
        set; //set an integer
    }
}
