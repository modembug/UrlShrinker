using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShrinker.Models
{
    public interface IHomeModel
    {
        string Url { get; set; }

        string Token { get; set; }

        string Host { get; set; }
    }
}
