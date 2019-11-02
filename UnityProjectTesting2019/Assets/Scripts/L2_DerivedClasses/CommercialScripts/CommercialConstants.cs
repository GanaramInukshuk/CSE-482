using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For reference:
// The powers of 2, starting at 1:                      { 1, 2, 4, 8, 16, 32, 64, 128, ... }
// The powers of 2, starting at 1, but multiplied by 3: { 3, 6, 12, 24, 48, 96, 192, ... }
// The previous two sets combined into one: { 1, 2, 3, 4, 6, 8, 12, 24, 32, 48, 64, 96, 128, 192, ... }

// An altered version of the set above, but 3 is excluded:
// { 1, 2, 4, 8, 12, 26, 24, 32, 48, 64, ... }

namespace CommercialScripts {

    public static class Constants {
        // Employment sizes (IE, employment capacity for a commercial building) are more or less
        // one of the sequences described above, multiplied by a labor unit size described here
        public static readonly int LaborUnit = 8;

        // An array of employment sizes; each store size supports a different size employment capacity
        // Employment is by labor units
        // If using a LaborUnit size of 16 and powers of 2 mixed with PO2's times 3, these are the effective capacities:
        // { 32, 48, 64, 96, 128, 192 }
        public static int[] LaborUnitCounts => new int[] { 1, 2, 4, 6, 8, 12 };
        public static int[] EmploymentSizes => ExtraMath.Linear.ScalarVectorMult(LaborUnit, LaborUnitCounts);

        // An enum for the different types of commercial specializations
        // I am counting commercial offices to be a distinct type (like with C:S)
        // A brief explanation of each specialization:
        // - Grocery: grocery stores (self-explanatory)
        // - Retail: retail stores (clothing, furniture, etc)
        // - Food: restaurants, including fast food
        // - Service: other commercial services, such as door-to-door services
        // - Automotive: gas stations and auto shops (and if EVs/alt-fuels become a thing, charging/refueling stations)
        // Possible specializations for future implementation:
        // - Tourism: gift shops and related services (not hotels, but maybe inns)
        // - Leisure: bars and clubs, perhaps spas/salons
        // - Tech: any place that sells electronic and other related items (phones, computers, smart appliances)
        public enum EMPLOYMENTSPECIALIZATION { GROCERY, RETAIL, FOOD, SERVICE, AUTOMOTIVE };

        // An array of the default commerce weights
        // Since I expect this to vary wildly between cities (think a place made for tourism vs a truck stop town), I
        // arbitrarily chose these values to be the default:
        // - Grocery: 3 parts
        // - Retail: 2 parts
        // - Food (think restaurants): 2 parts
        // - Supplementary: 2 parts
        // - Automotive: 1 part
        // - Everything else: 0 parts
        public static float[] DefaultEmploymentWeights => new float[] { 0.30f, 0.20f, 0.20f, 0.20f, 0.10f };

        // Effective lengths of datavectors; for use with loops and savedata helper
        public static readonly int StoreVectorLength      = EmploymentSizes.Length;
        public static readonly int EmploymentVectorLength = DefaultEmploymentWeights.Length;

        // For use with the savedata helper that handles jagged arrays
        public static int[] ExpectedVectorLengths => new int[] {
            StoreVectorLength     ,
            EmploymentVectorLength,
        };
    }
}