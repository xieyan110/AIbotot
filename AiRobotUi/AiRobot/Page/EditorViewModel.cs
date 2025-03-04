using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Nodify;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.InteropServices;
using WindowsAPI;
using OpenCvSharp.Text;
using System.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;
using System.Collections.ObjectModel;

namespace Aibot
{
    public class EditorViewModel : ObservableObject
    {
        public event Action<EditorViewModel, AibotViewModel>? OnOpenInnerCalculator;

        public EditorViewModel? Parent { get; set; }

        public EditorViewModel()
        {
            Calculator = new AibotViewModel();
            AvailableOperations = Calculator.OperationsMenu.AvailableOperations;
            AvailableGraphOperations = Calculator.OperationsMenu.GraphOperations;


            SaveNodeCommand = new DelegateCommand<AibotViewModel>(_ =>
            {
                InputDialog inputDialog = new InputDialog();
                bool? result = inputDialog.ShowDialog();

                if (result == true)
                {
                    string userInput = inputDialog.InputText;
                    var data = new OperationGraphData
                    {
                        Name = userInput,
                        SaveTime = DateTime.Now,
                        Operations = new(),
                        Connections = Calculator.Connections,
                    };
                    Calculator.Operations.ForEach(x =>
                        {
                            data.Operations.Add(new NodeBase
                            {
                                Title = x.Title,
                                ActionReference = x.ActionReference,
                                Location = x.Location,
                                NodeInfo = new NodeInfo
                                {
                                    Input = x.Input,
                                    Output = x.Output,
                                }
                            });
                        });


                    string currentDirectory = Directory.GetCurrentDirectory();

                    // 定义 workflow 文件夹的路径
                    string workflowFolderPath = Path.Combine(currentDirectory, "Workflow");
                    // 确保 workflow 文件夹存在
                    Directory.CreateDirectory(workflowFolderPath);
                    // 定义保存 JSON 文件的路径
                    string filePath = Path.Combine(workflowFolderPath, $"{data.Name}.json");

                    var josnData = data.ToJsonString();
                    // 将 JSON 字符串写入文件
                    File.WriteAllText(filePath, josnData);

                    var g = Calculator.OperationsMenu.LoldGraphOperations();
                    Calculator.OperationsMenu.GraphOperations.Clear();
                    Calculator.OperationsMenu.GraphOperations.AddRange(g);
                }

            });

            // 复制
            CopyCommand = new DelegateCommand<AibotViewModel>(_ =>
            {
                var connections = new NodifyObservableCollection<ConnectionViewModel>();

                var data = new OperationGraphData
                {
                    Name = "copy",
                    SaveTime = DateTime.Now,
                    Operations = new(),
                    Connections = connections,
                };
                Calculator.Operations
                .Where(x => x.IsSelected)
                .ForEach(x =>
                {
                    data.Operations.Add(new NodeBase
                    {
                        Title = x.Title,
                        ActionReference = x.ActionReference,
                        Location = x.Location,
                        NodeInfo = new NodeInfo
                        {
                            Input = x.Input,
                            Output = x.Output,
                        }
                    });
                });

                var josnData = data.ToJsonString();

            CopyLazy:
                if (!WinAPI.OpenClipboard(IntPtr.Zero))
                {
                    goto CopyLazy;
                }

                WinAPI.EmptyClipboard();
                WinAPI.SetClipboardData(13, Marshal.StringToHGlobalUni(josnData));
                WinAPI.CloseClipboard();

            });

            ImportFileCommand = new DelegateCommand(ImportFile);
            ImportFolderCommand = new DelegateCommand(ImportFolder);
        }

        public INodifyCommand ImportFolderCommand { get; }
        public INodifyCommand ImportFileCommand { get; }
        public INodifyCommand SaveNodeCommand { get; }
        public INodifyCommand CopyCommand { get; }

        private void ImportFile()
        {
            // 打开文件对话框
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                Multiselect = false
            };
            
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string sourceFilePath = openFileDialog.FileName;
                string currentDirectory = Directory.GetCurrentDirectory();
                string targetDirectory = Path.Combine(currentDirectory, "Workflow");

                // 创建 workflow 目录如果不存在
                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }

                string targetFilePath = Path.Combine(targetDirectory, Path.GetFileName(sourceFilePath));

                try
                {
                    // 复制文件到目标目录
                    File.Copy(sourceFilePath, targetFilePath, true);
                }
                catch (Exception ex)
                {
                    // 处理文件复制错误
                    Console.WriteLine($"Error copying file: {ex.Message}");
                }
                var g = Calculator.OperationsMenu.LoldGraphOperations();
                Calculator.OperationsMenu.GraphOperations.Clear();
                Calculator.OperationsMenu.GraphOperations.AddRange(g);
            }
        }

        private void ImportFolder()
        {
            // 打开文件夹对话框
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    string sourceFolderPath = folderBrowserDialog.SelectedPath;
                    string currentDirectory = Directory.GetCurrentDirectory();
                    string targetDirectory = Path.Combine(currentDirectory, "Workflow");

                    // 创建 workflow 目录如果不存在
                    if (!Directory.Exists(targetDirectory))
                    {
                        Directory.CreateDirectory(targetDirectory);
                    }

                    try
                    {
                        // 获取所有 JSON 文件
                        var jsonFiles = Directory.GetFiles(sourceFolderPath, "*.json", SearchOption.AllDirectories);
                        foreach (var sourceFilePath in jsonFiles)
                        {
                            string relativePath = sourceFilePath.Substring(sourceFolderPath.Length + 1);
                            string targetFilePath = Path.Combine(targetDirectory, relativePath);

                            // 创建子目录如果不存在
                            string targetFileDirectory = Path.GetDirectoryName(targetFilePath);
                            if (!Directory.Exists(targetFileDirectory))
                            {
                                Directory.CreateDirectory(targetFileDirectory);
                            }

                            // 复制文件到目标目录
                            File.Copy(sourceFilePath, targetFilePath, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        // 处理文件复制错误
                        Console.WriteLine($"Error copying files: {ex.Message}");
                    }
                    var g = Calculator.OperationsMenu.LoldGraphOperations();
                    Calculator.OperationsMenu.GraphOperations.Clear();
                    Calculator.OperationsMenu.GraphOperations.AddRange(g);

                }
            }
        }

        public Guid Id { get; } = Guid.NewGuid();

        private AibotViewModel _calculator = default!;
        public AibotViewModel Calculator
        {
            get => _calculator;
            set => SetProperty(ref _calculator, value);
        }


        private string? _name;
        public string? Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string? _nodeSearchText;
        public string? NodeSearchText
        {
            get => _nodeSearchText;
            set
            {
                if (SetProperty(ref _nodeSearchText, value))
                {
                    FilterAvailableOperations();
                }
            }
        }

        private NodifyObservableCollection<OperationViewModel> _availableOperations = new NodifyObservableCollection<OperationViewModel>();
        public NodifyObservableCollection<OperationViewModel> AvailableOperations
        {
            get => _availableOperations;
            set => SetProperty(ref _availableOperations, value);
        }



        private void FilterAvailableOperations()
        {
            if (string.IsNullOrWhiteSpace(NodeSearchText))
            {

                AvailableOperations = Calculator.OperationsMenu.AvailableOperations;
            }
            else
            {
                // 根据搜索文本过滤操作
                var filteredOperations = Calculator.OperationsMenu.AvailableOperations
                    .Where(op => op.Title.Contains(NodeSearchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                AvailableOperations = new NodifyObservableCollection<OperationViewModel>(filteredOperations);
            }
        }


        private string? _nodeGroupSearchText;
        public string? NodeGroupSearchText
        {
            get => _nodeGroupSearchText;
            set
            {
                if (SetProperty(ref _nodeGroupSearchText, value))
                {
                    FilterAvailableGraphOperations();
                }
            }
        }

        private NodifyObservableCollection<OperationGraphData> _availableGraphOperations = new NodifyObservableCollection<OperationGraphData>();
        public NodifyObservableCollection<OperationGraphData> AvailableGraphOperations
        {
            get => _availableGraphOperations;
            set => SetProperty(ref _availableGraphOperations, value);
        }

        private void FilterAvailableGraphOperations()
        {
            if (string.IsNullOrWhiteSpace(NodeGroupSearchText))
            {
                AvailableGraphOperations = Calculator.OperationsMenu.GraphOperations;
            }
            else
            {
                // 根据搜索文本过滤操作组
                var filteredOperations = Calculator.OperationsMenu.GraphOperations
                    .Where(op => op.Name.Contains(NodeGroupSearchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                AvailableGraphOperations = new NodifyObservableCollection<OperationGraphData>(filteredOperations);
            }
        }



    }
}
