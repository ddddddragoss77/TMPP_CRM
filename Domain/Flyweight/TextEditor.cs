using System.Collections.Generic;
using System.Text;

namespace TMPP_CRM.Domain.Flyweight
{
    public class TextEditor
    {
        private readonly CharacterFactory _factory;
        private readonly List<(ICharacter Character, int FontSize, string Color)> _textLines = new List<(ICharacter, int, string)>();

        public TextEditor(CharacterFactory factory)
        {
            _factory = factory;
        }

        public void InsertText(string text, int fontSize, string color)
        {
            foreach (char c in text)
            {
                var character = _factory.GetCharacter(c);
                _textLines.Add((character, fontSize, color));
            }
        }

        public void Render()
        {
            foreach (var item in _textLines)
            {
                item.Character.Display(item.FontSize, item.Color);
            }
        }

        public int GetTotalCharactersInDocument()
        {
            return _textLines.Count;
        }
    }
}
