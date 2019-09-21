using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommercialScripts {

    public static class Constants {
        // Employment goes by discrete units of 8 employees per unit of labor
        public static readonly int LaborUnit = 8;

        // An array of employment sizes; each store size supports a different size employment capacity
        // Employment is by labor units
        public static int[] EmploymentSizes => new int[] { 1, 2, 4, 8, 16, 32 };

        // An enum for the different types of commercial specializations
        // I am counting commercial offices to be a distinct type (like with C:S)
        // A brief explanation of each specialization:
        // - Grocery: grocery stores (self-explanatory)
        // - Retail: retail stores (clothing, furniture, etc)
        // - Food: restaurants, including fast food
        // - Service: other commercial services
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