using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;

namespace Project_bpi
{
    public partial class MainWindow : Window
    {
        private Dictionary<Border, Border> parents = new Dictionary<Border, Border>();
        private DateTime _currentCalendarDate = DateTime.Today;
        public MainWindow()
        {
            InitializeComponent();
            InitializeDateRange();
            InitializeMenuHierarchy();
        }
        private void InitializeMenuHierarchy()
        {
            // 1-й уровень → корневой заголовок НИР
            parents[Section1_Header] = NIR_Header;
            parents[Item_Title] = NIR_Header;
            parents[Section2_Header] = NIR_Header;
            parents[Section3_Header] = NIR_Header;
            parents[Section4_Header] = NIR_Header;
            parents[Section5_Header] = NIR_Header;
            parents[Section6_Header] = NIR_Header;

            // 2-й уровень → Раздел 1
            parents[Item_11] = Section1_Header;
            parents[Item_12] = Section1_Header;
            parents[Item_13] = Section1_Header;
            parents[Item_14] = Section1_Header;
            parents[Item_15] = Section1_Header;
        }

        public class CalendarDay
        {
            public string Day { get; set; }
            public string TextColor { get; set; }
            public string FontWeight { get; set; }
            public string BackgroundColor { get; set; }
            public DateTime? Date { get; set; }
        }
        private void InitializeDateRange()
        {
            // Устанавливаем начальный период - текущий год
            var today = DateTime.Today;
            var startDate = new DateTime(today.Year, 1, 1);
            var endDate = new DateTime(today.Year, 12, 31);

            UpdateDateDisplay(startDate, endDate);

            // Инициализируем комбобоксы
            InitializeDateComboBoxes();

            // Устанавливаем даты в комбобоксы
            SetDateInComboBoxes(startDate, endDate);

            // Обновляем отображение месяца
            UpdateCalendarDisplay(today);
        }
        private void InitializeDateComboBoxes()
        {
            // Заполняем дни (1-31)
            for (int i = 1; i <= 31; i++)
            {
                StartDayComboBox.Items.Add(i);
                EndDayComboBox.Items.Add(i);
            }

            // Заполняем месяцы
            var months = new[]
            {
        "Янв", "Фев", "Мар", "Апр", "Май", "Июн",
        "Июл", "Авг", "Сен", "Окт", "Ноя", "Дек"
    };

            for (int i = 0; i < months.Length; i++)
            {
                StartMonthComboBox.Items.Add(new { Text = months[i], Value = i + 1 });
                EndMonthComboBox.Items.Add(new { Text = months[i], Value = i + 1 });
            }

            // Заполняем годы (текущий год -5 до +5)
            int currentYear = DateTime.Today.Year;
            for (int i = currentYear - 5; i <= currentYear + 5; i++)
            {
                StartYearComboBox.Items.Add(i);
                EndYearComboBox.Items.Add(i);
            }

            // Настраиваем DisplayMemberPath для месяцев
            StartMonthComboBox.DisplayMemberPath = "Text";
            EndMonthComboBox.DisplayMemberPath = "Text";
        }
        private void StartDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var startDate = GetStartDateFromComboBoxes();
            if (startDate.HasValue)
            {
                // Обновляем отображение календаря при изменении даты начала
                UpdateCalendarDisplay(_currentCalendarDate);

                // Если конечная дата не установлена, устанавливаем её такую же
                var endDate = GetEndDateFromComboBoxes();
                if (!endDate.HasValue)
                {
                    SetDateInComboBoxes(startDate.Value, startDate.Value);
                }
            }
        }
        private void EndDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var endDate = GetEndDateFromComboBoxes();
            if (endDate.HasValue)
            {
                // Обновляем отображение календаря при изменении конечной даты
                UpdateCalendarDisplay(_currentCalendarDate);

                // Если начальная дата не установлена, устанавливаем её такую же
                var startDate = GetStartDateFromComboBoxes();
                if (!startDate.HasValue)
                {
                    SetDateInComboBoxes(endDate.Value, endDate.Value);
                }
            }
        }
        private DateTime? GetDateFromComboBoxes(ComboBox dayCombo, ComboBox monthCombo, ComboBox yearCombo)
        {
            if (dayCombo.SelectedItem != null && monthCombo.SelectedItem != null && yearCombo.SelectedItem != null)
            {
                try
                {
                    int day = (int)dayCombo.SelectedItem;
                    int month = ((dynamic)monthCombo.SelectedItem).Value;
                    int year = (int)yearCombo.SelectedItem;

                    // Проверяем валидность даты (например, 30 февраля)
                    if (DateTime.DaysInMonth(year, month) >= day)
                    {
                        return new DateTime(year, month, day);
                    }
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }
        private DateTime? GetStartDateFromComboBoxes()
        {
            return GetDateFromComboBoxes(StartDayComboBox, StartMonthComboBox, StartYearComboBox);
        }

        private void SetDateInComboBoxes(DateTime startDate, DateTime endDate)
        {
            // Устанавливаем начальную дату
            StartDayComboBox.SelectedItem = startDate.Day;
            StartMonthComboBox.SelectedIndex = startDate.Month - 1;
            StartYearComboBox.SelectedItem = startDate.Year;

            // Устанавливаем конечную дату
            EndDayComboBox.SelectedItem = endDate.Day;
            EndMonthComboBox.SelectedIndex = endDate.Month - 1;
            EndYearComboBox.SelectedItem = endDate.Year;
        }
        private void UpdateCalendarDisplay(DateTime date)
        {
            _currentCalendarDate = date;

            // Обновляем отображение месяца и года
            CurrentMonthYear.Text = date.ToString("MMMM yyyy", new CultureInfo("ru-RU"));

            // Генерируем дни календаря
            GenerateCalendarDays(date);
        }
        private void GenerateCalendarDays(DateTime date)
        {
            var calendarDays = new List<CalendarDay>();

            // Первый день месяца
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            // Последний день месяца
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            // День недели первого дня месяца (1 - понедельник, 7 - воскресенье)
            int firstDayOfWeek = ((int)firstDayOfMonth.DayOfWeek == 0) ? 7 : (int)firstDayOfMonth.DayOfWeek;

            // Добавляем дни из предыдущего месяца
            var previousMonth = firstDayOfMonth.AddMonths(-1);
            var daysInPreviousMonth = DateTime.DaysInMonth(previousMonth.Year, previousMonth.Month);

            for (int i = daysInPreviousMonth - firstDayOfWeek + 2; i <= daysInPreviousMonth; i++)
            {
                calendarDays.Add(new CalendarDay
                {
                    Day = i.ToString(),
                    TextColor = "#adb5bd", // Серый цвет для дней другого месяца
                    FontWeight = "Normal",
                    BackgroundColor = "Transparent"
                });
            }

            // Добавляем дни текущего месяца
            for (int i = 1; i <= lastDayOfMonth.Day; i++)
            {
                var currentDay = new DateTime(date.Year, date.Month, i);
                bool isSelected = IsDateInSelectedRange(currentDay);
                bool isToday = currentDay.Date == DateTime.Today;

                calendarDays.Add(new CalendarDay
                {
                    Day = i.ToString(),
                    TextColor = isSelected ? "White" : (isToday ? "#0167a4" : "#495057"),
                    FontWeight = isToday ? "Bold" : "Normal",
                    BackgroundColor = isSelected ? "#0167a4" : "Transparent",
                    Date = currentDay
                });
            }

            // Добавляем дни следующего месяца чтобы заполнить сетку
            int totalCells = 42; // 6 строк × 7 колонок
            int nextMonthDay = 1;

            while (calendarDays.Count < totalCells)
            {
                calendarDays.Add(new CalendarDay
                {
                    Day = nextMonthDay.ToString(),
                    TextColor = "#adb5bd", // Серый цвет для дней другого месяца
                    FontWeight = "Normal",
                    BackgroundColor = "Transparent"
                });
                nextMonthDay++;
            }

            CalendarDays.ItemsSource = calendarDays;
        }
        private DateTime? GetEndDateFromComboBoxes()
        {
            return GetDateFromComboBoxes(EndDayComboBox, EndMonthComboBox, EndYearComboBox);
        }
        private bool IsDateInSelectedRange(DateTime date)
        {
            var startDate = GetStartDateFromComboBoxes();
            var endDate = GetEndDateFromComboBoxes();

            if (startDate.HasValue && endDate.HasValue)
            {
                return date.Date >= startDate.Value.Date && date.Date <= endDate.Value.Date;
            }
            else if (startDate.HasValue && !endDate.HasValue)
            {
                // Если выбрана только начальная дата - подсвечиваем только её
                return date.Date == startDate.Value.Date;
            }

            return false;
        }
        private void CalendarDay_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is CalendarDay day && day.Date.HasValue)
            {
                var clickedDate = day.Date.Value;

                // Если выбранная дата из другого месяца - переключаем отображение
                if (clickedDate.Month != _currentCalendarDate.Month || clickedDate.Year != _currentCalendarDate.Year)
                {
                    _currentCalendarDate = new DateTime(clickedDate.Year, clickedDate.Month, 1);
                }

                var currentStartDate = GetStartDateFromComboBoxes();
                var currentEndDate = GetEndDateFromComboBoxes();

                // Если даты не выбраны или выбраны обе - начинаем новый период
                if (!currentStartDate.HasValue || !currentEndDate.HasValue ||
                    (currentStartDate.HasValue && currentEndDate.HasValue))
                {
                    // Начинаем новый период (один день)
                    SetDateInComboBoxes(clickedDate, clickedDate);
                }
                else if (currentStartDate.HasValue && !currentEndDate.HasValue)
                {
                    // Продолжаем выбор периода - устанавливаем конечную дату
                    if (clickedDate < currentStartDate.Value)
                    {
                        // Если кликнули дату раньше начала - меняем начало
                        SetDateInComboBoxes(clickedDate, clickedDate);
                    }
                    else
                    {
                        // Если кликнули дату позже начала - устанавливаем конец
                        SetDateInComboBoxes(currentStartDate.Value, clickedDate);
                    }
                }

                // Обновляем отображение календаря
                UpdateCalendarDisplay(_currentCalendarDate);
            }
        }

        private void PreviousMonth_Click(object sender, RoutedEventArgs e)
        {
            // Переход к предыдущему месяцу
            _currentCalendarDate = _currentCalendarDate.AddMonths(-1);
            UpdateCalendarDisplay(_currentCalendarDate);
        }
        private void NextMonth_Click(object sender, RoutedEventArgs e)
        {
            // Переход к следующему месяцу
            _currentCalendarDate = _currentCalendarDate.AddMonths(1);
            UpdateCalendarDisplay(_currentCalendarDate);
        }
        private void PeriodSelector_Click(object sender, RoutedEventArgs e)
        {
            OpenCalendarPopup();
        }

        private void OpenCalendarPopup()
        {
            // Устанавливаем текущие даты из отображаемого периода в комбобоксы
            var currentText = DateRangeText.Text;
            var yearText = YearText.Text;

            if (!string.IsNullOrEmpty(currentText) && currentText.Contains(" - "))
            {
                var parts = currentText.Split(new[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    DateTime startDate, endDate;

                    // Если год один
                    if (int.TryParse(yearText, out int currentYear))
                    {
                        if (DateTime.TryParseExact(parts[0].Trim() + "." + currentYear, "dd.MM.yyyy",
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate) &&
                            DateTime.TryParseExact(parts[1].Trim() + "." + currentYear, "dd.MM.yyyy",
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
                        {
                            SetDateInComboBoxes(startDate, endDate);
                        }
                    }
                }
            }

            // Обновляем отображение календаря
            UpdateCalendarDisplay(_currentCalendarDate);

            // Открываем popup
            CalendarPopup.IsOpen = true;
        }
        private void ApplyDateRange_Click(object sender, RoutedEventArgs e)
        {
            var startDate = GetStartDateFromComboBoxes();
            var endDate = GetEndDateFromComboBoxes();

            if (startDate.HasValue && endDate.HasValue)
            {
                // Проверяем, чтобы начальная дата была раньше конечной
                if (startDate > endDate)
                {
                    MessageBox.Show("Начальная дата не может быть позже конечной даты", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                UpdateDateDisplay(startDate.Value, endDate.Value);
                CalendarPopup.IsOpen = false;
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите обе даты", "Внимание",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        private void CancelDateRange_Click(object sender, RoutedEventArgs e)
        {
            CalendarPopup.IsOpen = false;
        }
        private string FormatDateForCalendar(DateTime date)
        {
            return $"{date:dd MM yy}"; // Формат "27 10 25"
        }
        private void UpdateDateDisplay(DateTime startDate, DateTime endDate)
        {
            // Обновляем отображение периода
            DateRangeText.Text = $"{startDate:dd.MM} - {endDate:dd.MM}";

            // Определяем отображение года
            if (startDate.Year == endDate.Year)
            {
                // Если годы одинаковые - показываем один год
                YearText.Text = startDate.Year.ToString();
            }
            else
            {
                // Если годы разные - показываем оба года через дефис
                YearText.Text = $"{startDate.Year}-{endDate.Year}";
            }

            OnDateRangeChanged(startDate, endDate);
        }
        private bool TryParseCalendarDate(string dateText, out DateTime result)
        {
            // Парсим дату из формата "27 10 25"
            if (DateTime.TryParseExact(dateText, "dd MM yy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                return true;
            }

            // Альтернативные форматы, если основной не сработал
            return DateTime.TryParse(dateText, out result);
        }
        private void OnDateRangeChanged(DateTime startDate, DateTime endDate)
        {
            // Здесь можно добавить логику обновления данных при изменении периода
            Console.WriteLine($"Период изменен: {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}");

            // Обновление заголовка окна с информацией о периоде
            if (startDate.Year == endDate.Year)
            {
                this.Title = $"Project BPI - Период: {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}";
            }
            else
            {
                this.Title = $"Project BPI - Период: {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy} ({startDate.Year}-{endDate.Year})";
            }
        }

        // Остальной существующий код остается без изменений
        private void NIR_Header_Click(object sender, MouseButtonEventArgs e)
        {
            NIR_Menu.Visibility = NIR_Menu.Visibility == Visibility.Visible
                ? Visibility.Collapsed
                : Visibility.Visible;
            MenuItem_Click(sender, e);
        }

        private void Section1_Header_Click(object sender, MouseButtonEventArgs e)
        {
            Section1_Menu.Visibility = Section1_Menu.Visibility == Visibility.Visible
                ? Visibility.Collapsed
                : Visibility.Visible;
            MenuItem_Click(sender, e);
        }

        Border currentActive = null;

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield break;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                if (child == null) continue;

                if (child is T t) yield return t;

                foreach (T childOfChild in FindVisualChildren<T>(child))
                    yield return childOfChild;
            }
        }

        private void ResetTextBlocksForeground(Border border)
        {
            if (border == null) return;

            foreach (TextBlock tb in FindVisualChildren<TextBlock>(border))
            {
                tb.ClearValue(TextBlock.ForegroundProperty);
            }
        }

        private void SetTextBlocksForeground(Border border, Brush brush)
        {
            if (border == null) return;

            foreach (TextBlock tb in FindVisualChildren<TextBlock>(border))
            {
                tb.Foreground = brush;
            }
        }

        private void MenuItem_Click(object sender, MouseButtonEventArgs e)
        {

            ResetAllStyles();

            Border b = sender as Border;
            if (b == null) return;

            HighlightChain(b);

            currentActive = b;

            ShowContentForMenuItem(b);
            UpdateArrows();


        }
        private void ResetAllStyles()
        {
            foreach (var border in FindVisualChildren<Border>(this))
            {
                string tag = border.Tag as string;

                if (tag == "main" || tag == "sub" || tag == "sub2")
                {
                    border.Style = (Style)FindResource("MenuItemStyle");
                    ResetTextBlocksForeground(border);
                }
            }
        }
        private void ApplyActiveStyle(Border b)
        {
            string tag = b.Tag as string;

            switch (tag)
            {
                case "main":
                    b.Style = (Style)FindResource("ActiveMainItemStyle");
                    break;
                case "sub":
                    b.Style = (Style)FindResource("ActiveSubItemStyle");
                    break;
                case "sub2":
                    b.Style = (Style)FindResource("ActiveSubItemLevel2Style");
                    break;
            }

            SetTextBlocksForeground(b, Brushes.White);
        }

        private void ShowContentForMenuItem(Border menuItem)
        {
            if (menuItem == Item_11)
            {
                MainContentControl.Content = new Item11View();
            }
            else if (menuItem == Item_12)
            {
                MainContentControl.Content = new ContractResearchView();
            }
            else if (menuItem == Item_13)
            {
                MainContentControl.Content = new Item_13View();
            }
            else if (menuItem == Item_15)
            {
                MainContentControl.Content = new СooperationProductionView();
            }
            else if (menuItem == Item_Archive)
            {
                MainContentControl.Content = new ArchivePage();
            }
            else if (menuItem == Item_Templates)
            {
                MainContentControl.Content = new TemplatesPage(); 
            }
            else
            {
                MainContentControl.Content = new TextBlock
                {
                    Text = "Основная область контента",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = Brushes.Gray,
                    FontSize = 16
                };
            }
        }
        private void Item_11_Click(object sender, MouseButtonEventArgs e)
        {
            MenuItem_Click(sender, e);
        }
        private void Item_12_Click(object sender, MouseButtonEventArgs e)
        {
            MenuItem_Click(sender, e);
        }

        private void Item_13_Click(object sender, MouseButtonEventArgs e)
        {
            MenuItem_Click(sender, e);
        }
        private void Item_15_Click(object sender, MouseButtonEventArgs e)
        {
            MenuItem_Click(sender, e);
        }
        private void Item_Archive_Click(object sender, MouseButtonEventArgs e)
        {
            MenuItem_Click(sender, e);
        }

        private void HighlightChain(Border start)
        {
            Border current = start;

            while (current != null)
            {
                ApplyActiveStyle(current);

                if (parents.ContainsKey(current))
                    current = parents[current];
                else
                    break;
            }
        }
        private bool IsActive(Border b)
        {
            return b.Style == (Style)FindResource("ActiveMainItemStyle")
                || b.Style == (Style)FindResource("ActiveSubItemStyle")
                || b.Style == (Style)FindResource("ActiveSubItemLevel2Style");
        }
        private void UpdateArrows()
        {
            // Стрелка у "Отчет по НИР"
            if (IsActive(NIR_Header))
                NIR_Arrow.Visibility = Visibility.Collapsed;
            else
                NIR_Arrow.Visibility = Visibility.Visible;

            // Стрелка у "Раздел 1"
            if (IsActive(Section1_Header))
                NIR_Arrow1.Visibility = Visibility.Collapsed;
            else
                NIR_Arrow1.Visibility = Visibility.Visible;
        }
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Placeholder
            SearchPlaceholder.Visibility = string.IsNullOrEmpty(SearchBox.Text) ? Visibility.Visible : Visibility.Collapsed;

            string query = SearchBox.Text.ToLower();

            // Фильтруем меню
            FilterMenuPanel(NIR_Menu, query);

            // Фильтруем остальные пункты
            FilterMenuItem(Item_Study, query);
            FilterMenuItem(Item_Archive, query);
            FilterMenuItem(Item_Templates, query);
        }

        private void FilterMenuPanel(StackPanel panel, string query)
        {
            foreach (var child in panel.Children)
            {
                if (child is Border border)
                {
                    FilterMenuItem(border, query);

                    // Если есть подменю, фильтруем рекурсивно
                    StackPanel subPanel = null;
                    if (border.Name == "Section1_Header") subPanel = Section1_Menu;
                    else if (border.Name == "NIR_Header") subPanel = NIR_Menu;

                    if (subPanel != null)
                    {
                        FilterMenuPanel(subPanel, query);

                        // Показываем родителя, если есть видимые дети или совпадение
                        bool hasVisibleChild = subPanel.Children.Cast<UIElement>().Any(c => c.Visibility == Visibility.Visible);
                        border.Visibility = border.Visibility == Visibility.Visible || hasVisibleChild
                            ? Visibility.Visible
                            : Visibility.Collapsed;
                    }
                }
                else if (child is StackPanel sp)
                {
                    FilterMenuPanel(sp, query);
                }
            }
        }
        private void FilterMenuItem(Border border, string query)
        {
            TextBlock tb = null;
            if (border.Child is StackPanel sp)
                tb = sp.Children.OfType<TextBlock>().FirstOrDefault();
            else
                tb = border.Child as TextBlock;

            if (tb != null)
            {
                bool match = string.IsNullOrEmpty(query) || tb.Text.ToLower().Contains(query);
                border.Visibility = match ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}