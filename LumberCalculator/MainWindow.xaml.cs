using System.Windows;

namespace LumberCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _vm;

        public MainWindow()
        {
            InitializeComponent();

            _vm = new MainWindowViewModel();
            DataContext = _vm;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnCalculate_Click(object sender, RoutedEventArgs e)
        {
            _vm.CalculateLumberNeeded();
        }

        private void btnAddCutlistItem_Click(object sender, RoutedEventArgs e)
        {
            _vm.AddCutListLumber();
        }

        private void btnRemoveCutlistItem_Click(object sender, RoutedEventArgs e)
        {
            if (lvCutList.SelectedItem is CutListLumber item)
            {
                _vm.CutList.Remove(item);
                _vm.CalculateLumberNeeded();
            }
        }
    }
}
