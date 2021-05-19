using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LumberCalculator.Windows;

namespace LumberCalculator
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string _twoByFour = "2x4";
        private string _twoBySix = "2x6";
        private Dictionary<string, LumberDimension> _dimensions;
        private decimal _bladeWidth = 0.125m; //one eighth is typical blade width

        private ObservableCollection<StoreLumber> _availableLumber;

        public ObservableCollection<StoreLumber> AvailableLumber
        {
            get => _availableLumber;
            set
            {
                _availableLumber = value;
                OnPropertyChanged(nameof(AvailableLumber));
            }
        }

        private ObservableCollection<CutListLumber> _cutlist;

        public ObservableCollection<CutListLumber> CutList
        {
            get => _cutlist;
            set
            {
                _cutlist = value;
                OnPropertyChanged(nameof(CutList));
            }
        }

        private ObservableCollection<StoreLumber> _priceList;

        public ObservableCollection<StoreLumber> PriceList
        {
            get => _priceList;
            set
            {
                _priceList = value;
                OnPropertyChanged(nameof(PriceList));
            }
        }

        public MainWindowViewModel()
        {
            _dimensions = new Dictionary<string, LumberDimension>
            {
                {_twoByFour, new LumberDimension(_twoByFour, 1.5m, 3.5m)},
                {_twoBySix, new LumberDimension(_twoBySix, 1.5m, 5.5m)},
            };

            AvailableLumber = new ObservableCollection<StoreLumber>
            {
                new StoreLumber
                {
                    Dimensions = _dimensions[_twoByFour],
                    Length = 96.0m,
                    Price = 9.00m,
                },
                new StoreLumber
                {
                    Dimensions = _dimensions[_twoBySix],
                    Length = 96.0m,
                    Price = 11.00m,
                },
            };

            var twoBySix = AvailableLumber.FirstOrDefault(o => o.Dimensions.Name == _twoBySix);
            var twoByFour = AvailableLumber.FirstOrDefault(o => o.Dimensions.Name == _twoByFour);

            CutList = new ObservableCollection<CutListLumber>
            {
                new CutListLumber
                {
                    Identifier = 1,
                    SelectedStoreLumber = twoBySix,
                    Length = 48.0m - _bladeWidth,
                    Quantity = 4,
                },
                new CutListLumber
                {
                    Identifier = 2,
                    SelectedStoreLumber = twoBySix,
                    Length = 36.0m,
                    Quantity = 8,
                },
                new CutListLumber
                {
                    Identifier = 3,
                    SelectedStoreLumber = twoBySix,
                    Length = 25.766m,
                    Quantity = 4,
                },

                //tests
                new CutListLumber
                {
                    Identifier = 4,
                    SelectedStoreLumber = twoByFour,
                    Length = 17.5m,
                    Quantity = 4,
                },
                new CutListLumber
                {
                    Identifier = 5,
                    SelectedStoreLumber = twoByFour,
                    Length = 18.5m,
                    Quantity = 4,
                },
                new CutListLumber
                {
                    Identifier = 6,
                    SelectedStoreLumber = twoByFour,
                    Length = 22.375m,
                    Quantity = 4,
                },
                new CutListLumber
                {
                    Identifier = 7,
                    SelectedStoreLumber = twoByFour,
                    Length = 11.0m,
                    Quantity = 4,
                },
                new CutListLumber
                {
                    Identifier = 8,
                    SelectedStoreLumber = twoByFour,
                    Length = 6.125m,
                    Quantity = 4,
                },
            };

            PriceList = new ObservableCollection<StoreLumber>();
        }

        public void CalculateLumberNeeded()
        {
            PriceList.Clear();
            RefreshUI();

            foreach (CutListLumber item in CutList)
            {
                decimal storeLumberWidth = item.SelectedStoreLumber.Length; //TODO: How do we select this? Preferred option needed?
                decimal actualWidth = item.Length + _bladeWidth;
                decimal currentWidth = actualWidth;

                int currentQuantity = 0;

                foreach (var priceItem in PriceList.Where(o => o.Dimensions.Equals(item.SelectedStoreLumber.Dimensions) && o.ScrapLength > item.Length))
                {
                    while (priceItem.ScrapLength > item.Length && currentQuantity < item.Quantity)
                    {
                        currentQuantity++;

                        StoreLumber selectedStoreLumber = item.SelectedStoreLumber.Clone();

                        priceItem.CutLengths.Add(new CutDimension(actualWidth, selectedStoreLumber.Dimensions, item.Length, item.Identifier));
                    }

                    if (currentQuantity >= item.Quantity || !PriceList.Any(o => o.Dimensions.Equals(item.SelectedStoreLumber.Dimensions) && o.ScrapLength > item.Length))
                    {
                        break;
                    }
                }

                if (currentQuantity >= item.Quantity)
                {
                    continue;
                }

                while (currentQuantity < item.Quantity)
                {
                    int iterationQuantity = 0;

                    while (currentWidth <= storeLumberWidth)
                    {
                        if (currentQuantity >= item.Quantity)
                        {
                            break;
                        }

                        currentQuantity++;
                        iterationQuantity++;
                        currentWidth += actualWidth;
                    }

                    StoreLumber selectedStoreLumber = item.SelectedStoreLumber.Clone();

                    for (int i = 0; i < iterationQuantity; i++)
                    {
                        selectedStoreLumber.CutLengths.Add(new CutDimension(actualWidth, selectedStoreLumber.Dimensions, item.Length, item.Identifier));
                    }

                    PriceList.Add(selectedStoreLumber);
                    currentWidth = actualWidth; //reset
                }
            }

            RefreshUI();
        }

        public void RefreshUI()
        {
            OnPropertyChanged(nameof(AvailableLumber));
            OnPropertyChanged(nameof(CutList));
            OnPropertyChanged(nameof(PriceList));
        }

        public void AddCutListLumber()
        {
            int identifier = CutList.Any() ? CutList.Select(o => o.Identifier).Max() + 1 : 1;

            //Add UI to populate new cutlist item
            var addCutListLumberItemWindow = new AddCutListLumberItem(identifier, AvailableLumber);
            var result = addCutListLumberItemWindow.ShowDialog();

            if (result == true)
            {
                CutList.Add(addCutListLumberItemWindow.NewCutListLumberItem);

                //PriceList.Clear();
                //RefreshUI();

                CalculateLumberNeeded();
            }
        }
    }
}