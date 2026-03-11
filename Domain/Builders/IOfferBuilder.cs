namespace TMPP_CRM.Domain.Builders
{
    /// <summary>
    /// Interfata Builder pentru constructia pas cu pas a unei oferte comerciale.
    /// Permite personalizarea prin metode fluente (fluent API).
    /// </summary>
    public interface IOfferBuilder
    {
        IOfferBuilder SetTitle(string title);
        IOfferBuilder SetClient(string clientName);
        IOfferBuilder AddProduct(string product);
        IOfferBuilder SetDiscount(decimal discountPercent);
        IOfferBuilder SetValidity(int days);
        IOfferBuilder SetNotes(string notes);
        Offer Build();
        void Reset();
    }
}
