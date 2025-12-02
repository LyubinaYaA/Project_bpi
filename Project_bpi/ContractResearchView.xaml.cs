using System.Windows;
using System.Windows.Controls;

namespace Project_bpi
{
    public partial class ContractResearchView : UserControl
    {
        private bool _sortButtonsVisible = false;

        public ContractResearchView()
        {
            InitializeComponent();
            FilterButton.Click += OnFilterButtonClick;
            SetupSortButtonEvents();
        }

        private void OnFilterButtonClick(object sender, RoutedEventArgs e)
        {
            _sortButtonsVisible = !_sortButtonsVisible;
            var visibility = _sortButtonsVisible ? Visibility.Visible : Visibility.Collapsed;

            SortCol0.Visibility = visibility;
            SortCol1.Visibility = visibility;
            SortCol2.Visibility = visibility;
            SortCol3.Visibility = visibility;
            SortCol4.Visibility = visibility; // Объединённая колонка 4-5
            SortCol6.Visibility = visibility;
            // SortCol5 — НЕ СУЩЕСТВУЕТ
        }

        private void SetupSortButtonEvents()
        {
            SortCol0.Click += (s, e) => SortColumn(0);
            SortCol1.Click += (s, e) => SortColumn(1);
            SortCol2.Click += (s, e) => SortColumn(2);
            SortCol3.Click += (s, e) => SortColumn(3);
            SortCol4.Click += (s, e) => SortColumn(4); // Сортировка по "Фактическое выполнение"
            SortCol6.Click += (s, e) => SortColumn(6);
        }

        private void SortColumn(int columnIndex)
        {
            // Реализуйте логику сортировки по columnIndex
            // Например: 4 — может означать сортировку по "Актам" или общей сумме
            MessageBox.Show($"Сортировка по колонке {columnIndex + 1}");
        }
    }
}