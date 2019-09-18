using System;

namespace Tunaqui.Peru.Service.Model
{
    /// <summary>
    /// Clase de divisas
    /// </summary>
    [Serializable]
    public class Currency
    {
        /// <summary>
        /// Fecha
        /// </summary>
        public DateTime Day { get; set; }

        /// <summary>
        /// Precio de compra
        /// </summary>
        public decimal PurchasePrice { get; set; }

        /// <summary>
        /// Precio de venta
        /// </summary>
        public decimal SalePrice { get; set; }
    }
}
