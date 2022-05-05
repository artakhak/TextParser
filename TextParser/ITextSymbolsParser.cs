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

using System;
using System.Text;
using JetBrains.Annotations;

namespace TextParser
{
    /// <summary>
    /// A delegate that returns true if character at current position in text is a valid symbol character.
    /// </summary>
    /// <param name="character">Character to check.</param>
    /// <param name="positionInParsedText">Position in parsed text.</param>
    /// <param name="textSymbolsParserState">Text symbols parse state data. Example is an instance of <see cref="ITextSymbolsParser"/>.</param>
    public delegate bool IsValidSymbolCharacterDelegate(char character, int positionInParsedText, [NotNull] ITextSymbolsParserState textSymbolsParserState);

    /// <summary>
    /// An interface for parsing text symbols. The default implementation is <see cref="TextSymbolsParser"/>.
    /// </summary>
    public interface ITextSymbolsParser : ITextSymbolsParserState
    {
        /// <summary>
        /// Moves past current character, and skips the spaces.
        /// Returns true, if non-space character is encountered. Returns false, if end of text is reached.
        /// </summary>
        bool SkipCurrentCharacterAndSpaces();

        /// <summary>
        /// Skips <paramref name="numberOfCharactersToSkip"/> characters.
        /// Unlike <see cref="SkipCurrentCharacterAndSpaces"/> method, does not skip spaces after skipping <paramref name="numberOfCharactersToSkip"/> characters.
        /// To skip spaces, method <see cref="ITextSymbolsParser.SkipSpaces()"/> should be called explicitly.
        /// </summary>
        /// <param name="numberOfCharactersToSkip">Number of characters to skip</param>
        /// <returns>
        /// Returns false if (<see cref="ITextSymbolsParserState.PositionInText"/>+<paramref name="numberOfCharactersToSkip"/>) is greater than
        /// <see cref="ITextSymbolsParserState.ParsedTextEnd"/> or if <paramref name="numberOfCharactersToSkip"/> is negative.
        /// </returns>
        bool SkipCharacters(int numberOfCharactersToSkip);

        /// <summary>
        /// Moves to position <paramref name="positionInText"/>.
        /// The value of <see cref="ITextSymbolsParserState.PositionInText"/> property will be set to <paramref name="positionInText"/>, and there might be spaces
        /// after <see cref="ITextSymbolsParserState.PositionInText"/> position. To skip spaces, method <see cref="ITextSymbolsParser.SkipSpaces()"/> should be called explicitly.
        /// </summary>
        /// <param name="positionInText">
        /// Position in text to move to.
        /// </param>
        /// <returns>
        /// Returns true if <paramref name="positionInText"/> is less than <see cref="ITextSymbolsParserState.ParsedTextEnd"/> and greater
        /// or equal then <see cref="ITextSymbolsParserState.ParsedTextStartPosition"/>.
        /// Returns false otherwise. </returns>
        bool MoveToToPosition(int positionInText);

        /// <summary>
        /// Skips spaces and stops at first non-space character.
        /// </summary>
        /// <returns>
        /// Returns true if non-space character is encountered.
        /// returns false, if end of text is reached without encountering any non-space character.
        /// </returns>
        bool SkipSpaces();

        /// <summary>
        /// Reads a valid symbol.
        /// Returns true, if the character at <see cref="ITextSymbolsParserState.PositionInText"/> is a valid symbol character. Returns false otherwise.
        /// </summary>
        /// <param name="isValidSymbolCharacter">
        /// A delegate that will be used to check if current character is valid symbol character.
        /// The parameter is optional. If the value is null, the default <see cref="IsValidSymbolCharacterDelegate"/> will be used, which
        /// normally would be past as a constructor parameter of the implementation.
        /// </param>
        /// <returns>Returns true, if the character at <see cref="ITextSymbolsParserState.PositionInText"/> is a valid symbol character. Returns false otherwise.</returns>
        bool ReadSymbol([CanBeNull] IsValidSymbolCharacterDelegate isValidSymbolCharacter = null);

        /// <summary>
        /// Moves the position to next character. The value of <see cref="ITextSymbolsParserState.CurrentChar"/> will be the character at next position,
        /// if end of text was not reached. Otherwise, the value of <see cref="ITextSymbolsParserState.CurrentChar"/> will be '\0'.
        /// </summary>
        /// <returns>Returns false if the current character is the last character in parsed text (i.e., we cannot move to next character). Returns true otherwise.</returns>
        bool ReadNextCharacter();
    }

    /// <inheritdoc />
    public class TextSymbolsParser : ITextSymbolsParser
    {
        [NotNull]
        private readonly IsValidSymbolCharacterDelegate _isValidSymbolCharacter;
        [NotNull]
        private readonly string _textToParse;

        private const char EndOfTextChar = '\0';

        private int _positionInText;

        private readonly int _textStartPosition;
        private readonly int _textEndPosition;
        private char _currentChar = EndOfTextChar;
        private string _lastReadSymbol;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="textToParse">Text to parse.</param>
        /// <param name="isValidSymbolCharacter">An instance of <see cref="IsValidSymbolCharacterDelegate"/>.</param>
        public TextSymbolsParser([NotNull] string textToParse, [NotNull] IsValidSymbolCharacterDelegate isValidSymbolCharacter) : this(textToParse, 0, textToParse.Length, isValidSymbolCharacter)
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="textToParse">Text to parse.</param>
        /// <param name="positionInText">Position in text the created parser will be at.</param>
        /// <param name="numberOfCharactersToParse">Specifies the end of the text along with <paramref name="positionInText"/>. In other words the parser will start at <paramref name="positionInText"/> and stop at <paramref name="positionInText"/>+<paramref name="numberOfCharactersToParse"/>.
        /// </param>
        /// <param name="isValidSymbolCharacter">An instance of <see cref="IsValidSymbolCharacterDelegate"/>.</param>
        public TextSymbolsParser([NotNull] string textToParse, int positionInText, int numberOfCharactersToParse,
                                 [NotNull] IsValidSymbolCharacterDelegate isValidSymbolCharacter)
        {
            _textToParse = textToParse;
            _positionInText = positionInText;
            _isValidSymbolCharacter = isValidSymbolCharacter;

            _textStartPosition = positionInText;
            _textEndPosition = _textStartPosition + numberOfCharactersToParse;

            if (positionInText < 0)
                throw new ArgumentException($"Invalid value of '{nameof(positionInText)}'. The value is {positionInText}.", nameof(numberOfCharactersToParse));

            if (_textEndPosition > _textToParse.Length)
                throw new ArgumentException($"Invalid value of '{nameof(numberOfCharactersToParse)}'. The value is {numberOfCharactersToParse} and is too large.", nameof(numberOfCharactersToParse));

            if (!IsEndOfTextReached)
                _currentChar = _textToParse[_positionInText];
        }

        /// <inheritdoc />
        public string LastReadSymbol => _lastReadSymbol;

        /// <inheritdoc />
        public int ParsedTextStartPosition => _textStartPosition;

        /// <inheritdoc />
        public int ParsedTextEnd => _textEndPosition;

        /// <inheritdoc />
        public char CurrentChar => _currentChar;

        /// <inheritdoc />
        public int PositionInText => _positionInText;

        /// <inheritdoc />
        public bool IsEndOfTextReached => _positionInText >= _textEndPosition;

        /// <inheritdoc />
        public bool SkipSpaces()
        {
            while (_positionInText < _textEndPosition)
            {
                _currentChar = _textToParse[_positionInText];

                if (!Char.IsWhiteSpace(_currentChar))
                    return true;

                ++_positionInText;
            }

            _currentChar = EndOfTextChar;
            return false;
        }

        /// <inheritdoc />
        public bool ReadSymbol(IsValidSymbolCharacterDelegate isValidSymbolCharacter)
        {
            if (isValidSymbolCharacter == null)
                isValidSymbolCharacter = _isValidSymbolCharacter;

            var symbolStrBldr = new StringBuilder();
            int startPosition = _positionInText;

            while (_positionInText < _textEndPosition)
            {
                _currentChar = _textToParse[_positionInText];

                if (isValidSymbolCharacter(_currentChar, _positionInText - startPosition, this))
                {
                    symbolStrBldr.Append(_currentChar);
                    ++_positionInText;
                }
                else
                {
                    break;
                }
            }

            _lastReadSymbol = symbolStrBldr.ToString();

            if (_positionInText == _textEndPosition)
                _currentChar = EndOfTextChar;

            return _lastReadSymbol.Length > 0;
        }

        /// <inheritdoc />
        public string TextToParse => _textToParse;

        /// <inheritdoc />
        public bool ReadNextCharacter()
        {
            if (IsEndOfTextReached)
                return false;

            ++_positionInText;

            if (IsEndOfTextReached)
            {
                _currentChar = EndOfTextChar;
                return false;
            }

            _currentChar = _textToParse[_positionInText];
            return true;
        }

        /// <inheritdoc />
        public bool SkipCurrentCharacterAndSpaces()
        {
            ++_positionInText;

            if (this.IsEndOfTextReached)
            {
                _currentChar = EndOfTextChar;
                return false;
            }

            _currentChar = _textToParse[_positionInText];
            return SkipSpaces();
        }

        /// <inheritdoc />
        public bool MoveToToPosition(int positionInText)
        {
            _positionInText = positionInText;
            if (_positionInText < ParsedTextStartPosition)
            {
                _positionInText = ParsedTextStartPosition;
                _currentChar = _textToParse[_positionInText];
                return false;
            }

            if (_positionInText >= _textEndPosition)
            {
                _positionInText = _textEndPosition;
                _currentChar = EndOfTextChar;
                return false;
            }

            _currentChar = _textToParse[_positionInText];
            return true;
        }

        /// <inheritdoc />
        public bool SkipCharacters(int numberOfCharactersToSkip)
        {
            if (numberOfCharactersToSkip < 0)
                return false;

            _positionInText = PositionInText + numberOfCharactersToSkip;

            if (_positionInText >= _textEndPosition)
            {
                _currentChar = EndOfTextChar;

                if (_positionInText > _textEndPosition)
                {
                    _positionInText = _textEndPosition;
                    return false;
                }

                return true;
            }

            _currentChar = _textToParse[_positionInText];
            return true;
        }
    }
}