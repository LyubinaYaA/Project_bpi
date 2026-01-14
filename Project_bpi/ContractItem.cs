using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Project_bpi
{
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
