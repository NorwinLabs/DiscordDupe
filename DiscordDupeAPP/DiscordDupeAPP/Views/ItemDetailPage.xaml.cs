using DiscordDupeAPP.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace DiscordDupeAPP.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}