using PTLab07;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace PTLab08
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            attributesLabel.Content = "----";

        }

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new FolderBrowserDialog()
            {
                Description = "Select directory to open"
            };
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var root = new TreeViewItem { Header = System.IO.Path.GetFileName(dlg.SelectedPath), Tag = dlg.SelectedPath };
                Program.PrintDirectories(dlg.SelectedPath, root);
                myTreeView.Items.Add(root);
            }
        }
        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void File_Click(object sender, RoutedEventArgs e)
        {
            if (myTreeView.SelectedItem is TreeViewItem selectedItem)
            {
                string? filePath = selectedItem.Tag as string;
                if (File.Exists(filePath))
                {
                    var fileInfo = new FileInfo(filePath);
                    var attributes = fileInfo.Attributes;

                    StringBuilder sb = new("----");
                    if ((attributes & FileAttributes.ReadOnly) != 0) sb[0] = 'r';
                    if ((attributes & FileAttributes.Archive) != 0) sb[1] = 'a';
                    if ((attributes & FileAttributes.Hidden) != 0) sb[2] = 'h';
                    if ((attributes & FileAttributes.System) != 0) sb[3] = 's';

                    attributesLabel.Content = sb.ToString();
                }
            }
        }

        private void FileOpen_Click(object sender, RoutedEventArgs e)
        {
            if (myTreeView.SelectedItem is TreeViewItem selectedItem)
            {
                string? filePath = selectedItem.Tag as string;
                string fileContent = File.ReadAllText(path: filePath);
                fileContentTextBox.Text = fileContent;
            }
        }

        private void FileDelete_Click(object sender, RoutedEventArgs e)
        {
            if (myTreeView.SelectedItem is TreeViewItem selectedItem)
            {
                string? path = selectedItem.Tag as string;
                if (File.Exists(path))
                {
                    var fileInfo = new FileInfo(path);
                    if ((fileInfo.Attributes & FileAttributes.ReadOnly) != 0)
                    {
                        System.Windows.MessageBox.Show(path + "\nFile is read only");
                        return;
                    }
                    File.Delete(path);
                    if (selectedItem.Parent is ItemsControl parent)
                    {
                        parent.Items.Remove(selectedItem);
                    }


                }
                else if (Directory.Exists(path))
                {
                    var directoryInfo = new DirectoryInfo(path);
                    foreach (var file in directoryInfo.GetFiles("*", SearchOption.AllDirectories))
                    {
                        if ((file.Attributes & FileAttributes.ReadOnly) != 0)
                        {
                            System.Windows.MessageBox.Show(file.FullName + "\nFile is read only");
                            return;
                        }
                    }
                    Directory.Delete(path, true);
                    if (selectedItem.Parent is ItemsControl parent)
                    {
                        parent.Items.Remove(selectedItem);
                    }



                }
            }

        }

        private void DirectoryCreate_Click(object sender, RoutedEventArgs e)
        {
            if (myTreeView.SelectedItem is TreeViewItem selectedItem)
            {
                if (selectedItem.Parent != null && selectedItem.Tag is string path)
                {
                    var createForm = new CreateForm(selectedItem, path);
                    createForm.ShowDialog();
                }
            }
        }

        private void MyTreeView_SelectedItemChanged(object sender, RoutedEventArgs e)
        {
            if (myTreeView.SelectedItem is TreeViewItem selectedItem)
            {
                string? path = selectedItem.Tag as string;
                var contextMenu = new ContextMenu();

                if (File.Exists(path))
                {
                    var openMenuItem = new MenuItem { Header = "_Open" };
                    openMenuItem.Click += FileOpen_Click;
                    contextMenu.Items.Add(openMenuItem);
                }

                if (Directory.Exists(path))
                {
                    var createMenuItem = new MenuItem { Header = "_Create" };
                    createMenuItem.Click += DirectoryCreate_Click;
                    contextMenu.Items.Add(createMenuItem);
                }

                var deleteMenuItem = new MenuItem { Header = "_Delete" };
                deleteMenuItem.Click += FileDelete_Click;
                contextMenu.Items.Add(deleteMenuItem);

                myTreeView.ContextMenu = contextMenu;
            }

        }
    }
}