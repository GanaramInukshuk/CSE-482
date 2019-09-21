using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Functionality:
// - This class takes two numbers (n and s) and divides n by s to return a number p; n is
//   the number of things to divide into, s is the size of the partitions, and p is the number
//   of resulting partitions; these numbers are saved for however long they're needed
// - This is a barebones manager; only the minimum amount of functionality is included
//   To use this class, instantiate it directly, have it be a member of a larger class, or
//   if more functionality is needed, have it be inherited and have the inheriting class
//   add additional functionality

public class PartitionManager {

    private int _partitions = 0;
    private int _partitionSize = 1;
    
    public int Partitions {
        private set { _partitions = Mathf.Max(0, value); }
        get => _partitions;
    }

    public int PartitionSize {
        private set { _partitionSize = Mathf.Max(1, value); }     // Minimum partitition size of 1 is a failsafe to prevent div by zero
        get => _partitionSize;
    }

    public void Generate(int n, int p) {
        if (n < 0) return;
        PartitionSize = p;
        Partitions = Mathf.CeilToInt((float)n / PartitionSize);
    }



    //public void IncrementSize(int n) { _partitionSize.IncrementCount(n); }
}