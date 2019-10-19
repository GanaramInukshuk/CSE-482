using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Timekeeping is based on the Sym454 calendar (without leap year calculations), an alternate
// calendar where each year is 364 days long (372 every 5-6 years for leap years) and months
// are either 28 or 35 days long

// For gameplay purposes, the smallest unit of time to keep track of is the week; weeks are
// counted off using a preset number of ticks per week, and when 52 of those weeks have passed, 
// one in-game year will have passed; virtual "subdivisions" of time may also be counted off

// THE CONCEPT OF AN EPISODIC DAY AND EPISODIC TIME

// In general with simulator games, it's usually inconvenient to simulate the passage of
// time with the fidelity of real time. 

// Examples include:
// - Zoo Tycoon 2: Days are nonexistent and months are counted off instead; the game's day-night
//   cycle encompasses one in-game month
// - Parkitect: Months are counted off as 5 minutes of real-life time; weeks are also used
//   to an extent
// - Prison Architect: Game time and time of prison sentence really don't correspond to one 
//   another, so a prison sentence of a few years is served in the timespan of a few in-gmae days
// - Either Simcity 4 or Cities: Skylines: the day-night cycle of both games is completely 
//   separate from the actual passage of time reported by their respective timekeeping 
//   mechanisms; however, SC4 updates financial records on a monthly basis whereas C:S does 
//   that on a weekly basis

// The purpose of episodic time is to provide a sensible feeling of the passage of time, even though
// days are literally zooming by in-game (and most of a person's typical day-to-day experiences are
// generally boring details not worth remembering by the brain)

// Note: Since this is taking a tick count and reformatting it into something more understandable,
// this can simply be a static class

public static class Timekeeper {

    // Enums for convenience
    public enum SEASON { WIN, SPR, SUM, FAL };
    public enum MONTH  { JAN, FEB, MAR, APR, MAY, JUN, JUL, AUG, SEP, OCT, NOV, DEC };
    public enum DAY    { MON, TUE, WED, THU, FRI, SAT, SUN };

    // This is reserved for future use...
    //public struct TimekeeperBreakdown {
    //    int year;
    //    int month;
    //    int week;
    //    int day;
    //    int dayOfMonth;
    //    int episodicDay;
    //    int episodicDayHour;
    //    int episodicDayMins;
    //}

    // Constants
    // Unity's FixedUpdate is called once every 1/50 of a second (20 milliseconds) for an effective tickrate of
    // 50 ticks per second; 60 ticks per day translates to one day per 1.2 seconds
    public static readonly int _ticksPerDay     = 60;
    public static readonly int _daysPerWeek     = 7;
    public static readonly int _ticksPerWeek    = _ticksPerDay * _daysPerWeek;
    public static readonly int _weeksPerQuarter = 13;
    public static readonly int _quartersPerYear = 4;

    // Constants of lesser importance
    // These don't affect actual timekeeping and instead help show the passage of time using smaller time units
    private static readonly int _hoursPerDay    = 24;
    private static readonly int _minutesPerHour = 60;

    // The concept of an episodic day is explained in the doobly-doo up top
    // If FOURTeen NIGHTs = fortnight, can twentyEIGHT NIGHTs = eightnight/octnight?
    private static readonly int _weeksPerEpisodicDay = 4;

    // Other constants derived from existing constants; for conversions
    public static readonly int   _ticksPerQuarter      = _ticksPerWeek * _weeksPerQuarter;
    public static readonly int   _ticksPerYear         = _ticksPerWeek * _weeksPerQuarter * _quartersPerYear;
    public static readonly int   _ticksPerEpisodicDay  = _ticksPerWeek * _weeksPerEpisodicDay;
    public static readonly int   _episodicDaysPerYear  = _ticksPerYear / _ticksPerEpisodicDay;
    public static readonly float _ticksPerEpisodicHour = (float)_ticksPerEpisodicDay / _hoursPerDay;
    public static readonly float _ticksPerEpisodicMin  = _ticksPerEpisodicHour       / _minutesPerHour;

    // Integer-divide the tickCount by the number of ticks per year to get the year
    // Use the remainder from aforementioned division to get all other information
    // Date and (episodic) time is displayed in US Military format
    // For leading zeros: https://answers.unity.com/questions/312864/how-to-set-fixed-number-of-digits.html
    public static string SimpleDate(int tickCount) {
        int year          = tickCount     / _ticksPerYear;
        int tickRemainder = tickCount     % _ticksPerYear;
        int week          = tickRemainder / _ticksPerWeek;
        int day           = Mathf.RoundToInt(tickRemainder / _ticksPerDay);  
        int month         = MonthFromWeek(week);
        int dayOfMonth    = DayOfMonth(month, day);

        return (DAY)(day % _daysPerWeek) + ", " + (dayOfMonth + 1).ToString("00") + " " + (MONTH)month + " " + year.ToString("0000");
    }

    // Find the episodic time-of-day (think day-night cycle)
    public static string EpisodicTime(int tickCount) {
        int tickRemainder = tickCount % _ticksPerEpisodicDay;
        int hour = Mathf.RoundToInt(tickRemainder / _ticksPerEpisodicHour);
        int mins = Mathf.RoundToInt(tickRemainder % _ticksPerEpisodicHour / _ticksPerEpisodicMin);
        return hour.ToString("00") + ":" + mins.ToString("00");
    }

    public static int EpisodicDayOutOfYear(int tickCount) {
        return (tickCount / _ticksPerEpisodicDay) % _episodicDaysPerYear;
    }

    public static int EpisodicDay(int tickCount) {
        return tickCount / _ticksPerEpisodicDay;
    }

    public static string DetailedDate(int tickCount) {
        int tickRemainder = tickCount     % _ticksPerYear;
        int quarter       = tickRemainder / _ticksPerQuarter;
        int week          = tickRemainder / _ticksPerWeek;

        return SimpleDate(tickCount) + "; QTR: " + (SEASON)quarter + ", WK: " + (week+1) + "; ED: " + (EpisodicDay(tickCount) + 1) + ", ET: " + EpisodicTime(tickCount);
    }

    // Lookup table for the number of weeks that correspond to each month
    // Recall that months follow a 4-5-4 pattern, so JAN has 4 weeks, FEB
    // has 5, and so on; that means (disregarding modular arithmetic) weeks
    // 1-4 are JAN, 5-9 are FEB, and so on
    private static int MonthFromWeek(int week) {
        switch (week) {
            case int i when week <  4: return  0;
            case int i when week <  9: return  1;
            case int i when week < 13: return  2;
            case int i when week < 17: return  3;
            case int i when week < 22: return  4;
            case int i when week < 26: return  5;
            case int i when week < 30: return  6;
            case int i when week < 35: return  7;
            case int i when week < 39: return  8;
            case int i when week < 43: return  9;
            case int i when week < 48: return 10;
            case int i when week < 52: return 11;
            default: return -1; 
        }
    }

    // Lookup table for how many days to subtract so that the day
    // corresponds to the month it's in; EG, day 33 of the year is in
    // FEB since JAN has 28 days, so subtracting 28 translates day 33
    // into 05 FEB; day 90 translates to 27 MAR since JAN and FEB
    // combined have 63 days
    private static int DayOfMonth(int month, int day) {
        switch (month) {
            case  0: return day;
            case  1: return day -  4 * _daysPerWeek;
            case  2: return day -  9 * _daysPerWeek;
            case  3: return day - 13 * _daysPerWeek;
            case  4: return day - 17 * _daysPerWeek;
            case  5: return day - 22 * _daysPerWeek;
            case  6: return day - 26 * _daysPerWeek;
            case  7: return day - 30 * _daysPerWeek;
            case  8: return day - 35 * _daysPerWeek;
            case  9: return day - 39 * _daysPerWeek;
            case 10: return day - 43 * _daysPerWeek;
            case 11: return day - 48 * _daysPerWeek;
            default: return -1;
        }
    }

    // Not necessary if the month is casted to MONTH
    //private static string MonthName(int month) {
    //    switch (month) {
    //        case 0 : return "JAN";
    //        case 1 : return "FEB";
    //        case 2 : return "MAR";
    //        case 3 : return "APR";
    //        case 4 : return "MAY";
    //        case 5 : return "JUN";
    //        case 6 : return "JUL"; 
    //        case 7 : return "AUG";
    //        case 8 : return "SEP";
    //        case 9 : return "OCT";
    //        case 10: return "NOV";
    //        case 11: return "DEC";
    //        default: return "ERR";
    //    }
    //}

    // Solstices/equinoxes happen on the 21st of every third month and mark the beginning
    // of the next season; saying that the 1st, 2nd, 3rd, and 4th quarters correspond to
    // WIN, SPR, SUMM, FALL is close enough for my purposes, but may be different depending
    // on the calendar; for example, https://www.metoffice.gov.uk/weather/learn-about/weather/seasons/winter/when-does-winter-start
    // Not necessary if quarter is casted to SEASON
    //private static string QuarterToSeason(int quarter) {
    //    switch (quarter) {
    //        case  0: return "WIN";
    //        case  1: return "SPR";
    //        case  2: return "SUMM";
    //        case  3: return "FALL";
    //        default: return "SEASON";
    //    }
    //}
}
