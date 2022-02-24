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
    /// Creates instances of <see cref="ITextSymbolsParser"/>. Default implementation is <see cref="TextSymbolsParserFactory"/>.
    /// </summary>
    public interface ITextSymbolsParserFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="ITextSymbolsParser"/>.
        /// </summary>
        /// <param name="textToParse">Text to parse.</param>
        /// <param name="isValidSymbolCharacter">An instance of <see cref="IsValidSymbolCharacterDelegate"/>.</param>
        [NotNull]
        ITextSymbolsParser CreateTextSymbolsParser([NotNull] string textToParse, [NotNull] IsValidSymbolCharacterDelegate isValidSymbolCharacter);

        /// <summary>
        /// Creates an instance of <see cref="ITextSymbolsParser"/>.
        /// </summary>
        /// <param name="textToParse">Text to parse.</param>
        /// <param name="positionInText">Position in text the created parser will be at.</param>
        /// <param name="numberOfCharactersToParse">Specifies the end of the text along with <paramref name="positionInText"/>. In other words the parser will start at <paramref name="positionInText"/> and stop at <paramref name="positionInText"/>+<paramref name="numberOfCharactersToParse"/>.
        /// </param>
        /// <param name="isValidSymbolCharacter">An instance of <see cref="IsValidSymbolCharacterDelegate"/>.</param>
        [NotNull]
        ITextSymbolsParser CreateTextSymbolsParser([NotNull] string textToParse, int positionInText, int numberOfCharactersToParse,
                                                   [NotNull] IsValidSymbolCharacterDelegate isValidSymbolCharacter);
    }

    /// <inheritdoc />
    public class TextSymbolsParserFactory : ITextSymbolsParserFactory
    {
        /// <inheritdoc />
        public ITextSymbolsParser CreateTextSymbolsParser(string textToParse, IsValidSymbolCharacterDelegate isValidSymbolCharacter)
        {
            return new TextSymbolsParser(textToParse, isValidSymbolCharacter);
        }

        /// <inheritdoc />
        public ITextSymbolsParser CreateTextSymbolsParser(string textToParse, int positionInText, int numberOfCharactersToParse,
            IsValidSymbolCharacterDelegate isValidSymbolCharacter)
        {
            return new TextSymbolsParser(textToParse, positionInText, numberOfCharactersToParse, isValidSymbolCharacter);
        }
    }
}