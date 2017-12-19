using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Text;
using Microsoft.Extensions.Primitives;
using System;

namespace DashboardCode.Routines.AspNetCore
{
    public class RoutineController : Controller
    {
        public IConfigurationRoot ConfigurationRoot { get; private set;}
        public RoutineController(IConfigurationRoot configurationRoot): base()
        {
            ConfigurationRoot = configurationRoot;
        }


    }
}