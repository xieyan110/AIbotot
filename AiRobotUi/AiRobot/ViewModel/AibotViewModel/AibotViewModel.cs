using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using Nodify;
using WindowsAPI;

namespace Aibot
{
    public class AibotViewModel : ObservableObject
    {
        [ThreadStatic]
        private static Random random;

        public static int GetRandomNumber(int value1,int value2)
        {
            if (random == null)
            {
                random = new Random();
            }
            return random.Next(value1, value2);
        }

        public AibotViewModel()
        {
            CreateConnectionCommand = new DelegateCommand<ConnectorViewModel>(
                _ => CreateConnection(PendingConnection.Source, PendingConnection.Target),
                _ => CanCreateConnection(PendingConnection.Source, PendingConnection.Target));
            StartConnectionCommand = new DelegateCommand<ConnectorViewModel>(_ => PendingConnection.IsVisible = true, (c) => !(c.IsConnected && c.IsInput));
            DisconnectConnectorCommand = new DelegateCommand<ConnectorViewModel>(DisconnectConnector);
            DeleteSelectionCommand = new DelegateCommand(DeleteSelection);
            GroupSelectionCommand = new DelegateCommand(GroupSelectedOperations, () => SelectedOperations.Count > 0);

            Runner = new AibotRunnerViewModel();

            Connections.WhenAdded(c =>
            {
                c.Input.IsConnected = true;
                c.Output.IsConnected = true;

                c.Input.Value = c.Output.Value;

                //c.Output.ValueObservers.Add(c.Input);
            })
            .WhenRemoved(c =>
            {
                var ic = Connections.Count(con => con.Input == c.Input || con.Output == c.Input);
                var oc = Connections.Count(con => con.Input == c.Output || con.Output == c.Output);

                if (ic == 0)
                {
                    c.Input.IsConnected = false;
                }

                if (oc == 0)
                {
                    c.Output.IsConnected = false;
                }

                //c.Output.ValueObservers.Remove(c.Input);
            });

            Operations.WhenAdded(x =>
            {
                x.Input.WhenRemoved(RemoveConnection);

                if (x is AibotInputOperationViewModel ci)
                {
                    ci.Output.WhenRemoved(RemoveConnection);
                }

                void RemoveConnection(ConnectorViewModel i)
                {
                    var c = Connections.Where(con => con.Input == i || con.Output == i).ToArray();
                    c.ForEach(con => Connections.Remove(con));
                }
            })
            .WhenRemoved(x =>
            {
                foreach (var input in x.Input)
                {
                    DisconnectConnector(input);
                }

                foreach (var output in x.Output)
                {
                    DisconnectConnector(output);
                }
            });

            OperationsMenu = new OperationsMenuViewModel(this);

            RunCommand = new RequeryCommand(async () =>
            {

                RunnerState = RunnerState == MachineState.Running ? MachineState.Stopped : MachineState.Running;

                if (RunnerState != MachineState.Running) return;

                Operations.ForEach(op => op.NodeStatus = NodeStatus.NotExecuted);

                var node = Operations.Where(x => x.Title == typeof(StartRoot).Name).FirstOrDefault();

                await Application.Current.Dispatcher.InvokeAsync(async () => await StartNodeAsync(node!));
            });

            PauseCommand = new RequeryCommand(() =>
            {
                RunnerState = MachineState.Stopped;
                Operations.ForEach(op => op.IsActive = false);
            });

            // 粘贴
            PasteCommand = new DelegateCommand<NodifyEditor>(editor =>
            {

                int CF_TEXT = 1;
                string jsonData = string.Empty;

                if (WinAPI.OpenClipboard(IntPtr.Zero))
                {
                    IntPtr hData = WinAPI.GetClipboardData(CF_TEXT);
                    if (hData != IntPtr.Zero)
                    {
                        IntPtr pszText = WinAPI.GlobalLock(hData);
                        if (pszText != IntPtr.Zero)
                        {
                            jsonData = Marshal.PtrToStringAnsi(pszText) ?? "";
                            WinAPI.GlobalUnlock(hData);
                        }
                    }
                    WinAPI.CloseClipboard();
                }


                OperationsMenu.LoldJsonGraphOperations(jsonData, this, editor.MouseLocation);

            });
        }

        public void AddStartNode()
        {
            var action = new NodifyObservableCollection<AibotItemReferenceViewModel>(AibotDescriptor.GetAvailableItems<IAibotAction>()).FirstOrDefault(a => a.Type == typeof(StartRoot));
            // 稍后修改
            var op = new OperationViewModel
            {
                Title = action!.Name,
                ActionReference = action,
            };

            op.Location = new Point(300, 200);

            Operations.Add(op);
        }

        private NodifyObservableCollection<OperationViewModel> _operations = new NodifyObservableCollection<OperationViewModel>();
        public NodifyObservableCollection<OperationViewModel> Operations
        {
            get => _operations;
            set => SetProperty(ref _operations, value);
        }

        private NodifyObservableCollection<OperationViewModel> _selectedOperations = new NodifyObservableCollection<OperationViewModel>();
        public NodifyObservableCollection<OperationViewModel> SelectedOperations
        {
            get => _selectedOperations;
            set => SetProperty(ref _selectedOperations, value);
        }

        public NodifyObservableCollection<ConnectionViewModel> Connections { get; } = new NodifyObservableCollection<ConnectionViewModel>();
        public PendingConnectionViewModel PendingConnection { get; set; } = new PendingConnectionViewModel();
        public OperationsMenuViewModel OperationsMenu { get; set; }

        public INodifyCommand StartConnectionCommand { get; }
        public INodifyCommand CreateConnectionCommand { get; }
        public INodifyCommand DisconnectConnectorCommand { get; }
        public INodifyCommand DeleteSelectionCommand { get; }
        public INodifyCommand GroupSelectionCommand { get; }
        public INodifyCommand PasteCommand { get; }

        #region state

        private string? _name = "State Machine";
        public string? Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string? _nodeTitle = "选择";
        public string? NodeTitle
        {
            get => _nodeTitle;
            set => SetProperty(ref _nodeTitle, value);
        }

        private MachineState _runnerState;
        public MachineState RunnerState
        {
            get => _runnerState;
            protected set => SetProperty(ref _runnerState, value);
        }

        private ForeachDataX AiForeachDataX = new ForeachDataX();

        private bool _isSelected = false;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public bool IsRunning => RunnerState != MachineState.Stopped;
        public bool IsPaused => RunnerState == MachineState.Paused;
        public AibotRunnerViewModel Runner { get; }
        public AibotV Aibot { get; }

        public INodifyCommand CreateTransitionCommand { get; }
        public INodifyCommand RunCommand { get; }
        public INodifyCommand PauseCommand { get; }


        public async Task BaseStartRun()
        {
            RunnerState = RunnerState == MachineState.Running ? MachineState.Stopped : MachineState.Running;

            if (RunnerState != MachineState.Running) return;

            Operations.ForEach(op => op.NodeStatus = NodeStatus.NotExecuted);

            var node = Operations.Where(x => x.Title == typeof(StartRoot).Name).FirstOrDefault();
            await StartNodeAsync(node!);

        }

        /// <summary>
        /// isroot 且 output 只能连接一个,  isroot 且 input 可以连接多个
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        public OperationViewModel? Next(OperationViewModel operation)
        {
            //Operations
            var o = operation.Output.Where(x => x.IsRoot).Select(x => x.Id).FirstOrDefault();
            var i = Connections.Where(x => x.Output.Id == o).Select(x => x.Input.Id).FirstOrDefault();

            return Operations.Where(x => x.Input.Any(y => y.IsRoot && i == y.Id)).Select(x => x).FirstOrDefault();
        }

        public OperationViewModel? NextIF(OperationViewModel operation, bool isSuccess)
        {
            //Operations
            Guid o;
            if (isSuccess)
                o = operation.Output.Where(x => x.IsRoot && x.PropertyName == "IsSuccess")
                    .Select(x => x.Id).FirstOrDefault();
            else
                o = operation.Output.Where(x => x.IsRoot && x.PropertyName == "IsError")
                    .Select(x => x.Id).FirstOrDefault();

            var i = Connections.Where(x => x.Output.Id == o).Select(x => x.Input.Id).FirstOrDefault();

            return Operations.Where(x => x.Input.Any(y => y.IsRoot && i == y.Id)).Select(x => x).FirstOrDefault();
        }

        private async Task StartNodeAsync(OperationViewModel node)
        {
            AiForeachDataX = new();
            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                try
                {
                    while (node != null)
                    {

                        if (RunnerState == MachineState.Stopped) break;
                        node.IsActive = true;
                        node.NodeStatus = NodeStatus.Executing;

                        var action = Runner.CreateAction(node!.Action);
                        var type = action!.GetType();
                        var param = Runner.CreateAibot(node.Action);
                        param.Node = node;
                        NodeTitle = $"({node.Title}):执行中...";
                        CustomOverlayManager.AddLogMessage($"[{DateTime.Now.ToString("MM/dd HH:mm:ss")}]:({node.Title})执行中...");
                        await Task.Run(async () => await action.Execute(param));

                        node.NodeStatus = NodeStatus.Executed;
                        node.IsActive = false;

                        // for 相关
                        if (node is Foreach)
                        {
                            AiForeachDataX.ForNode.Add(node);
                        }

                        // if 相关
                        if (type.IsSubclassOf(typeof(IF)) || type == typeof(IF))
                        {
                            var isSuccess = param["IsSuccess"]?.Case<bool>() ?? false;
                            node = await Application.Current.Dispatcher.InvokeAsync(() => NextIF(node, isSuccess));
                        }
                        else
                        {
                            node = await Application.Current.Dispatcher.InvokeAsync(() => Next(node));
                        }

                        // for 相关
                        if (node is null || type == typeof(EndFor))
                        {
                            if (type == typeof(EndFor))
                                AiForeachDataX.EndForNode = node;

                            Lazy:
                            var forNdoe = AiForeachDataX.GetLast();
                            var isSuccessNext = (forNdoe as Foreach)?.Next() ?? false;
                            // 数据空了,到达尽头，做删除forNode,设置下一个node

                            if (!isSuccessNext)
                            {
                                AiForeachDataX.RemoveLast();
                                if (AiForeachDataX.ForNode.Count > 0)
                                {
                                    goto Lazy;
                                }
                                else if (!(AiForeachDataX.EndForNode is null))
                                {
                                    node = AiForeachDataX.EndForNode;
                                    AiForeachDataX.EndForNode = null;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else // 数据没有空,到达尽头，
                            {
                                node = Next(forNdoe);
                            }
                        }

                        Connections.ForEach(c =>
                        c.Input.Value = c.Output.Value);

                        await Task.Delay(GetRandomNumber(15, 23));
                    }
                }
                finally
                {
                    NodeTitle = "选择";
                    AiForeachDataX.ForNode.Clear();
                    RunnerState = MachineState.Stopped;
                }

            });
        }
        #endregion

        /// <summary>
        /// isroot 且 output 只能连接一个,  isroot 且 input 可以连接多个
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        private void DisconnectConnectorRoot(ConnectorViewModel connector)
        {
            var connections = Connections.Where(c => (c.Input == connector && c.Input.IsRoot)
                                                   || c.Output == connector).ToList();
            connections.ForEach(c => Connections.Remove(c));
        }

        private void DisconnectConnector(ConnectorViewModel connector)
        {
            var connections = Connections.Where(c => c.Input == connector
                                                   || c.Output == connector).ToList();
            connections.ForEach(c => Connections.Remove(c));
        }

        internal bool CanCreateConnection(ConnectorViewModel source, ConnectorViewModel? target)
            => target == null || (source != target && source.IsInput != target.IsInput);

        internal void CreateConnection(ConnectorViewModel source, ConnectorViewModel? target)
        {
            if (target == null)
            {
                PendingConnection.IsVisible = true;
                OperationsMenu.OpenAt(PendingConnection.TargetLocation);
                OperationsMenu.Closed += OnOperationsMenuClosed;
                return;
            }

            switch (source.IsRoot, target.IsRoot)
            {
                case (true, true):
                    {
                        var input = source.IsInput ? source : target;
                        var output = target.IsInput ? source : target;

                        PendingConnection.IsVisible = false;


                        var connections = Connections.Where(c => c.Input == output
                                       || c.Output == output).ToList();
                        connections.ForEach(c => Connections.Remove(c));


                        Connections.Add(new ConnectionViewModel
                        {
                            Input = input,
                            Output = output
                        });
                    }
                    break;
                case (false, false):
                    {
                        var input = source.IsInput ? source : target;
                        var output = target.IsInput ? source : target;


                        PendingConnection.IsVisible = false;

                        DisconnectConnectorRoot(input);

                        Connections.Add(new ConnectionViewModel
                        {
                            Input = input,
                            Output = output
                        });
                    }
                    break;
                default:
                    return;
            }
        }

        private void OnOperationsMenuClosed()
        {
            PendingConnection.IsVisible = false;
            OperationsMenu.Closed -= OnOperationsMenuClosed;
        }

        private void DeleteSelection()
        {
            var selected = SelectedOperations.ToList();
            selected.ForEach(o => Operations.Remove(o));
        }

        private void GroupSelectedOperations()
        {
            var selected = SelectedOperations.ToList();
            var bounding = selected.GetBoundingBox(50);

            Operations.Add(new OperationGroupViewModel
            {
                Title = "Operations",
                Location = bounding.Location,
                GroupSize = new Size(bounding.Width, bounding.Height)
            });
        }
    }
    public enum MachineState
    {
        Stopped,
        Running,
        Paused,
    }
}

