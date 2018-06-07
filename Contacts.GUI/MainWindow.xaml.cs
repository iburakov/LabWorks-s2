using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Contacts;

namespace Contacts.GUI {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();

            foreach (Contact.FieldKind fieldKind in Enum.GetValues(typeof(Contact.FieldKind))) {
                //<Label Content = "First Name:" Grid.Row = "0" Style="{StaticResource LabelStyle}" />
                //<TextBox Grid.Row = "0" Style = "{StaticResource TextBoxStyle}" />

                var label = new Label {
                    Content = Contact.GetFieldKindName(fieldKind),
                    // Grid.Row?
                    // Style?!
                };

                var textBox = new TextBox {
                    // ???
                };

                DetailsGrid.Children.Add(label);
                DetailsGrid.Children.Add(textBox);
            }
        }

    }
}
