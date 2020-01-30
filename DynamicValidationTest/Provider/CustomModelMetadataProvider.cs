using DynamicValidationTest.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DynamicValidationTest.Provider
{
    public class CustomModelMetadataProvider : DefaultModelMetadataProvider
    {
        public CustomModelMetadataProvider(ICompositeMetadataDetailsProvider detailsProvider)
            : base(detailsProvider)
        {
        }

        public CustomModelMetadataProvider(ICompositeMetadataDetailsProvider detailsProvider, IOptions<MvcOptions> optionsAccessor)
        : base(detailsProvider, optionsAccessor)
        {
        }

        public override ModelMetadata GetMetadataForType(Type modelType)
        {
            if (modelType != typeof(TestModel))
            {
                return base.GetMetadataForType(modelType);
            }

            var key = ModelMetadataIdentity.ForType(modelType);
            DefaultMetadataDetails details = CreateTypeDetails(key);
            RecreateMetadata(key, details);

            return CreateModelMetadata(details);
        }

        private void RecreateMetadata(ModelMetadataIdentity key, DefaultMetadataDetails details)
        {
            var validationContext = new ValidationMetadataProviderContext(key, details.ModelAttributes);
            var displayContext = new DisplayMetadataProviderContext(key, details.ModelAttributes);
            var bindingContext = new BindingMetadataProviderContext(key, details.ModelAttributes);
            DetailsProvider.CreateValidationMetadata(validationContext);
            DetailsProvider.CreateDisplayMetadata(displayContext);
            DetailsProvider.CreateBindingMetadata(bindingContext);
            details.ValidationMetadata = validationContext.ValidationMetadata;
            details.DisplayMetadata = displayContext.DisplayMetadata;
            details.BindingMetadata = bindingContext.BindingMetadata;
        }

        public override IEnumerable<ModelMetadata> GetMetadataForProperties(Type modelType)
        {
            if (modelType != typeof(TestModel))
            {
                return base.GetMetadataForProperties(modelType);
            }

            var key = ModelMetadataIdentity.ForType(modelType);
            var propertyDetails = CreatePropertyDetails(key);

            List<ModelMetadata> items = new List<ModelMetadata>();
            foreach (var item in propertyDetails)
            {
                var propKey = ModelMetadataIdentity.ForProperty(modelType, item.Key.Name, item.Key.ContainerType);
                RecreateMetadata(propKey, item);
                items.Add(CreateModelMetadata(item));
            }
            return items;
        }
    }
}
