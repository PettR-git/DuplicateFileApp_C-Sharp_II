using FindDuplicateFileApp.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static FindDuplicateFileApp.Entities.FileInfo;
using System.Security.Cryptography;

namespace FindDuplicateFileApp
{
    /// Author: Petter Rignell
    /// Created: 2024-04-25
    /// Latest update: 2024-04-30
    /// <summary>
    /// Find duplicate files from chosen folders. 
    /// Display them or delete them.
    /// 
    /// MainWindow: Logic for UI and it accesses 
    /// info from DuplicateFinder Class
    /// </summary>
    public partial class MainWindow : Window
    {
        private DuplicateFileFinder fileFinder;
        private int folderCounter;
        public MainWindow()
        {
            InitializeComponent();
            System.Windows.MessageBox.Show("Welcome to File Finder App! Choose criterias before adding folders.");
            fileFinder = new DuplicateFileFinder();
            folderCounter = 0;
        }

        /// <summary>
        /// Get folderpath from directory, chosen by user
        /// </summary>
        /// <returns>folderpath as string</returns>
        private string GetFolderPath()
        {
            string folderPath = "";
            
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Select folders";

            if (RootPath != null) 
                folderBrowserDialog.SelectedPath = RootPath;

            DialogResult dialogResult = folderBrowserDialog.ShowDialog();

            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                folderPath = folderBrowserDialog.SelectedPath;
                RootPath = folderPath;
            }

            return folderPath;
        }

        /// <summary>
        /// Get all filepaths from folderpath
        /// And set file criterias given check from checkboxes
        /// </summary>
        /// <param name="folderPath"></param>
        private void RetrieveFiles(string folderPath)
        {
            string[] filePaths = Directory.GetFiles(folderPath);
            FileRecord fileR = null;

            foreach (string filePath in filePaths)
            {
                if(filePath == null)
                    continue;

                fileR = new FileRecord();

                fileR.FileName = filePath;

                fileFinder.AddFile(fileR);
                lvwSeperateFiles.Items.Add(fileR.FileName);
            }
        }

        /// <summary>
        /// Calculate checksum
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>hashvalue/checksum</returns>
        private static string CalculateMD5Checksum(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    byte[] hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        /// <summary>
        /// Get the size of a file in kilo bytes
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private double GetFileSizeInKilobytes(string filePath)
        {
            double fileSize = 0;
            try
            {
                using (FileStream stream = File.OpenRead(filePath))
                {
                    fileSize = stream.Length / 1024.0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting file size: {ex.Message}");
            }

            return fileSize;
        }

        /// <summary>
        /// 1.Add folder
        /// 2.Add all files from folder
        /// 3.Update UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string folderPath = GetFolderPath();
                if (folderPath == null)
                    return;

                RetrieveFiles(folderPath);
                lblFolderCounter.Content = ++folderCounter;
                //DisableCheckBoxes();
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show("Could not get folder/files");
                Console.WriteLine(ex.Message);
            }
        }

        private void UpdateRecordsFrCheckbox()
        {
            for(int i = 0; i<fileFinder.Count(); i++)
            {
                FileRecord fileR = fileFinder.GetFileRecordAt(i);

                string filePath = fileR.FileName;

                if (cbxDateCreated.IsChecked == true)
                    fileR.DateCreated = File.GetCreationTime(filePath).ToShortDateString();
                if (cbxDateModified.IsChecked == true)
                    fileR.DateModified = File.GetLastWriteTime(filePath).ToShortDateString();
                if (cbxChecksum.IsChecked == true)
                    fileR.Checksum = CalculateMD5Checksum(filePath);
                if (cbxFileType.IsChecked == true)
                    fileR.FileType = System.IO.Path.GetExtension(filePath);
                if (cbxSize.IsChecked == true)
                    fileR.Size = GetFileSizeInKilobytes(filePath);
            }
        }

        /// <summary>
        /// Show all duplicate files in listview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnShowDuplicates_Click(object sender, RoutedEventArgs e)
        {
            string outStr;

            UpdateRecordsFrCheckbox();
            List<(FileRecord, FileRecord)> identicalPairOfFiles = fileFinder.GetAllIdenticalFiles();
            lvwDuplicateFileNames.Items.Clear();

            foreach ((FileRecord file1, FileRecord file2) in identicalPairOfFiles)
            {
                outStr = string.Format($"Similar files: {file1.FileName} AND {file2.FileName}");
                lvwDuplicateFileNames.Items.Add(outStr);
            }         
        }

        /// <summary>
        /// Delete duplicate files and display info in listview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteDuplicates_Click(object sender, RoutedEventArgs e)
        {
            string outStr = "";
            int index = -1;

            try {
                //User to confirm deleting the files
                MessageBoxResult result = System.Windows.MessageBox.Show("Are you sure you want to delete?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if(result == MessageBoxResult.Yes)
                {         
                    //Validate selected items, compared to list of files
                    if (lvwSeperateFiles.SelectedItem is string filePath)
                    {
                        index = lvwSeperateFiles.Items.IndexOf(filePath);

                        if (File.Exists(filePath) && index != -1)
                        {
                            //Delete file
                            File.Delete(filePath);
                            outStr = string.Format($"Deleted file: {filePath}");
                            fileFinder.DeleteFileAt(index);

                            //Update UI
                            lvwDuplicateFileNames.Items.Add(outStr);
                            lvwSeperateFiles.Items.Remove(filePath);
                        }
                    }                   
                }
                else
                {
                    return;
                }
            }
            catch( Exception ex)
            {
                Console.WriteLine($"Files does not exist: {ex.Message}");
            }
        }

        //Reset logic and UI in app to normal
        private void btnResetAll_Click(object sender, RoutedEventArgs e)
        {
            NewApp();
        }

        public void NewApp()
        {
            lvwSeperateFiles.Items.Clear();
            lvwDuplicateFileNames.Items.Clear();
            cbxSize.IsChecked = false;
            cbxFileType.IsChecked = false;
            cbxDateModified.IsChecked = false;
            cbxDateCreated.IsChecked = false;
            cbxChecksum.IsChecked = false;
            folderCounter = 0;
            lblFolderCounter.Content = folderCounter.ToString();

            fileFinder = new DuplicateFileFinder();
        }

        //RootPath for past chosen directory
        public string RootPath { get; private set; }
    }
}
