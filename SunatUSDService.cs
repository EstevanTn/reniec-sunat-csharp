using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tunaqui.Peru.Service.Interface;
using Tunaqui.Peru.Service.Model;

namespace Tunaqui.Peru.Service
{
    /// <summary>
    /// Sunat USD
    /// </summary>
    public class SunatUSDService : IScrappingService
    {
        /// <summary>
        /// URL service
        /// </summary>
        public const string URL = "http://www.sunat.gob.pe/cl-at-ittipcam/tcS01Alias?mes={0:D2}&anho={1:D4}";

        private readonly CookieContainer _cookies = new CookieContainer();
        private int _month, _year;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IList<Currency>> QueryAsync()
        {
            var today = DateTime.Now;
            return await QueryAsync(today.Month, today.Year);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public async Task<IList<Currency>> QueryAsync(int month, int year)
        {
            _month = month;
            _year = year;
            var response = await getRawResponseAsync(URL, month, year);
            return SearchNodes(response);
        }

        private List<Currency> SearchNodes(string html)
        {
            var document = new HtmlDocument();
            document.LoadHtml(html.Trim());

            var nodesTr = document.DocumentNode.SelectNodes("//table[@class='class=\"form-table\"']//tr");
            var list = new List<Currency>();
            if(nodesTr != null)
            {
                int rowIndex = 0;
                foreach (HtmlNode node in nodesTr)
                {
                    int colIndex = 0;
                    Currency currency = null;
                    if (rowIndex > 0)
                    {
                        foreach (var td in node.Elements("td"))
                        {
                            string innerHtml = td.InnerHtml.ToString().Trim();
                            innerHtml = Regex.Replace(innerHtml, "<.*?>", " ");
                            switch (colIndex)
                            {
                                case 0:
                                    currency = new Currency();
                                    currency.Day = Convert.ToDateTime(string.Format("{0:D2}/{1:D2}/{2}", Convert.ToInt32(innerHtml), Convert.ToInt32(_month), _year));
                                    colIndex++;
                                    break;
                                case 1:
                                    currency.PurchasePrice = Convert.ToDecimal(innerHtml);
                                    colIndex++;
                                    break;
                                case 2:
                                    currency.SalePrice = Convert.ToDecimal(innerHtml);
                                    list.Add(currency);
                                    colIndex = 0;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    rowIndex++;
                }
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<string> getRawResponseAsync(string url, params object[] parameters)
        {            
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format(url, parameters));
            //request.Credentials = CredentialCache.DefaultNetworkCredentials;
            request.ProtocolVersion = HttpVersion.Version11;
            request.UserAgent = ".NET Framework 4.0";
            request.CookieContainer = _cookies;
            request.Method = "GET";

            HttpWebResponse myhttpWebResponse = (HttpWebResponse)request.GetResponse();
            Stream myStream = myhttpWebResponse.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myStream, Encoding.GetEncoding("ISO-8859-1"));
            var str = await myStreamReader.ReadToEndAsync();
            myhttpWebResponse.Close();
            myStream.Close();
            myStreamReader.Close();

            return str;
        }
    }
}
