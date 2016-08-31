// <copyright file="Validator.cs" company="www.PublicDomain.tech">All rights waived.</copyright>

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
/// Validator.
/// </summary>
namespace PdBetsLib
{
    // Directives
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Validator class.
    /// </summary>
    public class Validator
    {
        /// <summary>
        /// Validates the internal message.
        /// </summary>
        /// <returns><c>true</c>, if internal message was validated, <c>false</c> otherwise.</returns>
        /// <param name="message">The message.</param>
        public bool ValidateInternalMessage(string message)
        {
            // The valid messages
            List<string> validMessages = new List<string>()
            {
                "-U", // Undo.
                "-R", // Redo.
                "-L", // Loop.
                "-Q", // Quit gracefully.
                "-H", // Halt, quit now.
                "-N", // New session.
                "-K", // Okay.
                "-E", // Error.
                "-S", // Show.
            };

            // Return contains result
            return validMessages.Contains(message);
        }
    }
}