
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using UrlShrinker.Models;

namespace UrlShrinker.Business
{
    public sealed class ValidationService : IValidationService
    {
        public async Task<IValidationContext> UrlValid(string url)
        {
            string text = url;
            return await Task.Run(() =>
            {
                ValidationContext context = new ValidationContext();

                if (string.IsNullOrEmpty(text))
                {
                    context.ErrorMessages.Add("Please enter a URL.");
                    text = string.Empty;
                }

                //load url with parsed model url for further testing.
                text = HttpUtility.HtmlEncode(text.Trim());

                //Max supported length of IE: https://support.microsoft.com/en-us/kb/208427 (Other browsers support more however, this targets the most restrictive.)
                if (text.Length > 2048) context.ErrorMessages.Add("Please ensure URL is less than 2048 characters.");

                if (!Regex.IsMatch(text,
                        @"^(?:http|https|ftp)://[a-zA-Z0-9\.\-]+(?:\:\d{1,5})?(?:[\\_\-\#\*\(\)A-Za-z0-9\.\;\:\@\&\=\+\$\,\?/]|%u[0-9A-Fa-f]{4}|%[0-9A-Fa-f]{2})*$")) context.ErrorMessages.Add("Please enter a valid URL.");

                if (context.ErrorMessages.Count == 0) context.IsValid = true;

                return context;
            });

        }

        public async Task<IValidationContext> ShortUrlValid(IHomeModel model)
        {
            string text = model.Url;
            return await Task.Run(() =>
            {
                ValidationContext context = new ValidationContext();

                if (string.IsNullOrEmpty(text))
                {
                    context.ErrorMessages.Add("Please enter a URL.");
                    text = string.Empty;
                }

                //load url with parsed model url for further testing.
                text = HttpUtility.HtmlEncode(text.Trim());

                if (!Regex.IsMatch(text, String.Format("(http://)({0})(/)(\\w*)(?!\\W)$", model.Host))) context.ErrorMessages.Add("Please enter a valid URL.");

                if (context.ErrorMessages.Count == 0) context.IsValid = true;

                return context;
            });

        }
    }
}