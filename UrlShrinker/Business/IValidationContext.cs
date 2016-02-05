
using System.Collections.Generic;

namespace UrlShrinker.Business
{
    public interface IValidationContext
    {
        List<string> ErrorMessages { get; set; }
        bool IsValid { get; set; }
    }
}
