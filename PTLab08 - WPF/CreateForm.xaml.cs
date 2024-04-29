using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using CheckBox = System.Windows.Controls.CheckBox;
using MessageBox = System.Windows.MessageBox;
using RadioButton = System.Windows.Controls.RadioButton;
using TextBox = System.Windows.Controls.TextBox;

namespace PTLab08
{
    /// <summary>
    /// Interaction logic for CreateForm.xaml
    /// </summary>
    public partial class CreateForm : Window
    {
        private readonly string currentPath;
        private readonly ItemsControl parent;
        private string name;
        private List<string> rahsValues;
        private bool file;

        public CreateForm(ItemsControl parent, string currentPath)
        {
            file = true;
            rahsValues = new List<string>();
            name = "";
            InitializeComponent();
            this.parent = parent;
            this.currentPath = currentPath;
        }


        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && !rahsValues.Contains(checkBox.Content.ToString()))
            {
                rahsValues.Add(checkBox.Content.ToString());
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                rahsValues.Remove(checkBox.Content.ToString());
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                file = (radioButton.Name == "File");
                if (radioButton.Name == "File")
                {
                    if (CheckBoxPanel != null)
                    {
                        CheckBoxPanel.Visibility = Visibility.Visible;
                    }
                }
                else if (radioButton.Name == "Directory")
                {
                    CheckBoxPanel.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void NameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                name = textBox.Text;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (name == "")
            {
                MessageBox.Show("Plese enter a name");
                return;
            }

            if (file)
            {
                string pattern = @"^[a-zA-Z0-9_~\-]{1,8}\.(txt|php|html)$";
                if (!Regex.IsMatch(name, pattern))
                {
                    MessageBox.Show("Invalid file name. The base name should be 1 to 8 characters (letters, digits, underscore, tilde, hyphen) and the extension should be txt, php, or html.");
                    return;
                }
                if (CheckIfNameIsFree())
                {
                    string path = currentPath + "\\" + name;
                    using (System.IO.File.Create(path)) { }

                    FileAttributes attributes = System.IO.File.GetAttributes(path);

                    if (rahsValues.Contains("ReadOnly"))
                    {
                        attributes |= FileAttributes.ReadOnly;
                    }
                    if (!rahsValues.Contains("Archive"))
                    {
                        attributes &= ~FileAttributes.Archive;
                    }
                    if (rahsValues.Contains("Hidden"))
                    {
                        attributes |= FileAttributes.Hidden;
                    }
                    if (rahsValues.Contains("System"))
                    {
                        attributes |= FileAttributes.System;
                    }

                    System.IO.File.SetAttributes(path, attributes);

                    var fileNode = new TreeViewItem { Header = name, Tag = path };
                    parent.Items.Add(fileNode);
                }


            }
            else
            {
                if (CheckIfNameIsFree())
                {
                    string path = currentPath + "\\" + name;
                    System.IO.Directory.CreateDirectory(path);
                    var directoryNode = new TreeViewItem { Header = name, Tag = path };
                    parent.Items.Add(directoryNode);

                }
            }
            Close();


        }

        private bool CheckIfNameIsFree()
        {
            DirectoryInfo directoryInfo = new(currentPath);
            foreach (var file in directoryInfo.GetFiles("*", SearchOption.TopDirectoryOnly))
            {
                if (file.Name == name)
                {
                    MessageBox.Show("The file name is already taken");
                    return false;
                }
            }

            foreach (var directory in directoryInfo.GetDirectories("*", SearchOption.TopDirectoryOnly))
            {
                if (directory.Name == name)
                {
                    MessageBox.Show("The directory name is already taken");
                    return false;
                }
            }
            return true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
