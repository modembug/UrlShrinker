using System.ComponentModel.DataAnnotations;

namespace UrlShrinker.Models
{
    public sealed class HomeModel : IHomeModel
    {
        public string Url { get; set; }

        public string Token { get; set; }

        public string Host { get; set; }
    }
}