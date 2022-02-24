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
    /// Text symbol parser state data.
    /// </summary>
    public interface ITextSymbolsParserState
    {
        /// <summary>
        /// Last parsed symbol.
        /// </summary>
        string LastReadSymbol { get; }

        /// <summary>
        /// Starting position in <see cref="TextToParse"/>, from which the symbols will be parsed.
        /// </summary>
        int ParsedTextStartPosition { get; }

        /// <summary>
        /// Position of parsed text end. For example if ParsedTextStartPosition is 2 and we want to parse 100 characters in <see cref="TextToParse"/>,
        /// the value of <see cref="ParsedTextEnd"/> will be 102.
        /// </summary>
        int ParsedTextEnd { get; }

        /// <summary>
        /// Returns true if the position in parsed text reached the end of text to parse.
        /// </summary>
        bool IsEndOfTextReached { get; }

        /// <summary>
        /// Current character.
        /// </summary>
        char CurrentChar { get; }

        /// <summary>
        /// Current position in text.
        /// </summary>
        int PositionInText { get; }

        /// <summary>
        /// Returns the text currently being parsed.
        /// </summary>
        [NotNull]
        string TextToParse { get; }
    }
}