using System.Collections.Generic;

namespace TMPP_CRM.Domain.Composite
{
    /// <summary>
    /// Interfata comuna pentru toate componentele din ierarhia meniului.
    /// Composite Pattern: "Component" interface – tratata uniform de client.
    /// </summary>
    public interface IMenuComponent
    {
        string GetName();
        string GetDescription();
        decimal GetPrice();

        /// <summary>Afiseaza componenta cu indentare pentru vizualizare ierarhie.</summary>
        void Display(int indent = 0);

        bool IsComposite();

        // Operatii de management al copiilor (relevante doar pentru MenuGroup)
        void Add(IMenuComponent component);
        void Remove(IMenuComponent component);
        IReadOnlyList<IMenuComponent> GetChildren();
    }
}
