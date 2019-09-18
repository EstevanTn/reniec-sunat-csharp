using System;

namespace Tunaqui.Peru.Service.Model
{

    /// <summary>
    /// Información básica de la persona
    /// </summary>
    [Serializable]
    public class IPerson
    {
        /// <summary>
        /// Documento nacional de identidad
        /// </summary>
        public virtual string Dni { get; set; }

        /// <summary>
        /// Nombres de la persona
        /// </summary>
        public virtual string Nombres { get; set; }

        /// <summary>
        /// Apellidos de la persona
        /// </summary>
        public virtual string Apellidos { get; set; }
    }
}
