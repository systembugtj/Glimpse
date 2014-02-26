﻿using System;
using System.Linq;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Framework;

namespace Glimpse.Core.Policy
{
    /// <summary>
    /// Policy which will set Glimpse's runtime policy to <c>Off</c> if a Http response's content type is not supported.
    /// </summary>
    public class ContentTypePolicy : IRuntimePolicy, IConfigurableExtended
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentTypePolicy" />
        /// </summary>
        public ContentTypePolicy()
        {
            Configurator = new ContentTypePolicyConfigurator();
        }

        /// <summary>
        /// Gets the <see cref="ContentTypePolicyConfigurator" /> used by the <see cref="ContentTypePolicy" />
        /// </summary>
        public ContentTypePolicyConfigurator Configurator { get; private set; }

        /// <summary>
        /// Gets the configurator
        /// </summary>
        IConfigurator IConfigurableExtended.Configurator { get { return Configurator; } }

        /// <summary>
        /// Gets the point in an Http request lifecycle that a policy should execute.
        /// </summary>
        /// <value>
        /// The moment to execute, <see cref="AjaxPolicy"/> uses <c>EndRequest</c>.
        /// </value>
        public RuntimeEvent ExecuteOn
        {
            get { return RuntimeEvent.EndRequest; }
        }

        /// <summary>
        /// Executes the specified policy with the given context.
        /// </summary>
        /// <param name="policyContext">The policy context.</param>
        /// <returns>
        /// <c>On</c> if the response content type is contained on the white list, otherwise <c>Off</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Exception thrown if <paramref name="policyContext"/> is <c>null</c>.</exception>
        public RuntimePolicy Execute(IRuntimePolicyContext policyContext)
        {
            try
            {
                var contentType = policyContext.RequestMetadata.ResponseContentType.ToLowerInvariant();
                return Configurator.SupportedContentTypes.Any(ct => contentType.Contains(ct.ToLowerInvariant())) ? RuntimePolicy.On : RuntimePolicy.Off;
            }
            catch (Exception exception)
            {
                policyContext.Logger.Warn(Resources.ExecutePolicyWarning, exception, GetType());
                return RuntimePolicy.Off;
            }
        }
    }
}