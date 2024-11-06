using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using LumberCalculator.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;

namespace LumberCalculator
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string _twoByFour = "2x4";
        private string _twoBySix = "2x6";
        private string _oneByEight = "1x8";
        private string _oneByFour = "1x4";
        private string _twoByTwo = "2x2";

        private Dictionary<string, LumberDimension> _dimensions;
        private static decimal _bladeWidth = 0.125m; //one eighth is typical blade width

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

        private decimal _minimumScrapLength;

        public decimal MinimumScrapLength
        {
            get => _minimumScrapLength;
            set
            {
                _minimumScrapLength = value;
                OnPropertyChanged(nameof(MinimumScrapLength));
            }
        }

        private decimal _estimatedPrice;

        public decimal EstimatedPrice
        {
            get => _estimatedPrice;
            set
            {
                _estimatedPrice = value;
                OnPropertyChanged(nameof(EstimatedPrice));
            }
        }

        private DelegateCommand _saveCutlistCommand;

        public DelegateCommand SaveCutlistCommand
        {
            get => _saveCutlistCommand ?? (_saveCutlistCommand = new DelegateCommand((o) => true, SaveCutList));
            set
            {
                _saveCutlistCommand = value;
                OnPropertyChanged(nameof(SaveCutlistCommand));
            }
        }

        private DelegateCommand _loadCutListCommand;

        public DelegateCommand LoadCutListCommand
        {
            get => _loadCutListCommand ?? (_loadCutListCommand = new DelegateCommand((o) => true, LoadCutList));
            set
            {
                _loadCutListCommand = value;
                OnPropertyChanged(nameof(LoadCutListCommand));
            }
        }

        private DelegateCommand _clearCutListCommand;

        public DelegateCommand ClearCutListCommand
        {
            get => _clearCutListCommand ?? (_clearCutListCommand = new DelegateCommand((o) => true, ClearCutList));
            set
            {
                _clearCutListCommand = value;
                OnPropertyChanged(nameof(ClearCutListCommand));
            }
        }

        public MainWindowViewModel()
        {
            _minimumScrapLength = _bladeWidth * 4;

            _dimensions = new Dictionary<string, LumberDimension>
            {
                {_twoByFour, new LumberDimension(_twoByFour, 1.5m, 3.5m)},
                {_twoBySix, new LumberDimension(_twoBySix, 1.5m, 5.5m)},
                {_oneByEight, new LumberDimension(_oneByEight, 0.75m, 7.5m)},
                {_oneByFour, new LumberDimension(_oneByFour, 0.75m, 3.5m)},
                {_twoByTwo, new LumberDimension(_twoByTwo, 1.5m, 1.5m)},

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
                new StoreLumber
                {
                    Dimensions = _dimensions[_oneByEight],
                    Length = 96.0m,
                    Price = 10.00m,
                },
                new StoreLumber
                {
                    Dimensions = _dimensions[_oneByFour],
                    Length = 96.0m,
                    Price = 9.00m,
                },
                new StoreLumber
                {
                    Dimensions = _dimensions[_twoByTwo],
                    Length = 96.0m,
                    Price = 4.00m,
                },
            };

            CutList = new ObservableCollection<CutListLumber>();
            PriceList = new ObservableCollection<StoreLumber>();
        }

        public void CalculateLumberNeeded()
        {
            PriceList.Clear();
            RefreshUI();

            foreach (var item in CutList)
            {
                var storeLumberWidth = item.SelectedStoreLumber.Length; //TODO: How do we select this? Preferred option needed?
                var actualWidth = item.Length + _bladeWidth;
                var currentWidth = actualWidth;

                var currentQuantity = 0;

                foreach (var priceItem in PriceList.Where(o => o.Dimensions.Equals(item.SelectedStoreLumber.Dimensions) && o.ScrapLength - MinimumScrapLength > actualWidth))
                {
                    while (priceItem.ScrapLength - MinimumScrapLength > actualWidth && currentQuantity < item.Quantity)
                    {
                        currentQuantity++;

                        priceItem.CutLengths.Add(new CutDimension(actualWidth, priceItem.Dimensions, item.Length, item.Identifier));
                    }

                    if (currentQuantity >= item.Quantity || !PriceList.Any(o => o.Dimensions.Equals(item.SelectedStoreLumber.Dimensions) && o.ScrapLength - MinimumScrapLength > actualWidth))
                        break;
                }

                if (currentQuantity >= item.Quantity)
                    continue;

                while (currentQuantity < item.Quantity)
                {
                    var iterationQuantity = 0;

                    while (currentWidth <= storeLumberWidth - MinimumScrapLength)
                    {
                        if (currentQuantity >= item.Quantity)
                            break;

                        currentQuantity++;
                        iterationQuantity++;
                        currentWidth += actualWidth;
                    }

                    var selectedStoreLumber = item.SelectedStoreLumber.Clone(new Guid());

                    for (var i = 0; i < iterationQuantity; i++)
                    {
                        selectedStoreLumber.CutLengths.Add(new CutDimension(actualWidth, selectedStoreLumber.Dimensions, item.Length, item.Identifier));
                    }

                    PriceList.Add(selectedStoreLumber);
                    currentWidth = actualWidth;
                }
            }

            OptimizePriceList();

            EstimatedPrice = PriceList?.Select(o => o.Price).Sum(o => o) ?? 0.0m;

            RefreshUI();
        }

        public void OptimizePriceList()
        {
            var priceListItemsToRemove = new List<StoreLumber>();

            foreach (var lumber in PriceList)
            {
                var moveCandidate = PriceList.FirstOrDefault(o => 
                    o.Dimensions.Equals(lumber.Dimensions) 
                    && o.ScrapLength - MinimumScrapLength > lumber.TotalCutLength
                    && o.Identifier != lumber.Identifier);

                if (moveCandidate == null) 
                    continue;

                moveCandidate.CutLengths.AddRange(lumber.CutLengths);
                priceListItemsToRemove.Add(lumber);
            }

            priceListItemsToRemove.ForEach(o => PriceList.Remove(o));
        }

        public void RefreshUI()
        {
            OnPropertyChanged(nameof(AvailableLumber));
            OnPropertyChanged(nameof(CutList));
            OnPropertyChanged(nameof(PriceList));
        }

        public void AddCutListLumber(Window owner)
        {
            var identifier = CutList.Any() ? CutList.Select(o => o.Identifier).Max() + 1 : 1;

            var addCutListLumberItemWindow = new AddCutListLumberItem(identifier, AvailableLumber, owner);
            var result = addCutListLumberItemWindow.ShowDialog();

            if (result != true) 
                return;

            CutList.Add(addCutListLumberItemWindow.NewCutListLumberItem);
            CalculateLumberNeeded();
        }

        private void SaveCutList(object obj)
        {
            if (!CutList.Any())
                return;

            var dialog = new CommonSaveFileDialog
            {
                Filters = { new CommonFileDialogFilter("JSON settings file", ".json") },
            };

            if (dialog.ShowDialog() != CommonFileDialogResult.Ok) 
                return;

            var cutListJson = JsonConvert.SerializeObject(CutList, Formatting.Indented).Replace("\\r\\n", "\r\n");

            var filename = string.IsNullOrEmpty(Path.GetExtension(dialog.FileName))
                ? $"{dialog.FileName}.json"
                : dialog.FileName;

            if (Path.GetExtension(filename) != ".json")
                filename = filename.Replace(Path.GetExtension(filename), ".json");

            using (var sw = new StreamWriter(filename))
            {
                sw.Write(cutListJson);
            }
        }

        private void LoadCutList(object obj)
        {
            CutList.Clear();

            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = false,
                Filters = { new CommonFileDialogFilter("JSON settings file", ".json") },
            };

            if (dialog.ShowDialog() != CommonFileDialogResult.Ok) 
                return;

            var items = JsonConvert.DeserializeObject<List<CutListLumber>>(File.ReadAllText(dialog.FileName));
            items.ForEach(item => { CutList.Add(item); });

            OnPropertyChanged(nameof(CutList));

            CalculateLumberNeeded();
        }

        private void ClearCutList(object obj)
        {
            CutList.Clear();
            OnPropertyChanged(nameof(CutList));

            CalculateLumberNeeded();
        }
    }
}