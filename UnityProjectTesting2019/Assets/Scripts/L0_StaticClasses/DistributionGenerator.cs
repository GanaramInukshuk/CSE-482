using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using System;

// This is a generalized version of the PopulationGenrator

// Ported from purely C# version and converted for use with Unity
// Conversions done:
// - Unity's Mathf instead of Math; Mathf also contains an epsilon value and approximately function
// - Doubles converted back to floats
// - Unity's RNG used instead

// It's basically this video but instead of generating a distribution from a histogram, we're
// generating a histogram out of a distribution: https://www.youtube.com/watch?v=bPFNxD3Yg6U

// I have since further generalized the functionality of this class so that it can also generate
// a probability distribution out of a histogram; basically, there are three sub-classes
// - The Verify class which contains verification functions (self-explanatory)
// - The Histogram class which mainly generates a histogram out of probabilities and a sample size
// - The Probability class which generates probabilities out of a histogram array

// How the Verify subclass works:
// - All it does is contain helper functions that ensure certain properties are met: all elements
//   are positive and, for probabilities, all the elements add up to 1

// How the Histogram subclass works:
// - A discrete probability distribution is provided by an outside source; EG suppose it's for a
//   coin flip, so 49.99% heads, 49.99% tails, and .02% on its side, so the array of probabilities
//   (or vector of weights as I call it) looks like this:
//   double[] coinFlipWeights = { 0.4999, 0.4999, 0.0002 };  // 49.99, 49.99, and 0.02 percent

//   Side note: for this to work properly, all the probabilities need to sum up to 1

// - A Generate() function takes in an (ideally unsigned) integer and multiplies it by each weight,
//   storing the resulting histogram in a second array; EG inputting 150 returns this:
//   returnedValues = { 76, 73, 1 };  // 76 heads, 73 tails, and 1 side-landing

//   Side note: there is another way to generate a histogram that is computationally more expensive
//   and, depending on the RNG, introduces random(ish) fluctuations

// How the Probability subclass works:
// - A histogram denoting the occurrences of certain events is provided by an outside source; EG
//   suppose it's for 100 coin flips from an experiment and the outcomes are heads, tails, and 
//   landing on its side, so the array of events (the histogram) looks like this:
//   int[] coinFlipOutcome = { 42, 57, 1 };   // For heads, tails, and landing on the side

// - A Generate() function takes in this histogram and generates a probability array by summing
//   up all the events, dividing each event by the total number of events, then returning it; EG:
//   returnedValues = { 0.42, 0.57, 0.01 }; 42%, 57%, and 1%

//   Side note: for small event sizes, it's possible that the experiment results may be far off
//   from the expected probabilities; in this example, there are far more tails than heads when both
//   numbers should be equal, or off by one since a coin landed on its side in this case

// Key similarities between both classes:
// - Both classes have a Generate() and Reconcile() function; the Reconcile() function tries to make
//   an invalid probability distribution or histogram valid (IE, no negative numbers for either, sum
//   of all elements is equal to 1 for probability)

// Key differences:
// - The Histogram subclass has two versions of the Generate() function and contains two helper
//   functions, one for finding the sum of events and another for finding the rounding error caused
//   by one of the Generate() functions; events in a histogram always sum up to a positive integer
// - The Probability subclass only has one Generate() function since all the probabilities always
//   add up to 1; this also makes the Probability subclass simpler than the Histogram subclass

// Time complexities (worst-case):
// - Nearly every function: O(length)
// - Histogram.GenerateByWeights(): O((size+2) * length)
// - Histogram.Merge(): O(size+2)

public class DistributionGen {

    // Debug functions
    // Note: these return a string rather than directly printing to Unity's console; to print,
    // call the function saving the result to a string, then print out the string
    public static class Debug {
        // For printing a histogram's datapoints and the datapoints for each data "bucket"
        public static string HistToString(int[] hist) {
            string outputString = "Length: " + hist.Length + " Total: " + Histogram.SumOfElements(hist) + " Hist:";
            foreach (int i in hist) outputString += " " + i;
            return outputString;
        }

        // For printing out a vector of probabilities
        public static string ProbToString(float[] prob) {
            string outputString = "Length: " + prob.Length + " Prob:";
            foreach (float f in prob) outputString += " " + f;
            return outputString;
        }
    }

    // Verification functions
    // I separated out the verification functions to avoid a possible circular reference
    // It shouldn't happen, but I wanted to be safe
    public static class Verify {
        // This just determines whether a histogram is valid (IE, no negatives)
        public static bool HistValid(int[] hist) {
            bool isPositive = true;
            foreach (int i in hist) if (i < 0) isPositive = false;
            return isPositive;
        }

        // This function verifies that the array of probabilities adds up to 1
        // Due to inevitable rounding errors inherent with floating-point, instead of ensuring the sum 
        // is exactly 0, just make sure the rounding errors are no bigger than a sufficiently small
        // epsilon; note that Unity's math library has an epsilon built in
        public static bool ProbSum(float[] prob) {
            // LINQ version
            //return Mathf.Abs(prob.Sum() - 1.0) < 1e-6;

            // No-LINQ version
            float sum = 0.0f;
            foreach (float f in prob) sum += f;
            //return Math.Abs(sum - 1.0f) < 1e-6f;        // Purely C# version
            return Mathf.Approximately(sum, 1.0f);      // Unity version; 
        }

        // ...verifies that each probability is positive
        public static bool ProbPositive(float[] prob) {
            bool isPositive = true;
            foreach (float f in prob) if (f < 0) isPositive = false;
            return isPositive;
        }

        // Combination of last two Verify functions in one function
        public static bool ProbValid(float[] prob) {
            return ProbSum(prob) && ProbPositive(prob);
        }
    }

    // Functions specific to histograms
    public static class Histogram {
        // This is the easiest way to generate a histogram given a sample size and probabilities, but
        // introduces rounding errors, especially with really small sample sizes (rule of thumb with
        // statistics is that you need a minimum sample size of 30; results will inevitable vary)
        // Returns an array of zeros as a fail-safe
        // Time complexity is O(prob.Length)
        public static int[] GenerateByMult(int size, float[] prob) {
            int[] hist = new int[prob.Length];    // Apparently, this array is { 0, ... , 0 }
            if (Verify.ProbValid(prob) && size > 0) 
                for (int i = 0; i < hist.Length; i++) hist[i] = Mathf.RoundToInt(size * prob[i]);
            return hist;
        }

        // Same as GenerateByMult but the result is rounded down; results in a histogram that's always
        // slightly less than the expected amount of datapoints
        // A rough estimate of the time complexity is something like O(prob.Length)
        public static int[] GenerateByRoundDown(int size, float[] prob) {
            int[] hist = new int[prob.Length];
            if (Verify.ProbValid(prob) && size > 0) 
                for (int i = 0; i < hist.Length; i++) hist[i] = Mathf.FloorToInt(size * prob[i]);
            return hist;
        }

        // Generates a histogram that at most has as many datapoints as the length of the probability array
        // Only use this in conjunction with compensating for any discrepancy
        // Operation:
        // - Make a blank histogram
        // - Iterating through the probabilities, add 1 to the element corresponding to the first largest 
        //   weight, then second-largest, and so on; repeat for how large the size is
        // - Return the histogram
        // Time complexity is O(prob.Length^2)
        // Notes:
        // - With the round-down method, discrepancies tend to be no larger than about half the number of weights,
        //   but to be on the safe side, I'm capping this to only generate histograms that have as many datapoints
        //   as weights
        // - In other words, the largest histogram this will generate will be an array full of 1's
        // - This function won't work at all if the size exceeds the number of weights (prob.Length) and instead
        //   returns an empty histogram
        public static int[] GenerateByDescending(int size, float[] prob) {
            int   loopAmt = prob.Length;
            int[] hist    = new int[loopAmt];
            if (Verify.ProbValid(prob) && 0 < size && size <= loopAmt) {
                float[] probCopy = prob;        // Make a copy of the probabilities
                for (int i = 0; i < size; i++) {
                    int largest = 0;            // Index of largest element; initially 0
                    for (int j = 0; j < loopAmt; j++) if (probCopy[j] >= probCopy[largest]) largest = j;
                    hist[largest] = 1;          // Set the corresponding element to 1
                    probCopy[largest] = 0;      // Set the largest element to 0 so it doesn't get "captured" again
                }
            }
            return hist;
        }

        // This method guarantees that rounding errors never happen, but is computationally more expensive 
        // the bigger the sample size is and introduces fluctuations that, 1, may or may not be desirable,
        // and 2, may not be sufficiently random depending on the RNG used
        // Use weighted probabilities to determine which element in the histogram vector to increment
        // by one, and repeat this for however large the sample size is
        // Also returns an array of zeros as a fail-safe
        // Time complexity is O(size);
        public static int[] GenerateByWeights(int size, float[] prob) {
            int[] hist = new int[prob.Length];
            if (Verify.ProbValid(prob) && size > 0) {

                // Replace this RNG with a different one if need be
                //Random rng = new Random(Guid.NewGuid().GetHashCode());

                for (int i = 0; i < size; i++) {
                    //double randomNumber = rng.NextDouble();
                    float randomNumber = UnityEngine.Random.value;
                    for (int j = 0; j < prob.Length; j++) {
                        if (randomNumber < prob[j]) {
                            hist[j]++;
                            break;
                        } else randomNumber -= prob[j];
                    }
                }
            }
            return hist;
        }

        // This will attempt to reconcile an array of probabilities by doing the following:
        // - Copy the probabilities into a duplicate array
        // - Assign each element in the array its absolute value (to eliminate negatives)
        //   and return the resulting array
        // Returns an array of zeros as a failsafe
        public static int[] Reconcile(int[] hist) {
            int[] newHist = new int[hist.Length];
            for (int i = 0; i < hist.Length; i++) newHist[i] = Mathf.Abs(hist[i]);
            return newHist;
        }

        // Rounding errors will more or less happen with the GenerateHistByMult() function, meaning
        // the resulting histogram will have slightly more or less than the expected sample size; this
        // function calculates how far off the resulting histogram is from the expected size; positive
        // for a larger histogram than expected, negative for smaller than expected
        // If using LINQ, this is not necessary; just do hist.Sum() - size;
        public static int Discrepancy(int size, int[] hist) {
            int sum = 0;
            foreach (int i in hist) sum += i;
            return (Verify.HistValid(hist)) ? sum - size : -size;
        }

        // This just finds the sum of all the elements in a histogram array
        // If using LINQ, this is not necessary; just do hist.Sum();
        public static int SumOfElements(int[] hist) {
            int sum = 0;
            if (Verify.HistValid(hist)) foreach (int i in hist) sum += i;
            return sum;
        }

        // This finds the sum of a subarray of consecutive elements in a histogram
        // Only use this if it makes sense to do so (EG, probabilities of events on a continuum)
        // The FOR loop operates on the range [lower, upper] within the range [0, hist.Length), given that
        // 0 <= start <= stop < hist.Length; should return 0 as a failsafe
        public static int SumOfElements(int[] hist, int lower, int upper) {
            int sum = 0;
            bool validRange = 0 <= lower && lower <= upper && upper < hist.Length;
            if (Verify.HistValid(hist) && validRange) for (int i = lower; i < upper + 1; i++) sum += hist[i];
            return sum;
        }

        // This is for combining two histograms together
        // Only do this if it makes sense to do so (if both histograms represent the same events)
        // EG if experiment 1 yielded hist1 = { 45, 51, 0 } for coin flips and experiment 2 yielded
        // hist2 = { 174, 178, 1 } for coin flips, then it may make sense to combine the two histograms
        // together; however, if hist2 didn't account for coins landing on its edge ( { 174, 178 } )
        // or represented something else entirely (like dice rolls, political affiliations, or ice cream
        // preferences), then it wouldn't make sense to combine the two histograms
        public static int[] Merge(int[] hist1, int[] hist2) {
            int[] newHist = new int[hist1.Length];
            if (Verify.HistValid(hist1) && Verify.HistValid(hist2) && hist1.Length == hist2.Length)
                newHist = ExtraMath.Linear.VectorSum(hist1, hist2);
            return newHist;
        }
    }

    // Functions specific to probabilities
    public static class Probability {
        // There's only one way to generate probabilities from a histogram, unlike the other way
        // around where there are two methods to generate a histogram from a probability array; this
        // basically makes a copy of the histogram that's a float[], then passes it to
        // ReconcileProbabilities(), which does the necessary steps of generating probabilities
        // Returns an array of zeros as a fail-safe
        public static float[] GenerateFromHist(int[] hist) {
            float[] prob = new float[hist.Length];
            if (Verify.HistValid(hist)) {
                for (int i = 0; i < hist.Length; i++) prob[i] = hist[i];
                prob = Reconcile(prob);
            }
            return prob;
        }

        // This will attempt to reconcile an array of probabilities by doing the following:
        // - Copy the probabilities into a duplicate array
        // - Assign each element in the array with its absolute value (to eliminate negatives)
        // - Sum up all the weights, then divide each element by the weight sum (to ensure sum of
        //   probabilities sum up to 1; this is readily apparent if it were an int[] instead)
        // If all the probabilities add up to 0, set the first element in the array to 1 with
        // all other elements after it to 0 as a failsafe
        public static float[] Reconcile(float[] prob) {
            float[] newProb = new float[prob.Length];
            float weightSum = 0;
            for (int i = 0; i < prob.Length; i++) weightSum += newProb[i] = Mathf.Abs(prob[i]);
            if (weightSum == 0) {
                newProb[0] = 1;
                for (int i = 1; i < newProb.Length; i++) newProb[i] = 0;
            } else for (int i = 0; i < newProb.Length; i++) newProb[i] /= weightSum;
            return newProb;
        }
    }

    // This class generates probabilities out of actual functions
    // TODO: Switch out failsafes with exception handling
    public static class Function {

        // This generates Poisson-based probabilities for the range [lower, upper], given a mean
        // The probabilities generated represent the probability that x events occur, with x being in the
        // range [lower, upper]; since [lower, upper] is within the range [0, infinity) (Poisson probability
        // mass function continues all the way to infinity), it may be necessary to reconcile the resulting
        // array of probabilities (this function doesn't do that in case there's a reason not to)
        public static float[] Poisson(float mean, int lower, int upper) {
            float[] prob = new float[Mathf.Abs(upper - lower) + 1];
            if (lower <= upper && lower >= 0)
                for (int i = 0; i < prob.Length; i++) prob[i] = ExtraMath.Stats.Poisson(mean, i + lower);
            return prob;
        }

        // Poisson with reconcile
        public static float[] PoissonReconcile(float mean, int lower, int upper) {
            return Probability.Reconcile(Poisson(mean, lower, upper));
        }

        // Zero-truncated Poisson distribution for a range of [lower, upper]
        public static float[] PoissonZero(float mean, int lower, int upper) {
            float[] prob = new float[Mathf.Abs(upper - lower) + 1];
            if (lower <= upper && lower > 0)
                for (int i = 0; i < prob.Length; i++) prob[i] = ExtraMath.Stats.PoissonZero(mean, i + lower);
            return prob;
        }

        // Zero-truncated Poisson with reconcile
        public static float[] PoissonZeroReconcile(float mean, int lower, int upper) {
            return Probability.Reconcile(PoissonZero(mean, lower, upper));
        }
    }
}