using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofiller.Data;
using Autofiller.Data.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Autofiller.Web.Pages
{
    public class AppSearchModel : PageModel
    {
        public AppList AppList { get; set; } = DataManager.GetInstance().Apps;
        public void OnGet()
        {
        }
    }
}
