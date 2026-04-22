using System;
using System.Collections.Generic;
using System.Linq;

namespace TMPP_CRM.Domain.Composite
{
    /// <summary>
    /// Nod compus (Composite) din ierarhia meniului.
    /// Poate contine atat MenuItem (frunze) cat si alte MenuGroup (submeniuri).
    /// GetPrice() insumeaza recursiv preturile tuturor copiilor.
    /// </summary>
    public class MenuGroup : IMenuComponent
    {
        private readonly List<IMenuComponent> _children = new();
        public string Name { get; }
        public string Description { get; }

        public MenuGroup(string name, string description = "")
        {
            Name = name;
            Description = description;
        }

        public string GetName() => Name;
        public string GetDescription() => Description;
        public bool IsComposite() => true;

        /// <summary>
        /// Calculeaza pretul total al grupului, sumand recursiv toti copiii.
        /// </summary>
        public decimal GetPrice() => _children.Sum(c => c.GetPrice());

        public void Add(IMenuComponent component) => _children.Add(component);

        public void Remove(IMenuComponent component) => _children.Remove(component);

        public IReadOnlyList<IMenuComponent> GetChildren() => _children.AsReadOnly();

        /// <summary>
        /// Afiseaza recursiv ierarhia completa a meniului.
        /// </summary>
        public void Display(int indent = 0)
        {
            string pad = new string(' ', indent * 2);
            Console.WriteLine($"{pad}📋 {Name} ({_children.Count} componente) - Total: {GetPrice():F2} RON");
            if (!string.IsNullOrEmpty(Description))
                Console.WriteLine($"{pad}   {Description}");

            foreach (var child in _children)
                child.Display(indent + 1);
        }
    }
}
