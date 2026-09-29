using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;

namespace Project_IT.Helpers
{
    public static class CultureHelper
    {
        public const string Default = "mk";      // language of existing indexed URLs
        public const string Fallback = "en";     // for unsupported browser languages
        public static readonly string[] Supported = { "mk", "en", "sq", "sr", "de", "pl", "it", "el", "es" };

        static readonly Dictionary<string, string> Locale = new Dictionary<string, string>
        {
            { "mk", "mk-MK" },
            { "en", "en-GB" },
            { "sq", "sq-AL" },
            { "sr", "sr-RS" },
            { "de", "de-DE" },
            { "pl", "pl-PL" },
            { "it", "it-IT" },
            { "el", "el-GR" },
            { "es", "es-ES" }
        };

        static readonly Dictionary<string, string> Og = new Dictionary<string, string>
        {
            { "mk", "mk_MK" },
            { "en", "en_GB" },
            { "sq", "sq_AL" },
            { "sr", "sr_RS" },
            { "de", "de_DE" },
            { "pl", "pl_PL" },
            { "it", "it_IT" },
            { "el", "el_GR" },
            { "es", "es-ES" }
        };

        public static string Current
        {
            get
            {
                var two = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLowerInvariant();
                return IsSupported(two) ? two : Default;
            }
        }

        public static bool IsSupported(string c)
        {
            return !string.IsNullOrEmpty(c) && Supported.Contains(c.ToLowerInvariant());
        }

        public static string OgLocale(string c)
        {
            if (string.IsNullOrEmpty(c)) return Og[Default];
            string lang = c.ToLowerInvariant();
            return Og.ContainsKey(lang) ? Og[lang] : Og[Default];
        }

        public static void Apply(string lang)
        {
            if (string.IsNullOrWhiteSpace(lang) || !IsSupported(lang))
            {
                lang = Fallback;
            }
            lang = lang.ToLowerInvariant();
            var cultureName = Locale.ContainsKey(lang) ? Locale[lang] : Locale[Fallback];
            var ci = CultureInfo.GetCultureInfo(cultureName);
            Thread.CurrentThread.CurrentCulture = ci;     // dates, numbers
            Thread.CurrentThread.CurrentUICulture = ci;   // resource lookup
        }

        public static string Detect(HttpRequestBase req)
        {
            if (req == null) return Fallback;

            var cookie = req.Cookies["lang"]?.Value;
            if (IsSupported(cookie)) return cookie.ToLowerInvariant();

            if (req.UserLanguages != null)
            {
                foreach (var raw in req.UserLanguages)
                {
                    if (string.IsNullOrWhiteSpace(raw)) continue;
                    var two = raw.Split(';')[0].Split('-')[0].Trim().ToLowerInvariant();
                    if (IsSupported(two)) return two;
                }
            }

            return Fallback;
        }
    }
}
