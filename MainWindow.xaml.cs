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
using AutoLotModel;
using System.Data.Entity;
using System.Data;

namespace ionita_andreea_lab5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    enum ActionState
    {
        New,
        Edit,
        Delete,
        Nothing
    }
    public partial class MainWindow : Window
    {
        ActionState action = ActionState.Nothing;

        AutoLotEntitiesModel ctx = new AutoLotEntitiesModel();

        CollectionViewSource customerVSource;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
            customerVSource =
           ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));
            customerVSource.Source = ctx.Customers.Local;
            ctx.Customers.Load();
        }
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
        }
        private void btnEditO_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
        }
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            customerVSource.View.MoveCurrentToNext();
        }
        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            customerVSource.View.MoveCurrentToPrevious();
        }
        private void SaveCustomers()
        {
            Customer customer = null;
            if (action == ActionState.New)
            {
                try
                {
                    //instantiem Customer entity
                    customer = new Customer()
                    {
                        FirstName = fNameTextBox.Text.Trim(),
                        LastName = lNameTextBox.Text.Trim()
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Customers.Add(customer);
                    customerVSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
               
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
           if (action == ActionState.Edit)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    customer.FirstName = firstNameTextBox.Text.Trim();
                    customer.LastName = lastNameTextBox.Text.Trim();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    ctx.Customers.Remove(customer);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerVSource.View.Refresh();
            }

        }


    }
    private void SetValidationBinding()
    {
        Binding firstNameValidationBinding = new Binding();
        firstNameValidationBinding.Source = customerVSource;
        firstNameValidationBinding.Path = new PropertyPath("FirstName");
        firstNameValidationBinding.NotifyOnValidationError = true;
        firstNameValidationBinding.Mode = BindingMode.TwoWay;
        firstNameValidationBinding.UpdateSourceTrigger =
       UpdateSourceTrigger.PropertyChanged;
        //string required
        firstNameValidationBinding.ValidationRules.Add(new StringNotEmpty());
        firstNameTextBox.SetBinding(TextBox.TextProperty,
       firstNameValidationBinding);
        Binding lastNameValidationBinding = new Binding();
        lastNameValidationBinding.Source = customerVSource;
        lastNameValidationBinding.Path = new PropertyPath("LastName");
        lastNameValidationBinding.NotifyOnValidationError = true;
        lastNameValidationBinding.Mode = BindingMode.TwoWay;
        lastNameValidationBinding.UpdateSourceTrigger =
       UpdateSourceTrigger.PropertyChanged;
        //string min length validator
        lastNameValidationBinding.ValidationRules.Add(new
       StringMinLengthValidator());
        lastNameTextBox.SetBinding(TextBox.TextProperty,
       lastNameValidationBinding); //setare binding nou
    }
    private void btnEdit_Click(object sender, RoutedEventArgs e)
    {
        action = ActionState.Edit;
        BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
        BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
        SetValidationBinding();
    }
}
