using System;
using System.Linq;
using System.Windows;
using Microsoft.VisualBasic;
using Nodify;

namespace Aibot
{
    /// <summary>
    /// 所有的ActionViewType 都是 继承 OperationViewModel
    /// </summary>
    public static class ActionViewFactory
    {

        public static OperationViewModel ToCopy(this OperationViewModel operation)
        {
            return operation.ActionReference!.CreatAtionView();
        }
        
        public static OperationViewModel? CreatAtionView(this AibotItemReferenceViewModel action)
        {
            if (action is null) return null;
            switch (action.ActionViewType)
            {
                case ActionViewType.ForView:
                    {
                        var o = new ForeachDataViewModel
                        {
                            Title = action.Name,
                            ActionReference = action,
                        };
                        return o;
                    }
                case ActionViewType.ForJsonView:
                    {
                        var o = new ForeachJsonDataViewModel
                        {
                            Title = action.Name,
                            ActionReference = action,
                        };
                        return o;
                    }
                case ActionViewType.JsonView:
                    {
                        var o = new JsonDataViewModel
                        {
                            Title = action.Name,
                            ActionReference = action,
                        };
                        return o;
                    }

                case ActionViewType.Print:
                    {
                        var o = new PrintViewModel
                        {
                            Title = action.Name,
                            ActionReference = action,
                        };
                        return o;
                    }
                case ActionViewType.SetStr:
                    {
                        var o = new SetStrViewModel
                        {
                            Title = action.Name,
                            ActionReference = action,
                        };
                        return o;
                    }
                case ActionViewType.Normal:
                default:
                    {
                        var o = new OperationViewModel
                        {
                            Title = action.Name,
                            ActionReference = action,
                        };
                        return o;
                    }

            }
        }

        public static Graph OpenOperation(this OperationGraphData graph, Point? location = null)
        {
            var operations = new NodifyObservableCollection<OperationViewModel>();
            var connections = new NodifyObservableCollection<ConnectionViewModel>();

            var op = graph.Operations.FirstOrDefault();

            if(location is null)
            {
                location = WindowsAPI.Conversion.GetCoordinateRelativeToWindow(WindowsAPI.Window.GetFocused()).CastTo<Point>();
            }

            var cPoint = new Point(location.Value.X - op.Location.X, location.Value.Y - op.Location.Y);

            foreach (var operation in graph.Operations)
            {
                var node = operation.ActionReference?.CreatAtionView();
                if (node is null) continue;//修改这可解决无法保存节点组，还需要保存大小
                node.Location = new Point(operation.Location.X + cPoint.X, operation.Location.Y + cPoint.Y);
                node.Input.Clear();
                node.Output.Clear();

                operation.NodeInfo.Input.ForEach(input =>
                {
                    input.IsConnected = false;
                });

                operation.NodeInfo.Output.ForEach(output =>
                {
                    output.IsConnected = false;
                });

                node.Input.AddRange(operation.NodeInfo.Input);
                node.Output.AddRange(operation.NodeInfo.Output);

                operations.Add(node);
            }
            var inputs = operations.SelectMany(o => o.Input.Select(x => x)).ToList();
            var outputs = operations.SelectMany(o => o.Output.Select(x => x)).ToList();
            foreach (var connection in graph.Connections)
            {
                var input =  inputs.FirstOrDefault(x => x.Id == connection.Input.Id);
                var output = outputs.FirstOrDefault(x => x.Id == connection.Output.Id);

                connections.Add(new ConnectionViewModel
                {
                    Input = input ?? new(),
                    Output = output ?? new(),
                });
            }
            inputs.ForEach(x => x.Id = Guid.NewGuid());
            outputs.ForEach(x => x.Id = Guid.NewGuid());


            var model = new Graph
            {
                Name = graph.Name,
                SaveTime = graph.SaveTime,
                Operations = operations,
                Connections = connections,
            };


            return model;
        }

    }
}
