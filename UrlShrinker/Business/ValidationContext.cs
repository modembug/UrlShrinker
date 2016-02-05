
using System;
using System.Collections.Generic;

namespace UrlShrinker.Business
{
    public sealed class ValidationContext : IValidationContext
    {
        public ValidationContext()
        {
            ErrorMessages = new List<string>();
        }
        public List<string> ErrorMessages { get; set; }

        public bool IsValid { get; set; }
    }
}