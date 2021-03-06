﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace System
{
    public static class StringExtensions
    {
        public static string TrimParameter(this string input)
        {
            return input.Trim().Trim('"', '\'');
        }

        public static IEnumerable<string> ParseParameters(this string input)
        {
            var parametersCommaSeparated = input.Trim();
            return parametersCommaSeparated
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(parameter => parameter.TrimParameter());
        }
    }
}