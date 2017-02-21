using System.ComponentModel;
using System.Windows.Input;

namespace AJF.PhotoMover.Selector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext=new Vm();
        }

        private void MainWindow_OnKeyUp(object sender, KeyEventArgs e)
        {
            var vm = (DataContext as Vm);
            if(vm==null)
                return;

            switch (e.Key)
            {
                case Key.Up:
                case Key.Left:
                    {
                        vm.Index--;
                        break;
                    }
                case Key.Down:
                case Key.Right:
                    {
                        vm.Index++;
                        break;
                    }
                case Key.Enter:
                    {
                        vm.MoveToKeep();
                        break;
                    }
                case Key.Space:
                    {
                        vm.MoveToDiscard();
                        break;
                    }
                case Key.Home:
                    {
                        vm.Reset();
                        break;
                    }
                case Key.Escape:
                    {
                        vm.UndoOneEach();
                        break;
                    }
            }
        }
    }
}
