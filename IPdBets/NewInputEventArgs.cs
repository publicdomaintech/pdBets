// <copyright file="NewInputEventArgs.cs" company="www.PublicDomain.tech">All rights waived.</copyright>

// Programmed by Victor L. Senior (VLS) <publicdomaintech@gmail.com>, 2016
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
/// New input event arguments.
/// </summary>
namespace PdBets
{
    // Directives
    using System;

    /// <summary>
    /// New input event arguments.
    /// </summary>
    public class NewInputEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PdBets.NewInputEventArgs"/> class.
        /// </summary>
        /// <param name="inputString">Input string.</param>
        public NewInputEventArgs(string inputString)
        {
            // Set input string
            this.InputString = inputString;
        }

        /// <summary>
        /// Gets or sets the input string.
        /// </summary>
        /// <value>The input string.</value>
        public string InputString { get; set; }
    }
}