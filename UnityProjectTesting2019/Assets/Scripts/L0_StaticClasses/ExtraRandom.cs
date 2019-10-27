using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Much like the ExtraMath class, this contains extra RNGs

public static class ExtraRandom {
    // Gaussian distributed RNG based on Box-Muller Transform
    // https://stackoverflow.com/questions/218060/random-gaussian-variables
    // http://keyonvafa.com/box-muller-transform/
    // Returning bmt as-is results in returning a value corresponding to a mean of 0 and
    // stddev of 1, or a raw Z-score
    public static float RandomGauss() { 
        float r1 = UnityEngine.Random.value;
        float r2 = UnityEngine.Random.value;
        float bmt = Mathf.Sqrt(-2 * Mathf.Log(r1)) * Mathf.Cos(ExtraMath.Constants.TAU * r2);
        return bmt;
    }

    // Using the Z-score from the zero-param function allows for use with a custom mean and stddev
    public static float RandomGauss(float mean, float stddev) {
        return mean + stddev * RandomGauss();
    }

    // Generates an integer array with random numbers within the range [min, max]
    public static int[] RandomArray(int length, int min, int max) {
        int[] randArray = new int[length];
        for (int i = 0; i < length; i++) randArray[i] = min + Mathf.RoundToInt(UnityEngine.Random.value * max);
        return randArray;
    }

    // Generates a float array with random numbers within the range [min, max]
    public static float[] RandomArray(int length, float min, float max) {
        float[] randArray = new float[length];
        for (int i = 0; i < length; i++) randArray[i] = min + UnityEngine.Random.value * max;
        return randArray;
    }

    // Generates a float array with random numbers within the range [0, 1]
    public static float[] RandomArray(int length) {
        float[] randArray = new float[length];
        for (int i = 0; i < length; i++) randArray[i] = UnityEngine.Random.value;
        return randArray;
    }
}
