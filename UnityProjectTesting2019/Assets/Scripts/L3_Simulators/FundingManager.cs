using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// If I'm to do this all over again, I'd write an event system

public class FundingManager {

    // Costs for everything
    // - BaseZoningCost is a base cost for a building of an occupancy of 1
    // - BaseCivicCeatCost is how much a seat or opening in a civic building costs per week
    // - BaseCivicMultiplier is used to calculate the cost of building a new civic
    //   structure; that cost is the max number of seats in the new building times the base seat cost
    //   times the multiplier; so an elementary school with 600 seats will have a cost of 2400
    // - Building sizes greater than 1 incur a discount, incentivizing the construction of high density structures
    // - Demolition costs are the construction cost times some demolition multiplier; note that
    //   the multiplier is a float that ideally should be a percentage of the construction cost, and
    //   that percentage may be negative so that the player can recoup construction costs
    // - BaseTaxRevenue is how much a zoning occupant gives in tax revenue
    // NOTE: Revenues are to be added, costs are to be subtracted
    public static class Constants {
        public static int   BaseZoningCost    = 200;
        public static int[] BaseCivicSeatCost     = { 10, 16 };      // Again, so I can make healthcare more of an endgame feature
        public static int[] BaseCivicMultiplier   = {  8, 12 };      // This is an array of multipliers that correspond to education and health; I basically wanna make healthcare cost more for the sake of gameplay
        public static float HighDensityMultiplier = 0.875f;
        public static float DemolitionMultiplier  = -0.5f;
        public static int   BaseTaxRevenue = 6;
    }

    // Alternative getters for constants
    public int   ConstBaseZoningCost        => Constants.BaseZoningCost       ;
    public int[] ConstBaseCivicSeatCost     => Constants.BaseCivicSeatCost    ;
    public int[] ConstBaseCivicMultiplier   => Constants.BaseCivicMultiplier  ;
    public float ConstHighDensityMultiplier => Constants.HighDensityMultiplier;
    public float ConstDemolitionMultiplier  => Constants.DemolitionMultiplier ;
    public int   ConstBaseTaxRevenue        => Constants.BaseTaxRevenue       ;

    // Setter-getter for funds
    public int Funds { set; get; } = 0;

    // References to text labels
    private Text _textFunds;
    private Text _textIncome;

    // Constructor; accepts initial funds and the text labels
    public FundingManager(int initialFunds, Text textFunds, Text textIncome) {
        Funds = initialFunds;
        _textFunds = textFunds;
        _textIncome = textIncome;
    }

    public bool ConstructZoning(int numBuildings, int bldgSize) {
        int constructionCost = numBuildings * CalculateZoningConstructionCost(bldgSize);
        int newFunds = Funds - constructionCost;
        if (newFunds >= 0) {
            Funds = newFunds;
            UpdateText();
            return true;
        } else return false;
    }

    // Demolition is allowed if the demolition cost recoups construction costs, even if the current funds are negative
    public bool DemolishZoning(int numBuildings, int bldgSize) {
        int demolitionCost = numBuildings * CalculateZoningDemolitionCost(bldgSize);
        int newFunds = Funds - demolitionCost;
        if (newFunds >= Funds) {
            Funds = newFunds;
            UpdateText();
            return true;
        } else return false;
    }
    
    public bool ConstructCivic(int seatsPerBuilding, int civicID) {
        int constructionCost = seatsPerBuilding * Constants.BaseCivicSeatCost[civicID] * Constants.BaseCivicMultiplier[civicID];
        int newFunds = Funds - constructionCost;
        if (newFunds >= 0) {
            Funds = newFunds;
            UpdateText();
            return true;
        } else return false;
    }

    // Demolition is allowed if the demolition cost recoups construction costs, even if the current funds are negative
    public bool DemolishCivic(int seatsPerBuilding, int civicID) {
        int demolitionCost = Mathf.RoundToInt(seatsPerBuilding * Constants.BaseCivicSeatCost[civicID] * Constants.BaseCivicMultiplier[civicID] * Constants.DemolitionMultiplier);
        int newFunds = Funds - demolitionCost;
        if (newFunds >= Funds) {
            Funds = newFunds;
            UpdateText();
            return true;
        } else return false;
    }

    // Note: to calculate this, the number of civic seats and zoning occupants must be tallied up and passed into this function;
    // these are multiplied by the base civic seat cost and base tax revenue respectively, then the seat cost is subtracted
    // from the base tax revenue
    // I wanted to add some unpredictability, so I decided to throw in the "magic die" that I added into the ExtraRandom library
    // Tax revenue from zoning is calculated like this: take the base revenue and divide it by 2, then multiply that by the number
    // zoning occupants, then take a die whose side count corresponds to the base revenue and roll it once for every zoning occupant
    // and the resulting score from all of those dice rolls is added to the total revenue (this can be done with the MagicDice
    // function I wrote in the ExtraRandom function)
    public void GenerateIncome(int zoningOccupants, int eduSeats, int healthSeats) {
        int revenue = (ConstBaseTaxRevenue / 2) * zoningOccupants + ExtraRandom.MagicDice(ConstBaseTaxRevenue, zoningOccupants);
        Funds += -(eduSeats * Constants.BaseCivicSeatCost[0]) + (zoningOccupants * Constants.BaseTaxRevenue);
        UpdateText();
    }

    public int CalculateZoningConstructionCost(int bldgSize) {
        int constructionCost = bldgSize * Constants.BaseZoningCost;
        if (bldgSize > 1) constructionCost = Mathf.RoundToInt(constructionCost * Constants.HighDensityMultiplier);
        return constructionCost;
    }

    public int CalculateZoningDemolitionCost(int bldgSize) {
        int demolitionCost = Mathf.RoundToInt(CalculateZoningConstructionCost(bldgSize) * Constants.DemolitionMultiplier);
        return demolitionCost;
    }

    public int CalculateCivicConstructionCost(int bldgSeats, int civicID) {
        return bldgSeats * Constants.BaseCivicMultiplier[civicID] * Constants.BaseCivicSeatCost[civicID];
    }

    public int CalculateCivicDemolitionCost(int bldgSeats, int civicID) {
        return Mathf.RoundToInt(CalculateCivicConstructionCost(bldgSeats, civicID) * Constants.DemolitionMultiplier);
    }

    private void UpdateText() {
        _textFunds.text = "Funds: " + Funds.ToString();
    }
}