// <copyright file="Roulette.cs" company="www.PublicDomain.tech">All rights waived.</copyright>

// Programmed by Victor L. Senior (VLS) <support@publicdomain.tech>, 2016
//
// Web: http://publicdomain.tech
//
// Sources: http://github.com/publicdomaintech/
//
// This software and associated documentation files (the "Software") is
// released under the CC0 Public Domain Dedication, version 1.0, as
// published by Creative Commons. To the extent possible under law, the
// author(s) have dedicated all copyright and related and neighboring
// rights to the Software to the public domain worldwide. The Software is
// distributed WITHOUT ANY WARRANTY.
//
// If you did not receive a copy of the CC0 Public Domain Dedication
// along with the Software, see
// <http://creativecommons.org/publicdomain/zero/1.0/>

/// <summary>
/// Roulette.
/// </summary>
namespace PdBets
{
    // Directives
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Roulette class.
    /// </summary>
    public class Roulette
    {
        /// <summary>
        /// The locations list.
        /// </summary>
        private List<string> locationsList = null;

        /// <summary>
        /// The wheel numbers. Defaults to European.
        /// </summary>
        private int wheelNumbers = 37;

        /// <summary>
        /// Initializes a new instance of the <see cref="PdBets.Roulette"/> class.
        /// </summary>
        public Roulette()
        {
            // Populate locations list
            this.PopulateLocationsList();
        }

        /// <summary>
        /// Sets the wheel numbers.
        /// </summary>
        /// <param name="wheelNumbers">Wheel numbers.</param>
        public void SetWheelNumbers(int wheelNumbers)
        {
            // Set wheel numbers
            this.wheelNumbers = wheelNumbers;

            // Populate locations list
            this.PopulateLocationsList();
        }

        /// <summary>
        /// Validates the roulette number integer.
        /// </summary>
        /// <returns><c>true</c>, if roulette number was validated, <c>false</c> otherwise.</returns>
        /// <param name="numberInteger">Number integer.</param>
        public bool ValidateRouletteNumber(int numberInteger)
        {
            // Check range
            if (numberInteger >= (this.wheelNumbers == 36 ? 1 : 0) && numberInteger < (this.wheelNumbers == 36 ? 37 : (this.wheelNumbers == 38 ? 38 : 37)))
            {
                // Valid
                return true;
            }

            // Invalid
            return false;
        }

        /// <summary>
        /// Validates the roulette number string.
        /// </summary>
        /// <returns><c>true</c>, if roulette number was validated, <c>false</c> otherwise.</returns>
        /// <param name="numberString">Number string.</param>
        public bool ValidateRouletteNumber(string numberString)
        {
            // Declare number integer
            int numberInteger;

            // Try to parse as integer
            if (int.TryParse(numberString, out numberInteger) && this.ValidateRouletteNumber(numberInteger))
            {
                // Valid
                return true;
            }

            // Invalid
            return false;
        }

        /// <summary>
        /// Validates the bet string.
        /// </summary>
        /// <returns><c>true</c>, if bet string was validated, <c>false</c> otherwise.</returns>
        /// <param name="betString">Bet string.</param>
        public bool ValidateBetString(string betString)
        {
            // Declare bets list
            List<string> betsList = new List<string>();

            // Split bet string
            betsList.AddRange(betString.Split(new char[] { ',' }));

            // Iterate
            foreach (string bet in betsList)
            {
                // Check there's an at sign
                if (bet.IndexOf("@") == -1)
                {
                    // Halt flow
                    return false;
                }

                // Split
                string[] betSides = bet.Split(new char[] { '@' });

                // Parsed int
                int parsedInt;

                // Check left side is numeric
                if (!int.TryParse(betSides[0], out parsedInt))
                {
                    // Halt flow
                    return false;
                }

                // Check right side is a valid location
                if (!this.locationsList.Contains(betSides[1]))
                {
                    // Halt flow
                    return false;
                }
            }

            // Validated correctly
            return true;
        }

        /// <summary>
        /// Populates the locations list.
        /// </summary>
        private void PopulateLocationsList()
        {
            // Add all locations
            this.locationsList = new List<string>()
            {
                "B",
                "R",
                "L",
                "H",
                "E",
                "O",
                "D1",
                "D2",
                "D3",
                "C1",
                "C2",
                "C3",
                "1-6",
                "4-9",
                "7-12",
                "10-15",
                "13-18",
                "16-21",
                "19-24",
                "22-27",
                "25-30",
                "28-33",
                "31-36",
                "1-5",
                "2-6",
                "4-8",
                "5-9",
                "7-11",
                "8-12",
                "10-14",
                "11-15",
                "13-17",
                "14-18",
                "16-20",
                "17-21",
                "19-23",
                "20-24",
                "22-26",
                "23-27",
                "25-29",
                "26-30",
                "28-32",
                "29-33",
                "31-35",
                "32-36",
                "1-3",
                "4-6",
                "7-9",
                "10-12",
                "13-15",
                "16-18",
                "19-21",
                "22-24",
                "25-27",
                "28-30",
                "31-33",
                "34-36",
                "0-1",
                "0-2",
                "0-3",
                "0-37",
                "1-2",
                "1-4",
                "2-3",
                "2-5",
                "3-6",
                "3-37",
                "4-5",
                "4-7",
                "5-6",
                "5-8",
                "6-9",
                "7-8",
                "7-10",
                "8-9",
                "8-11",
                "9-12",
                "10-11",
                "10-13",
                "11-12",
                "11-14",
                "12-15",
                "13-14",
                "13-16",
                "14-15",
                "14-17",
                "15-18",
                "16-17",
                "16-19",
                "17-18",
                "17-20",
                "18-21",
                "19-20",
                "19-22",
                "20-21",
                "20-23",
                "21-24",
                "22-23",
                "22-25",
                "23-24",
                "23-26",
                "24-27",
                "25-26",
                "25-28",
                "26-27",
                "26-29",
                "27-30",
                "28-29",
                "28-31",
                "29-30",
                "29-32",
                "30-33",
                "31-32",
                "31-34",
                "32-33",
                "32-35",
                "33-36",
                "34-35",
                "35-36",
            };

            // Add numbers according to current wheel numbers / wheel type
            for (int n = this.wheelNumbers == 36 ? 1 : 0; n <= this.wheelNumbers; n++)
            {
                // Add current number
                this.locationsList.Add(n.ToString());
            }
        }
    }
}