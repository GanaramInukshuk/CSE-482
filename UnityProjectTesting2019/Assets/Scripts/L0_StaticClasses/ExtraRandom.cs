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

    //// Same functions as above but with a clamped stddev; the generated z-score is clamped within a
    //// range specified by the two additional parameters
    //public static float RandomGaussWithClamp(float zLower, float zUpper) {
    //    return Mathf.Clamp(RandomGauss(), zLower, zUpper);
    //}

    //public static float RandomGaussWithClamp(float mean, float stddev, float zLower, float zUpper) {
    //    return mean + stddev * RandomGaussWithClamp(zLower, zUpper);
    //}



    // Generates an integer array with random numbers within the range [min, max)
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

    // This is a function that I call a "MagicDie"; turns out that physically, there is a limited number of
    // fair dice, so this function represents a fair die of any number of sides, even if physics won't allow it;
    // it's basically a shorthand for the function within
    // The inner function has an output range (sample space) of [0, sides), or { 0, 1, 2, ... , sides - 1 },
    // so the +1 shifts it so that the sample space is { 1, 2, 3, ... , sides }, like a proper die
    public static int MagicDie(int sides) {
        return UnityEngine.Random.Range(0, sides) + 1;      // Random.Range(1, sides + 1) also works
    }

    // This function will roll multiple MagicDie and return the sum of those dice
    public static int MagicDice(int sides, int quantity) {
        int sidesAbs = Mathf.Abs(sides);
        int quantityAbs = Mathf.Abs(quantity);
        int[] dice = RandomArray(quantityAbs, 1, sidesAbs + 1);
        int sumOfDice = 0;
        foreach (int i in dice) sumOfDice += i;
        return sumOfDice;
    }
}
