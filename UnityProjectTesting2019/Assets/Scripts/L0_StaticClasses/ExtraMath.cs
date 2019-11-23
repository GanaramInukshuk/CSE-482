using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Some extra math stuff that I ended up needing, mostly for statistics
// Any other math function I may need that Mathf doesn't have will end up here

public static class ExtraMath {

    // Constants; in general, the first 6 or 7 DPs get used
    public static class Constants {
        public static float E   => 2.718281828f;    // "The number whose exponential function is the derivative of itself" (quoted from The Simpsons)
        public static float PHI => 1.618033988f;    // The golden ratio
        public static float TAU => 6.283185307f;    // 2 times pi
    }

    // Extra math in general goes here
    public static class General {
        // Arc-cotangent function
        // Because apparently I need an arccot function
        public static float Acot(float f) {
            return Mathf.PI/2 - Mathf.Atan(f);
        }

        // Geometric series; this generates a sequence of the following form:
        // 1/(n^0), 1/(n^1), 1/(n^2), 1/(n^3), etc...
        // Technically a geometric progression if it's finite since this creates
        // a finite sequence; I mispelled base because it's a reserved word
        public static float[] Geometric(float basse, int length) {
            float[] array = new float[Mathf.Abs(length)];
            for (int i = 0; i < array.Length; i++) array[i] = 1 / Mathf.Pow(basse, i);
            return array;
        }

        // Lg function
        // Lg is often used as a shorthand for log-base-2
        public static float Lg(int   number) { return Mathf.Log(number, 2); }
        public static float Lg(float number) { return Mathf.Log(number, 2); }
    }

    // Anything related to combinatorics and maybe discrete math
    public static class Comb {
        // The factorial function
        public static int Factorial(int n) {
            if (n < 0) throw new Exception("[ExtraMath]: Cannot compute factorial of negative value.");
            else if (n == 0 || n == 1) return 1;
            else return Factorial(n-1) * n;
        }

        // Fibonacci number; this finds the nth Fibonacci number given
        // the 0th and 1st numbers are 0 and 1
        public static int Fibonacci(int n) {
            if (n < 0) throw new Exception("[ExtraMath]: Cannot compute Fibonacci number with negative index.");
            else if (n == 0 || n == 1) return n;
            else return Fibonacci(n-1) + Fibonacci(n-2);
        }

        // Lucas numbers; the 0th and 1st numbers are 2 and 1
        public static int Lucas(int n) {
            if (n < 0) throw new Exception("[ExtraMath]: Cannot compute Lucas number with negative index.");
            else if (n == 0) return 2;
            else if (n == 1) return 1;
            else return Lucas(n-1) + Lucas(n-2);
        }
    }

    // Anything related to statistics
    public static class Stats {

        public static float Average(float[] datapoints) {
            if (datapoints.Length == 0) throw new Exception("[ExtraMath]: Divide-by-zero detected.");
            float sum = 0;
            foreach (float f in datapoints) sum += f;
            return sum / datapoints.Length - 1;
        }

        public static float Average(int[] datapoints) {
            if (datapoints.Length == 0) throw new Exception("[ExtraMath]: Divide-by-zero detected.");
            float sum = 0;
            foreach (float f in datapoints) sum += f;
            return sum / datapoints.Length - 1;
        }

        // This is overloaded four times to account for int and float datatypes
        public static float WeightedAverage(int[] weights, int[] datapoints) {
            if (weights.Length != datapoints.Length) throw new Exception("[ExtraMath]: Number of datapoints and weights don't match.");
            float sum = 0;
            float weightSum = 0;
            for (int i = 0; i < weights.Length; i++) {
                sum += weights[i] * datapoints[i];
                weightSum += weights[i];
            }
            if (Mathf.Approximately(weightSum, 0)) throw new Exception("[ExtraMath]: Divide-by-zero detected.");
            return sum / weightSum;
        }

        #region WeightedAverageOverloads
        public static float WeightedAverage(float[] weights, int[] datapoints) {
            if (weights.Length != datapoints.Length) throw new Exception("[ExtraMath]: Number of datapoints and weights don't match.");
            float sum = 0;
            float weightSum = 0;
            for (int i = 0; i < weights.Length; i++) {
                sum += weights[i] * datapoints[i];
                weightSum += weights[i];
            }
            if (Mathf.Approximately(weightSum, 0)) throw new Exception("[ExtraMath]: Divide-by-zero detected.");
            return sum / weightSum;
        }

        public static float WeightedAverage(int[] weights, float[] datapoints) {
            if (weights.Length != datapoints.Length) throw new Exception("[ExtraMath]: Number of datapoints and weights don't match.");
            float sum = 0;
            float weightSum = 0;
            for (int i = 0; i < weights.Length; i++) {
                sum += weights[i] * datapoints[i];
                weightSum += weights[i];
            }
            if (Mathf.Approximately(weightSum, 0)) throw new Exception("[ExtraMath]: Divide-by-zero detected.");
            return sum / weightSum;
        }

        public static float WeightedAverage(float[] weights, float[] datapoints) {
            if (weights.Length != datapoints.Length) throw new Exception("[ExtraMath]: Number of datapoints and weights don't match.");
            float sum = 0;
            float weightSum = 0;
            for (int i = 0; i < weights.Length; i++) {
                sum += weights[i] * datapoints[i];
                weightSum += weights[i];
            }
            if (Mathf.Approximately(weightSum, 0)) throw new Exception("[ExtraMath]: Divide-by-zero detected.");
            return sum / weightSum;
        }
        #endregion

        // Poisson; returns a single value given a count and a mean
        public static float Poisson(float mean, int count) {
            return Mathf.Pow(mean, count) * Mathf.Pow(Constants.E, -mean) / Comb.Factorial(count);
        }

        // Zero-truncated Poisson; also returns a single value given a count and a mean
        public static float PoissonZero(float mean, int count) {
            return Mathf.Pow(mean, count) / (Comb.Factorial(count) * (Mathf.Pow(Constants.E, mean) - 1));
        }
    }

    // Anything remotely related to linear algebra
    // Perhaps one day I can replace this with an actual linear algebra library... maybe...
    // Anyway, this is tailored for use with Unity in that the two main datatypes are int and float
    public static class Linear {

        public static int[] ZeroVector(int length) {
            if (length < 0) throw new Exception("[ExtraMath.Linear]: Cannot create a zero vector of negative length.");
            return new int[length];
        }

        // Overloaded three times; if adding a float vector and an int vector, the float vector should come first
        public static float[] VectorSum(float[] a, float[] b) {
            if (a.Length != b.Length) throw new Exception("[ExtraMath.Linear]: Cannot find sum of two vectors of unequal size.");
            float[] sum = a;
            for (int i = 0; i < a.Length; i++) a[i] += b[i];
            return sum;
        }

        #region VectorSumOverloads
        public static float[] VectorSum(float[] a, int[] b) {
            if (a.Length != b.Length) throw new Exception("[ExtraMath.Linear]: Cannot find sum of two vectors of unequal size.");
            float[] sum = a;
            for (int i = 0; i < a.Length; i++) a[i] += b[i];
            return sum;
        }

        public static int[] VectorSum(int[] a, int[] b) {
            if (a.Length != b.Length) throw new Exception("[ExtraMath.Linear]: Cannot find sum of two vectors of unequal size.");
            int[] sum = a;
            for (int i = 0; i < a.Length; i++) a[i] += b[i];
            return sum;
        }
        #endregion
        
        // I needed something more general than Mathf's dot product
        // Also overloaded three times; if DP-ing a float and int vector, the float vector should come first
        public static float DotProduct(float[] a, float[] b) {
            if (a.Length != b.Length) throw new Exception("[ExtraMath.Linear]: Cannot find dot product of two vectors of unequal size.");
            float dp = 0;
            for (int i = 0; i < a.Length; i++) dp += a[i] * b[i];
            return dp;
        }

        #region DotProductOverloads
        public static float DotProduct(float[] a, int[] b) {
            if (a.Length != b.Length) throw new Exception("[ExtraMath.Linear]: Cannot find dot product of two vectors of unequal size.");
            float dp = 0;
            for (int i = 0; i < a.Length; i++) dp += a[i] * b[i];
            return dp;
        }

        public static int DotProduct(int[] a, int[] b) {
            if (a.Length != b.Length) throw new Exception("[ExtraMath.Linear]: Cannot find dot product of two vectors of unequal size.");
            int dp = 0;
            for (int i = 0; i < a.Length; i++) dp += a[i] * b[i];
            return dp;
        }
        #endregion

        public static float[] ScalarVectorMult(float scalar, float[] vector) {
            float[] product = vector;
            for (int i = 0; i < product.Length; i++) product[i] *= scalar;
            return product;
        }

        #region ScalarVectorMultOverloads
        public static int[] ScalarVectorMult(int scalar, int[] vector) {
            int[] product = vector;
            for (int i = 0; i < product.Length; i++) product[i] *= scalar;
            return product;
        }

        public static float[] ScalarVectorMult(int scalar, float[] vector) {
            float[] product = vector;
            for (int i = 0; i < product.Length; i++) product[i] *= scalar;
            return product;
        }

        public static float[] ScalarVectorMult(float scalar, int[] vector) {
            float[] product = new float[vector.Length];
            for (int i = 0; i < product.Length; i++) product[i] = scalar * vector[i];
            return product;
        }

        public static int[] ScalarVectorMultToInt(float scalar, int[] vector) {
            int[] product = new int[vector.Length];
            for (int i = 0; i < product.Length; i++) product[i] = Mathf.RoundToInt(scalar * vector[i]);
            return product;
        }
        #endregion

        // This can be defined using matrix multiplication, but I'm short-circuiting that process:
        // a = { a1, a2, a3 }, b = { b1, b2, b3 }, c = AVP(a, b) = { a1*b1, a2*b2, a3*b3 }
        // Also overloaded three times; if AVP-ing a float and int vector, the float vector should come first
        public static float[] AlignedVectorProduct(float[] a, float[] b) {
            if (a.Length != b.Length) throw new Exception("[ExtraMath.Linear]: Cannot find aligned vector product of two vectors of unequal size.");
            float[] avp = a;
            for (int i = 0; i < a.Length; i++) avp[i] *= b[i];
            return avp;
        }

        #region AlignedVectorProducOverloads
        public static float[] AlignedVectorProduct(float[] a, int[] b) {
            if (a.Length != b.Length) throw new Exception("[ExtraMath.Linear]: Cannot find aligned vector product of two vectors of unequal size.");
            float[] avp = a;
            for (int i = 0; i < a.Length; i++) avp[i] *= b[i];
            return avp;
        }

        public static int[] AlignedVectorProduct(int[] a, int[] b) {
            if (a.Length != b.Length) throw new Exception("[ExtraMath.Linear]: Cannot find aligned vector product of two vectors of unequal size.");
            int[] avp = a;
            for (int i = 0; i < a.Length; i++) avp[i] *= b[i];
            return avp;
        }
        #endregion

        // If you ever need to write a Matrix Multiplication function, here's a tip on parallelization:
        // https://jamesmccaffrey.wordpress.com/2012/04/22/matrix-multiplication-in-parallel-with-c-and-the-tpl/
        // https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/how-to-write-a-simple-parallel-for-loop
    }
}