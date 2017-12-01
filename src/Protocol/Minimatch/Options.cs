/*
VIA https://github.com/SLaks/Minimatch
The MIT License (MIT)

Copyright (c) 2014 SLaks

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System.Text.RegularExpressions;

// ReSharper disable once CheckNamespace
namespace Minimatch
{
    ///<summary>Contains options that control how Minimatch matches strings.</summary>
    public class Options
    {
        ///<summary>Suppresses the behavior of treating # at the start of a pattern as a comment.</summary>
        public bool NoComment { get; set; }
        ///<summary>Suppresses the behavior of treating a leading ! character as negation.</summary>
        public bool NoNegate { get; set; }
        ///<summary>Do not expand {a,b} and {1.3} brace sets.</summary>
        public bool NoBrace { get; set; }
        ///<summary>Disable ** matching against multiple folder names.</summary>
        public bool NoGlobStar { get; set; }
        ///<summary>Ignores case differences when matching.</summary>
        public bool NoCase { get; set; }
        ///<summary>Disable "extglob" style patterns like <c>+(a|b)</c>.</summary>
        public bool NoExt { get; set; }
        ///<summary>Allow patterns to match filenames starting with a period, even if the pattern does not explicitly have a period in that spot.
        ///Note that by default, <c>a/**/b</c>  will not match <c>a/.d/b</c>, unless dot is set.</summary>
        public bool Dot { get; set; }
        ///<summary>When a match is not found by Match(), return a list containing the pattern itself. If not set, an empty list is returned if there are no matches.</summary>
        public bool NoNull { get; set; }
        ///<summary>Returns from negate expressions the same as if they were not negated. (ie, true on a hit, false on a miss).</summary>
        public bool FlipNegate { get; set; }

        ///<summary>If set, then patterns without slashes will be matched against the basename of the path if it contains slashes. For example, <c>a?b</c> would match the path <c>/xyz/123/acb</c>, but not <c>/xyz/acb/123</c>.</summary>
        public bool MatchBase { get; set; }

        internal RegexOptions RegexOptions { get { return NoCase ? RegexOptions.IgnoreCase : RegexOptions.None; } }

        ///<summary>If true, backslahes in patterns and paths will be treated as forward slashes.  This disables escape characters.</summary>
        public bool AllowWindowsPaths { get; set; }

        // Aliases:
        ///<summary>Ignores case differences when matching.  This is the same as NoCase.</summary>
        public bool IgnoreCase
        {
            get { return NoCase; }
            set { NoCase = value; }
        }
    }
}