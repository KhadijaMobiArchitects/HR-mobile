using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace XForms.Controls
{
    public partial class MaterialDatePicker : StackLayout
    {
        public static readonly BindableProperty TitleProperty =
BindableProperty.Create(nameof(Title), typeof(string), typeof(View), string.Empty, BindingMode.TwoWay);

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set
            {
                SetValue(TitleProperty, value);
            }
        }

        public static readonly BindableProperty DateProperty =
BindableProperty.Create(nameof(Date), typeof(DateTime), typeof(View),null, BindingMode.TwoWay);

        public DateTime? Date
        {
            get { return (DateTime)GetValue(DateProperty); }
            set
            {
                SetValue(DateProperty, value);
            }
        }

        private bool IsSelecetedDate;
        public string DateString => IsSelecetedDate ? Date.Value.ToShortDateString() : string.Empty;

        public static readonly BindableProperty PlaceholderProperty =
BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(View), string.Empty, BindingMode.TwoWay);

        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set
            {
                SetValue(PlaceholderProperty, value);
            }
        }

        public DateTime? defaultDate=null;

        public MaterialDatePicker()
        {


            InitializeComponent();
            myDatePicker.Date = default;
            defaultDate = myDatePicker.Date;

            myDatePicker.MinimumDate = DateTime.Now;
            myDatePicker.MaximumDate = DateTime.Now.AddYears(1);

            //myDatePicker. = new System.Globalization.CultureInfo("fr-FR");

            //Date = null;
        }

        void DatePicker_DateSelected(System.Object sender, Xamarin.Forms.DateChangedEventArgs e)
        {
            //var s = new DatePicker() { Date = default};
            
            if ((e.NewDate.Date != defaultDate) && (defaultDate != null) && (e.NewDate.Date.ToShortDateString() != DateTime.Now.ToShortDateString()))
            {
            Date = e.NewDate;
                IsSelecetedDate = true;
                OnPropertyChanged(nameof(DateString));
            }
          
        }

        //void CustomButton_Clicked(System.Object sender, System.EventArgs e)
        //{
        //    DatePicker.Focus();
        //}
    }
}
