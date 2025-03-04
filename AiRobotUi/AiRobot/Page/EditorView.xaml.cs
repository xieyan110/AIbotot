using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Nodify;

namespace Aibot
{
    public partial class EditorView : UserControl
    {
        public EditorView()
        {
            InitializeComponent();

            EventManager.RegisterClassHandler(typeof(NodifyEditor), MouseLeftButtonDownEvent, new MouseButtonEventHandler(CloseOperationsMenu));
            EventManager.RegisterClassHandler(typeof(ItemContainer), ItemContainer.DragStartedEvent, new RoutedEventHandler(CloseOperationsMenu));
            EventManager.RegisterClassHandler(typeof(NodifyEditor), MouseRightButtonUpEvent, new MouseButtonEventHandler(OpenOperationsMenu));
        }


        private void OpenOperationsMenu(object sender, MouseButtonEventArgs e)
        {
            if (!e.Handled && e.OriginalSource is NodifyEditor editor && !editor.IsPanning && editor.DataContext is AibotViewModel calculator)
            {
                e.Handled = true;
                calculator.OperationsMenu.OpenAt(editor.MouseLocation);
            }
        }

        private void CloseOperationsMenu(object sender, RoutedEventArgs e)
        {
            ItemContainer? itemContainer = sender as ItemContainer;
            NodifyEditor? editor = sender as NodifyEditor ?? itemContainer?.Editor;

            if (!e.Handled && editor?.DataContext is AibotViewModel calculator)
            {
                calculator.OperationsMenu.Close();
            }
        }

        private void OnDropNode(object sender, DragEventArgs e)
        {
            if(e.Source is NodifyEditor editor && editor.DataContext is AibotViewModel calculator
                && e.Data.GetData(typeof(OperationViewModel)) is OperationViewModel operation)
            {
                var op = operation.ToCopy();
                op.Location = editor.GetLocationInsideEditor(e);
                calculator.Operations.Add(op);
                e.Handled = true;
            }
            if (e.Source is NodifyEditor editorG && editorG.DataContext is AibotViewModel calculatorG
                    && e.Data.GetData(typeof(OperationGraphData)) is OperationGraphData operationG)
            {
                var location = editorG.GetLocationInsideEditor(e);
                var graph = operationG.OpenOperation(location);

                calculatorG.Operations.AddRange(graph.Operations);
                foreach (var op in graph.Connections)
                {
                    calculatorG.CreateConnection(op.Input, op.Output);
                }
                e.Handled = true;
            }
        }

        private void OnNodeDrag(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed && ((FrameworkElement)sender).DataContext is OperationViewModel operation)
            { 

                var data = new DataObject(typeof(OperationViewModel), operation.ToCopy());
                DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);
            }

            if (e.LeftButton == MouseButtonState.Pressed && ((FrameworkElement)sender).DataContext is OperationGraphData operationG)
            {
                var data = new DataObject(typeof(OperationGraphData), operationG.ToJsonString().CastTo<OperationGraphData>());
                DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);
            }
        }
    }
}
