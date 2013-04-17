using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;


namespace atelier5.ViewModel
{
    public class FilteredListViewModel : INotifyPropertyChanged
    {
        private int _selectedFilter = 0;
        private readonly string[] _filters;
        private Atelier5.Model.EntrepriseEntities _context;
        public FilteredListViewModel(Atelier5.Model.EntrepriseEntities context)
        {
            _context = context;
            _filters = "Tout le staff,10$ et moins".Split(',');
        }
        public IEnumerable<object> FilteredList
        {
            get
            {
                switch (this._selectedFilter)
                {
                    case 0:
                        return from employee in _context.Employees
                               select new
                               {
                                   Prénom = employee.First_Name,
                                   Nom = employee.Last_Name
                               };
                    case 1:
                        return from product in _context.Products
                               where product.Unit_Price < 10.0m
                               select new
                               {
                                   Produit = product.Product_Name,
                                   Prix = product.Unit_Price
                               };
                    case 2:
                        return from employee in _context.Employees
                               where employee.Birth_Date.Value.Month == 1
                               select new
                               {
                                   JourDateAnniversaire = employee.Birth_Date.Value.Day,
                                   Age = DateTime.Now.Year - employee.Birth_Date.Value.Year
                               };
                    case 3:
                        return from employee in _context.Employees
                               let Age = DateTime.Now.Year - employee.Birth_Date.Value.Year
                               orderby Age
                               select new
                               {
                                   Age
                               };

                    case 4:
                        return from customers in _context.Customers
                               where customers.Country == "France"
                               select new
                               {
                                   NameCompany = customers.Company_Name,
                                   OrdersCount = customers.Orders.Count
                               };
                    case 5:
                        return from categories in _context.Categories
                               select new
                               {
                                   average = categories.Products.Average(unProduit => unProduit.Unit_Price)

                               };
                    case 6:
                        return _context.Orders;

                    default:
                        return new string[] {
                            "(Not implemented filter)"
                        };
                }
            }
        }
        public IEnumerable<String> Filters
        {
            get { return _filters; }
        }
        public int SelectedFilter
        {
            get { return this._selectedFilter; }
            set
            {
                this._selectedFilter = value;
                if (PropertyChanged != null)
                    PropertyChanged(this,
                    new PropertyChangedEventArgs("FilteredList")
                    );
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public partial class Order
    {
        private Decimal _amount;

        public decimal amount
        {
            get { return this._amount; }
            set { _amount = value; }

        }


        public Order(int orderId)
        {
            //InitializeComponent();
            Atelier5.Model.EntrepriseEntities context = new Atelier5.Model.EntrepriseEntities();
            var unAmount = from ordersDetails in context.Order_Details
                   where ordersDetails.Order_ID == orderId
                   select ((ordersDetails.Quantity * ordersDetails.Unit_Price) + ordersDetails.Orders.Freight);
            _amount = unAmount.First().Value;
        }
    }
}