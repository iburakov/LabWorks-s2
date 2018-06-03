using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Contacts.CommandLine;

namespace Contacts {
    class WebApiContactsStorage : RemoteContactsStorage {
        public Uri BaseUri { get; private set; }
        private readonly HttpClient httpClient;

        public WebApiContactsStorage(string hostBaseUrl) {
            httpClient = new HttpClient {
                Timeout = new TimeSpan(0, 0, 15)
            };

            if (!IO.TryParseUri(hostBaseUrl, out Uri parsedUri)) {
                throw new ArgumentException("Invalid URL was given. Format: http://host[:port]");
            } else BaseUri = parsedUri;

            try {
                Console.WriteLine($"Connecting to WebAPI remote storage at {BaseUri}");
                IsGreetingSuccessful = DoHttpReqeust(httpClient.GetAsync(new Uri(BaseUri, "/api/greet")), out string response);
            }
            catch (AggregateException notFlattenedAe) {
                AggregateException ae = notFlattenedAe.Flatten();
                Console.WriteLine($"Connection failed: {ae.InnerExceptions[ae.InnerExceptions.Count - 1].Message}");
                IsGreetingSuccessful = false;
            }
        }

        private bool DoHttpReqeust(Task<HttpResponseMessage> requestTask, out string response) {
            HttpResponseMessage httpResponse = WaitForTaskResult(requestTask);
            
            try {
                httpResponse.EnsureSuccessStatusCode();

                Task<string> readTask = httpResponse.Content.ReadAsStringAsync();
                while (!readTask.IsCompleted) {
                    Thread.Sleep(50);
                }
                response = readTask.Result;
                return true;
            }
            catch (HttpRequestException) {
                response = $"Network exception ({(int)requestTask.Result.StatusCode})! ";

                switch (requestTask.Result.StatusCode) {
                    case HttpStatusCode.NotFound:
                    case HttpStatusCode.Redirect:
                    response += "Bad server.";
                    break;
                    case HttpStatusCode.InternalServerError:
                    response += "Internal server error.";
                    break;
                    default:
                    response += "Unknown exception.";
                    break;
                }

                return false;
            }

        }

        public override void AddContact(Contact newContact, out string message) {
            HttpContent requestContent = new MultipartFormDataContent {
                { new StringContent(newContact.ToVCard()), "contact" }
            };

            Console.WriteLine($"Sending a new contact to {BaseUri}");

            if (DoHttpReqeust(httpClient.PostAsync(new Uri(BaseUri, "/api/addContact"), requestContent), out message)) {
                message = $"[@{BaseUri}] " + message;
            }
        }

        public override IReadOnlyCollection<Contact> GetAllContacts() {
            Console.WriteLine($"Getting all contacts from {BaseUri}");
            if (!DoHttpReqeust(httpClient.GetAsync(new Uri(BaseUri, "/api/getAllContacts")), out string response)) {
                Console.WriteLine(response);
                return new ReadOnlyCollection<Contact>(new List<Contact> { });
            }

            List<Contact> parsed = Contact.ParseMany(response, out int parsedCounter, out int totalCounter);

            Console.WriteLine(IO.ComposeSummaryString("parsed", parsedCounter, totalCounter));

            return new ReadOnlyCollection<Contact>(parsed);
        }

        public override IReadOnlyCollection<Contact> FindByField(Contact.FieldKind fieldKind, string query) {
            Console.WriteLine($"Searching contacts by {Contact.GetFieldKindName(fieldKind)} at {BaseUri}");
            if (!DoHttpReqeust(httpClient.GetAsync(new Uri(BaseUri, $"/api/findBy?field={fieldKind}&query={query}")), out string response)) {
                Console.WriteLine(response);
                return new ReadOnlyCollection<Contact>(new List<Contact> { });
            }

            List<Contact> parsed = Contact.ParseMany(response, out int parsedCounter, out int totalCounter);

            Console.WriteLine(IO.ComposeSummaryString("parsed", parsedCounter, totalCounter));

            return new ReadOnlyCollection<Contact>(parsed);
        }
    }
}
