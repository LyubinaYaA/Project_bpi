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
        private void InitializeDateRange()
        {
            // Устанавливаем начальный период - текущий год
            var today = DateTime.Today;
            var startDate = new DateTime(today.Year, 1, 1);
            var endDate = new DateTime(today.Year, 12, 31);

            UpdateDateDisplay(startDate, endDate);

            // Устанавливаем даты в календари
            StartDatePicker.SelectedDate = startDate;
            EndDatePicker.SelectedDate = endDate;
        }

        private void PeriodSelector_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenCalendarPopup();
        }

        private void OpenCalendarPopup()
        {
            // Устанавливаем текущие даты из отображаемого периода
            var currentText = DateRangeText.Text;
            var yearText = YearText.Text;

            if (!string.IsNullOrEmpty(currentText) && currentText.Contains(" - "))
            {
                var parts = currentText.Split(new[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    // Если годы разные (формат "2024-2025")
                    if (yearText.Contains("-"))
                    {
                        var yearParts = yearText.Split('-');
                        if (yearParts.Length == 2 && int.TryParse(yearParts[0], out int startYear))
                        {
                            if (DateTime.TryParseExact(parts[0].Trim() + "." + startYear, "dd.MM.yyyy",
                                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate))
                            {
                                StartDatePicker.SelectedDate = startDate;
                            }
                        }
                        if (yearParts.Length == 2 && int.TryParse(yearParts[1], out int endYear))
                        {
                            if (DateTime.TryParseExact(parts[1].Trim() + "." + endYear, "dd.MM.yyyy",
                                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate))
                            {
                                EndDatePicker.SelectedDate = endDate;
                            }
                        }
                    }
                    else
                    {
                        // Если год один
                        if (int.TryParse(yearText, out int currentYear))
                        {
                            if (DateTime.TryParseExact(parts[0].Trim() + "." + currentYear, "dd.MM.yyyy",
                                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate))
                            {
                                StartDatePicker.SelectedDate = startDate;
                            }

                            if (DateTime.TryParseExact(parts[1].Trim() + "." + currentYear, "dd.MM.yyyy",
                                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate))
                            {
                                EndDatePicker.SelectedDate = endDate;
                            }
                        }
                    }
                }
            }

            // Открываем popup
            CalendarPopup.IsOpen = true;

            // Фокусируем первый календарь и открываем его dropdown
            Dispatcher.BeginInvoke(new Action(() =>
            {
                StartDatePicker.Focus();
                StartDatePicker.IsDropDownOpen = true;
            }), System.Windows.Threading.DispatcherPriority.Render);
        }

        private void ApplyDateRange_Click(object sender, RoutedEventArgs e)
        {
            if (StartDatePicker.SelectedDate.HasValue && EndDatePicker.SelectedDate.HasValue)
            {
                var startDate = StartDatePicker.SelectedDate.Value;
                var endDate = EndDatePicker.SelectedDate.Value;

                // Проверяем, чтобы начальная дата была раньше конечной
                if (startDate > endDate)
                {
                    MessageBox.Show("Начальная дата не может быть позже конечной даты", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                UpdateDateDisplay(startDate, endDate);
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
            if (menuItem == Item_12)
            {
                MainContentControl.Content = new ContractResearchView();
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

        private void Item_12_Click(object sender, MouseButtonEventArgs e)
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