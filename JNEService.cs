using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Tunaqui.Peru.Service.Interface;
using Tunaqui.Peru.Service.Model;

namespace Tunaqui.Peru.Service
{
    /// <summary>
    /// Consulta de DNI - Jurado Nacional de Elecciones
    /// </summary>
    public class JNEService : IScrappingService, ISimpleQuery<IPerson>
    {
        /// <summary>
        /// URL JNE
        /// </summary>
        public const string JNE_URL = "http://aplicaciones007.jne.gob.pe/srop_publico/Consulta/Afiliado/GetNombresCiudadano?DNI={0}";
        
        /// <summary>
        /// Ejecuta la consulta de manera asyncrona
        /// </summary>
        /// <param name="dni">DNI</param>
        /// <returns></returns>
        public async Task<IPerson> QueryAsync(string dni)
        {
            if (dni.Length != 8)
                throw new ArgumentException(message: "DNI debe tener 8 dígitos");
            var url = string.Format(JNE_URL, dni);
            var response = await getRawResponseAsync(url);
            var person = getPerson(response, dni);
            return person;
        }

        /// <summary>
        /// Obtiene el contenido de la petición
        /// </summary>
        /// <param name="url">URL de la petición</param>
        /// <param name="parameters">Parámetros de la petición</param>
        /// <returns></returns>
        public async Task<string> getRawResponseAsync(string url, params object[] parameters)
        {
            HttpWebRequest myWebRequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse myhttpWebResponse = (HttpWebResponse)myWebRequest.GetResponse();
            Stream myStream = myhttpWebResponse.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myStream);

            var response = await myStreamReader.ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(response))
                throw new ArgumentException(message: "No se pudo conectar a JNE");
            return response;
        }

        private IPerson getPerson(string response, string dni)
        {
            var data = response.Split('|');
            if (data.Length == 3)
            {
                var person = new IPerson();
                person.Dni = dni;
                person.Apellidos = $"{data[0]} {data[1]}";
                person.Nombres = data[2];
                return person;
            }
            throw new InvalidOperationException(message: "DNI no encontrado en el padrón electoral");
        }
    }
}
