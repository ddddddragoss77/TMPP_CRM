namespace TMPP_CRM.Domain.Builders
{
    /// <summary>
    /// Director-ul gestioneaza procesul de constructie a ofertelor.
    /// Define oferte predefinite (Standard, Premium) folosind un IOfferBuilder.
    /// Separa logica de constructie de reprezentarea finala.
    /// </summary>
    public class OfferDirector
    {
        private readonly IOfferBuilder _builder;

        public OfferDirector(IOfferBuilder builder)
        {
            _builder = builder;
        }

        /// <summary>
        /// Construieste o oferta Standard: discount mic, valabilitate 30 zile.
        /// </summary>
        public Offer BuildStandardOffer(string clientName)
        {
            return _builder
                .SetTitle("Oferta Standard CRM")
                .SetClient(clientName)
                .AddProduct("Modul Gestionare Clienti")
                .AddProduct("Modul Rapoarte de baza")
                .SetDiscount(5)
                .SetValidity(30)
                .SetNotes("Oferta standard valabila 30 zile. Suport inclus.")
                .Build();
        }

        /// <summary>
        /// Construieste o oferta Premium: discount mare, valabilitate 90 zile, mai multe module.
        /// </summary>
        public Offer BuildPremiumOffer(string clientName)
        {
            return _builder
                .SetTitle("Oferta Premium CRM")
                .SetClient(clientName)
                .AddProduct("Modul Gestionare Clienti")
                .AddProduct("Modul Gestionare Leads")
                .AddProduct("Modul Deals & Pipeline")
                .AddProduct("Modul Rapoarte Avansate")
                .AddProduct("Modul Integrari API")
                .SetDiscount(20)
                .SetValidity(90)
                .SetNotes("Oferta Premium cu toate modulele incluse, suport prioritar 24/7.")
                .Build();
        }

        /// <summary>
        /// Construieste o oferta personalizata cu titlu si produse specificate.
        /// </summary>
        public Offer BuildCustomOffer(string title, string clientName, string[] products, 
                                      decimal discount, int validityDays)
        {
            _builder.SetTitle(title).SetClient(clientName);
            foreach (var product in products)
                _builder.AddProduct(product);
            return _builder
                .SetDiscount(discount)
                .SetValidity(validityDays)
                .Build();
        }
    }
}
