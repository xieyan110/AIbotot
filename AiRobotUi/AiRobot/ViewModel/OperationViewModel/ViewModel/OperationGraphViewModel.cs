using Newtonsoft.Json;
using Nodify;
using Newtonsoft.Json.Linq;
using System.Windows;
using System;
using System.Collections.Generic;


namespace Aibot
{
    #region 
    /// <summary>
    /// 用来创建 OperationGraphViewModel
    /// </summary>
    public class OperationGraphData
    {
        public string Name { get; set; }
        public DateTime SaveTime { get; set; }
        public NodifyObservableCollection<NodeBase> Operations { get; set; }
        public NodifyObservableCollection<ConnectionViewModel> Connections { get; set; }
    }

    public class Graph
    {
        public string Name { get; set; }
        public DateTime SaveTime { get; set; }
        public NodifyObservableCollection<OperationViewModel> Operations { get; set; }
        public NodifyObservableCollection<ConnectionViewModel> Connections { get; set; }
    }

    public class NodeBase
    {
        public string Title { get; set; }
        public AibotItemReferenceViewModel? ActionReference { get; set; }
        public Point Location { get; set; }

        public NodeInfo NodeInfo { get; set; }
    }
    public class NodeInfo
    {
        public NodifyObservableCollection<ConnectorViewModel> Input { get; set; }
        public NodifyObservableCollection<ConnectorViewModel> Output { get; set; }
    }
    #endregion

    public class OperationGraphViewModel : OperationViewModel
    {
        public OperationGraphViewModel(Graph graph)
        {
            AibotView.Operations.Clear();
            AibotView.Connections.Clear();
            AibotView.Operations.AddRange(graph.Operations);
            AibotView.Connections.AddRange(graph.Connections);

        }

        public AibotViewModel AibotView { get; } = new AibotViewModel();
        private OperationViewModel InnerOutput { get; } = new OperationViewModel
        {
            Title = "Output Parameters",
            Input = { new ConnectorViewModel() },
            Location = new Point(500, 300),
            IsReadOnly = true
        };


        private Size _size;
        public Size DesiredSize
        {
            get => _size;
            set => SetProperty(ref _size, value);
        }

        private Size _prevSize;

        private bool _isExpanded = true;
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (SetProperty(ref _isExpanded, value))
                {
                    if (_isExpanded)
                    {
                        DesiredSize = _prevSize;
                    }
                    else
                    {
                        _prevSize = Size;
                        // Fit content
                        DesiredSize = new Size(double.NaN, double.NaN);
                    }
                }
            }
        }

        protected override void OnInputValueChanged()
        {

        }

    }
}
