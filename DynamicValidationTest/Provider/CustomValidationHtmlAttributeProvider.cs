using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;

namespace DynamicValidationTest.Provider
{
    public class CustomValidationHtmlAttributeProvider : DefaultValidationHtmlAttributeProvider
    {
        private readonly IModelMetadataProvider _metadataProvider;
        private readonly ClientValidatorCache _clientValidatorCache;
        private readonly IClientModelValidatorProvider _clientModelValidatorProvider;

        public CustomValidationHtmlAttributeProvider(IOptions<MvcViewOptions> optionsAccessor, IModelMetadataProvider metadataProvider, ClientValidatorCache clientValidatorCache)
            : base(optionsAccessor, metadataProvider, clientValidatorCache)
        {
            _metadataProvider = metadataProvider;
            _clientValidatorCache = clientValidatorCache;
            var clientValidatorProviders = optionsAccessor.Value.ClientModelValidatorProviders;
            _clientModelValidatorProvider = new CompositeClientModelValidatorProvider(clientValidatorProviders);
        }

        public override void AddValidationAttributes(ViewContext viewContext, ModelExplorer modelExplorer, IDictionary<string, string> attributes)
        {
            if (viewContext == null)
            {
                throw new ArgumentNullException(nameof(viewContext));
            }

            if (modelExplorer == null)
            {
                throw new ArgumentNullException(nameof(modelExplorer));
            }

            if (attributes == null)
            {
                throw new ArgumentNullException(nameof(attributes));
            }

            var formContext = viewContext.ClientValidationEnabled ? viewContext.FormContext : null;
            if (formContext == null)
            {
                return;
            }

            var validators = ((CustomClientValidatorCache)_clientValidatorCache).GetValidatorsWithoutCache(
                modelExplorer.Metadata,
                _clientModelValidatorProvider);
            if (validators.Count > 0)
            {
                var validationContext = new ClientModelValidationContext(
                    viewContext,
                    modelExplorer.Metadata,
                    _metadataProvider,
                    attributes);

                for (var i = 0; i < validators.Count; i++)
                {
                    var validator = validators[i];
                    validator.AddValidation(validationContext);
                }
            }
        }
    }
}
