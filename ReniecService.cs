using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Tunaqui.Peru.Service.Interface;
using Tunaqui.Peru.Service.Model;

namespace Tunaqui.Peru.Service
{
    /// <summary>
    /// Consulta de DNI - RENIEC
    /// </summary>
    public class ReniecService : IScrappingService, ISimpleQuery<Person>
    {
        /// <summary>
        /// URL de la página que creo la petición
        /// </summary>
        public const string URL_REFERER = "http://clientes.reniec.gob.pe/padronElectoral2012/padronPEMDistrito.htm";
        
        /// <summary>
        /// URL consulta - RENIEC
        /// </summary>
        public const string URL_CONSULT = "http://clientes.reniec.gob.pe/padronElectoral2012/consulta.htm";


        private readonly CookieContainer _cookies = new CookieContainer();
        private string _captcha = string.Empty;

        /// <summary>
        /// Obtiene el contenido HTML de la consulta (POST)
        /// </summary>
        /// <param name="url">URL de la petición</param>
        /// <param name="parameters">Parametros POST</param>
        /// <returns>HTML</returns>
        public async Task<string> getRawResponseAsync(string url, params object[] parameters)
        {
            var dni = Convert.ToString(parameters[0]);
            var param = $"hTipo=2&hDni={dni}&hApPat=&hApMat=&hNombre=";
            var data = Encoding.ASCII.GetBytes(param);
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";
            httpRequest.Referer = URL_REFERER;
            httpRequest.CookieContainer = _cookies;
            httpRequest.ContentType = "application/x-www-form-urlencoded";
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            using (var stream = httpRequest.GetRequestStream())
                stream.Write(data, 0, data.Length);

            HttpWebResponse myhttpWebResponse = (HttpWebResponse)httpRequest.GetResponse();
            Stream myStream = myhttpWebResponse.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myStream);

            return await myStreamReader.ReadToEndAsync();
        }

        /// <summary>
        /// Ejecuta la consulta de manera asyncrona
        /// </summary>
        /// <param name="dni">DNI</param>
        /// <returns>Datos de la persona <see cref="Person"/>.</returns>
        public async Task<Person> QueryAsync(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni))
                throw new ArgumentNullException("El parámetro DNI debe tener un valor");
            if (dni.Length != 8)
                throw new ArgumentException("DNI debe tener 8 dígitos");

            var html = await getRawResponseAsync(URL_CONSULT, dni);

            var xml = new HtmlDocument();
            xml.LoadHtml(html);
            var tables = xml.DocumentNode.SelectNodes("//table");
            if (tables.Count == 7)
            {
                var person = new Person();
                person.Dni = dni.Trim();
                var td = tables[4].SelectNodes("tr/td");
                var nCompleto = td[1].InnerHtml.Split(',');
                person.Nombres = nCompleto[1].TrimStart().TrimEnd();
                person.Apellidos = nCompleto[0].TrimStart().TrimEnd();
                person.gVotacion = td[5].InnerHtml.TrimStart().TrimEnd();
                person.Distrito = td[7].InnerHtml.TrimStart().TrimEnd();
                person.Provincia = td[9].InnerHtml.TrimStart().TrimEnd();
                person.Departamento = td[11].InnerHtml.TrimStart().TrimEnd();
                return person;
            }
            throw new InvalidOperationException("Vuelva a intentar, no pudimos conectar con RENIEC.");
        }
    }
}
