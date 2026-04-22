using System.Collections.Generic;

namespace TMPP_CRM.Domain.Flyweight
{
    public class CharacterFactory
    {
        private readonly Dictionary<char, ICharacter> _characters = new Dictionary<char, ICharacter>();

        public ICharacter GetCharacter(char key)
        {
            // Default font family for shared characters
            string defaultFont = "Arial";

            if (!_characters.ContainsKey(key))
            {
                _characters.Add(key, new Character(key, defaultFont));
            }
            return _characters[key];
        }

        public int GetTotalCharactersCreated()
        {
            return _characters.Count;
        }
    }
}
