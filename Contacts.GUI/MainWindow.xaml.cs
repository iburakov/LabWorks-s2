using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Contacts.GUI {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private List<Contact> localContacts;
        private WcfContactsStorage storage;
        private bool isUpdating;
        private const int UPDATE_DELAY_MS = 5000;
        private const string TITLE = "Contacts GUI";

        private List<TextBox> contactFields = new List<TextBox>();
        private List<Contact.FieldKind> fieldKinds = new List<Contact.FieldKind>();
        private Contact tempContact;

        public MainWindow(WcfContactsStorage storage) {
            InitializeComponent();

            Style labelStyle = DetailsGrid.FindResource("LabelStyle") as Style;
            Style textBoxStyle = DetailsGrid.FindResource("TextBoxStyle") as Style;

            foreach (Contact.FieldKind fieldKind in Enum.GetValues(typeof(Contact.FieldKind))) {
                if (fieldKind != Contact.FieldKind.FullName) {
                    fieldKinds.Add(fieldKind);
                }
            }
                
            int i = -1;
            foreach (Contact.FieldKind fieldKind in fieldKinds) {
                i++;

                DetailsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                var label = new Label {
                    Content = Contact.GetFieldKindName(fieldKind) + ":",
                    Style = labelStyle
                };
                label.SetValue(Grid.RowProperty, i);

                var textBox = new TextBox {
                    Style = textBoxStyle
                };
                textBox.SetValue(Grid.RowProperty, i);
                textBox.IsReadOnly = true;

                DetailsGrid.Children.Add(label);
                DetailsGrid.Children.Add(textBox);
                contactFields.Add(textBox);
            }
            i++;
            DetailsGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            ButtonsPanel.SetValue(Grid.RowProperty, i);

            this.storage = storage;
            StartContactsUpdaterAsync();
            Title = $"{TITLE} - Connected to {storage.ConnectedToURI}";
        }

        private async void StartContactsUpdaterAsync() {
            isUpdating = true;
            while (isUpdating) {
                await UpdateContactsAsync();
                await Task.Delay(UPDATE_DELAY_MS);
            }
        }

        private async Task UpdateContactsAsync() {
            ContactsListBox.IsEnabled = false;
            ProgressBar.IsIndeterminate = true;

            try {
                var newContacts = new List<Contact>(await storage.GetAllContactsAsync());
                bool equal = false;
                if (localContacts != null && newContacts.Count == localContacts.Count) {
                    equal = true;
                    for (int i = 0; i < newContacts.Count; i++) {
                        if (!newContacts[i].Equals(localContacts[i])) {
                            equal = false;
                            break;
                        }
                    }
                }

                if (!equal) {
                    localContacts = newContacts;
                    ContactsListBox.ItemsSource = localContacts;
                }

                ProgressBar.IsIndeterminate = false;
                ContactsListBox.IsEnabled = true;
            }
            catch (Exception e) when( 
                e is AggregateException ||
                e is FaultException ||
                e is CommunicationException ||
                e is TimeoutException
            ) {
                HandleException(e);
                return;
            }
        }

        private void HandleException(Exception e) {
            string msg;
            if (e is AggregateException notFlattenedAe) {
                AggregateException ae = notFlattenedAe.Flatten();
                msg = ae.InnerExceptions[ae.InnerExceptions.Count - 1].Message;
            } else {
                msg = e.Message;
            }
            
            ProgressBar.IsIndeterminate = false;
            ProgressBar.Foreground = Brushes.Red;
            CreateNewContactButton.IsEnabled = false;
            SubmitButton.IsEnabled = false;
            Title = $"{TITLE} - Lost connection to {storage.ConnectedToURI}";
            MessageBox.Show($"Unfortunately, connection to {storage.ConnectedToURI} was lost.\n\n" +
                $"Technical details: {msg}", "Fatal Error", MessageBoxButton.OK);
            new ConnectWindow().Show();
            Close();
        }

        private void CreateNewContactButton_Click(object sender, RoutedEventArgs e) {
            tempContact = new Contact();
            DetailsGrid.DataContext = tempContact;

            int i = -1;
            foreach (Contact.FieldKind fieldKind in fieldKinds) {
                i++;
                contactFields[i].SetBinding(TextBox.TextProperty, new Binding(fieldKind.ToString()) {
                    ValidatesOnExceptions = true
                });
            }

            contactFields.ForEach((TextBox field) => {
                field.IsReadOnly = false;
                field.Text = "";
            });
            ProgressBar.Foreground = Brushes.Blue;
            ProgressBar.Value = 100;
            ContactsListBox.IsEnabled = false;
            SubmitButton.IsEnabled = true;
            isUpdating = false;
        }

        private async void SubmitButton_Click(object sender, RoutedEventArgs e) {
            if (!tempContact.IsValid()) return;
            ProgressBar.Foreground = Brushes.LimeGreen;
            ProgressBar.IsIndeterminate = true;
            try {
                string message = await storage.AddContactAsync(tempContact, rethrowException: true);
                ProgressBar.IsIndeterminate = false;
                ProgressBar.Foreground = Brushes.LimeGreen;
                MessageBox.Show(message, "Response", MessageBoxButton.OK);
            }
            catch (Exception ex) when (
                ex is AggregateException ||
                ex is FaultException ||
                ex is CommunicationException ||
                ex is TimeoutException
            ) {
                HandleException(ex);
                return;
            }
            finally {
                foreach (TextBox field in contactFields) {
                    BindingOperations.ClearAllBindings(field);
                }
            }

            contactFields.ForEach((TextBox field) => {
                field.IsReadOnly = true;
            });
            SubmitButton.IsEnabled = false;
            ContactsListBox.IsEnabled = true;
            ProgressBar.IsIndeterminate = false;
            ProgressBar.Foreground = Brushes.LimeGreen;
            ProgressBar.Value = 0;
            StartContactsUpdaterAsync();
        }

        private void ContactsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            Contact selected = e.AddedItems[0] as Contact;

            foreach (TextBox field in contactFields) {
                BindingOperations.ClearAllBindings(field);
            }

            DetailsGrid.DataContext = selected ;

            int i = -1;
            foreach (Contact.FieldKind fieldKind in fieldKinds) {
                i++;
                contactFields[i].SetBinding(TextBox.TextProperty, new Binding(fieldKind.ToString()) {
                    ValidatesOnExceptions = true
                });
            }
        }
    }
}
