#region Copyright, Author Details and Related Context   
// Copyright (C) SquidEyes, LLC. - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and Confidential
// Written by Louis S. Berman <louis@squideyes.com>, 6/20/2016
#endregion  

using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace WebJobsPoisonDemo.CloudHelpers
{
    public static class StringExtenders
    {
        private static Regex dnsNameRegex = new Regex(
            @"^[a-z0-9](?:[a-z0-9]|(\-(?!\-))){1,61}[a-z0-9]$", RegexOptions.Compiled);

        public static bool IsBlobName(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            if (value.Length < 1 || value.Length > 1024)
                return false;

            if (value.Count(c => c == '/') > 254)
                return false;

            var ext = Path.GetExtension(value);

            if (string.IsNullOrWhiteSpace(ext))
                return false;

            if (ext == ".")
                return false;

            if (value.Split('/').Any(p => string.IsNullOrWhiteSpace(p)))
                return false;

            return true;
        }

        public static bool IsQueueName(this string value) =>
            dnsNameRegex.IsMatch(value);

        public static bool IsContainerName(this string value) =>
            dnsNameRegex.IsMatch(value);
    }
}
