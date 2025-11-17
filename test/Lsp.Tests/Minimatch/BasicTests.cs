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
namespace Minimatch.Tests
{
    public class BasicTests
    {
        private static readonly List<Tuple<string, string>> actualRegexes = new List<Tuple<string, string>>();

        private static void TestCase(string pattern, IList<string> expected, Options? options = null, IEnumerable<string>? input = null)
        {
            input ??= Files;

            Assert.Equal(
                string.Join(Environment.NewLine, expected.OrderBy(s => s)),
                string.Join(Environment.NewLine, Minimatcher.Filter(input, pattern, options).OrderBy(s => s))
            );

            var regex = Minimatcher.CreateRegex(pattern, options);
            actualRegexes.Add(Tuple.Create(pattern, regex == null ? "false" : "/" + regex + "/" + ( regex.Options == RegexOptions.IgnoreCase ? "i" : "" )));
        }

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private static void AssertRegexes(params string[] expectedRegexes)
        {
            Assert.Equal(expectedRegexes.Length, actualRegexes.Count);
            for (var i = 0; i < actualRegexes.Count; i++)
            {
                Assert.Equal(expectedRegexes[i], actualRegexes[i].Item2);
            }
        }

        private static void AddFiles(params string[] entries) => Files.AddRange(entries);

        private static void ReplaceFiles(params string[] entries)
        {
            Files.Clear();
            Files.AddRange(entries);
        }

        private static readonly List<string> Files = new List<string>();

        public BasicTests()
        {
            ReplaceFiles(
                "a", "b", "c", "d", "abc"
              , "abd", "abe", "bb", "bcd"
              , "ca", "cb", "dd", "de"
              , "bdir/", "bdir/cfile"
            );
            actualRegexes.Clear();
        }

        [Fact]
        public void BashCookBook()
        {
            //"http://www.bashcookbook.com/bashinfo/source/bash-1.14.7/tests/glob-test"
            TestCase("a*", new[] { "a", "abc", "abd", "abe" });
            TestCase("X*", new[] { "X*" }, new Options { NoNull = true });

            // allow null glob expansion
            TestCase("X*", new string[0]);

            // isaacs: Slightly different than bash/sh/ksh
            // \\* is not un-escaped to literal "*" in a failed match,
            // but it does make it get treated as a literal star
            TestCase("\\*", new[] { "\\*" }, new Options { NoNull = true });
            TestCase("\\**", new[] { "\\**" }, new Options { NoNull = true });
            TestCase("\\*\\*", new[] { "\\*\\*" }, new Options { NoNull = true });

            TestCase("b*/", new[] { "bdir/" });
            TestCase("c*", new[] { "c", "ca", "cb" });
            TestCase("**", Files);

            TestCase("\\.\\./*/", new[] { "\\.\\./*/" }, new Options { NoNull = true });
            TestCase("s/\\..*//", new[] { "s/\\..*//" }, new Options { NoNull = true });

            AssertRegexes(
                "/^(?:(?=.)a[^/]*?)$/",
                "/^(?:(?=.)X[^/]*?)$/",
                "/^(?:(?=.)X[^/]*?)$/",
                "/^(?:\\*)$/",
                "/^(?:(?=.)\\*[^/]*?)$/",
                "/^(?:\\*\\*)$/",
                "/^(?:(?=.)b[^/]*?\\/)$/",
                "/^(?:(?=.)c[^/]*?)$/",
                "/^(?:(?:(?!(?:\\/|^)\\.).)*?)$/",
                "/^(?:\\.\\.\\/(?!\\.)(?=.)[^/]*?\\/)$/",
                "/^(?:s\\/(?=.)\\.\\.[^/]*?\\/)$/"
            );
        }

        [Fact]
        public void LegendaryLarryCrashesBashes()
        {
            TestCase(
                "/^root:/{s/^[^:]*:[^:]*:([^:]*).*$/\\1/"
              , new[] { "/^root:/{s/^[^:]*:[^:]*:([^:]*).*$/\\1/" }, new Options { NoNull = true }
            );
            TestCase(
                "/^root:/{s/^[^:]*:[^:]*:([^:]*).*$/\u0001/"
              , new[] { "/^root:/{s/^[^:]*:[^:]*:([^:]*).*$/\u0001/" }, new Options { NoNull = true }
            );

            AssertRegexes(
                "/^(?:\\/\\^root:\\/\\{s\\/(?=.)\\^[^:][^/]*?:[^:][^/]*?:\\([^:]\\)[^/]*?\\.[^/]*?\\$\\/1\\/)$/",
                "/^(?:\\/\\^root:\\/\\{s\\/(?=.)\\^[^:][^/]*?:[^:][^/]*?:\\([^:]\\)[^/]*?\\.[^/]*?\\$\\/\u0001\\/)$/"
            );
        }

        [Fact]
        public void CharacterClasses()
        {
            TestCase("[a-c]b*", new[] { "abc", "abd", "abe", "bb", "cb" });
            TestCase(
                "[a-y]*[^c]", new[] {
                    "abd", "abe", "bb", "bcd",
                    "bdir/", "ca", "cb", "dd", "de"
                }
            );
            TestCase("a*[^c]", new[] { "abd", "abe" });
            AddFiles("a-b", "aXb");
            TestCase("a[X-]b", new[] { "a-b", "aXb" });
            AddFiles(".x", ".y");
            TestCase("[^a-c]*", new[] { "d", "dd", "de" });
            AddFiles("a*b/", "a*b/ooo");
            TestCase("a\\*b/*", new[] { "a*b/ooo" });
            TestCase("a\\*?/*", new[] { "a*b/ooo" });
            TestCase("*\\\\!*", new string[0], new Options(), new[] { "echo !7" });
            TestCase("*\\!*", new[] { "echo !7" }, null, new[] { "echo !7" });
            TestCase("*.\\*", new[] { "r.*" }, null, new[] { "r.*" });
            TestCase("a[b]c", new[] { "abc" });
            TestCase("a[\\b]c", new[] { "abc" });
            TestCase("a?c", new[] { "abc" });
            TestCase("a\\*c", new string[0], new Options(), new[] { "abc" });
            TestCase("", new[] { "" }, new Options(), new[] { "" });

            AssertRegexes(
                "/^(?:(?!\\.)(?=.)[a-c]b[^/]*?)$/",
                "/^(?:(?!\\.)(?=.)[a-y][^/]*?[^c])$/",
                "/^(?:(?=.)a[^/]*?[^c])$/",
                "/^(?:(?=.)a[X-]b)$/",
                "/^(?:(?!\\.)(?=.)[^a-c][^/]*?)$/",
                "/^(?:a\\*b\\/(?!\\.)(?=.)[^/]*?)$/",
                "/^(?:(?=.)a\\*[^/]\\/(?!\\.)(?=.)[^/]*?)$/",
                "/^(?:(?!\\.)(?=.)[^/]*?\\\\\\![^/]*?)$/",
                "/^(?:(?!\\.)(?=.)[^/]*?\\![^/]*?)$/",
                "/^(?:(?!\\.)(?=.)[^/]*?\\.\\*)$/",
                "/^(?:(?=.)a[b]c)$/",
                "/^(?:(?=.)a[b]c)$/",
                "/^(?:(?=.)a[^/]c)$/",
                "/^(?:a\\*c)$/",
                "false"
            );
        }

        [Fact]
        public void AppleBash()
        {
            AddFiles("a-b", "aXb", ".x", ".y", "a*b/", "a*b/ooo");

            //http://www.opensource.apple.com/source/bash/bash-23/bash/tests/glob-test"
            AddFiles("man/", "man/man1/", "man/man1/bash.1");
            TestCase("*/man*/bash.*", new[] { "man/man1/bash.1" });
            TestCase("man/man1/bash.1", new[] { "man/man1/bash.1" });
            TestCase("a***c", new[] { "abc" }, null, new[] { "abc" });
            TestCase("a*****?c", new[] { "abc" }, null, new[] { "abc" });
            TestCase("?*****??", new[] { "abc" }, null, new[] { "abc" });
            TestCase("*****??", new[] { "abc" }, null, new[] { "abc" });
            TestCase("?*****?c", new[] { "abc" }, null, new[] { "abc" });
            TestCase("?***?****c", new[] { "abc" }, null, new[] { "abc" });
            TestCase("?***?****?", new[] { "abc" }, null, new[] { "abc" });
            TestCase("?***?****", new[] { "abc" }, null, new[] { "abc" });
            TestCase("*******c", new[] { "abc" }, null, new[] { "abc" });
            TestCase("*******?", new[] { "abc" }, null, new[] { "abc" });
            TestCase("a*cd**?**??k", new[] { "abcdecdhjk" }, null, new[] { "abcdecdhjk" });
            TestCase("a**?**cd**?**??k", new[] { "abcdecdhjk" }, null, new[] { "abcdecdhjk" });
            TestCase("a**?**cd**?**??k***", new[] { "abcdecdhjk" }, null, new[] { "abcdecdhjk" });
            TestCase("a**?**cd**?**??***k", new[] { "abcdecdhjk" }, null, new[] { "abcdecdhjk" });
            TestCase("a**?**cd**?**??***k**", new[] { "abcdecdhjk" }, null, new[] { "abcdecdhjk" });
            TestCase("a****c**?**??*****", new[] { "abcdecdhjk" }, null, new[] { "abcdecdhjk" });
            TestCase("[-abc]", new[] { "-" }, null, new[] { "-" });
            TestCase("[abc-]", new[] { "-" }, null, new[] { "-" });
            TestCase("\\", new[] { "\\" }, null, new[] { "\\" });
            TestCase("[\\\\]", new[] { "\\" }, null, new[] { "\\" });
            TestCase("[[]", new[] { "[" }, null, new[] { "[" });
            TestCase("[", new[] { "[" }, null, new[] { "[" });


            // a right bracket shall lose its special meaning and
            // represent itself in a bracket expression if it occurs
            // first in the list.  -- POSIX.2 2.8.3.2
            TestCase("[*", new[] { "[abc" }, null, new[] { "[abc" });


            TestCase("[]]", new[] { "]" }, null, new[] { "]" });
            TestCase("[]-]", new[] { "]" }, null, new[] { "]" });
            TestCase(@"[a-\z]", new[] { "p" }, null, new[] { "p" });
            TestCase("??**********?****?", new string[0], new Options(), new[] { "abc" });
            TestCase("??**********?****c", new string[0], new Options(), new[] { "abc" });
            TestCase("?************c****?****", new string[0], new Options(), new[] { "abc" });
            TestCase("*c*?**", new string[0], new Options(), new[] { "abc" });
            TestCase("a*****c*?**", new string[0], new Options(), new[] { "abc" });
            TestCase("a********???*******", new string[0], new Options(), new[] { "abc" });
            TestCase("[]", new string[0], new Options(), new[] { "a" });
            TestCase("[abc", new string[0], new Options(), new[] { "[" });

            AssertRegexes(
                "/^(?:(?!\\.)(?=.)[^/]*?\\/(?=.)man[^/]*?\\/(?=.)bash\\.[^/]*?)$/",
                "/^(?:man\\/man1\\/bash\\.1)$/",
                "/^(?:(?=.)a[^/]*?[^/]*?[^/]*?c)$/",
                "/^(?:(?=.)a[^/]*?[^/]*?[^/]*?[^/]*?[^/]*?[^/]c)$/",
                "/^(?:(?!\\.)(?=.)[^/][^/]*?[^/]*?[^/]*?[^/]*?[^/]*?[^/][^/])$/",
                "/^(?:(?!\\.)(?=.)[^/]*?[^/]*?[^/]*?[^/]*?[^/]*?[^/][^/])$/",
                "/^(?:(?!\\.)(?=.)[^/][^/]*?[^/]*?[^/]*?[^/]*?[^/]*?[^/]c)$/",
                "/^(?:(?!\\.)(?=.)[^/][^/]*?[^/]*?[^/]*?[^/][^/]*?[^/]*?[^/]*?[^/]*?c)$/",
                "/^(?:(?!\\.)(?=.)[^/][^/]*?[^/]*?[^/]*?[^/][^/]*?[^/]*?[^/]*?[^/]*?[^/])$/",
                "/^(?:(?!\\.)(?=.)[^/][^/]*?[^/]*?[^/]*?[^/][^/]*?[^/]*?[^/]*?[^/]*?)$/",
                "/^(?:(?!\\.)(?=.)[^/]*?[^/]*?[^/]*?[^/]*?[^/]*?[^/]*?[^/]*?c)$/",
                "/^(?:(?!\\.)(?=.)[^/]*?[^/]*?[^/]*?[^/]*?[^/]*?[^/]*?[^/]*?[^/])$/",
                "/^(?:(?=.)a[^/]*?cd[^/]*?[^/]*?[^/][^/]*?[^/]*?[^/][^/]k)$/",
                "/^(?:(?=.)a[^/]*?[^/]*?[^/][^/]*?[^/]*?cd[^/]*?[^/]*?[^/][^/]*?[^/]*?[^/][^/]k)$/",
                "/^(?:(?=.)a[^/]*?[^/]*?[^/][^/]*?[^/]*?cd[^/]*?[^/]*?[^/][^/]*?[^/]*?[^/][^/]k[^/]*?[^/]*?[^/]*?)$/",
                "/^(?:(?=.)a[^/]*?[^/]*?[^/][^/]*?[^/]*?cd[^/]*?[^/]*?[^/][^/]*?[^/]*?[^/][^/][^/]*?[^/]*?[^/]*?k)$/",
                "/^(?:(?=.)a[^/]*?[^/]*?[^/][^/]*?[^/]*?cd[^/]*?[^/]*?[^/][^/]*?[^/]*?[^/][^/][^/]*?[^/]*?[^/]*?k[^/]*?[^/]*?)$/",
                "/^(?:(?=.)a[^/]*?[^/]*?[^/]*?[^/]*?c[^/]*?[^/]*?[^/][^/]*?[^/]*?[^/][^/][^/]*?[^/]*?[^/]*?[^/]*?[^/]*?)$/",
                "/^(?:(?!\\.)(?=.)[-abc])$/",
                "/^(?:(?!\\.)(?=.)[abc-])$/",
                "/^(?:\\\\)$/",
                "/^(?:(?!\\.)(?=.)[\\\\])$/",
                "/^(?:(?!\\.)(?=.)[\\[])$/",
                "/^(?:\\[)$/",
                "/^(?:(?=.)\\[(?!\\.)(?=.)[^/]*?)$/",
                "/^(?:(?!\\.)(?=.)[\\]])$/",
                "/^(?:(?!\\.)(?=.)[\\]-])$/",
                "/^(?:(?!\\.)(?=.)[a-z])$/",
                "/^(?:(?!\\.)(?=.)[^/][^/][^/]*?[^/]*?[^/]*?[^/]*?[^/]*?[^/]*?[^/]*?[^/]*?[^/]*?[^/]*?[^/][^/]*?[^/]*?[^/]*?[^/]*?[^/])$/",
                "/^(?:(?!\\.)(?=.)[^/][^/][^/]*?[^/]*?[^/]*?[^/]*?[^/]*?[^/]*?[^/]*?[^/]*?[^/]*?[^/]*?[^/][^/]*?[^/]*?[^/]*?[^/]*?c)$/",
                "/^(?:(?!\\.)(?=.)[^/][^/]*?[^/]*?[^/]*?[^/]*?[^/]*?[^/]*?[^/]*?[^/]*?[^/]*?[^/]*?[^/]*?[^/]*?c[^/]*?[^/]*?[^/]*?[^/]*?[^/][^/]*?[^/]*?[^/]*?[^/]*?)$/",
                "/^(?:(?!\\.)(?=.)[^/]*?c[^/]*?[^/][^/]*?[^/]*?)$/",
                "/^(?:(?=.)a[^/]*?[^/]*?[^/]*?[^/]*?[^/]*?c[^/]*?[^/][^/]*?[^/]*?)$/",
                "/^(?:(?=.)a[^/]*?[^/]*?[^/]*?[^/]*?[^/]*?[^/]*?[^/]*?[^/]*?[^/][^/][^/][^/]*?[^/]*?[^/]*?[^/]*?[^/]*?[^/]*?[^/]*?)$/",
                "/^(?:\\[])$/",
                "/^(?:\\[abc)$/"
            );
        }

        [Fact]
        public void NoCase()
        {
            AddFiles("a-b", "aXb", ".x", ".y", "a*b/", "a*b/ooo", "man/", "man/man1/", "man/man1/bash.1");


            TestCase(
                "XYZ", new[] { "xYz" }, new Options { NoCase = true, /*null = true*/ }
              , new[] { "xYz", "ABC", "IjK" }
            );
            TestCase(
                "ab*", new[] { "ABC" }, new Options { NoCase = true, /*null = true*/ }
              , new[] { "xYz", "ABC", "IjK" }
            );
            TestCase(
                "[ia]?[ck]", new[] { "ABC", "IjK" }, new Options { NoCase = true, /*null = true*/ }
              , new[] { "xYz", "ABC", "IjK" }
            );


            AssertRegexes(
                "/^(?:(?=.)XYZ)$/i",
                "/^(?:(?=.)ab[^/]*?)$/i",
                "/^(?:(?!\\.)(?=.)[ia][^/][ck])$/i"
            );
        }

        [Fact]
        public void OneStar_TwoStar()
        {
            AddFiles("a-b", "aXb", ".x", ".y", "a*b/", "a*b/ooo", "man/", "man/man1/", "man/man1/bash.1");

            // [ pattern, new [] { matches }, MM opts, files, TAP opts]
            TestCase("{/*,*}", new string[0], new Options(), new[] { "/asdf/asdf/asdf" });
            TestCase("{/?,*}", new[] { "/a", "bb" }, new Options(), new[] { "/a", "/b/b", "/a/b/c", "bb" });

            AssertRegexes(
                "/^(?:\\/(?!\\.)(?=.)[^/]*?|(?!\\.)(?=.)[^/]*?)$/",
                "/^(?:\\/(?!\\.)(?=.)[^/]|(?!\\.)(?=.)[^/]*?)$/"
            );
        }

        [Fact]
        public void DotMatching()
        {
            AddFiles("a-b", "aXb", ".x", ".y", "a*b/", "a*b/ooo", "man/", "man/man1/", "man/man1/bash.1");

            //"Dots should not match unless requested"
            TestCase("**", new[] { "a/b" }, new Options(), new[] { "a/b", "a/.d", ".a/.d" });

            // .. and . can only match patterns starting with .,
            // even when options.Dot is set.
            ReplaceFiles("a/./b", "a/../b", "a/c/b", "a/.d/b");
            TestCase("a/*/b", new[] { "a/c/b", "a/.d/b" }, new Options { Dot = true });
            TestCase("a/.*/b", new[] { "a/./b", "a/../b", "a/.d/b" }, new Options { Dot = true });
            TestCase("a/*/b", new[] { "a/c/b" }, new Options { Dot = false });
            TestCase("a/.*/b", new[] { "a/./b", "a/../b", "a/.d/b" }, new Options { Dot = false });


            // this also tests that changing the options needs
            // to change the cache key, even if the pattern is
            // the same!
            TestCase(
                "**", new[] { "a/b", "a/.d", ".a/.d" }, new Options { Dot = true }
              , new[] { ".a/.d", "a/.d", "a/b" }
            );

            AssertRegexes(
                "/^(?:(?:(?!(?:\\/|^)\\.).)*?)$/",
                "/^(?:a\\/(?!(?:^|\\/)\\.{1,2}(?:$|\\/))(?=.)[^/]*?\\/b)$/",
                "/^(?:a\\/(?=.)\\.[^/]*?\\/b)$/",
                "/^(?:a\\/(?!\\.)(?=.)[^/]*?\\/b)$/",
                "/^(?:a\\/(?=.)\\.[^/]*?\\/b)$/",
                "/^(?:(?:(?!(?:\\/|^)(?:\\.{1,2})($|\\/)).)*?)$/"
            );
        }

        [Fact]
        public void ParenSlashes()
        {
            //AddFiles("a-b", "aXb", ".x", ".y", "a*b/", "a*b/ooo", "man/", "man/man1/", "man/man1/bash.1");

            //"paren sets cannot contain slashes"
            TestCase("*(a/b)", new[] { "*(a/b)" }, new Options { NoNull = true }, new[] { "a/b" });

            // brace sets trump all else.
            //
            // invalid glob pattern.  fails on bash4 and bsdglob.
            // however, in this implementation, it's easier just
            // to do the intuitive thing, and let brace-expansion
            // actually come before parsing any extglob patterns,
            // like the documentation seems to say.
            //
            // XXX: if anyone complains about this, either fix it
            // or tell them to grow up and stop complaining.
            //
            // bash/bsdglob says this:
            // , new [] { "*(a|{b),c)}", ["*(a|{b),c)}" }, new Options {}, new [] { "a", "ab", "ac", "ad" });
            // but we do this instead:
            TestCase("*(a|{b),c)}", new[] { "a", "ab", "ac" }, new Options(), new[] { "a", "ab", "ac", "ad" });

            // test partial parsing in the presence of comment/negation chars
            TestCase("[!a*", new[] { "[!ab" }, new Options(), new[] { "[!ab", "[ab" });
            TestCase("[#a*", new[] { "[#ab" }, new Options(), new[] { "[#ab", "[ab" });

            // like: {a,b|c\\,d\\\|e} except it's unclosed, so it has to be escaped.
            TestCase(
                "+(a|*\\|c\\\\|d\\\\\\|e\\\\\\\\|f\\\\\\\\\\|g"
              , new[] { "+(a|b\\|c\\\\|d\\\\|e\\\\\\\\|f\\\\\\\\|g" }
              , new Options(), new[] { "+(a|b\\|c\\\\|d\\\\|e\\\\\\\\|f\\\\\\\\|g", "a", "b\\c" }
            );


            // crazy nested {,,} and *(||) tests.
            ReplaceFiles(
                "a", "b", "c", "d"
              , "ab", "ac", "ad"
              , "bc", "cb"
              , "bc,d", "c,db", "c,d"
              , "d)", "(b|c", "*(b|c"
              , "b|c", "b|cc", "cb|c"
              , "x(a|b|c)", "x(a|c)"
              , "(a|b|c)", "(a|c)"
            );
            TestCase("*(a|{b,c})", new[] { "a", "b", "c", "ab", "ac" });
            TestCase("{a,*(b|c,d)}", new[] { "a", "(b|c", "*(b|c", "d)" });
            // a
            // *(b|c)
            // *(b|d)
            TestCase("{a,*(b|{c,d})}", new[] { "a", "b", "bc", "cb", "c", "d" });
            TestCase("*(a|{b|c,c})", new[] { "a", "b", "c", "ab", "ac", "bc", "cb" });


            // test various flag settings.
            TestCase(
                "*(a|{b|c,c})", new[] { "x(a|b|c)", "x(a|c)", "(a|b|c)", "(a|c)" }
              , new Options { NoExt = true }
            );
            TestCase(
                "a?b", new[] { "x/y/acb", "acb/" }, new Options { MatchBase = true }
              , new[] { "x/y/acb", "acb/", "acb/d/e", "x/y/acb/d" }
            );
            TestCase("#*", new[] { "#a", "#b" }, new Options { NoComment = true }, new[] { "#a", "#b", "c#d" });

            AssertRegexes(
                "/^(?:(?!\\.)(?=.)[^/]*?\\(a\\/b\\))$/",
                "/^(?:(?!\\.)(?=.)(?:a|b)*|(?!\\.)(?=.)(?:a|c)*)$/",
                "/^(?:(?=.)\\[(?=.)\\!a[^/]*?)$/",
                "/^(?:(?=.)\\[(?=.)#a[^/]*?)$/",
                "/^(?:(?=.)\\+\\(a\\|[^/]*?\\|c\\\\\\\\\\|d\\\\\\\\\\|e\\\\\\\\\\\\\\\\\\|f\\\\\\\\\\\\\\\\\\|g)$/",
                "/^(?:(?!\\.)(?=.)(?:a|b)*|(?!\\.)(?=.)(?:a|c)*)$/",
                "/^(?:a|(?!\\.)(?=.)[^/]*?\\(b\\|c|d\\))$/",
                "/^(?:a|(?!\\.)(?=.)(?:b|c)*|(?!\\.)(?=.)(?:b|d)*)$/",
                "/^(?:(?!\\.)(?=.)(?:a|b|c)*|(?!\\.)(?=.)(?:a|c)*)$/",
                "/^(?:(?!\\.)(?=.)[^/]*?\\(a\\|b\\|c\\)|(?!\\.)(?=.)[^/]*?\\(a\\|c\\))$/",
                "/^(?:(?=.)a[^/]b)$/",
                "/^(?:(?=.)#[^/]*?)$/"
            );
        }

        [Fact]
        public void NegationTests()
        {
            // begin channelling Boole and deMorgan...

            ReplaceFiles("d", "e", "!ab", "!abc", "a!b", "\\!a");
            // anything that is NOT a* matches.
            TestCase("!a*", new[] { "\\!a", "d", "e", "!ab", "!abc" });

            // anything that IS !a* matches.
            TestCase("!a*", new[] { "!ab", "!abc" }, new Options { NoNegate = true });

            // anything that IS a* matches
            TestCase("!!a*", new[] { "a!b" });

            // anything that is NOT !a* matches
            TestCase("!\\!a*", new[] { "a!b", "d", "e", "\\!a" });

            // negation nestled within a pattern
            ReplaceFiles(
                "foo.js"
              , "foo.bar"
                // can't match this one without negative lookbehind.
              , "foo.js.js"
              , "blar.js"
              , "foo."
              , "boo.js.boo"
            );
            TestCase("*.!(js)", new[] { "foo.bar", "foo.", "boo.js.boo" });

            // https://github.com/isaacs/minimatch/issues/5
            ReplaceFiles(
                "a/b/.x/c"
              , "a/b/.x/c/d"
              , "a/b/.x/c/d/e"
              , "a/b/.x"
              , "a/b/.x/"
              , "a/.x/b"
              , ".x"
              , ".x/"
              , ".x/a"
              , ".x/a/b"
              , "a/.x/b/.x/c"
              , ".x/.x"
            );

            TestCase("**/.x/**", new[] { ".x/", ".x/a", ".x/a/b", "a/.x/b", "a/b/.x/", "a/b/.x/c", "a/b/.x/c/d", "a/b/.x/c/d/e" });

            AssertRegexes(
                "/^(?!^(?:(?=.)a[^/]*?)$).*$/",
                "/^(?:(?=.)\\!a[^/]*?)$/",
                "/^(?:(?=.)a[^/]*?)$/",
                "/^(?!^(?:(?=.)\\!a[^/]*?)$).*$/",
                "/^(?:(?!\\.)(?=.)[^/]*?\\.(?:(?!js)[^/]*?))$/",
                "/^(?:(?:(?!(?:\\/|^)\\.).)*?\\/\\.x\\/(?:(?!(?:\\/|^)\\.).)*?)$/"
            );
        }
    }
}
