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

        public ContractResearchView()
        {
            InitializeComponent();
            FilterButton.Click += OnFilterButtonClick;
            SetupSortButtonEvents();

            // Установка обработчиков для окна фильтрации
            SortAscButton.Click += OnSortAscButtonClick;
            SortDescButton.Click += OnSortDescButtonClick;
            CancelButton.Click += OnCancelButtonClick;
            OkButton.Click += OnOkButtonClick;
            SearchTextBox.GotFocus += OnSearchTextBoxGotFocus;
            SearchTextBox.LostFocus += OnSearchTextBoxLostFocus;
            SearchTextBox.TextChanged += OnSearchTextBoxTextChanged;

            // Закрытие окна при клике вне его
            this.MouseDown += OnUserControlMouseDown;

            // Обработчик загрузки UserControl
            this.Loaded += (s, e) =>
            {
                // Инициализация позиции окна
                FilterSortPanel.Visibility = Visibility.Collapsed;
            };
        }

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

            // Получаем позицию кнопки относительно UserControl
            Point buttonPosition = sortButton.TransformToAncestor(this).Transform(new Point(0, 0));

            // Рассчитываем позицию окна
            double panelWidth = 246; // Ширина окна фильтрации
            double panelHeight = 348; // Высота окна фильтрации

            // Координаты для позиционирования справа от кнопки
            double left = buttonPosition.X + sortButton.ActualWidth + 5;
            double top = buttonPosition.Y - 20;

            // Проверяем, не выходит ли окно за правую границу UserControl
            double userControlWidth = this.ActualWidth;

            if (left + panelWidth > userControlWidth)
            {
                // Если выходит за правую границу, показываем слева от кнопки
                left = buttonPosition.X - panelWidth - 5;
            }

            // Проверяем, не выходит ли окно за нижнюю границу
            double userControlHeight = this.ActualHeight;

            if (top + panelHeight > userControlHeight)
            {
                // Если выходит за нижнюю границу, сдвигаем вверх
                top = userControlHeight - panelHeight - 10;
            }

            // Проверяем, не выходит ли окно за верхнюю границу
            if (top < 0)
            {
                top = 10;
            }

            // Устанавливаем позицию окна
            FilterSortPanel.Margin = new Thickness(left, top, 0, 0);

            SortAscButton.Content = $"Сортировать по";
            SortDescButton.Content = $"Сортировать по";

            // Очищаем результаты поиска
            SearchResultsPanel.Children.Clear();
            SearchTextBox.Text = "Поиск...";

            // Показываем окно
            FilterSortPanel.Visibility = Visibility.Visible;
        }

        private void SortColumn(int columnIndex, bool ascending)
        {


            // Закрываем окно после сортировки
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

            // Очищаем предыдущие результаты
            SearchResultsPanel.Children.Clear();

            if (searchText != "Поиск..." && !string.IsNullOrWhiteSpace(searchText))
            {
                // Здесь должна быть логика поиска по данным
                // Для примера добавляем тестовые результаты
                for (int i = 1; i <= 5; i++)
                {
                    var resultItem = new TextBlock
                    {
                        Text = $"Результат {i} для '{searchText}'",
                        FontSize = 12,
                        Margin = new Thickness(5, 2, 5, 2),
                        Foreground = Brushes.Black
                    };

                    SearchResultsPanel.Children.Add(resultItem);
                }
            }
        }

        private void OnUserControlMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Проверяем, был ли клик вне окна фильтрации
            if (FilterSortPanel.Visibility == Visibility.Visible)
            {
                // Проверяем, не является ли кликнутый элемент частью окна фильтрации
                var originalSource = e.OriginalSource as DependencyObject;
                var isPartOfFilterPanel = false;

                // Проверяем все родительские элементы
                while (originalSource != null)
                {
                    if (originalSource == FilterSortPanel)
                    {
                        isPartOfFilterPanel = true;
                        break;
                    }
                    originalSource = VisualTreeHelper.GetParent(originalSource);
                }

                // Если клик был вне окна фильтрации, закрываем его
                if (!isPartOfFilterPanel)
                {
                    FilterSortPanel.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}