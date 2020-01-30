using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DynamicValidationTest.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DynamicValidationTest.Pages
{
    public class IndexModel : PageModel
    {
        public TestModel TestModel { get; set; }

        public void OnGet()
        {
            TestModel = new TestModel();
        }
    }
}
