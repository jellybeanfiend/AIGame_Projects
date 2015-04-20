// Sevena Skeels
// Homework 2
// CAP4053

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIClass
{
    public class Heap<T> where T : IComparable<T>
    {
        private List<T> heapArray;

        public Heap()
        {
            heapArray = new List<T>();
        }

        public bool isEmpty()
        {
            return heapArray.Count == 0;
        }

        public void insert(T elem)
        {
            heapArray.Add(elem);
            int index = heapArray.Count - 1;
            bubbleUp(index);

        }

        private void bubbleUp(int elemIndex)
        {
            // base case 1: are we at the root of the tree?
            if (elemIndex == 0)
                return;

            // base case 2: is the heap invariant true?
            int parentIndex = getParentIndex(elemIndex);

            T parentElem = heapArray[parentIndex];
            T elem = heapArray[elemIndex];
            
            if (parentElem.CompareTo(elem) <= 0) 
                return;

            // Recursive case: Swap elem with parent
            heapArray[parentIndex] = elem;
            heapArray[elemIndex] = parentElem;

            bubbleUp(parentIndex);

        }

        private int getParentIndex(int elemIndex)
        {
            if (elemIndex % 2 == 0)
                elemIndex--;
            return elemIndex / 2;
        }

        private void bubbleDown(int elemIndex)
        {
            int leftChildIndex = getLeftChildIndex(elemIndex);
            int rightChildIndex = getRightChildIndex(elemIndex);

            // Base Case 1: No Children
            if (leftChildIndex > heapArray.Count - 1)
                return;
            T leftChild = heapArray[leftChildIndex];
            T elem = heapArray[elemIndex];

            int elemToSwitchIndex = leftChildIndex;

            if (rightChildIndex > heapArray.Count - 1)
            {
                // Base Case 2: is the heap invariant true?
                if (leftChild.CompareTo(elem) >= 0)
                    return;
            }
            else // There is a right child
            {
                T rightChild = heapArray[rightChildIndex];
               
                // Base Case 2: is the heap invariant true?
                if (leftChild.CompareTo(elem) >= 0 && rightChild.CompareTo(elem) >= 0)
                    return;

                elemToSwitchIndex = leftChild.CompareTo(rightChild) < 0 ? leftChildIndex : rightChildIndex;
            }

            // Recursive Step: Switch with smallest child
            heapArray[elemIndex] = heapArray[elemToSwitchIndex];
            heapArray[elemToSwitchIndex] = elem;

            bubbleDown(elemToSwitchIndex);
        }

        private int getLeftChildIndex(int parentIndex)
        {
            return 2 * parentIndex + 1;
        }

        private int getRightChildIndex(int parentIndex)
        {
            return 2 * parentIndex + 2;
        }

        public T peek()
        {
            return heapArray[0];
        }

        public List<T> slice(int num)
        {
            List<T> result = new List<T>();

            if (heapArray.Count < num)
                num = heapArray.Count;

            for (int i = 0; i < num; i++)
            {
                    result.Add(remove());
            }
            return result;
        }

        public T remove()
        {
            T smallestElem = heapArray[0];
            int lastIndex = heapArray.Count - 1;

            // Place last element into top of heap
            heapArray[0] = heapArray[lastIndex];
            heapArray.RemoveAt(lastIndex);

            bubbleDown(0);

            return smallestElem;
        }
    }

}
