// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using JuliusSweetland.OptiKey.Models;
using JuliusSweetland.OptiKey.Properties;
using log4net;

namespace JuliusSweetland.OptiKey.Services.PluginEngine
{
    public class PluginEngine
    {

        #region Private Member Vars

        private static Dictionary<string, Plugin> availablePlugins = new Dictionary<string, Plugin>();
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Public methods

        public static void LoadAvailablePlugins(string pathName = "")
        {
            if (String.IsNullOrEmpty(pathName))
            {
                pathName = Settings.Default.PluginsLocation;
            }

            foreach (string file in Directory.GetFiles(pathName, "*.dll"))
            {
                List<Plugin> plugins = ValidateAndCreatePlugins(file);
                foreach (Plugin plugin in plugins)
                {
                    if (availablePlugins.ContainsKey(plugin.Id))
                    {
                        Log.ErrorFormat("Cannot load plugin ID: '{0}' from {1} due to duplicate name.", plugin.Id, pathName);
                    }
                    else { 
                        availablePlugins.Add(plugin.Id, plugin);
                    }
                }
            }
        }

        public static void RunPlugin_Legacy(Dictionary<string, string> context, XmlPluginKey key)
        {
            Plugin plugin = availablePlugins[key.Plugin];
            List<string> methodArgs = null;
            if (key.Arguments?.Argument?.Count > 0)
            {
                // FIXME: This logic does not support two methods with the same name and different arguments
                methodArgs = new List<String>();
                foreach (MethodInfo pluginMethod in plugin.Type.GetMethods())
                {
                    if (pluginMethod.Name.Equals(key.Method))
                    {
                        foreach (ParameterInfo pluginMethodParam in pluginMethod.GetParameters())
                        {
                            string argValue = null;
                            foreach (PluginArgument arg in key.Arguments.Argument)
                            {
                                if (arg.Name.Equals(pluginMethodParam.Name))
                                {
                                    argValue = GetArgumentValue(context, arg.Value);
                                    break;
                                }
                            }
                            methodArgs.Add(argValue);
                        }
                        break;
                    }
                }
            }
            plugin.Type.InvokeMember(key.Method, BindingFlags.InvokeMethod, null, plugin.Instance, methodArgs?.ToArray());
        }

        public static bool IsPluginAvailable(string key)
        {
            return availablePlugins.ContainsKey(key);
        }

        public static void RunDynamicPlugin(Dictionary<string, string> context, PluginCommand key)
        {
            Plugin plugin = availablePlugins[key.Value];
            List<string> methodArgs = null;
            if (key.Arguments.Any())
            {
                // FIXME: This logic does not support two methods with the same name and different arguments
                methodArgs = new List<string>();
                foreach (MethodInfo pluginMethod in plugin.Type.GetMethods())
                {
                    if (pluginMethod.Name.Equals(key.Method))
                    {
                        foreach (ParameterInfo pluginMethodParam in pluginMethod.GetParameters())
                        {
                            string argValue = null;
                            foreach (DynamicArgument arg in key.Arguments)
                            {
                                if (arg.Name.Equals(pluginMethodParam.Name))
                                {
                                    argValue = GetArgumentValue(context, arg.Value);
                                    break;
                                }
                            }
                            methodArgs.Add(argValue);
                        }
                        break;
                    }
                }
            }
            plugin.Type.InvokeMember(key.Method, BindingFlags.InvokeMethod, null, plugin.Instance, methodArgs?.ToArray());
        }

        public static bool PluginExists(string pluginId)
        {
            return availablePlugins.ContainsKey(pluginId);
        }

        public static void RefreshAvailablePlugins(string pathName)
        {
            availablePlugins = new Dictionary<string, Plugin>();
            LoadAvailablePlugins(pathName);
        }

        public static List<Plugin> GetAllAvailablePlugins()
        {
            return availablePlugins.Values.ToList();
        }
        
        #endregion

        #region Private methods

        private static List<Plugin> ValidateAndCreatePlugins(string file)
        {
            List<Plugin> plugins = new List<Plugin>();

            try
            {
                Assembly dll = Assembly.LoadFile(file);
                string metadataResName = Array.Find(dll.GetManifestResourceNames(), new Predicate<String>(FindMetadataResource));
                if (null != metadataResName)
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Plugins));
                    Plugins pluginMetadata = (Plugins)serializer.Deserialize(new StringReader(new StreamReader(dll.GetManifestResourceStream(metadataResName)).ReadToEnd()));
                    foreach (Plugin plugin in pluginMetadata.Plugin)
                    {
                        Type pluginType = dll.GetType(plugin.Impl);
                        if (null != pluginType)
                        {
                            plugin.Instance = Activator.CreateInstance(pluginType);
                            plugin.Type = pluginType;
                            plugins.Add(plugin);
                        }
                    }
                }
                else
                {
                    Log.ErrorFormat("Error handling library: [{0}]. No manifest found.", file);                    
                }
            }
            catch (Exception e)
            {
                Log.ErrorFormat("Error handling library: [{0}].", file, e);
                // Ensure that all plugins from this DLL are discarded.
                plugins = new List<Plugin>();
            }
            return plugins;
        }

        private static bool FindMetadataResource(string resource)
        {
            return resource.ToLower().StartsWith("juliussweetland.optikey.") && resource.EndsWith("metadata.xml");
        }

        private static string GetArgumentValue(Dictionary<String, String> context, string value)
        {
            string result = value;
            if (value.StartsWith("$"))
            {
                string contextValue = context[value.Substring(1)];
                if (contextValue != null)
                {
                    result = contextValue;
                }
            }
            return result;
        }

        #endregion
    }
}