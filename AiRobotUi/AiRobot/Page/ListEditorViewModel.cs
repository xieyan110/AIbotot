using Nodify;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Aibot
{

    public class ListEditorViewModel : ObservableObject
    {
        private ObservableCollection<AibotViewModel> _operationList = new ObservableCollection<AibotViewModel>();
        public ObservableCollection<AibotViewModel> OperationList
        {
            get => _operationList;
            set => SetProperty(ref _operationList, value);
        }

        private ObservableCollection<OperationGraphData> _workflowFiles = new ObservableCollection<OperationGraphData>();
        public ObservableCollection<OperationGraphData> WorkflowFiles
        {
            get => _workflowFiles;
            set => SetProperty(ref _workflowFiles, value);

        }

        private string _selectedWorkflowName;
        public string SelectedWorkflowName
        {
            get => _selectedWorkflowName;
            set => SetProperty(ref _selectedWorkflowName, value);
        }


        private OperationGraphData _selectedWorkflow;
        public OperationGraphData SelectedWorkflow
        {
            get => _selectedWorkflow;
            set
            {
                if (SetProperty(ref _selectedWorkflow, value))
                {
                    SelectedWorkflowName = value?.Name ?? "";
                }
            }
        }
        private string _adbDeviceName;
        public string AdbDeviceName
        {
            get => _adbDeviceName;
            set => SetProperty(ref _adbDeviceName, value);
        }
        private string _ipAddress;
        public string IpAddress
        {
            get => _ipAddress;
            set => SetProperty(ref _ipAddress, value);
        }


        public INodifyCommand AdbConnectCommand { get; }
        public INodifyCommand CreateWorkflowCommand { get; }
        public INodifyCommand StartCommand { get; }
        public INodifyCommand StartBaseCommand { get; }
        public INodifyCommand PauseCommand { get; }
        public INodifyCommand AddNewCommand { get; }
        public INodifyCommand DeleteSelectedCommand { get; }
        public INodifyCommand RefreshWorkfCommand { get; }
        public INodifyCommand SaveListCommand { get; }
        public INodifyCommand LoadListCommand { get; }

        public INodifyCommand SelectCommand { get; }

        public ListEditorViewModel()
        {
            OperationList = new ObservableCollection<AibotViewModel>();
            WorkflowFiles = new ObservableCollection<OperationGraphData>();

            AdbConnectCommand = new DelegateCommand(async () => await ExecuteAdbConnect());
            StartCommand = new DelegateCommand(async () => await ExecuteStart());
            StartBaseCommand = new DelegateCommand(async () => await ExecuteBaseStart());
            PauseCommand = new DelegateCommand(async () => await ExecutePause());
            AddNewCommand = new DelegateCommand(ExecuteAddNew);
            DeleteSelectedCommand = new DelegateCommand(ExecuteDeleteSelected);
            RefreshWorkfCommand = new DelegateCommand(LoadWorkflowFiles);

            SaveListCommand = new DelegateCommand(ExecuteSaveList);
            LoadListCommand = new DelegateCommand(ExecuteLoadList);


            SelectCommand = new DelegateCommand(() =>
            {
                if(OperationList.All(x => x.IsSelected))
                {
                    OperationList.ForEach(x =>
                    {
                        x.IsSelected = false;
                    });
                }
                else
                {
                    OperationList.ForEach(x =>
                    {
                        x.IsSelected = true;
                    });
                }
            });

            LoadWorkflowFiles();
            ExecuteLoadList();
        }

        private void ExecuteSaveList()
        {
            var data = new List<OperationGraphData>();
            foreach (var aibot in OperationList)
            {
                var graphData = new OperationGraphData
                {
                    Name = aibot.Name ?? "Unnamed",
                    SaveTime = DateTime.Now,
                    Operations = new(),
                    Connections = aibot.Connections
                };

                foreach (var operation in aibot.Operations)
                {
                    graphData.Operations.Add(new NodeBase
                    {
                        Title = operation.Title,
                        ActionReference = operation.ActionReference,
                        Location = operation.Location,
                        NodeInfo = new NodeInfo
                        {
                            Input = operation.Input,
                            Output = operation.Output,
                        }
                    });
                }

                data.Add(graphData);
            }

            string currentDirectory = Directory.GetCurrentDirectory();
            string fileFolderPath = Path.Combine(currentDirectory, "File");
            Directory.CreateDirectory(fileFolderPath);
            string filePath = Path.Combine(fileFolderPath, "首页列表.json");

            var jsonData = data.ToJsonString();
            File.WriteAllText(filePath, jsonData);
            MessageBox.Show("保存成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);

        }

        private void LoadWorkflowFiles()
        {
            var graphOperations = new List<OperationGraphData>();


            WorkflowFiles.Clear();

            string workflowPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Workflow");
            if (Directory.Exists(workflowPath))
            {
                var jsonFiles = Directory.GetFiles(workflowPath, "*.json");
                foreach (var file in jsonFiles)
                {
                    try
                    {
                        var work = File.ReadAllText(file);
                        var d = work.CastTo<OperationGraphData>();
                        graphOperations.Add(d);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            WorkflowFiles.AddRange(graphOperations);
        }

        /// <summary>
        /// 实现 ADB 连接逻辑
        /// </summary>
        private async Task ExecuteAdbConnect()
        {

            await Task.Run(async () =>
            {
                if (!string.IsNullOrWhiteSpace(IpAddress))
                {
                    // 如果提供了IP地址，先尝试连接
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        AdbHelper.RemoteDebugOverWifi(IpAddress);
                    });
                }
            });

            // 获取设备列表
            var devices = await Task.Run(() => AdbHelper.GetAdbDeviceList());

            // 更新UI需要在主线程上进行
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                if (devices.Length == 0)
                {
                    AdbDeviceName = "没有找到设备";
                }
                else if (devices.Length == 1)
                {
                    AdbDeviceName = string.Join(", ", devices);
                }
                else
                {
                    AdbDeviceName = $"找到多个设备: {string.Join(", ", devices)}";
                }
            });
        }


        /// <summary>
        /// 并发执行
        /// </summary>
        private async Task ExecuteStart()
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                OperationList.Where(x => x.IsSelected && !x.IsRunning).ForEach(x =>
                {
                    x.RunCommand.Execute(null);
                });
            });
        }

        /// <summary>
        /// 顺序执行 界面会卡住
        /// </summary>
        /// <returns></returns>
        private async Task ExecuteBaseStart()
        {
            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                foreach(var x in OperationList.Where(x => x.IsSelected && !x.IsRunning))
                {
                    x.BaseStartRun().Wait();
                }
            });
        }

        /// <summary>
        /// 实现暂停逻辑
        /// </summary>
        private async Task ExecutePause()
        {
            var task = new List<Task>();

            OperationList.Where(x => x.IsSelected).ForEach(x =>
            {
                task.Add(Task.Run(() =>
                {
                    x.PauseCommand.Execute(null);
                }));
            });

            await Application.Current.Dispatcher.InvokeAsync(async () => await Task.WhenAll(task));

        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        private void ExecuteDeleteSelected()
        {
            var selectedItems = OperationList.Where(x => x.IsSelected).ToList();
            foreach (var item in selectedItems)
            {
                OperationList.Remove(item);
            }
        }


        private void ExecuteLoadList()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string filePath = Path.Combine(currentDirectory, "File", "首页列表.json");

            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                var data = jsonData.CastTo<List<OperationGraphData>>();

                OperationList.Clear();

                foreach (var newWork in data)
                {

                    var aibotView = new AibotViewModel();

                    var sortedOperations = newWork.Operations
                        .OrderBy(op => op.Location.X)  // 首先按 Y 坐标排序（从上到下）
                        .ThenBy(op => op.Location.Y)   // 然后按 X 坐标排序（从左到右）
                        .ToList();  // 将结果转换为 List

                    // 清空原有集合并添加排序后的项
                    newWork.Operations.Clear();
                    foreach (var operation in sortedOperations)
                    {
                        newWork.Operations.Add(operation);
                    }

                    var graph = newWork.OpenOperation(new Point(0, 0));
                    aibotView.Name = newWork.Name;
                    aibotView.Operations.AddRange(graph.Operations);

                    foreach (var op in graph.Connections)
                    {
                        aibotView.CreateConnection(op.Input, op.Output);
                    }

                    OperationList.Add(aibotView);
                }
            }
        }

        private void ExecuteAddNew()
        {
            if (SelectedWorkflow is null)
            {
                MessageBox.Show("请选择旁边的工作流");
                return;
            }
            var aibotView = new AibotViewModel();
            var json = SelectedWorkflow.ToJsonString();
            var newWork = json.CastTo<OperationGraphData>();

            var sortedOperations = newWork.Operations
                .OrderBy(op => op.Location.X)  // 首先按 Y 坐标排序（从上到下）
                .ThenBy(op => op.Location.Y)   // 然后按 X 坐标排序（从左到右）
                .ToList();  // 将结果转换为 List

            // 清空原有集合并添加排序后的项
            newWork.Operations.Clear();
            foreach (var operation in sortedOperations)
            {
                newWork.Operations.Add(operation);
            }

            var graph = newWork.OpenOperation(new Point(0, 0));
            aibotView.Name = newWork.Name;
            aibotView.Operations.AddRange(graph.Operations);

            foreach (var op in graph.Connections)
            {
                aibotView.CreateConnection(op.Input, op.Output);
            }

            OperationList.Add(aibotView);
        }

    }


}
