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

namespace Aibot
{

    #region Base For 其他的For视图，也必须用 也可以用
    public  class ForeachDataX
    {
        public  List<OperationViewModel> ForNode = new List<OperationViewModel>();
        public  OperationViewModel? EndForNode;

        /// <summary>
        /// 获取最后一个元素
        /// </summary>
        /// <returns></returns>
        public  OperationViewModel? GetLast()
        {
            if (ForNode.Count > 0)
                return ForNode[ForNode.Count - 1];
            else
                return null;
        }

        /// <summary>
        /// 获取第一个元素
        /// </summary>
        /// <returns></returns>
        public  OperationViewModel? GetFirst()
        {
            if (ForNode.Count > 0)
                return ForNode[0];
            else
                return null;
        }

        /// <summary>
        /// 删除最后一个元素
        /// </summary>
        public  void RemoveLast()
        {
            if (ForNode.Count > 0)
                ForNode.RemoveAt(ForNode.Count - 1);
        }

        /// <summary>
        /// 删除第一个元素
        /// </summary>
        public  void RemoveFirst()
        {
            if (ForNode.Count > 0)
                ForNode.RemoveAt(0);
        }
    }

    public interface Foreach
    {
        public void InitEnumerator();
        public bool Next();
    }
    #endregion
    public class ForeachDataViewModel : OperationViewModel,Foreach
    {

        private NodifyObservableCollection<List<string>> _dataList;
        public NodifyObservableCollection<List<string>> DataList
        {
            get => _dataList;
            set => SetProperty(ref _dataList, value);
        }

        private NodifyObservableCollection<Dictionary<string, string>> _data;
        public NodifyObservableCollection<Dictionary<string, string>> Data
        {
            get => _data;
            set => SetProperty(ref _data, value);
        }

        private NodifyObservableCollection<string> _dataLine;
        public NodifyObservableCollection<string> DataLine
        {
            get => _dataLine;
            set => SetProperty(ref _dataLine, value);
        }

        public INodifyCommand OpenFileCommand { get; private set; }

        private IEnumerator<IEnumerable<(string key,string value)>>? Enumerator { get; set; }

        /// <summary>
        /// 很重要
        /// </summary>
        public void InitEnumerator()
        {
            LoadJsonFile(FilePath);
        }

        /// <summary>
        /// 更重要
        /// </summary>
        /// <returns></returns>
        public bool Next()
        {
            if (Enumerator!.MoveNext())
            {
                var item = Enumerator.Current.ToList();
                var count = item.Count;
                for (int i = 0; i < count; i++)
                {
                    connectorViews[i].Value = item[i].value;
                }
            }
            else
            {
                return false;
            }
            return true;

        }
        public ForeachDataViewModel()
        {
            Data = new NodifyObservableCollection<Dictionary<string, string>>();
            DataLine = new NodifyObservableCollection<string>();
            DataList = new NodifyObservableCollection<List<string>>();
            OpenFileCommand = new RequeryCommand(OpenJsonFile);
        }

        private void OpenJsonFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON files (*.json)|*.json";
            if (openFileDialog.ShowDialog() == true)
            {
                LoadJsonFile(openFileDialog.FileName);
            }
        }

        

        private List<ConnectorViewModel> connectorViews = new List<ConnectorViewModel>();
        private string FilePath {  get; set; }

        public void InitLoad()
        {

            var jsonData = File.ReadAllText(FilePath);
            JArray jsonArray = JArray.Parse(jsonData);
            Data.Clear();
            DataList.Clear();
            DataLine.Clear();


            foreach (JObject obj in jsonArray)
            {
                var item = new Dictionary<string, string>();
                foreach (var property in obj.Properties())
                {
                    item[property.Name] = property.Value?.ToString() ?? "";
                }
                Data.Add(item);
            }

            DataList.Add(Data[0].Keys.ToList());
            DataList.AddRange(Data.Select(x => x.Values.ToList()));

            DataLine.AddRange(DataList.Select(x => string.Join(", ", x.Select(s => s.PadRight(10)))));

            connectorViews.ForEach(x => Output.Remove(x));
            connectorViews.Clear();
        }
        public void LoadJsonFile(string filePath)
        {
            try
            {
                FilePath = filePath;
                InitLoad();

                foreach (var (k,v) in Data[0].Select(x => (x.Key,x.Value)))
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

                Enumerator = Data.Select(z => z.Select(x => (x.Key, x.Value))).GetEnumerator();
                Enumerator.MoveNext(); // 因为第一次就是上面的不需要重复
            }
            catch
            {
                // Handle exception
            }


        }
    }

    public class ForeachJsonDataViewModel : OperationViewModel, Foreach
    {

        private NodifyObservableCollection<List<string>> _dataList;
        public NodifyObservableCollection<List<string>> DataList
        {
            get => _dataList;
            set => SetProperty(ref _dataList, value);
        }

        private NodifyObservableCollection<Dictionary<string, string>> _data;
        public NodifyObservableCollection<Dictionary<string, string>> Data
        {
            get => _data;
            set => SetProperty(ref _data, value);
        }

        private NodifyObservableCollection<string> _dataLine;
        public NodifyObservableCollection<string> DataLine
        {
            get => _dataLine;
            set => SetProperty(ref _dataLine, value);
        }
                


        public INodifyCommand LoadJsonCommand { get; private set; }

        private IEnumerator<IEnumerable<(string key, string value)>>? Enumerator { get; set; }

        /// <summary>
        /// 很重要
        /// </summary>
        public void InitEnumerator()
        {
            InitLoad();
            Next();
        }

        /// <summary>
        /// 更重要
        /// </summary>
        /// <returns></returns>
        public bool Next()
        {
            if (Enumerator!.MoveNext())
            {
                var item = Enumerator.Current.ToList();
                var count = item.Count;
                for (int i = 0; i < count; i++)
                {
                    if(connectorViews.Count == 0)
                    {
                        Output.Where(x => !x.IsRoot).ForEach(x => connectorViews.Add(x));
                    }
                    connectorViews[i].Value = item[i].value;
                }
            }
            else
            {
                return false;
            }
            return true;

        }
        public ForeachJsonDataViewModel()
        {
            Data = new NodifyObservableCollection<Dictionary<string, string>>();
            DataLine = new NodifyObservableCollection<string>();
            DataList = new NodifyObservableCollection<List<string>>();
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

            JArray jsonArray;
            try
            {
                jsonArray = JArray.Parse(jsonData);
            }
            catch
            {
                var text = jsonData.Split('\n').Select(x => new { text = x.Trim() }).ToList();
                var json = text.ToJsonString();
                jsonArray = JArray.Parse(json);
            }

            Data.Clear();
            DataList.Clear();
            DataLine.Clear();


            foreach (JObject obj in jsonArray)
            {
                var item = new Dictionary<string, string>();
                foreach (var property in obj.Properties())
                {
                    item[property.Name] = property.Value?.ToString() ?? "";
                }
                Data.Add(item);
            }

            DataList.Add(Data[0].Keys.ToList());
            DataList.AddRange(Data.Select(x => x.Values.ToList()));

            DataLine.AddRange(DataList.Select(x => string.Join(", ", x.Select(s => s.PadRight(10)))));

            Enumerator = Data.Select(z => z.Select(x => (x.Key, x.Value))).GetEnumerator();
        }

        public void LoadJson()
        {
            try
            {
                InitLoad();

                connectorViews.ForEach(x => Output.Remove(x));
                connectorViews.Clear();

                foreach (var (k, v) in Data[0].Select(x => (x.Key, x.Value)))
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
