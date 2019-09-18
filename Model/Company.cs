using System;
using System.Collections.Generic;

namespace Tunaqui.Peru.Service.Model
{
    /// <summary>
    /// Información de la empresa
    /// </summary>
    [Serializable]
    public class Company
    {
        /// <summary>
        /// Registro único del contribuyente
        /// </summary>
        public virtual string Ruc { get; set; }

        /// <summary>
        /// Razón social
        /// </summary>
        public virtual string RazonSocial { get; set; }

        /// <summary>
        /// Nombre comercial
        /// </summary>
        public virtual string NombreComercial { get; set; }

        /// <summary>
        /// Tipo del contribuyente
        /// </summary>
        public virtual string Tipo { get; set; }

        /// <summary>
        /// Estado del contribuyente
        /// </summary>
        public virtual string Estado { get; set; }
        
        /// <summary>
        /// Condición del contribuyente
        /// </summary>
        public virtual string Condicion { get; set; }

        /// <summary>
        /// Fecha de inscripción
        /// </summary>
        public virtual string FechaInscripcion { get; set; }

        /// <summary>
        /// Fecha inicio de actividades
        /// </summary>
        public virtual string FechaInicioActividades { get; set; }

        /// <summary>
        /// Dirección fiscal
        /// </summary>
        public virtual string Direccion { get; set; }

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
        /// Pofesión u oficio (RUC 10)
        /// </summary>
        public virtual string ProfesionOficio { get; set; }

        /// <summary>
        /// Sistema emisión de comprobantes
        /// </summary>
        public virtual string SisEmisionComprobante { get; set; }

        /// <summary>
        /// Sistema de contabilidad
        /// </summary>
        public virtual string SisContabilidad { get; set; }

        /// <summary>
        /// Actividades de comercio exterior
        /// </summary>
        public virtual string ActComercioExterior { get; set; }

        /// <summary>
        /// Actividades económicas
        /// </summary>
        public virtual List<string> ActividadesEconomicas { get; set; }

        /// <summary>
        /// Comprobantes de pago Aut. Impresión
        /// </summary>
        public virtual List<string> ComprobantesPagoAutImpresion { get; set; }

        /// <summary>
        /// Obligado a emitir comprobantes de pago electrónico
        /// </summary>
        public virtual string ObligadoEmitirCPE { get; set; }

        /// <summary>
        /// Sistema de emisión electrónica
        /// </summary>
        public virtual List<string> SisEmisionElectronica { get; set; }

        /// <summary>
        /// Emisor electrónico desde (Fecha)
        /// </summary>
        public virtual string EmisorElectronicoDesde { get; set; }

        /// <summary>
        /// Comprobantes electrónicos
        /// </summary>
        public virtual List<string> ComprobantesElectronicos { get; set; }

        /// <summary>
        /// Afiliado al Programa de Libros Electrónicos (PLE)
        /// </summary>
        public virtual string AfiliadoPLEDesde { get; set; }

        /// <summary>
        /// Padrones
        /// </summary>
        public virtual List<string> Padrones { get; set; }

        /// <summary>
        /// Inicializa valores
        /// </summary>
        public Company()
        {
            Direccion = "-";
            Departamento = "-";
            Distrito = "-";
            Provincia = "-";
            NombreComercial = "-";
            ObligadoEmitirCPE = "-";
            ActividadesEconomicas = new List<string>();
            ComprobantesPagoAutImpresion = new List<string>();
            SisEmisionElectronica = new List<string>();
            Padrones = new List<string>();
            ComprobantesElectronicos = new List<string>();
        }

    }
}
