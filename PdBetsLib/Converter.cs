// <copyright file="Converter.cs" company="www.PublicDomain.tech">All rights waived.</copyright>

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
/// Converter.
/// </summary>
namespace PdBets
{
    // Directives
    using System;
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Converter class.
    /// </summary>
    public class Converter
    {
        /// <summary>
        /// Changes passed display name to file name.
        /// </summary>
        /// <returns>Resulting file name.</returns>
        /// <param name="displayName">Display name.</param>
        public string DisplayNameToFileName(string displayName)
        {
            // Check there's something to work with
            if (displayName.Length > 0)
            {
                // Match with regular expression
                MatchCollection matches = Regex.Matches(displayName, @"[^a-zA-Z]");

                // Walk reversed
                for (int i = matches.Count - 1; i >= 0; i--)
                {
                    // Set encoding
                    UTF32Encoding encoding = new UTF32Encoding();

                    // Get current bytes
                    byte[] bytes = encoding.GetBytes(matches[i].Value.ToCharArray());

                    // Remove original
                    displayName = displayName.Remove(matches[i].Index, 1);

                    // Insert replacement
                    displayName = displayName.Insert(matches[i].Index, "_" + BitConverter.ToInt32(bytes, 0).ToString() + "_");
                }

                // Return processed display name
                return displayName;
            }

            // Return empty string by default
            return string.Empty;
        }

        /// <summary>
        /// Changes file name to display name
        /// </summary>
        /// <param name="fileName">The passed file name</param>
        /// <returns>String with replacements</returns>
        public string FileNameToDisplayName(string fileName)
        {
            // Match with regular expression
            MatchCollection matches = Regex.Matches(fileName, @"_[0-9]+_");

            // Walk reversed
            for (int i = matches.Count - 1; i >= 0; i--)
            {
                // Convert
                try
                {
                    // Declare
                    int intVal;

                    // Parse from string
                    if (int.TryParse(matches[i].Value.Replace("_", string.Empty), NumberStyles.Integer, null, out intVal))
                    {
                        // Remove original
                        fileName = fileName.Remove(matches[i].Index, matches[i].Length);

                        // Insert replacement
                        fileName = fileName.Insert(matches[i].Index, char.ConvertFromUtf32(intVal).ToString());
                    }
                }
                catch (Exception)
                {
                    // Let it fall through
                }
            }

            // Processed namespace back
            return fileName;
        }
    }
}