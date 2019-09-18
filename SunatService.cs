using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using tessnet2;
using Tunaqui.Peru.Service.Interface;
using Tunaqui.Peru.Service.Model;

namespace Tunaqui.Peru.Service
{
    /// <summary>
    /// Consulta RUC - SUNAT
    /// </summary>
    public class SunatService : IScrappingService, ISimpleQuery<Company>
    {
        /// <summary>
        /// URL de captcha
        /// </summary>
        public const string SUNAT_CAPTCHA = "http://www.sunat.gob.pe/cl-ti-itmrconsruc/captcha?accion=image";

        /// <summary>
        /// URL consulta RUC - SUNAT
        /// </summary>
        public const string SUNAT_URL = "http://www.sunat.gob.pe/cl-ti-itmrconsruc/jcrS00Alias?accion=consPorRuc&nroRuc={0}&codigo={1}&tipdoc=1";

        private readonly CookieContainer _cookies = new CookieContainer();

        private string _captcha = string.Empty;

        /// <summary>
        /// Ejecuta la consulta de manera asyncrona
        /// </summary>
        /// <param name="ruc">RUC de empresa</param>
        /// <returns></returns>
        public async Task<Company> QueryAsync(string ruc)
        {
            if (ruc.Length != 11)
                throw new ArgumentException("RUC debe tener 11 dígitos");

            generateCaptcha();

            var url = string.Format(SUNAT_URL, ruc, _captcha);

            var html = await getRawResponseAsync(url);

            var document = new HtmlDocument();
            document.LoadHtml(html.Trim());

            var title = document.DocumentNode.SelectNodes("//html[1]/head[1]/title[1]").FirstOrDefault();

            if (!title.InnerText.Equals("Consulta RUC"))
                throw new InvalidOperationException("Vuelva a intentar, no pudimos conectar con SUNAT.");

            var table = document.DocumentNode.SelectNodes("//html[1]/body[1]/table[1]").FirstOrDefault();

            return mappingTable(table);
        }
        
        /// <summary>
        /// Obtiene el contenido HTML de la petición
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<string> getRawResponseAsync(string url, params object[] parameters)
        {
            HttpWebRequest myWebRequest = (HttpWebRequest)WebRequest.Create(url);
            myWebRequest.CookieContainer = _cookies;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;

            HttpWebResponse myhttpWebResponse = (HttpWebResponse)myWebRequest.GetResponse();
            Stream myStream = myhttpWebResponse.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myStream, Encoding.GetEncoding("ISO-8859-1"));

            return await myStreamReader.ReadToEndAsync();
        }

        /// <summary>
        /// Obtiene la imagen Captcha e identifica su valor atravez del OCR
        /// </summary>
        protected void generateCaptcha()
        {
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(SUNAT_CAPTCHA);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            httpRequest.CookieContainer = _cookies;
            Bitmap image = new Bitmap(((HttpWebResponse)httpRequest.GetResponse()).GetResponseStream());
            Tesseract tesseract = new Tesseract();
            string path = Path.Combine(Environment.CurrentDirectory, "Content/tessdata");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            tesseract.Init(path, "eng", false);
            foreach (Word word in tesseract.DoOCR(image, Rectangle.Empty))
            {
                _captcha = word.Text;
            }
        }

        private Company mappingTable(HtmlNode table)
        {
            if (table == null)
                throw new InvalidOperationException("Vuelva a intentar, no pudimos conectar con SUNAT.");

            IEnumerable<HtmlNode> arg_1F_0 = table.SelectNodes("tr");
            Company company = new Company();
            using (IEnumerator<HtmlNode> enumerator = arg_1F_0.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    HtmlNodeCollection cells = enumerator.Current.SelectNodes("td");
                    if (cells[0].InnerHtml.Contains("Estado del"))
                    {
                        company.Estado = beautifulString(cells[1].InnerText);
                    }
                    if (cells.Count == 2)
                    {
                        if (cells[0].InnerHtml.Contains("RUC:"))
                        {
                            string[] xstr = cells[1].InnerText.Split('-');
                            company.Ruc = beautifulString(xstr[0]);
                            company.RazonSocial = beautifulString(xstr[1].TrimStart());
                        }
                        if (cells[0].InnerHtml.Contains("Tipo Contribuyente:"))
                        {
                            company.Tipo = cells[1].InnerText;
                        }
                        if (cells[0].InnerHtml.Contains("Nombre Comercial:"))
                        {
                            company.NombreComercial = beautifulString(cells[1].InnerHtml);
                        }
                        if (cells[0].InnerHtml.Contains("Direcci&oacute;n del Domicilio Fiscal"))
                        {
                            var parts = cells[1].InnerText.Split('-');
                            var regex = new Regex("  ", RegexOptions.RightToLeft);
                            if (parts.Length == 3)
                            {
                                var indice0 = regex.Match(beautifulString(parts[0])).Index;
                                company.Direccion = beautifulString(parts[0].Substring(0, indice0));
                                company.Departamento = beautifulString(parts[0].Substring(indice0));
                            }
                            if (parts.Length > 3)
                            {
                                var direccion = string.Empty;
                                for (int i = 0; i < parts.Length -2; i++)
                                    direccion += parts[i];

                                var indice1 = regex.Match(beautifulString(direccion)).Index;
                                if(indice1 != 0)
                                {
                                    company.Direccion = beautifulString(direccion.Substring(0, indice1));
                                    company.Departamento = beautifulString(direccion.Substring(indice1));
                                } else
                                {
                                    company.Direccion = beautifulString(direccion);
                                    company.Departamento = "-";
                                }
                            }
                            company.Provincia = beautifulString(parts[parts.Length - 2]);
                            company.Distrito = beautifulString(parts[parts.Length - 1]);
                            
                        }
                        if (cells[0].InnerHtml.Contains("Sistema de Contabilidad:"))
                        {
                            company.SisContabilidad = beautifulString(cells[1].InnerText);
                        }
                        if (cells[0].InnerHtml.Contains("Emisor electr&oacute;nico desde:"))
                        {
                            company.EmisorElectronicoDesde = beautifulString(cells[1].InnerText);
                        }
                        if (cells[0].InnerHtml.Contains("Comprobantes Electr&oacute;nicos"))
                        {
                            var list = cells[1].InnerText.Split(',');
                            company.ComprobantesElectronicos = list.ToList();
                        }
                        if (cells[0].InnerHtml.Contains("Afiliado al PLE desde:"))
                        {
                            company.AfiliadoPLEDesde = beautifulString(cells[1].InnerText);
                        }
                        if(cells[0].InnerHtml.Contains("Actividad(es) Econ&oacute;mica(s):"))
                        {
                            HtmlNodeCollection activitiesNodes = cells[1].SelectNodes("select/option");
                            if(activitiesNodes != null)
                            {
                                foreach (var item in ((IEnumerable<HtmlNode>)activitiesNodes))
                                {
                                    company.ActividadesEconomicas.Add(beautifulString(item.InnerText));
                                }
                            }
                        }
                        if (cells[0].InnerHtml.Contains("Sistema de Emision Electronica:"))
                        {
                            HtmlNodeCollection options = cells[1].SelectNodes("select/option");
                            if (options != null)
                            {
                                foreach (HtmlNode opt in ((IEnumerable<HtmlNode>)options))
                                {
                                    company.SisEmisionElectronica.Add(beautifulString(opt.InnerText));
                                }
                            }
                        }
                        if (cells[0].InnerHtml.Contains("Padrones"))
                        {
                            HtmlNodeCollection options2 = cells[1].SelectNodes("select/option");
                            if (options2 != null)
                            {
                                foreach (HtmlNode opt2 in ((IEnumerable<HtmlNode>)options2))
                                {
                                    company.Padrones.Add(beautifulString(opt2.InnerText));
                                }
                            }
                        }
                        if (cells[0].InnerHtml.Contains("Condici&oacute;n del Contribuyente:"))
                        {
                            company.Condicion = beautifulString(cells[1].InnerText);
                        }
                        if(cells[0].InnerHtml.Contains("Comprobantes de Pago c/aut."))
                        {
                            HtmlNodeCollection opt3 = cells[1].SelectNodes("select/option");
                            if (opt3 != null)
                            {
                                foreach (HtmlNode itm in ((IEnumerable<HtmlNode>)opt3))
                                {
                                    company.ComprobantesPagoAutImpresion.Add(beautifulString(itm.InnerText));
                                }
                            }
                        }
                    }
                    if (cells.Count == 3 && cells[0].InnerHtml.Contains("Comprobantes de Pago c/aut."))
                    {
                        HtmlNodeCollection options3 = cells[1].SelectNodes("select/option");
                        if (options3 != null)
                        {
                            foreach (HtmlNode opt3 in ((IEnumerable<HtmlNode>)options3))
                            {
                                company.ComprobantesPagoAutImpresion.Add(beautifulString(opt3.InnerText));
                            }
                        }
                        company.ObligadoEmitirCPE = beautifulString(cells[2].InnerText.Substring(22));
                    }
                    if (cells.Count == 4)
                    {
                        if (cells[0].InnerHtml.Contains("Fecha de Inscripci&oacute;n:"))
                        {
                            company.FechaInscripcion = beautifulString(cells[1].InnerText);
                            company.FechaInicioActividades = beautifulString(cells[3].InnerText);
                        }
                        if (cells[0].InnerHtml.Contains("Condici&oacute;n del Contribuyente:"))
                        {
                            company.Condicion = beautifulString(cells[1].InnerText);
                            company.ProfesionOficio = beautifulString(cells[3].InnerText);
                        }
                        if (cells[0].InnerHtml.Contains("Sistema de Emisi&oacute;n de Comprobante:"))
                        {
                            company.SisEmisionComprobante = beautifulString(cells[1].InnerText);
                            company.ActComercioExterior = beautifulString(cells[3].InnerText);
                        }
                    }
                }
            }
            return company;
        }

        private string beautifulString(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var patterns = new string[] { "\t", "\n", "\r" };
            var replacements = new string[] { "", "", "" };

            for (var i = 0; i < patterns.Length; i++)
                text = Regex.Replace(text, patterns[i], replacements[i]);

            text = text
                .TrimStart()
                .TrimEnd();

            return text;
        }

        /// <summary>
        /// Codifica el texto a UTF8
        /// </summary>
        /// <param name="message">Mensaje</param>
        /// <param name="originalEncoding">Encoding</param>
        /// <returns>string</returns>
        public string ConvertToUTF8(string message, string originalEncoding = "ISO-8859-1")
        {
            Encoding iso = Encoding.GetEncoding(originalEncoding);
            Encoding utf8 = Encoding.UTF8;
            byte[] utfBytes = utf8.GetBytes(message);
            byte[] isoBytes = Encoding.Convert(utf8, iso, utfBytes);
            return iso.GetString(isoBytes);
        }
    }
}
