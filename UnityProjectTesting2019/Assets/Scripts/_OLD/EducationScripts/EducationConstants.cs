//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//// This class is a place to offload constants needed for the education simulator and its classes
//// (no pun intended)

//// This also includes counts necssary for functionality of very important counters

//namespace EducationScripts {

//    public static class Constants {
//        // Enums for different kinds of education; should be at least "K-12"
//        // Most of the world uses primary-secondary subdivisions (K-6, then 7-12), which may include
//        // further subdivisions; however, even the US is variable with its subdivisions (6th grade
//        // may be considered middle school, middle and high school may be combined, high school may
//        // start at 10th grade instead, etc)
//        // I grew up with K-6 elementary, 7-8 middle school, and 9-12 high school, and SC4 and C:S
//        // divide education into primary and secondary; I've never seen a game implement preschool
//        // outside of mods/assets; because I divide population into buckets of 5-year ranges, education
//        // effectively spans pop buckets 5-9, 10-14, and part of 15-19, so I'm tempted to use three
//        // subdivisions (4 if preschool is included)
//        // The EducationSimulator stops at college-level education, but this "college-level" is a
//        // form of 2-year college (effectively simulating K-14); going beyond that may require its own
//        // simulator (UniversitySimulator?) with its own tiers (associate, bachelor, master, doctorate,
//        // postdoc?)
//        public enum EDUCATIONLEVEL { PRESCHOOL, ELEMENTARY, MIDDLE, HIGH, COLLEGE };
    
//        // Number of classrooms for one instance of a school for each type
//        // Schools tend to increase in size the higher the education level, at least what I've seen
//        // There are two schools of thought on how to deal with larger school sizes of the same type:
//        // 0 - Don't bother (C:S, SC4's predecessors?)
//        // 1 - Introduce more than one size of school (SC4)
//        // 2 - Upgradable schools: schools have a base size and expansions add to that size (SC2013?)
//        public static int[] SchoolSizes => new int[] { 10, 20, 30, 40, 50 };

//        // Number of education levels; use this for loops
//        public static readonly int NumEducationLevels = SchoolSizes.Length;
//    }
//}