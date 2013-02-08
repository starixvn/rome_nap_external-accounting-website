// -----------------------------------------------------------------------
// <copyright file="ResourceHelper.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.ExternalAccount.Utilities
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Web;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ResourceHelper
    {
        public static Stream GetEmbeddedResource(string physicalPath, string resourceName)
        {
            try
            {
                Assembly assembly = LoadPluginByPathName<Assembly>(physicalPath);

                if (assembly != null)
                {
                    string[] tempResourceName = assembly.GetManifestResourceNames();
                    if (tempResourceName == null)
                    {
                        return null;
                    }

                    return assembly.GetManifestResourceStream(resourceName);
                }
            }
            catch (Exception)
            {

            }

            return null;
        }

        public static T LoadPluginByPathName<T>(string pathName)
        {
            string viewType = typeof(T).GUID.ToString();

            if (HttpRuntime.Cache[viewType] != null)
                return HttpRuntime.Cache[viewType] is T ? (T)HttpRuntime.Cache[viewType] : default(T);

            object plugin = Assembly.LoadFrom(pathName);
            if (plugin != null)
            {
                //Cache this object as we want to only load this assembly into memory once.
                HttpRuntime.Cache.Insert(viewType, plugin);
                return (T)plugin;
            }

            return default(T);
        }
    }
}
