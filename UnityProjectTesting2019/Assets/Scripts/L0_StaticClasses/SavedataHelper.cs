using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is a static class that holds a bunch of helper scripts for savedata

public static class SavedataHelper {
    // This converts a vector (array) of the wrong length into one of the expected length
    // by either padding or truncating a copy of the array
    // EG the mismatched array is { 1, 2, 3, 4, 5, 6 }
    // Possibility 1: if the expected length is 4, then the function returns { 1, 2, 3, 4 }
    // Possibility 2: if the expected length is 8, then the function returns { 1, 2, 3, 4, 5, 6, 0, 0 }
    // Possibility 3: if the expected length matches array.Length, then the original array is returned
    public static int[] LoadMismatchedVector(int[] mismatchedVector, int expectedLength) {
        if (mismatchedVector.Length != expectedLength) {
            int[] returnVector = new int[expectedLength];
            int loopAmt = Mathf.Min(mismatchedVector.Length, expectedLength);
            for (int i = 0; i < loopAmt; i++) returnVector[i] = mismatchedVector[i];
            return returnVector;
        } else return mismatchedVector;
    }

    public static float[] LoadMismatchedVector(float[] mismatchedVector, int expectedSize) {
        if (mismatchedVector.Length != expectedSize) {
            float[] returnVector = new float[expectedSize];
            int loopAmt = Mathf.Min(mismatchedVector.Length, expectedSize);
            for (int i = 0; i < loopAmt; i++) returnVector[i] = mismatchedVector[i];
            return returnVector;
        } else return mismatchedVector;
    }

    // This is a more advanced version of the original function in that it loads jagged arrays
    // (given an array of expected lengths, one length for each vector), then loads the arrays in the
    // jagged array
    // Worst-case operating time is O(n^2), but loading and saving saves is expected to take a while
    // when dealing with really complex games
    // Steps:
    // 0 - Create a jagged array with the expected number of arrays
    // 1 - Copy the arrays from the mismatched vector into the jagged array
    // 2 - For each array in the jagged array, use the LoadMismatchedVector() function on them
    public static int[][] LoadMismatchedVector(int[][] mismatchedVector, int[] expectedLength) {
        int [][] returnVector = new int[expectedLength.Length][];
        if (mismatchedVector.Length != expectedLength.Length) {
            int loopAmt = Mathf.Min(mismatchedVector.Length, expectedLength.Length);
            for (int i = 0; i < loopAmt; i++) returnVector[i] = mismatchedVector[i];
        }
        for (int i = 0; i < returnVector.Length; i++) returnVector[i] = LoadMismatchedVector(mismatchedVector[i], expectedLength[i]);
        return returnVector;
    }
}
