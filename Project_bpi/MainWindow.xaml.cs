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
        public MainWindow()
        {
            InitializeComponent();
            InitializeDateRange();
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
        }

        private void Section1_Header_Click(object sender, MouseButtonEventArgs e)
        {
            Section1_Menu.Visibility = Section1_Menu.Visibility == Visibility.Visible
                ? Visibility.Collapsed
                : Visibility.Visible;
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
            if (currentActive != null)
            {
                currentActive.Style = (Style)FindResource("MenuItemStyle");
                ResetTextBlocksForeground(currentActive);

                var previousArrow = currentActive.FindName("NIR_Arrow") as Image;
                if (previousArrow != null) previousArrow.Visibility = Visibility.Visible;
            }

            Border b = sender as Border;
            if (b == null) return;

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
                default:
                    b.Style = (Style)FindResource("ActiveMainItemStyle");
                    break;
            }

            SetTextBlocksForeground(b, Brushes.White);

            var arrow = b.FindName("NIR_Arrow") as Image;
            if (arrow != null) arrow.Visibility = Visibility.Collapsed;

            currentActive = b;

            ShowContentForMenuItem(b);
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
    }
}