namespace TMPP_CRM.Domain.Prototypes
{
    /// <summary>
    /// Interfata Prototype generica.
    /// Clasele concrete implementeaza clone superficiala si profunda.
    /// </summary>
    /// <typeparam name="T">Tipul obiectului care poate fi clonat.</typeparam>
    public interface IPrototype<T>
    {
        /// <summary>
        /// Copie superficiala (Shallow Copy): creeaza un nou obiect,
        /// dar referintele interne sunt partajate cu originalul.
        /// </summary>
        T ShallowCopy();

        /// <summary>
        /// Copie profunda (Deep Copy): creeaza un nou obiect complet independent,
        /// inclusiv toate obiectele referentiate.
        /// </summary>
        T DeepCopy();
    }
}
