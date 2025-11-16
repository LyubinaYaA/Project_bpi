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


namespace Project_bpi
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
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


        // Универсальный итератор по визуальному дереву для поиска TextBlock внутри Border
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

        // Сброс локального Foreground у всех TextBlock в Border (возврат к стилю)
        private void ResetTextBlocksForeground(Border border)
        {
            if (border == null) return;

            foreach (TextBlock tb in FindVisualChildren<TextBlock>(border))
            {
                // удаляем локальное значение Foreground — тогда TextBlock снова использует своё style.Foreground
                tb.ClearValue(TextBlock.ForegroundProperty);
            }
        }

        // Установка цвета текста (например, белого) для всех TextBlock в Border
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
                // Сброс стиля активного
                currentActive.Style = (Style)FindResource("MenuItemStyle");

                // Сброс текста к исходному цвету
                ResetTextBlocksForeground(currentActive);

                // Восстановление стрелки у предыдущего элемента
                var previousArrow = currentActive.FindName("NIR_Arrow") as Image;
                if (previousArrow != null) previousArrow.Visibility = Visibility.Visible;
            }

            Border b = sender as Border;
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

            // Меняем цвет текста всех TextBlock внутри активного элемента
            SetTextBlocksForeground(b, Brushes.White);

            // Скрываем стрелку, если она есть
            var arrow = b.FindName("NIR_Arrow") as Image;
            if (arrow != null) arrow.Visibility = Visibility.Collapsed;

            currentActive = b;
        }



    }
}
