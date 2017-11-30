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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

// ReSharper disable once CheckNamespace
namespace Minimatch
{
    ///<summary>Parses a single glob pattern and tests strings against it.</summary>
    public class Minimatcher
    {
        ///<summary>Creates a filter function that tests input against a pattern.</summary>
        public static Func<string, bool> CreateFilter(string pattern, Options options = null)
        {
            if (pattern == null) throw new ArgumentNullException("pattern");
            // "" only matches ""
            if (string.IsNullOrWhiteSpace(pattern)) return string.IsNullOrEmpty;

            var m = new Minimatcher(pattern, options);
            return m.IsMatch;
        }
        ///<summary>Tests a single input against a pattern.</summary>
        ///<remarks>This function reparses this input on each invocation.  For performance, avoid this function and reuse a Minimatcher instance instead.</remarks>
        public static bool Check(string input, string pattern, Options options = null)
        {
            if (input == null) throw new ArgumentNullException("input");
            if (pattern == null) throw new ArgumentNullException("pattern");

            // shortcut: comments match nothing.
            if (options != null && !options.NoComment && pattern[0] == '#')
            {
                return false;
            }

            // "" only matches ""
            if (string.IsNullOrWhiteSpace(pattern)) return input == "";

            return new Minimatcher(pattern, options).IsMatch(input);
        }

        ///<summary>Filters a list of inputs against a single pattern.</summary>
        ///<remarks>This function reparses this input on each invocation.  For performance, avoid this function and reuse a Minimatcher instance instead.</remarks>
        public static IEnumerable<string> Filter(IEnumerable<string> list, string pattern, Options options = null)
        {
            var mm = new Minimatcher(pattern, options);
            list = list.Where(mm.IsMatch);
            if (options != null && options.NoNull)
                list = list.DefaultIfEmpty(pattern);
            return list;
        }

        ///<summary>Compiles a pattern into a single regular expression.</summary>
        public static Regex CreateRegex(string pattern, Options options = null)
        {
            return new Minimatcher(pattern, options).MakeRegex();
        }


        readonly Options options;

        string pattern;
        bool negate = false;
        bool comment = false;
        bool empty = false;

        ///<summary>Creates a new Minimatcher instance, parsing the pattern into a regex.</summary>
        public Minimatcher(string pattern, Options options = null)
        {
            if (pattern == null) throw new ArgumentNullException("pattern");
            this.options = options ?? new Options();
            this.pattern = pattern.Trim();
            if (this.options.AllowWindowsPaths)
                this.pattern = this.pattern.Replace('\\', '/');

            this.Make();
        }

        ///<summary>Checks whether a given string matches this pattern.</summary>
        public bool IsMatch(string input) { return Match(input, false); }

        ///<summary>Filters a list of inputs against this pattern.</summary>
        public IEnumerable<string> Filter(IEnumerable<string> list)
        {
            list = list.Where(IsMatch);
            if (options.NoNull)
                list = list.DefaultIfEmpty(pattern);
            return list;
        }


        Regex regexp;
        bool isError;

        IEnumerable<string> globSet;
        IEnumerable<IEnumerable<ParseItem>> set;
        IEnumerable<IEnumerable<string>> globParts;


        // any single thing other than /
        // don't need to escape / when using new RegExp()
        const string qmark = "[^/]"

            // * => any number of characters
            , star = qmark + "*?"

            // ** when dots are allowed.  Anything goes, except .. and .
            // not (^ or / followed by one or two dots followed by $ or /),
            // followed by anything, any number of times.
            , twoStarDot = "(?:(?!(?:\\/|^)(?:\\.{1,2})($|\\/)).)*?"

            // not a ^ or / followed by a dot,
            // followed by anything, any number of times.
            , twoStarNoDot = "(?:(?!(?:\\/|^)\\.).)*?";

        // characters that need to be escaped in RegExp.
        static readonly HashSet<char> reSpecials = new HashSet<char>("().*{}+?[]^$\\!".ToCharArray());
        static readonly Regex slashSplit = new Regex("/+");

        void Make()
        {
            // empty patterns and comments match nothing.
            if (!options.NoComment && !string.IsNullOrEmpty(pattern) && pattern[0] == '#')
            {
                this.comment = true;
                return;
            }
            if (string.IsNullOrEmpty(pattern))
            {
                this.empty = true;
                return;
            }

            // step 1: figure out negation, etc.
            this.ParseNegate();

            // step 2: expand braces
            this.globSet = BraceExpand(pattern, options);

            // step 3: now we have a set, so turn each one into a series of path-portion
            // matching patterns.
            // These will be regexps, except in the case of "**", which is
            // set to the GLOBSTAR object for globstar behavior,
            // and will not contain any / characters
            this.globParts = globSet.Select(s => slashSplit.Split(s)).ToList();

            // glob --> regexps
            this.set = globParts.Select(g => g.Select(t => this.Parse(t, false)))
                .Where(g => !g.Contains(null))
                .Select(g => g.Select(t => t.Item1))
                .ToList();
        }

        void ParseNegate()
        {
            var negateOffset = 0;

            if (options.NoNegate) return;

            for (var i = 0; i < pattern.Length && pattern[i] == '!'; i++)
            {
                negate = !negate;
                negateOffset++;
            }

            if (negateOffset > 0) this.pattern = pattern.Substring(negateOffset);
        }

        static readonly Regex hasBraces = new Regex(@"\{.*\}");
        static readonly Regex numericSet = new Regex(@"^\{(-?[0-9]+)\.\.(-?[0-9]+)\}");
        // Brace expansion:
        // a{b,c}d -> abd acd
        // a{b,}c -> abc ac
        // a{0..3}d -> a0d a1d a2d a3d
        // a{b,c{d,e}f}g -> abg acdfg acefg
        // a{b,c}d{e,f}g -> abdeg acdeg abdeg abdfg
        //
        // Invalid sets are not expanded.
        // a{2..}b -> a{2..}b
        // a{b}c -> a{b}c
        ///<summary>Expands all brace ranges in a pattern, returning a sequence containing every possible combination.</summary>
        public static IEnumerable<string> BraceExpand(string pattern, Options options)
        {
            if (options.NoBrace || !hasBraces.IsMatch(pattern))
            {
                // shortcut. no need to expand.
                return new[] { pattern };
            }
            bool escaping = false;
            int i;
            // examples and comments refer to this crazy pattern:
            // a{b,c{d,e},{f,g}h}x{y,z}
            // expected:
            // abxy
            // abxz
            // acdxy
            // acdxz
            // acexy
            // acexz
            // afhxy
            // afhxz
            // aghxy
            // aghxz

            // everything before the first \{ is just a prefix.
            // So, we pluck that off, and work with the rest,
            // and then prepend it to everything we find.
            if (pattern[0] != '{')
            {
                // console.error(pattern)
                string prefix = null;
                for (i = 0; i < pattern.Length; i++)
                {
                    var c = pattern[i];
                    // console.error(i, c)
                    if (c == '\\')
                    {
                        escaping = !escaping;
                    }
                    else if (c == '{' && !escaping)
                    {
                        prefix = pattern.Substring(0, i);
                        break;
                    }
                }

                // actually no sets, all { were escaped.
                if (prefix == null)
                {
                    // console.error("no sets")
                    return new[] { pattern };
                }

                return BraceExpand(pattern.Substring(i), options).Select(t => prefix + t);
            }

            // now we have something like:
            // {b,c{d,e},{f,g}h}x{y,z}
            // walk through the set, expanding each part, until
            // the set ends.  then, we'll expand the suffix.
            // If the set only has a single member, then'll put the {} back

            // first, handle numeric sets, since they're easier
            var numset = numericSet.Match(pattern);
            if (numset.Success)
            {
                // console.error("numset", numset[1], numset[2])
                var suf = BraceExpand(pattern.Substring(numset.Length), options).ToList();
                int start = int.Parse(numset.Groups[1].Value),
                    end = int.Parse(numset.Groups[2].Value),
                    inc = start > end ? -1 : 1;
                var retVal = new List<string>();
                for (var w = start; w != (end + inc); w += inc)
                {
                    // append all the suffixes
                    for (var ii = 0; ii < suf.Count; ii++)
                    {
                        retVal.Add(w.ToString() + suf[ii]);
                    }
                }
                return retVal;
            }

            // ok, walk through the set
            // We hope, somewhat optimistically, that there
            // will be a } at the end.
            // If the closing brace isn't found, then the pattern is
            // interpreted as braceExpand("\\" + pattern) so that
            // the leading \{ will be interpreted literally.
            i = 1; // skip the \{
            int depth = 1;
            var set = new List<string>();
            string member = "";

            for (i = 1; i < pattern.Length && depth > 0; i++)
            {
                var c = pattern[i];
                // console.error("", i, c)

                if (escaping)
                {
                    escaping = false;
                    member += "\\" + c;
                }
                else
                {
                    switch (c)
                    {
                        case '\\':
                            escaping = true;
                            continue;

                        case '{':
                            depth++;
                            member += "{";
                            continue;

                        case '}':
                            depth--;
                            // if this closes the actual set, then we're done
                            if (depth == 0)
                            {
                                set.Add(member);
                                member = "";
                                // pluck off the close-brace
                                break;
                            }
                            else
                            {
                                member += c;
                                continue;
                            }

                        case ',':
                            if (depth == 1)
                            {
                                set.Add(member);
                                member = "";
                            }
                            else
                            {
                                member += c;
                            }
                            continue;

                        default:
                            member += c;
                            continue;
                    } // switch
                } // else
            } // for

            // now we've either finished the set, and the suffix is
            // pattern.substr(i), or we have *not* closed the set,
            // and need to escape the leading brace
            if (depth != 0)
            {
                // console.error("didn't close", pattern)
                return BraceExpand("\\" + pattern, options);
            }

            // ["b", "c{d,e}","{f,g}h"] ->
            //   ["b", "cd", "ce", "fh", "gh"]
            var addBraces = set.Count == 1;

            set = set.SelectMany(p => BraceExpand(p, options)).ToList();

            if (addBraces)
                set = set.Select(s => "{" + s + "}").ToList();
            // now attach the suffixes.
            // x{y,z} -> ["xy", "xz"]
            // console.error("set", set)
            // console.error("suffix", pattern.substr(i))
            return BraceExpand(pattern.Substring(i), options).SelectMany(s1 => set.Select(s2 => s2 + s1));
        }

        private class PatternListEntry
        {
            public char Type { get; set; }
            public int Start { get; set; }
            public int ReStart { get; set; }
        }

        abstract class ParseItem
        {
            public string Source { get; protected set; }

            public static readonly ParseItem Empty = new LiteralItem("");
            public static ParseItem Literal(string source) { return new LiteralItem(source); }
            public abstract string RegexSource(Options options);

            public abstract bool Match(string input, Options options);
        }
        class LiteralItem : ParseItem
        {
            public LiteralItem(string source) { Source = source; }
            public override string RegexSource(Options options) { return Regex.Escape(Source); }
            public override bool Match(string input, Options options)
            {
                return input.Equals(Source, options.NoCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
            }
        }
        class MagicItem : ParseItem
        {
            public MagicItem(string source, Options options)
            {
                Source = source;
                regex = new Lazy<Regex>(() => new Regex("^" + source + "$", options.RegexOptions));
            }
            readonly Lazy<Regex> regex;

            public override string RegexSource(Options options) { return Source; }
            public override bool Match(string input, Options options)
            {
                return regex.Value.IsMatch(input);
            }
        }
        class GlobStar : ParseItem
        {
            private GlobStar() { }
            public static readonly ParseItem Instance = new GlobStar();

            public override string RegexSource(Options options)
            {
                return options.NoGlobStar ? star
                    : options.Dot ? twoStarDot
                        : twoStarNoDot;
            }

            public override bool Match(string input, Options options) { throw new NotSupportedException(); }
        }

        static readonly Regex escapeCheck = new Regex(@"((?:\\{2})*)(\\?)\|");
        // parse a component of the expanded set.
        // At this point, no pattern may contain "/" in it
        // so we're going to return a 2d array, where each entry is the full
        // pattern, split on '/', and then turned into a regular expression.
        // A regexp is made at the end which joins each array with an
        // escaped /, and another full one which joins each regexp with |.
        //
        // Following the lead of Bash 4.1, note that "**" only has special meaning
        // when it is the *only* thing in a path portion.  Otherwise, any series
        // of * is equivalent to a single *.  Globstar behavior is enabled by
        // default, and can be disabled by setting options.noglobstar.
        private Tuple<ParseItem, bool> Parse(string pattern, bool isSub)
        {
            // shortcuts
            if (!options.NoGlobStar && pattern == "**") return Tuple.Create(GlobStar.Instance, false);
            if (pattern == "") return Tuple.Create(ParseItem.Empty, false);

            string re = "";
            bool hasMagic = options.NoCase, escaping = false, inClass = false;
            // ? => one single character
            var patternListStack = new Stack<PatternListEntry>();
            char plType;
            char? statechar = null;

            int reClassStart = -1, classStart = -1;
            // . and .. never match anything that doesn't start with .,
            // even when options.dot is set.
            string patternStart = pattern[0] == '.' ? "" // anything
                // not (start or / followed by . or .. followed by / or end)
                : options.Dot ? "(?!(?:^|\\/)\\.{1,2}(?:$|\\/))"
                    : "(?!\\.)";

            Action clearStatechar = () => {
                if (statechar != null)
                {
                    // we had some state-tracking character
                    // that wasn't consumed by this pass.
                    switch (statechar)
                    {
                        case '*':
                            re += star;
                            hasMagic = true;
                            break;
                        case '?':
                            re += qmark;
                            hasMagic = true;
                            break;
                        default:
                            re += "\\" + statechar;
                            break;
                    }
                    statechar = null;
                }
            };

            for (var i = 0; i < pattern.Length; i++)
            {
                var c = pattern[i];
                //if (options.debug) {
                //  console.error("%s\t%s %s %j", pattern, i, re, c)
                //}

                // skip over any that are escaped.
                if (escaping && reSpecials.Contains(c))
                {
                    re += "\\" + c;
                    escaping = false;
                    continue;
                }

                switch (c)
                {
                    case '/':
                        // completely not allowed, even escaped.
                        // Should already be path-split by now.
                        return null;

                    case '\\':
                        clearStatechar();
                        escaping = true;
                        continue;

                    // the various statechar values
                    // for the 'extglob' stuff.
                    case '?':
                    case '*':
                    case '+':
                    case '@':
                    case '!':
                        //if (options.debug) {
                        //  console.error("%s\t%s %s %j <-- statechar", pattern, i, re, c)
                        //}

                        // all of those are literals inside a class, except that
                        // the glob [!a] means [^a] in regexp
                        if (inClass)
                        {
                            if (c == '!' && i == classStart + 1) c = '^';
                            re += c;
                            continue;
                        }

                        // if we already have a statechar, then it means
                        // that there was something like ** or +? in there.
                        // Handle the statechar, then proceed with this one.
                        clearStatechar();
                        statechar = c;
                        // if extglob is disabled, then +(asdf|foo) isn't a thing.
                        // just clear the statechar *now*, rather than even diving into
                        // the patternList stuff.
                        if (options.NoExt) clearStatechar();
                        continue;

                    case '(':
                        if (inClass)
                        {
                            re += "(";
                            continue;
                        }

                        if (statechar == null)
                        {
                            re += "\\(";
                            continue;
                        }

                        plType = statechar.Value;
                        patternListStack.Push(new PatternListEntry { Type = plType, Start = i - 1, ReStart = re.Length });
                        // negation is (?:(?!js)[^/]*)
                        re += statechar == '!' ? "(?:(?!" : "(?:";
                        statechar = null;
                        continue;

                    case ')':
                        if (inClass || !patternListStack.Any())
                        {
                            re += "\\)";
                            continue;
                        }

                        hasMagic = true;
                        re += ')';
                        plType = patternListStack.Pop().Type;
                        // negation is (?:(?!js)[^/]*)
                        // The others are (?:<pattern>)<type>
                        switch (plType)
                        {
                            case '!':
                                re += "[^/]*?)";
                                break;
                            case '?':
                            case '+':
                            case '*': re += plType; break;
                            case '@': break; // the default anyway
                        }
                        continue;

                    case '|':
                        if (inClass || !patternListStack.Any() || escaping)
                        {
                            re += "\\|";
                            escaping = false;
                            continue;
                        }

                        re += "|";
                        continue;

                    // these are mostly the same in regexp and glob
                    case '[':
                        // swallow any state-tracking char before the [
                        clearStatechar();

                        if (inClass)
                        {
                            re += "\\" + c;
                            continue;
                        }

                        inClass = true;
                        classStart = i;
                        reClassStart = re.Length;
                        re += c;
                        continue;

                    case ']':
                        //  a right bracket shall lose its special
                        //  meaning and represent itself in
                        //  a bracket expression if it occurs
                        //  first in the list.  -- POSIX.2 2.8.3.2
                        if (i == classStart + 1 || !inClass)
                        {
                            re += "\\" + c;
                            escaping = false;
                            continue;
                        }

                        // finish up the class.
                        hasMagic = true;
                        inClass = false;
                        re += c;
                        continue;

                    default:
                        // swallow any state char that wasn't consumed
                        clearStatechar();

                        if (escaping)
                        {
                            // no need
                            escaping = false;
                        }
                        else if (reSpecials.Contains(c) && !(c == '^' && inClass))
                        {
                            re += "\\";
                        }

                        re += c;
                        break;
                } // switch
            } // for


            // handle the case where we left a class open.
            // "[abc" is valid, equivalent to "\[abc"
            if (inClass)
            {
                // split where the last [ was, and escape it
                // this is a huge pita.  We now have to re-walk
                // the contents of the would-be class to re-translate
                // any characters that were passed through as-is
                string cs = pattern.Substring(classStart + 1);
                var sp = this.Parse(cs, true);
                re = re.Substring(0, reClassStart) + "\\[" + sp.Item1.Source;
                hasMagic = hasMagic || sp.Item2;
            }

            // handle the case where we had a +( thing at the *end*
            // of the pattern.
            // each pattern list stack adds 3 chars, and we need to go through
            // and escape any | chars that were passed through as-is for the regexp.
            // Go through and escape them, taking care not to double-escape any
            // | chars that were already escaped.
            while (patternListStack.Any())
            {
                var pl = patternListStack.Pop();
                var tail = re.Substring(pl.ReStart + 3);
                // maybe some even number of \, then maybe 1 \, followed by a |
                tail = escapeCheck.Replace(tail, m => {
                    string escape = m.Groups[2].Value;
                    // the | isn't already escaped, so escape it.
                    if (string.IsNullOrEmpty(escape)) escape = "\\";

                    // need to escape all those slashes *again*, without escaping the
                    // one that we need for escaping the | character.  As it works out,
                    // escaping an even number of slashes can be done by simply repeating
                    // it exactly after itself.  That's why this trick works.
                    //
                    // I am sorry that you have to see this.
                    return m.Groups[1].Value + m.Groups[1].Value + escape + "|";
                });

                // console.error("tail=%j\n   %s", tail, tail)
                var t = pl.Type == '*' ? star
                    : pl.Type == '?' ? qmark
                        : "\\" + pl.Type;

                hasMagic = true;
                re = re.Remove(pl.ReStart)
                     + t + "\\("
                     + tail;
            }

            // handle trailing things that only matter at the very end.
            clearStatechar();
            if (escaping)
            {
                // trailing \\
                re += "\\\\";
            }

            // only need to apply the nodot start if the re starts with
            // something that could conceivably capture a dot
            var addPatternStart = false;
            switch (re[0])
            {
                case '.':
                case '[':
                case '(': addPatternStart = true; break;
            }

            // if the re is not "" at this point, then we need to make sure
            // it doesn't match against an empty path part.
            // Otherwise a/* will match a/, which it should not.
            if (re != "" && hasMagic) re = "(?=.)" + re;

            if (addPatternStart) re = patternStart + re;

            // parsing just a piece of a larger pattern.
            if (isSub)
            {
                return Tuple.Create(ParseItem.Literal(re), hasMagic);
            }

            // skip the regexp for non-magical patterns
            // unescape anything in it, though, so that it'll be
            // an exact match against a file etc.
            if (!hasMagic)
            {
                return Tuple.Create(ParseItem.Literal(GlobUnescape(pattern)), false);
            }
            return new Tuple<ParseItem, bool>(new MagicItem(re, options), false);
        }


        Regex MakeRegex()
        {
            if (this.regexp != null || isError) return this.regexp;

            // at this point, this.set is a 2d array of partial
            // pattern strings, or "**".
            //
            // It's better to use .match().  This function shouldn't
            // be used, really, but it's pretty convenient sometimes,
            // when you just want to work with a regex.
            if (comment || empty || !set.Any())
            {
                this.isError = true;
                return null;
            }
            var re = string.Join("|", set.Select(pattern =>
                string.Join("\\/", pattern.Select(p => p.RegexSource(options))
                )));

            // must match entire pattern
            // ending in a * or ** will make it less strict.
            re = "^(?:" + re + ")$";

            // can match anything, as long as it's not this.
            if (this.negate) re = "^(?!" + re + ").*$";

            try
            {
                return this.regexp = new Regex(re, options.RegexOptions);
            }
            catch
            {
                this.isError = true;
                return null;
            }
        }


        private bool Match(string input, bool partial)
        {
            // console.error("match", f, this.pattern)
            // short-circuit in the case of busted things.
            // comments, etc.
            if (this.comment) return false;
            if (this.empty) return input == "";

            if (input == "/" && partial) return true;

            // windows: need to use /, not \
            // On other platforms, \ is a valid (albeit bad) filename char.

            if (options.AllowWindowsPaths)
                input = input.Replace("\\", "/");

            // treat the test path as a set of pathparts.
            var f = slashSplit.Split(input);
            //if (options.debug) {
            //  console.error(this.pattern, "split", f)
            //}

            // just ONE of the pattern sets in this.set needs to match
            // in order for it to be valid.  If negating, then just one
            // match means that we have failed.
            // Either way, return on the first hit.

            foreach (var pattern in set)
            {
                var hit = this.MatchOne(f, pattern.ToList(), partial);
                if (hit)
                {
                    if (options.FlipNegate) return true;
                    return !this.negate;
                }

            }

            // didn't get any hits.  this is success if it's a negative
            // pattern, failure otherwise.
            if (options.FlipNegate) return false;
            return this.negate;
        }

        // set partial to true to test if, for example,
        // "/a/b" matches the start of "/*/b/*/d"
        // Partial means, if you run out of file before you run
        // out of pattern, then that's fine, as long as all
        // the parts match.
        bool MatchOne(IList<string> file, IList<ParseItem> pattern, bool partial)
        {

            //if (options.debug) {
            //  console.error("matchOne",
            //                { "this": this
            //                , file: file
            //                , pattern: pattern })
            //}

            if (options.MatchBase && pattern.Count == 1)
            {
                file = new[] { file.Last(s => !string.IsNullOrEmpty(s)) };
            }

            //if (options.debug) {
            //  console.error("matchOne", file.length, pattern.length)
            //}
            int fi = 0, pi = 0;
            for (; (fi < file.Count) && (pi < pattern.Count); fi++, pi++)
            {

                //if (options.debug) {
                //  console.error("matchOne loop")
                //}
                ParseItem p = pattern[pi];
                string f = file[fi];

                //if (options.debug) {
                //  console.error(pattern, p, f)
                //}

                // should be impossible.
                // some invalid regexp stuff in the set.
                if (p == null) return false;

                if (p is GlobStar)
                {
                    //if (options.debug)
                    //  console.error('GLOBSTAR', [pattern, p, f])

                    // "**"
                    // a/**/b/**/c would match the following:
                    // a/b/x/y/z/c
                    // a/x/y/z/b/c
                    // a/b/x/b/x/c
                    // a/b/c
                    // To do this, take the rest of the pattern after
                    // the **, and see if it would match the file remainder.
                    // If so, return success.
                    // If not, the ** "swallows" a segment, and try again.
                    // This is recursively awful.
                    //
                    // a/**/b/**/c matching a/b/x/y/z/c
                    // - a matches a
                    // - doublestar
                    //   - matchOne(b/x/y/z/c, b/**/c)
                    //     - b matches b
                    //     - doublestar
                    //       - matchOne(x/y/z/c, c) -> no
                    //       - matchOne(y/z/c, c) -> no
                    //       - matchOne(z/c, c) -> no
                    //       - matchOne(c, c) yes, hit
                    int fr = fi, pr = pi + 1;
                    if (pr == pattern.Count)
                    {
                        //if (options.debug)
                        //  console.error('** at the end')
                        // a ** at the end will just swallow the rest.
                        // We have found a match.
                        // however, it will not swallow /.x, unless
                        // options.dot is set.
                        // . and .. are *never* matched by **, for explosively
                        // exponential reasons.
                        for (; fi < file.Count; fi++)
                        {
                            if (file[fi] == "." || file[fi] == ".." ||
                                (!options.Dot && !string.IsNullOrEmpty(file[fi]) && file[fi][0] == '.')) return false;
                        }
                        return true;
                    }

                    // ok, let's see if we can swallow whatever we can.
                    while (fr < file.Count)
                    {
                        var swallowee = file[fr];

                        //if (options.debug) {
                        //  console.error('\nglobstar while',
                        //                file, fr, pattern, pr, swallowee)
                        //}

                        // XXX remove this slice.  Just pass the start index.
                        if (this.MatchOne(file.Skip(fr).ToList(), pattern.Skip(pr).ToList(), partial))
                        {
                            //if (options.debug)
                            //  console.error('globstar found match!', fr, file.Count, swallowee)
                            // found a match.
                            return true;
                        }
                        else
                        {
                            // can't swallow "." or ".." ever.
                            // can only swallow ".foo" when explicitly asked.
                            if (swallowee == "." || swallowee == ".." ||
                                (!options.Dot && swallowee[0] == '.'))
                            {
                                //if (options.debug)
                                //  console.error("dot detected!", file, fr, pattern, pr)
                                break;
                            }

                            // ** swallows a segment, and continue.
                            //if (options.debug)
                            //  console.error('globstar swallow a segment, and continue')
                            fr++;
                        }
                    }
                    // no match was found.
                    // However, in partial mode, we can't say this is necessarily over.
                    // If there's more *pattern* left, then
                    if (partial)
                    {
                        // ran out of file
                        // console.error("\n>>> no match, partial?", file, fr, pattern, pr)
                        if (fr == file.Count) return true;
                    }
                    return false;
                }

                // something other than **
                // non-magic patterns just have to match exactly
                // patterns with magic have been turned into regexps.
                if (!p.Match(f, options))
                    return false;
            }

            // Note: ending in / means that we'll get a final ""
            // at the end of the pattern.  This can only match a
            // corresponding "" at the end of the file.
            // If the file ends in /, then it can only match a
            // a pattern that ends in /, unless the pattern just
            // doesn't have any more for it. But, a/b/ should *not*
            // match "a/b/*", even though "" matches against the
            // [^/]*? pattern, except in partial mode, where it might
            // simply not be reached yet.
            // However, a/b/ should still satisfy a/*

            // now either we fell off the end of the pattern, or we're done.
            if (fi == file.Count && pi == pattern.Count)
            {
                // ran out of pattern and filename at the same time.
                // an exact hit!
                return true;
            }
            else if (fi == file.Count)
            {
                // ran out of file, but still had pattern left.
                // this is ok if we're doing the match as part of
                // a glob fs traversal.
                return partial;
            }
            else if (pi == pattern.Count)
            {
                // ran out of pattern, still have file left.
                // this is only acceptable if we're on the very last
                // empty segment of a file with a trailing slash.
                // a/* should match a/b/
                var emptyFileEnd = (fi == file.Count - 1) && (file[fi] == "");
                return emptyFileEnd;
            }

            // should be unreachable.
            throw new InvalidOperationException("wtf?");
        }


        // replace stuff like \* with *
        static readonly Regex globUnescaper = new Regex(@"\\(.)");
        static string GlobUnescape(string s)
        {
            return globUnescaper.Replace(s, "$1");
        }
    }
}