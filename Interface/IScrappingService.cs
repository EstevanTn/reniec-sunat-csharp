using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tunaqui.Peru.Service.Interface
{
    /// <summary>
    /// Interface Web Scraping
    /// </summary>
    public interface IScrappingService : IService
    {
        /// <summary>
        /// Obtiene el contenido de la petición
        /// </summary>
        /// <param name="url">Url de la petición</param>
        /// <param name="parameters">Parámetros POST or GET</param>
        /// <returns></returns>
        Task<string> getRawResponseAsync(string url, params object[] parameters);
    }
}
