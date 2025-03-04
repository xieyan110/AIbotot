using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Nodify;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Windows.Markup;
using System.Collections;
using System.Text.Json.Nodes;

namespace Aibot
{



    public class JsonDataViewModel : OperationViewModel
    {


        private Dictionary<string, string> _data;
        public Dictionary<string, string> Data
        {
            get => _data;
            set => SetProperty(ref _data, value);
        }
  

        public INodifyCommand LoadJsonCommand { get; private set; }



        public JsonDataViewModel()
        {
            Data = new Dictionary<string, string>();
            LoadJsonCommand = new RequeryCommand(() =>
            {
                LoadJson();
            });
        }

        private List<ConnectorViewModel> connectorViews = new List<ConnectorViewModel>();

        public void InitLoad()
        {
            var jsonData = string.Empty;
            Input.ForEach(x =>
            {
                if (x.PropertyName == "JsonData")
                {
                    if (!string.IsNullOrWhiteSpace(x.Value?.ToString()))
                        jsonData = x.Value.ToString();
                }
            });
            //JArray
            JObject jsonObj = new JObject();
            try
            {
                var jToken = JArray.Parse(jsonData).FirstOrDefault();
                jsonObj = jToken.Value<JObject>();
            }
            catch
            {
                jsonObj = JObject.Parse(jsonData);
            }

            Data = new Dictionary<string, string>();
            foreach (var property in jsonObj.Properties())
            {
                Data[property.Name] = property.Value?.ToString() ?? "";
            }
            Output.Where(x => !x.IsRoot).ForEach(x =>
            {
                if(connectorViews.Count == 0)
                {
                    connectorViews.Add(x);
                }
                var v = Data?.GetValueOrDefault(x.Title);
                x.Value = v;
            });


        }
        public void LoadJson()
        {
            try
            {
                InitLoad();

                connectorViews.ForEach(x => Output.Remove(x));
                connectorViews.Clear();

                foreach (var (k, v) in Data.Select(x => (x.Key, x.Value)))
                {
                    connectorViews.Add(new ConnectorViewModel
                    {
                        Title = k,
                        Type = AibotKeyType.String,
                        PropertyName = k,
                        CanChangeType = true,
                        Value = v,
                        IsRoot = false,
                        ValueIsKey = true,
                    });
                }
                Output.AddRange(connectorViews);
            }
            catch
            {
                // Handle exception
            }


        }
    }
}
