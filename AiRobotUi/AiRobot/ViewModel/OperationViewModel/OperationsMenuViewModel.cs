using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Nodify;

namespace Aibot
{
    public class OperationsMenuViewModel : ObservableObject
    {
        private bool _isVisible;
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                SetProperty(ref _isVisible, value);
                if (!value)
                {
                    Closed?.Invoke();
                }
            }
        }

        private Point _location;
        public Point Location
        {
            get => _location;
            set => SetProperty(ref _location, value);
        }

        public event Action? Closed;

        public void OpenAt(Point targetLocation)
        {
            Close();
            Location = targetLocation;
            IsVisible = true;
        }

        public void Close()
        {
            IsVisible = false;
        }

        public NodifyObservableCollection<OperationViewModel> AvailableOperations { get; set; }
        public NodifyObservableCollection<OperationGraphData> GraphOperations { get; }

        public INodifyCommand CreateOperationCommand { get; }
        private readonly AibotViewModel _calculator;

        public OperationsMenuViewModel(AibotViewModel calculator)
        {
            _calculator = calculator;
            var actions = new NodifyObservableCollection<AibotItemReferenceViewModel>(AibotDescriptor.GetAvailableItems<IAibotAction>());

            var operations = new NodifyObservableCollection<OperationViewModel>();

            //operations.ActionReference.ActionType
            foreach (var action in actions.Where(op => op.Type != typeof(BaseAibotAction) && op.Type != typeof(BaseAdbAibotAction))
                                          .OrderBy(op => (op.ActionType)))
            {
                operations.Add(action.CreatAtionView());
            }

            AvailableOperations = new NodifyObservableCollection<OperationViewModel>(operations);

            GraphOperations = new NodifyObservableCollection<OperationGraphData>();
            GraphOperations.AddRange(LoldGraphOperations());

            CreateOperationCommand = new DelegateCommand<OperationViewModel>(CreateOperation);
        }

        /// <summary>
        /// op.Output
        /// </summary>
        /// <param name="operationInfo"></param>
        private void CreateOperation(OperationViewModel operation)
        {
            var op = operation.ToCopy();

            op.Location = Location;

            _calculator.Operations.Add(op);

            var pending = _calculator.PendingConnection;
            if (pending.IsVisible)
            {
                var connector = pending.Source.IsInput ? op.Output.FirstOrDefault() : op.Input.FirstOrDefault();
                if (connector != null && _calculator.CanCreateConnection(pending.Source, connector))
                {
                    _calculator.CreateConnection(pending.Source, connector);
                }
            }
            Close();
        }

        public List<OperationGraphData> LoldGraphOperations()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string workflowFolderPath = Path.Combine(currentDirectory, "Workflow");
            Directory.CreateDirectory(workflowFolderPath);

            string[] jsonFiles = Directory.GetFiles(workflowFolderPath, "*.json");

            var graphOperations = new List<OperationGraphData>();
            foreach (string jsonFile in jsonFiles)
            {
                var jsonData = File.ReadAllText(jsonFile);
                var d = jsonData.CastTo<OperationGraphData>();
                graphOperations.Add(d);
            }

            return graphOperations.ToList();
        }

        public void LoldJsonGraphOperations(string jsonData,AibotViewModel calculatorG,Point location)
        {
            OperationGraphData? operationG;
            try
            {
                operationG = jsonData.CastTo<OperationGraphData>();
            }
            catch
            {
                operationG = null;
            }
             
            if (operationG is null) return;
            var graph = operationG.OpenOperation(location);

            calculatorG.Operations.AddRange(graph.Operations);
            foreach (var op in graph.Connections)
            {
                calculatorG.CreateConnection(op.Input, op.Output);
            }

        }
    }
}
