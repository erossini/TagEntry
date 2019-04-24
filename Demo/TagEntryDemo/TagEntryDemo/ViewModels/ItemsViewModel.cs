using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using TagEntryDemo.Models;
using TagEntryDemo.Views;
using PSC.Xam.Components.TagEntry.Models;
using System.Linq;
using System.Windows.Input;

namespace TagEntryDemo.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        public ObservableCollection<TagItem> WordEndItems { get; set; }
        public ObservableCollection<TagItem> WordStartItems { get; set; }
        public ObservableCollection<Item> Items { get; set; }
        public Command LoadItemsCommand { get; set; }

        public ItemsViewModel()
        {
            Title = "Browse";
            Items = new ObservableCollection<Item>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            WordEndItems = new ObservableCollection<TagItem>();
            WordStartItems = new ObservableCollection<TagItem>();

            MessagingCenter.Subscribe<NewItemPage, Item>(this, "AddItem", async (obj, item) =>
            {
                var newItem = item as Item;
                Items.Add(newItem);
                await DataStore.AddItemAsync(newItem);
            });
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await DataStore.GetItemsAsync(true);
                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        #region Tags
        public TagItem ValidateAndReturn(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return null;

            if (WordStartItems != null)
                if (WordStartItems.Any(v => v.Name.Equals(tag, StringComparison.OrdinalIgnoreCase)))
                    return null;

            OnPropertyChanged("WordStartItemCount");
            return new TagItem()
            {
                Name = tag
            };
        }

        public TagItem ValidateAndReturn2(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return null;

            if (WordEndItems != null)
                if (WordEndItems.Any(v => v.Name.Equals(tag, StringComparison.OrdinalIgnoreCase)))
                    return null;

            OnPropertyChanged("WordEndItemCount");
            return new TagItem()
            {
                Name = tag
            };
        }
        #endregion

        public int WordStartItemCount
        {
            get {
                if (WordStartItems != null)
                    return WordStartItems.Count;

                return 0;
            }
        }

        ICommand _removeTagCommand = null;
        public ICommand RemoveTagCommand
        {
            get {
                return _removeTagCommand ?? (_removeTagCommand =
                    new Command((tag) => RemoveTag((TagItem)tag)));
            }
        }

        public void RemoveTag(TagItem tagItem)
        {
            if (tagItem == null)
                return;

            WordStartItems.Remove(tagItem);
            OnPropertyChanged("WordStartItemCount");
        }
    }
}