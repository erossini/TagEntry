using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using WordBankEasy.Models;
using WordBankEasy.Views;
using WordBankEasy.ViewModels;

namespace WordBankEasy.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class ItemsPage : ContentPage
    {
        ItemsViewModel viewModel;

        public List<string> mySeparators = new List<string>() { "*" };

        public ItemsPage()
        {
            InitializeComponent();

            tagEntryEnd.TagValidatorFactory = new Func<string, object>(
                            (arg) => viewModel?.ValidateAndReturn2(arg));
            tagEntryStart.TagValidatorFactory = new Func<string, object>(
                          (arg) => viewModel?.ValidateAndReturn(arg));

            tagEntryStart.TagSeparators = mySeparators;

            BindingContext = viewModel = new ItemsViewModel();
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as Item;
            if (item == null)
                return;

            await Navigation.PushAsync(new ItemDetailPage(new ItemDetailViewModel(item)));

            // Manually deselect item.
            //ItemsListView.SelectedItem = null;
        }

        async void AddItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new NewItemPage()));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);
        }
    }
}