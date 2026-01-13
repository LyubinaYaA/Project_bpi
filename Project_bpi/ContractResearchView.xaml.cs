using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Project_bpi
{
    public partial class ContractResearchView : UserControl
    {
        private bool _sortButtonsVisible = false;
        private int _currentColumnIndex = -1;
        private Button _currentSortButton;

        // === НОВАЯ ФУНКЦИОНАЛЬНОСТЬ ===
        private ObservableCollection<ContractItem> _contracts;
        private bool _isDeletionMode = false;
        private Brush _defaultDeleteButtonBackground;

        public ContractResearchView()
        {
            InitializeComponent();

            // === СТАРАЯ ФУНКЦИОНАЛЬНОСТЬ ===
            FilterButton.Click += OnFilterButtonClick;
            SetupSortButtonEvents();

            SortAscButton.Click += OnSortAscButtonClick;
            SortDescButton.Click += OnSortDescButtonClick;
            CancelButton.Click += OnCancelButtonClick;
            OkButton.Click += OnOkButtonClick;
            SearchTextBox.GotFocus += OnSearchTextBoxGotFocus;
            SearchTextBox.LostFocus += OnSearchTextBoxLostFocus;
            SearchTextBox.TextChanged += OnSearchTextBoxTextChanged;
            this.MouseDown += OnUserControlMouseDown;

            this.Loaded += (s, e) =>
            {
                FilterSortPanel.Visibility = Visibility.Collapsed;
            };

            // === ИНИЦИАЛИЗАЦИЯ НОВОЙ ФУНКЦИОНАЛЬНОСТИ ===
            _contracts = new ObservableCollection<ContractItem>();
            ContractsDataGrid.ItemsSource = _contracts;

            if (DeleteButton != null)
                _defaultDeleteButtonBackground = DeleteButton.Background.Clone();
        }

        // === СТАРАЯ ЛОГИКА: ФИЛЬТРАЦИЯ И СОРТИРОВКА ===
        private void OnFilterButtonClick(object sender, RoutedEventArgs e)
        {
            _sortButtonsVisible = !_sortButtonsVisible;
            var visibility = _sortButtonsVisible ? Visibility.Visible : Visibility.Collapsed;

            SortCol0.Visibility = visibility;
            SortCol1.Visibility = visibility;
            SortCol2.Visibility = visibility;
            SortCol3.Visibility = visibility;
            SortCol4.Visibility = visibility;
            SortCol6.Visibility = visibility;
        }

        private void SetupSortButtonEvents()
        {
            SortCol0.Click += (s, e) => ShowFilterSortPanel(0, SortCol0);
            SortCol1.Click += (s, e) => ShowFilterSortPanel(1, SortCol1);
            SortCol2.Click += (s, e) => ShowFilterSortPanel(2, SortCol2);
            SortCol3.Click += (s, e) => ShowFilterSortPanel(3, SortCol3);
            SortCol4.Click += (s, e) => ShowFilterSortPanel(4, SortCol4);
            SortCol6.Click += (s, e) => ShowFilterSortPanel(6, SortCol6);
        }

        private void ShowFilterSortPanel(int columnIndex, Button sortButton)
        {
            _currentColumnIndex = columnIndex;
            _currentSortButton = sortButton;

            Point buttonPosition = sortButton.TransformToAncestor(this).Transform(new Point(0, 0));
            double panelWidth = 246;
            double panelHeight = 348;
            double left = buttonPosition.X + sortButton.ActualWidth + 5;
            double top = buttonPosition.Y - 20;

            if (left + panelWidth > this.ActualWidth)
                left = buttonPosition.X - panelWidth - 5;
            if (top + panelHeight > this.ActualHeight)
                top = this.ActualHeight - panelHeight - 10;
            if (top < 0)
                top = 10;

            FilterSortPanel.Margin = new Thickness(left, top, 0, 0);

            SortAscButton.Content = "Сортировать по";
            SortDescButton.Content = "Сортировать по";
            SearchResultsPanel.Children.Clear();
            SearchTextBox.Text = "Поиск...";
            FilterSortPanel.Visibility = Visibility.Visible;
        }

        private void SortColumn(int columnIndex, bool ascending)
        {
            // Здесь можно реализовать сортировку _contracts
            FilterSortPanel.Visibility = Visibility.Collapsed;
        }

        private void OnSortAscButtonClick(object sender, RoutedEventArgs e)
        {
            SortColumn(_currentColumnIndex, true);
        }

        private void OnSortDescButtonClick(object sender, RoutedEventArgs e)
        {
            SortColumn(_currentColumnIndex, false);
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            FilterSortPanel.Visibility = Visibility.Collapsed;
        }

        private void OnOkButtonClick(object sender, RoutedEventArgs e)
        {
            FilterSortPanel.Visibility = Visibility.Collapsed;
        }

        private void OnSearchTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchTextBox.Text == "Поиск...")
            {
                SearchTextBox.Text = "";
                SearchTextBox.Foreground = Brushes.Black;
            }
        }

        private void OnSearchTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                SearchTextBox.Text = "Поиск...";
                SearchTextBox.Foreground = Brushes.Gray;
            }
        }

        private void OnSearchTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text;
            SearchResultsPanel.Children.Clear();

            if (searchText != "Поиск..." && !string.IsNullOrWhiteSpace(searchText))
            {
                foreach (var item in _contracts)
                {
                    bool matches =
                        item.ContractNumber.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        item.NameAndManager.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        item.Customer.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        item.Cost.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        item.Acts.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        item.Payment.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        item.Description.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;

                    if (matches)
                    {
                        var resultItem = new TextBlock
                        {
                            Text = $"«{item.NameAndManager}»",
                            FontSize = 12,
                            Margin = new Thickness(5, 2, 5, 2),
                            Foreground = Brushes.Black
                        };
                        SearchResultsPanel.Children.Add(resultItem);
                    }
                }
            }
        }

        private void OnUserControlMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (FilterSortPanel.Visibility == Visibility.Visible)
            {
                var originalSource = e.OriginalSource as DependencyObject;
                var isPartOfFilterPanel = false;
                while (originalSource != null)
                {
                    if (originalSource == FilterSortPanel)
                    {
                        isPartOfFilterPanel = true;
                        break;
                    }
                    originalSource = VisualTreeHelper.GetParent(originalSource);
                }
                if (!isPartOfFilterPanel)
                {
                    FilterSortPanel.Visibility = Visibility.Collapsed;
                }
            }
        }

        // === НОВАЯ ЛОГИКА: УПРАВЛЕНИЕ ДАННЫМИ ===
        private void OnCreateClick(object sender, RoutedEventArgs e)
        {
            ExitDeletionMode();
            _contracts.Add(new ContractItem());
        }

        private void OnEditClick(object sender, RoutedEventArgs e)
        {
            ExitDeletionMode();
            ContractsDataGrid.IsReadOnly = false;
        }

        private void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            if (!_isDeletionMode)
            {
                _isDeletionMode = true;
                DeleteButton.Background = Brushes.Red;
                DeleteButton.Content = "Подтвердить удаление";
                ContractsDataGrid.IsReadOnly = true;
            }
            else
            {
                var selectedItems = new System.Collections.ArrayList(ContractsDataGrid.SelectedItems);
                foreach (var item in selectedItems)
                {
                    if (item is ContractItem contractItem)
                        _contracts.Remove(contractItem);
                }
                ExitDeletionMode();
            }
        }

        private void ExitDeletionMode()
        {
            if (_isDeletionMode)
            {
                _isDeletionMode = false;
                DeleteButton.Background = _defaultDeleteButtonBackground;
                DeleteButton.Content = "Удалить";
                ContractsDataGrid.IsReadOnly = true;
            }
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            ExitDeletionMode();
            // Сохранение данных — замените на реальную логику
            MessageBox.Show("Данные успешно сохранены!", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    // Модель данных
    public class ContractItem : INotifyPropertyChanged
    {
        private string _contractNumber = "";
        private string _nameAndManager = "";
        private string _cost = "";
        private string _customer = "";
        private string _acts = "";
        private string _payment = "";
        private string _description = "";

        public string ContractNumber
        {
            get => _contractNumber;
            set { _contractNumber = value; OnPropertyChanged(); }
        }

        public string NameAndManager
        {
            get => _nameAndManager;
            set { _nameAndManager = value; OnPropertyChanged(); }
        }

        public string Cost
        {
            get => _cost;
            set { _cost = value; OnPropertyChanged(); }
        }

        public string Customer
        {
            get => _customer;
            set { _customer = value; OnPropertyChanged(); }
        }

        public string Acts
        {
            get => _acts;
            set { _acts = value; OnPropertyChanged(); }
        }

        public string Payment
        {
            get => _payment;
            set { _payment = value; OnPropertyChanged(); }
        }

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}