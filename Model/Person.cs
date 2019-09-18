using System;

namespace Tunaqui.Peru.Service.Model
{
    /// <summary>
    /// Información de la persona - RENIEC
    /// </summary>
    [Serializable]
    public class Person : IPerson
    {
        /// <summary>
        /// Grupo de votación
        /// </summary>
        public virtual string gVotacion { get; set; }

        /// <summary>
        /// Distrito
        /// </summary>
        public virtual string Distrito { get; set; }

        /// <summary>
        /// Provincia
        /// </summary>
        public virtual string Provincia { get; set; }

        /// <summary>
        /// Departamento
        /// </summary>
        public virtual string Departamento { get; set; }

        /// <summary>
        /// Inicializa valores vacios
        /// </summary>
        public Person()
        {
            gVotacion = string.Empty;
            Distrito = string.Empty;
            Provincia = string.Empty;
            Departamento = string.Empty;
        }
    }
}
