// <copyright file="IPdBets.cs" company="www.PublicDomain.tech">All rights waived.</copyright>

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
/// IPdBets interface.
/// </summary>
namespace PdBets
{
    /// <summary>
    /// The IPdBets interface.
    /// </summary>
    public interface IPdBets
    {
        /// <summary>
        /// Processes input and bet strings.
        /// </summary>
        /// <param name="inputString">Input string to be casted to current/target game input type.</param>
        /// <param name="betString">Bet string in proper format.</param>
        /// <returns>Processed bet string.</returns>
        string Input(string inputString, string betString);
    }
}