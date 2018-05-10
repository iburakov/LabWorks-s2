﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Contacts.CommandLine;

namespace Contacts {
    class RemoteContactsStorage : IContactsStorage {
        private const string defaultScheme = "https";
        private const int delayBetweenDotsMs = 500;

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

        private void DoHttpPostReqeust(Uri requestUri, HttpContent requestContent, out string response) {
            Task<HttpResponseMessage> postTask = httpClient.PostAsync(requestUri, requestContent);
            while (!postTask.IsCompleted) {
                Console.Write(".");
                Thread.Sleep(delayBetweenDotsMs);
            }
            Console.WriteLine();
            HttpResponseMessage httpResponse = postTask.Result;

            httpResponse.EnsureSuccessStatusCode();

            response = httpResponse.Content.ToString();
        }

        private void DoHttpGetReqeust(Uri requestUri, out string response) {
            Task<HttpResponseMessage> getTask = httpClient.GetAsync(requestUri);
            while (!getTask.IsCompleted) {
                Console.Write(".");
                Thread.Sleep(delayBetweenDotsMs);
            }
            Console.WriteLine();

            HttpResponseMessage httpResponse = getTask.Result.EnsureSuccessStatusCode();

            response = httpResponse.Content.ToString();
        }

        public void AddContact(Contact newContact, out string message) {
            HttpContent requestContent = new MultipartFormDataContent {
                { new StringContent(newContact.ToVCard()), "contact" }
            };

            Console.WriteLine($"Sending a new contact to {BaseUri}");
            DoHttpPostReqeust(new Uri(BaseUri, "/api/addContact"), requestContent, out message);
        }

        public ReadOnlyCollection<Contact> GetAllContacts() {
            Console.WriteLine($"Getting all contacts from {BaseUri}");
            DoHttpGetReqeust(new Uri(BaseUri, "/api/getAllContacts"), out string response);

            List<Contact> parsed = Contact.ParseMany(response, out int parsedCounter, out int totalCounter);

            Console.WriteLine(IO.ComposeSummaryString("parsed", parsedCounter, totalCounter));

            return new ReadOnlyCollection<Contact>(parsed);
        }

        public ReadOnlyCollection<Contact> FindByField(Contact.FieldKind fieldKind, string query) {
            Console.WriteLine($"Searching contacts by {fieldKind} in {BaseUri}");
            DoHttpGetReqeust(new Uri(BaseUri, $"/api/findBy?field={fieldKind}&query={query}"), out string response);

            List<Contact> parsed = Contact.ParseMany(response, out int parsedCounter, out int totalCounter);

            Console.WriteLine(IO.ComposeSummaryString("parsed", parsedCounter, totalCounter));

            return new ReadOnlyCollection<Contact>(parsed);
        }
    }
}
