using System;
using System.Collections.Generic;

namespace TMPP_CRM.Domain.Builders
{
    /// <summary>
    /// Implementare concreta a builder-ului pentru oferta comerciala.
    /// Foloseste fluent API - fiecare metoda returneaza 'this' pentru inlantuire.
    /// </summary>
    public class OfferBuilder : IOfferBuilder
    {
        private Offer _offer;

        public OfferBuilder()
        {
            _offer = new Offer();
        }

        public IOfferBuilder SetTitle(string title)
        {
            _offer.Title = title;
            return this;
        }

        public IOfferBuilder SetClient(string clientName)
        {
            _offer.ClientName = clientName;
            return this;
        }

        public IOfferBuilder AddProduct(string product)
        {
            _offer.Products.Add(product);
            return this;
        }

        public IOfferBuilder SetDiscount(decimal discountPercent)
        {
            if (discountPercent < 0 || discountPercent > 100)
                throw new ArgumentOutOfRangeException(nameof(discountPercent), 
                    "Discountul trebuie sa fie intre 0 si 100.");
            _offer.Discount = discountPercent;
            return this;
        }

        public IOfferBuilder SetValidity(int days)
        {
            if (days <= 0)
                throw new ArgumentOutOfRangeException(nameof(days), 
                    "Valabilitatea trebuie sa fie pozitiva.");
            _offer.ValidityDays = days;
            return this;
        }

        public IOfferBuilder SetNotes(string notes)
        {
            _offer.Notes = notes;
            return this;
        }

        /// <summary>
        /// Construieste si returneaza oferta finala.
        /// Dupa Build(), builder-ul se reseteaza automat.
        /// </summary>
        public Offer Build()
        {
            var result = _offer;
            Reset();
            return result;
        }

        /// <summary>
        /// Reseteaza builder-ul la starea initiala pentru o noua constructie.
        /// </summary>
        public void Reset()
        {
            _offer = new Offer();
        }
    }
}
