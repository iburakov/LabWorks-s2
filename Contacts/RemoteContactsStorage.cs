using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Contacts.CommandLine;

namespace Contacts {
    class RemoteContactsStorage : IContactsStorage {
        private const string defaultScheme = "https";

        public Uri BaseUri { get; private set; }
        private readonly HttpClient httpClient;

        public RemoteContactsStorage(string hostBaseUrl) {
            httpClient = new HttpClient {
                Timeout = new TimeSpan(0, 0, 15)
            };

            try {
                BaseUri = new Uri(hostBaseUrl, UriKind.Absolute);
                if (BaseUri.Scheme != "http" && BaseUri.Scheme != "https") {
                    BaseUri = new Uri($"{defaultScheme}://{hostBaseUrl}", UriKind.Absolute);
                    if (BaseUri.Scheme != "http" && BaseUri.Scheme != "https") {
                        throw new UriFormatException();
                    }
                }
            } catch (UriFormatException) {
                throw new ArgumentException("Invalid URL was given. Format: http[s]://host[:port]");
            }
            
        }

        private void DoHttpReqeust(Task<HttpResponseMessage> requestTask, out string response) {
            int checks = 0;
            int delay = 25;
            while (!requestTask.IsCompleted) {
                if (checks++ > 4) {
                    Console.Write(".");
                }
                Thread.Sleep(delay += 25);
            }
            if (checks > 5) {
                Console.WriteLine();
            }

            HttpResponseMessage httpResponse = requestTask.Result.EnsureSuccessStatusCode();

            Task<string> readTask = httpResponse.Content.ReadAsStringAsync();
            while (!readTask.IsCompleted) {
                Thread.Sleep(50);
            }
            response = readTask.Result;
        }

        public void AddContact(Contact newContact, out string message) {
            HttpContent requestContent = new MultipartFormDataContent {
                { new StringContent(newContact.ToVCard()), "contact" }
            };

            Console.WriteLine($"Sending a new contact to {BaseUri}");
            DoHttpReqeust(httpClient.PostAsync(new Uri(BaseUri, "/api/addContact"), requestContent), out message);
            message = $"[@{BaseUri}] " + message;
        }

        public ReadOnlyCollection<Contact> GetAllContacts() {
            Console.WriteLine($"Getting all contacts from {BaseUri}");
            DoHttpReqeust(httpClient.GetAsync(new Uri(BaseUri, "/api/getAllContacts")), out string response);

            List<Contact> parsed = Contact.ParseMany(response, out int parsedCounter, out int totalCounter);

            Console.WriteLine(IO.ComposeSummaryString("parsed", parsedCounter, totalCounter));

            return new ReadOnlyCollection<Contact>(parsed);
        }

        public ReadOnlyCollection<Contact> FindByField(Contact.FieldKind fieldKind, string query) {
            Console.WriteLine($"Searching contacts by {Contact.GetFieldKindName(fieldKind)} at {BaseUri}");
            DoHttpReqeust(httpClient.GetAsync(new Uri(BaseUri, $"/api/findBy?field={fieldKind}&query={query}")), out string response);

            List<Contact> parsed = Contact.ParseMany(response, out int parsedCounter, out int totalCounter);

            Console.WriteLine(IO.ComposeSummaryString("parsed", parsedCounter, totalCounter));

            return new ReadOnlyCollection<Contact>(parsed);
        }
    }
}
