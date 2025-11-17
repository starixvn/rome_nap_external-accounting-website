namespace Fanex.ExternalAccount.Reports.Helpers
{
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Html;
    using Microsoft.Extensions.Hosting;

    public static class StaticResources
    {
        private static string? staticResourcesVersion;

        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate", Justification = "Reviewed!")]
        private static string minText = ".min";

        private static IWebHostEnvironment? _environment;

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Reviewed!")]
        public static void Init(IWebHostEnvironment environment, string? version = null)
        {
            _environment = environment;

            if (IsDebugMode())
            {
                minText = string.Empty;
            }

            staticResourcesVersion = version ?? GetBuildVersion();
        }

        private static string GetBuildVersion()
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            return System.DateTime.Now.Ticks.ToString();
            //return versionInfo.FileVersion;
        }

        private static bool IsDebugMode()
        {
            return _environment?.IsDevelopment() ?? false;
        }

        private static IHtmlContent RenderStyle(params string[] paths)
        {
            var styleHtml = string.Join(string.Empty, paths.Select(path => $"<link href=\"{path}\" rel=\"stylesheet\" type=\"text/css\">"));

            return new HtmlString(styleHtml);
        }

        private static IHtmlContent RenderScript(params string[] paths)
        {
            var scriptHtml = string.Join(string.Empty, paths.Select(path => $"<script src=\"{path}\" type=\"text/javascript\"></script>"));

            return new HtmlString(scriptHtml);
        }

        [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "Reviewed!")]
        public static IHtmlContent RenderScripts(params string[] paths)
        {
            var scriptPaths = paths.Select(path => $"/{path.TrimStart('/')}{minText}.js?v={staticResourcesVersion}");

            return RenderScript(scriptPaths.ToArray());
        }

        [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "Reviewed!")]
        public static IHtmlContent RenderStyles(params string[] paths)
        {
            var stylePaths = paths.Select(path => $"/{path.TrimStart('/')}{minText}.css?v={staticResourcesVersion}");

            return RenderStyle(stylePaths.ToArray());
        }
    }
}