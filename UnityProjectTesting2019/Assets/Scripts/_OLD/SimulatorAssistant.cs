//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System;

//// This is a collection of simulator assistant scripts that processes data going from simulator
//// to simulator in some way; this is due to each simulator developed in a standalone fashion

//// Alternatively, I can write an overloaded function for each simulator that takes in another
//// simulator as a parameter

//public static class SimulatorAssistant {

//    public static class Education {
//        // This takes in the population breakdown from the ResidentialSimulator and returns
//        // an array of students; the EduSim has five levels of education accounted for:
//        // Infant -> 1/5th of this will be the preschool student count (Pre-K)
//        // Child -> All of this will be the elementary student count (About K-4)
//        // Teen1 -> All of this will be the middle school student count (About 5-9)
//        // Teen2 -> 4/5ths of this will be the high school student count (About 10-12)
//        // Adult populations -> 1/10th, 1/15th, and 1/20th(?) of each population will
//        // be the college student population; this includes part of the other adult populations
//        public static int[] K14Students(ResidentialScripts.IPopulation pop) {
//            int educationTypes = Enum.GetValues(typeof(EducationScripts.Constants.EDUCATIONLEVEL)).Length;
//            int[] students = new int[educationTypes];

//            students[0] = Mathf.CeilToInt(pop.InfantPopulation * 0.200f);
//            students[1] = pop.ChildPopulation;
//            students[2] = pop.Teen1Population;
//            students[3] = Mathf.CeilToInt(pop.Teen2Population * 0.800f);
//            students[4] = Mathf.CeilToInt(pop.YoungAdultPopulation * 0.100f) +
//                          Mathf.CeilToInt(pop.AdultPopulation * 0.067f);

//            return students;
//        }
//    }
//}
