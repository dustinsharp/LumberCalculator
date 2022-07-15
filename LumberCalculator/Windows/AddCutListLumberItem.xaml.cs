using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace LumberCalculator.Windows
{
    /// <summary>
    /// Interaction logic for AddCutListLumberItem.xaml
    /// </summary>
    public partial class AddCutListLumberItem : Window
    {
        private AddCutListLumberItemViewModel _vm;

        public CutListLumber NewCutListLumberItem => _vm.NewCutListLumberItem;

        public AddCutListLumberItem(int identifier, ObservableCollection<StoreLumber> availableLumber, Window owner)
        {
            InitializeComponent();

            _vm = new AddCutListLumberItemViewModel(identifier, availableLumber);
            DataContext = _vm;

            Owner = owner;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.VerifyNewItem())
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbSelectedStoreLumber.SelectedItem is StoreLumber item)
            {
                _vm.NewCutListLumberItem.SelectedStoreLumber = item;
            }
        }
    }

    public class AddCutListLumberItemViewModel : ViewModelBase
    {
        private CutListLumber _newCutListLumberItem;

        public CutListLumber NewCutListLumberItem
        {
            get => _newCutListLumberItem;
            set
            {
                _newCutListLumberItem = value;
                OnPropertyChanged(nameof(NewCutListLumberItem));
            }
        }

        public ObservableCollection<StoreLumber> AvailableLumber { get; }

        public AddCutListLumberItemViewModel(int identifier, ObservableCollection<StoreLumber> availableLumber)
        {
            AvailableLumber = availableLumber;

            _newCutListLumberItem = new CutListLumber
            {
                Identifier = identifier,
            };
        }

        public bool VerifyNewItem()
        {
            StringBuilder errors = new StringBuilder();
            bool isValid = true;

            if (NewCutListLumberItem.SelectedStoreLumber == null)
            {
                errors.AppendLine($"Must select lumber type!");
                isValid = false;
            }

            if (NewCutListLumberItem.Quantity == 0.0m)
            {
                errors.AppendLine("Must add a quantity!");
                isValid = false;
            }

            if (NewCutListLumberItem.Length <= 0.0m)
            {
                errors.AppendLine("Must add a valid length!");
                isValid = false;
            }

            if (!isValid)
            {
                MessageBox.Show(errors.ToString(), "Add New CutList Lumber Item", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            return isValid;
        }
    }
}
