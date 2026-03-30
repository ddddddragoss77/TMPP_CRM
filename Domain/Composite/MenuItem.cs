using System;
using System.Collections.Generic;

namespace TMPP_CRM.Domain.Composite
{
    /// <summary>
    /// Frunza (Leaf) din ierarhia Composite.
    /// Reprezinta un produs individual din meniu (mancare, bautura etc).
    /// Nu poate contine alti copii – Add/Remove arunca NotSupportedException.
    /// </summary>
    public class MenuItem : IMenuComponent
    {
        public string Name { get; }
        public string Description { get; }
        public decimal Price { get; }
        public string Category { get; }

        public MenuItem(string name, decimal price, string description = "", string category = "General")
        {
            Name = name;
            Price = price;
            Description = description;
            Category = category;
        }

        public string GetName() => Name;
        public string GetDescription() => Description;
        public decimal GetPrice() => Price;
        public bool IsComposite() => false;

        public void Display(int indent = 0)
        {
            string pad = new string(' ', indent * 2);
            Console.WriteLine($"{pad}🍽 {Name} .......... {Price:F2} RON");
            if (!string.IsNullOrEmpty(Description))
                Console.WriteLine($"{pad}   ({Description})");
        }

        // Frunzele nu suporta copii
        public void Add(IMenuComponent component)
            => throw new NotSupportedException("MenuItem nu poate contine sub-componente.");

        public void Remove(IMenuComponent component)
            => throw new NotSupportedException("MenuItem nu poate contine sub-componente.");

        public IReadOnlyList<IMenuComponent> GetChildren()
            => Array.Empty<IMenuComponent>();
    }
}
