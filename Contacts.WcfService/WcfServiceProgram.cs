using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using Contacts.CommandLine;
using System.Configuration;

namespace Contacts.WcfService {
    public sealed class WcfServiceProgram {
        public static void Main(string[] args) {
            string baseUriStr = ConfigurationManager.ConnectionStrings["baseUri"]?.ConnectionString;
            string relativeUriStr = ConfigurationManager.ConnectionStrings["serviceRelativeUri"]?.ConnectionString;

            if (baseUriStr is null || relativeUriStr is null ||
                !IO.TryParseUri(baseUriStr, out Uri baseUri) ||
                baseUri.Scheme != "http") {

                Console.WriteLine("Bad config.");
                return;
            }

            ServiceHost selfHost = new ServiceHost(typeof(ContactsWcfService), baseUri);

            try {
                selfHost.Open();
                Console.WriteLine($"Service is running at {baseUri}.");
                Console.WriteLine("Press <ENTER> to terminate the service.");

                bool isTerminatingRequired = false;
                while (!isTerminatingRequired) {
                    Console.ReadLine();
                    Console.WriteLine("Are you sure you want to terminate the service?");
                    isTerminatingRequired = IO.ReadBoolean(yesByDefault: false);
                    if (isTerminatingRequired == false) {
                        Console.WriteLine("Still running...");
                    }
                }

                selfHost.Close();
            }
            catch (CommunicationException e) {
                Console.WriteLine($"An exception occurred: {e.Message}");
                selfHost.Abort();
            }
        }
    }
}
