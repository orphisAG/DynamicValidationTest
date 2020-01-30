using DynamicValidationTest.Model;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TestModel = DynamicValidationTest.Model.TestModel;

namespace DynamicValidationTest.Provider
{
    public class ValidationMetaProvider : IValidationMetadataProvider
    {
       
        public static string CounterStatus
        {
            get
            {
                return Counter >= 5 ? "Inverted Mode" : "Default Mode";
            }
        }

        private static int Counter = 0;


        public void CreateValidationMetadata(ValidationMetadataProviderContext context)
        {
            if (context.Key.Name == nameof(TestModel.Required) || context.Key.Name == nameof(TestModel.Optional))
            {
                Counter++;
                if (Counter < 5)
                {
                    context.ValidationMetadata.IsRequired = context.Key.Name == nameof(TestModel.Required);
                }
                else
                {
                    context.ValidationMetadata.IsRequired = context.Key.Name != nameof(TestModel.Required);
                }
            }
        }
    }
}
