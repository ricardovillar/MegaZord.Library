﻿using System.Collections.Generic;
using System.Web.Mvc;
using MegaZord.Library.Common;

namespace MegaZord.Library.Controllers
{
    public static class ControllerExtensions
    {
        

        public static void AddModelErrors(this Controller controller, IEnumerable<MZValidationResult> validationResults, string defaultErrorKey = null)
        {
            if (validationResults != null)
            {
                foreach (var validationResult in validationResults)
                {
                    if (!string.IsNullOrEmpty(validationResult.MemberName))
                    {
                        controller.ModelState.AddModelError(validationResult.MemberName, validationResult.Message);
                    }
                    else if (defaultErrorKey != null)
                    {
                        controller.ModelState.AddModelError(defaultErrorKey, validationResult.Message);
                    }
                    else
                    {
                        controller.ModelState.AddModelError(string.Empty, validationResult.Message);
                    }
                }
            }
        }    
        public static void AddModelErrors(this ModelStateDictionary modelState, IEnumerable<MZValidationResult> validationResults, string defaultErrorKey = null)
        {
            if (validationResults == null) return;

            foreach (var validationResult in validationResults)
            {
                string key = validationResult.MemberName ?? defaultErrorKey ?? string.Empty;
                modelState.AddModelError(key, validationResult.Message);
            }
        }
    }
}
