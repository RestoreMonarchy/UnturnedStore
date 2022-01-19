﻿using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Website.Shared.Models.Database;

namespace Website.Client.Pages.User.LicensesPage
{
    public partial class LicensesPage
    {
        [Inject]
        public HttpClient HttpClient { get; set; }

        public IEnumerable<MProductCustomer> Customers { get; set; }
        public IEnumerable<MProductCustomer> OrderedCustomers => Customers.OrderByDescending(x => x.CreateDate);

        private List<int> showLicenses = new List<int>();

        protected override async Task OnInitializedAsync()
        {
            Customers = await HttpClient.GetFromJsonAsync<List<MProductCustomer>>("api/products/my");
        }

        public void ShowLicense(int customerId)
        {
            showLicenses.Add(customerId);
        }
    }
}
