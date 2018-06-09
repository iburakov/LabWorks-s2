using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Contacts.GUI {
    /// <summary>
    /// Interaction logic for ConnectWindow.xaml
    /// </summary>
    public partial class ConnectWindow : Window {
        public ConnectWindow() {
            InitializeComponent();
        }

        public WcfContactsStorage Storage { get; set; }

        private async void Button_Click(object sender, RoutedEventArgs e) {
            Lock();

            Log($"Establishing connection to {UriTextBox.Text}");
            string endpoint = UriTextBox.Text;
            var connectionTask = new Task<WcfContactsStorage>(() => {
                    return new WcfContactsStorage(false, endpoint);
            });
            connectionTask.Start();
            
            try {
                Storage = await connectionTask;
            }
            catch (AggregateException notFlattenedAe) {
                AggregateException ae = notFlattenedAe.Flatten();
                Log("Could not connect to server!");
                Log($"Technical details: {ae.InnerExceptions[ae.InnerExceptions.Count - 1].Message}");
                Storage = null;
                await UnlockAsync(true);
                return;
            }

            if (Storage.IsGreetingSuccessful) {
                var mainWindow = new MainWindow(Storage);
                mainWindow.Show();
                Close();
            } else {
                Log("Could not connect to server: greeting wasn't successful (wrong or bad server?)");
                Storage = null;
                await UnlockAsync(true);
            }
        }

        private void Log(string msg) {
            LogTextBlock.Text += $"[ {DateTime.Now.ToString("HH:mm:ss fff")} ] {msg}\n";
            LogScrollViewer.ScrollToBottom();
        }

        private async Task UnlockAsync(bool failure = false) {
            ConnectProgressBar.IsIndeterminate = false;
            ConnectProgressBar.Background = Brushes.Red;

            if (!failure) {
                await Task.Delay(100);
                ConnectProgressBar.Background = BackgroundProperty.DefaultMetadata.DefaultValue as Brush;
            }

            UriTextBox.IsEnabled = true;
            ConnectButton.IsEnabled = true;
        }

        private void Lock() {
            ConnectProgressBar.IsIndeterminate = true;
            ConnectProgressBar.Background = BackgroundProperty.DefaultMetadata.DefaultValue as Brush;
            UriTextBox.IsEnabled = false;
            ConnectButton.IsEnabled = false;
        }

    }
}
