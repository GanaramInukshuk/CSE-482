//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using SimulatorInterfaces;

//// Don't attach to a regular old GameObject; instead have it be a member of a larger class 
//// that's attached to a UI or empty GameObject

//// Functionality:
//// - The EmploymentManager takes in a count of employees and a distribution of specializations and
////   generates a histogram for use with other managers

//namespace CommercialScripts {

//    public interface IEmployment {
//        //int this[int i]          { get; }
//        int TotalEmployment      { get; }
//        int GroceryEmployment    { get; }
//        int RetailEmployment     { get; }
//        int FoodEmployment       { get; }
//        int ServiceEmployment    { get; }
//        int AutomotiveEmployment { get; }
//        int[] EmploymentVector   { get; }
//    }

//    public sealed class EmploymentManager : BasicManager, IEmployment {
//        // Getters for each commercial specialization
//        public int TotalEmployment      => DistributionGen.Histogram.SumOfElements(DataVector);
//        public int GroceryEmployment    => DataVector[0];
//        public int RetailEmployment     => DataVector[1];
//        public int FoodEmployment       => DataVector[2];
//        public int ServiceEmployment    => DataVector[3];
//        public int AutomotiveEmployment => DataVector[4];
//        public int[] EmploymentVector   => DataVector;

//        // Constructor
//        public EmploymentManager() : base(Constants.EmploymentVectorLength) { }

//        // This Generate function needs to scalar-vector multiply the resulting datavector by the size of
//        // an employment unit
//        public new void Generate(int n, float[] weights) {
//            base.Generate(n, weights);
//            //DataVector = ExtraMath.Linear.ScalarVectorMult(Constants.LaborUnit, DataVector);
//        }

//        // Extra Generate() function uses default weights
//        public void Generate(int n) {
//            Generate(n, Constants.DefaultEmploymentWeights);
//        }

//        // Overridden debug function
//        public override string GetDebugString() {
//            return "[EmploymentManager]: " + DistributionGen.Debug.HistToString(DataVector);
//        }
//    }
//}