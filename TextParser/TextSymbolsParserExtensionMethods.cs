// This software is part of the TextParser library
// Copyright © 2018 TextParser Contributors
// http://oroptimizer.com
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using JetBrains.Annotations;

namespace TextParser
{
    /// <summary>
    /// Extension methods for <see cref="ITextSymbolsParser"/>.
    /// </summary>
    public static class TextSymbolsParserExtensionMethods
    {
        /// <summary>
        /// Parses a symbol that might be enclosed in braces. For example if the value of <see cref="ITextSymbolsParserState.PositionInText"/> is 1 in the two texts
        /// below " [Table1] (..." Table1 (...", the parsed symbol will be Table1, and the value of <see cref="ITextSymbolsParserState.PositionInText"/> after the method is called will
        /// be the position of "(" character.
        /// </summary>
        public static bool ReadSymbolEnclosedInBraces([NotNull] this ITextSymbolsParser textSymbolsParser, bool enclosingBracesAreOptional,
                                                      char openingBrace, char closingBrace,
                                                      [CanBeNull] IsValidSymbolCharacterDelegate isValidSymbolCharacterDelegate = null)
        {
            var startedWithBraces = false;

            if (textSymbolsParser.CurrentChar == openingBrace)
            {
                startedWithBraces = true;

                if (!textSymbolsParser.SkipCurrentCharacterAndSpaces())
                    return false;
            }
            else if (!enclosingBracesAreOptional)
            {
                throw new ParseTextException(new ParseTextErrorDetails(textSymbolsParser.PositionInText, $"Invalid character. Expected '{openingBrace}'."));
            }

            if (!textSymbolsParser.ReadSymbol(isValidSymbolCharacterDelegate))
                return false;

            if (startedWithBraces)
            {
                if (!textSymbolsParser.SkipSpaces())
                    return false;

                if (textSymbolsParser.CurrentChar != closingBrace)
                    return false;

                textSymbolsParser.SkipCurrentCharacterAndSpaces();
            }

            return true;
        }
    }
}