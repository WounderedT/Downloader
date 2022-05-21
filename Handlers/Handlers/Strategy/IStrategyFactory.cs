using Handlers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Handlers.Strategy
{
    internal interface IStrategyFactory
    {
        /// <summary>
        /// Gets <see cref="StrategyEnum"/> for provided <paramref name="headers"/>.
        /// </summary>
        /// <param name="domain">Domain to get download stategy for.</param>
        /// <returns>
        /// A new <see cref="StrategyEnum"/> object based on <see cref="DownloadHandlerConfig"/> configuration parameters, or <see cref="StrategyEnum.Default"/>, if
        /// no configuration rules are applicable to <paramref name="domain"/>, or no configuration rules were configured.
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        StrategyEnum GetStrategy(String domain);

        /// <summary>
        /// Gets <see cref="StrategyEnum"/> for provided <paramref name="headers"/>.
        /// </summary>
        /// <param name="headers"><see cref="HttpContentHeaders"/> headers to get download stategy for.</param>
        /// <returns>
        /// A new <see cref="StrategyEnum"/> object based on <see cref="DownloadHandlerConfig"/> configuration parameters, or <see cref="StrategyEnum.Default"/>, if
        /// no configuration rules are applicable to <paramref name="headers"/>, or no configuration rules were configured.
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        StrategyEnum GetStrategy(HttpContentHeaders headers);
    }
}
