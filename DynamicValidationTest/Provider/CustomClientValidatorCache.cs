using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DynamicValidationTest.Model;

namespace DynamicValidationTest.Provider
{
    public class CustomClientValidatorCache : ClientValidatorCache
    {

        public IReadOnlyList<IClientModelValidator> GetValidatorsWithoutCache(ModelMetadata metadata, IClientModelValidatorProvider validatorProvider)
        {

            var items = new List<ClientValidatorItem>(metadata.ValidatorMetadata.Count);
            for (var i = 0; i < metadata.ValidatorMetadata.Count; i++)
            {
                items.Add(new ClientValidatorItem(metadata.ValidatorMetadata[i]));
            }

            ExecuteProvider(validatorProvider, metadata, items);

            var validators = ExtractValidators(items);

            foreach (var item in items)
            {
                if (!item.IsReusable)
                {
                    item.Validator = null;
                }
            }

            return validators;
        }


        private void ExecuteProvider(IClientModelValidatorProvider validatorProvider, ModelMetadata metadata, List<ClientValidatorItem> items)
        {
            var context = new ClientValidatorProviderContext(metadata, items);

            validatorProvider.CreateValidators(context);
        }

        private IReadOnlyList<IClientModelValidator> ExtractValidators(List<ClientValidatorItem> items)
        {
            var count = 0;
            for (var i = 0; i < items.Count; i++)
            {
                if (items[i].Validator != null)
                {
                    count++;
                }
            }

            if (count == 0)
            {
                return Array.Empty<IClientModelValidator>();
            }

            var validators = new IClientModelValidator[count];
            var clientValidatorIndex = 0;
            for (int i = 0; i < items.Count; i++)
            {
                var validator = items[i].Validator;
                if (validator != null)
                {
                    validators[clientValidatorIndex++] = validator;
                }
            }

            return validators;
        }
    }
}
