using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AibotLibrary.Config
{
    public class AppConfig
    {
        public Basic basic { get; set; }

        public AppConfig()
        {
            try
            {
                this.basic = Newtonsoft.Json.JsonConvert.DeserializeObject<Basic>(this.getConfig("basic.json"));
            }
            catch(Exception ex)
            {

            }
        }

        /// <summary>
        /// 获取配置文件的 json 内容，不再用这个方法
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public string getConfig(string filename)
        {
            string file_path = System.Environment.CurrentDirectory + "\\config\\" + filename;
            if (File.Exists(file_path) == true)
                return File.ReadAllText(file_path);
            else
                return "";
        }

        private static AppConfig _instance;

        /// <summary>
        /// 获取单列的实例
        /// </summary>
        /// <returns></returns>
        public static AppConfig Instance()
        {
            if (_instance == null)
            {
                _instance = new AppConfig();
            }

            return _instance;
        }
    }
}
