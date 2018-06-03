using HttpMultipartParser;
using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;

namespace Contacts.Server {
    public class ContactsServer {
        private readonly HttpListener httpListener;
        private readonly Uri listenerUri;
        private readonly LocalContactsStorage storage;

        public ContactsServer(Uri uriToListen, LocalContactsStorage storage) {
            httpListener = new HttpListener();
            httpListener.Prefixes.Add(uriToListen.ToString());
            listenerUri = uriToListen;
            this.storage = storage;
        }

        public void Run() {
            httpListener.Start();
            Console.WriteLine("Server is listening at " + listenerUri.ToString());

            while (true) {
                var context = httpListener.GetContext();
                ThreadPool.QueueUserWorkItem(o => HandleRequest(context));
            }
        }

        private void HandleRequest(object contextObject) {
            var context = (HttpListenerContext)contextObject;
            context.Response.SendChunked = true;

            Console.Write($"[{DateTime.Now}] {context.Request.HttpMethod} request to {context.Request.Url.AbsolutePath}... ");

            void SetResponseWithString(string response) {
                var bytes = Encoding.UTF8.GetBytes(response);
                context.Response.OutputStream.Write(bytes, 0, bytes.Length);
                context.Response.StatusCode = 200;
            }

            switch (context.Request.Url.AbsolutePath) {
                case "/api/getAllContacts" when context.Request.HttpMethod == "GET": {
                    SetResponseWithString(Contact.ToVCardMany(storage.GetAllContacts()));
                }
                break;
                case "/api/findBy" when context.Request.HttpMethod == "GET": {
                    string field = context.Request.QueryString["field"];
                    string query = context.Request.QueryString["query"];
                    Enum.TryParse(field, out Contact.FieldKind fieldKind);

                    SetResponseWithString(Contact.ToVCardMany(storage.FindByField(fieldKind, query)));
                }
                break;
                case "/api/addContact" when context.Request.HttpMethod == "POST": {
                    var parser = new MultipartFormDataParser(context.Request.InputStream);
                    string vcard = parser.GetParameterValue("contact");
                    Debug.Assert(vcard != null, "Request should have a 'contact' field");
                    storage.AddContact(Contact.Parse(vcard), out string message);

                    SetResponseWithString(message);
                }
                break;
                case "/api/greet" when context.Request.HttpMethod == "GET": {
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                }
                break;
                default: {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                break;
            }

            Console.WriteLine($" [{DateTime.Now}] -> {context.Response.StatusCode} {(HttpStatusCode)context.Response.StatusCode}");

            context.Response.Close();
        }
    }
}
