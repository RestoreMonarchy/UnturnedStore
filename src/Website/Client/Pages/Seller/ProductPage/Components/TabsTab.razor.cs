using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.JSInterop;
using Blazored.TextEditor;
using Website.Shared.Models.Database;

namespace Website.Client.Pages.Seller.ProductPage.Components
{
    public partial class TabsTab
    {
        [Parameter]
        public MProduct Product { get; set; }

        [Inject]
        public HttpClient HttpClient { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        private MProductTab Tab { get; set; }

        private BlazoredTextEditor editor;
        private string htmlString => Tab.Content;
        private void EditTab(MProductTab tab)
        {
            Tab = tab;
        }

        private void CreateTab()
        {
            Tab = new MProductTab()
            {IsEnabled = true};
        }

        private bool isLoading2;
        private async Task DeleteAsync()
        {
            isLoading2 = true;
            await HttpClient.DeleteAsync($"api/products/tabs/{Tab.Id}");
            Product.Tabs.Remove(Tab);
            Tab = null;
            isLoading2 = false;
        }

        private bool isLoading;
        private async Task SubmitAsync()
        {
            isLoading = true;
            Tab.Content = await editor.GetHTML();
            if (Tab.Id == default)
            {
                Tab.ProductId = Product.Id;
                var response = await HttpClient.PostAsJsonAsync("api/products/tabs", Tab);
                Product.Tabs.Add(await response.Content.ReadFromJsonAsync<MProductTab>());
                Tab = new MProductTab();
            }
            else
            {
                await HttpClient.PutAsJsonAsync("api/products/tabs", Tab);
            }

            Tab = null;
            isLoading = false;
        }
    }
}