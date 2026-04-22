using System;

namespace TMPP_CRM.Domain.Flyweight
{
    public class Character : ICharacter
    {
        private readonly char _symbol;
        private readonly string _fontFamily;

        // The intrinsic state is shared
        public Character(char symbol, string fontFamily)
        {
            _symbol = symbol;
            _fontFamily = fontFamily;
        }

        // The extrinsic state is passed in
        public void Display(int fontSize, string color)
        {
            Console.WriteLine($"Character: {_symbol}, Font: {_fontFamily}, Size: {fontSize}, Color: {color}");
        }

        public char Symbol => _symbol;
    }
}
