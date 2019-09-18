using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tunaqui.Peru.Service.Interface
{
    /// <summary>
    /// Simple query service
    /// </summary>
    public interface ISimpleQuery<T> : IService
    {
        /// <summary>
        /// Query async
        /// </summary>
        /// <param name="q">Dato de entrada</param>
        /// <returns></returns>
        Task<T> QueryAsync(string q);
    }
}
