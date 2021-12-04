using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProcessResizer.Resizer
{
    internal class ResizerSetting
    {
        #region Singleton Logic
        private static ResizerSetting s_instance = new ResizerSetting();
        public static ResizerSetting GetInstance() => s_instance;
        #endregion


        private readonly static string SETTING_FILENAME = "setting.json";
        private string ProcessName { get; set; } = string.Empty;
        private uint X { get; set; }
        private uint Y { get; set; }
        private uint Width { get; set; }
        private uint Height { get; set; }

        public void Save()
        {
            try
            {
                JObject root = new JObject();

                root.Add("ProcessName", ProcessName);
                root.Add("X", X);
                root.Add("Y", Y);
                root.Add("Width", Width);
                root.Add("Height", Height);

                File.WriteAllText(SETTING_FILENAME, root.ToString());
            }
            catch (Exception ex)
            {
                ResizerExceptionHandler.Handle(ex);
            }
        }

        public void Load()
        {
            try
            {
                JObject root = JObject.Parse(File.ReadAllText(SETTING_FILENAME));

                ProcessName = root["ProcessName"].ToString();
                X = uint.Parse(root["X"].ToString());
                Y = uint.Parse(root["Y"].ToString());
                Width = uint.Parse(root["Width"].ToString());
                Height = uint.Parse(root["Height"].ToString());
            }
            catch (Exception ex)
            {
                ResizerExceptionHandler.Handle(ex);
            }
        }
    }
}
