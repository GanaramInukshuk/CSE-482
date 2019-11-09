//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace EducationScripts {

//    public static class Constants {
//        // Seats per classroom
//        public static readonly int SeatsPerClassroom = 32;

//        // Classrooms per school; the education ID for the school manager is used as the index
//        // for this array, and that element is used for the ratio (classrooms per school) for the
//        // hidden bicounter in the school manager
//        // Effective capacities in seats: 600, 1200, 1800
//        // For community college, proposed capacity is 80 classrooms/school (2400 seats)
//        // 30 * { 30, 40, 50 } => { 900, 1200, 1500 }
//        // 30 * { 24, 36, 48 } => { 720, 1080, 1440 }
//        // 32 * { 24, 36, 48 } => { 768, 1152, 1536 }
//        public static int[] ClassroomsPerSchool => new int[] { 24, 36, 48 };

//        // Enum for school type
//        public enum EDUCATIONLEVEL { ELEMENTARY, MIDDLE, HIGH };
//    }
//}