using Microsoft.Win32;
using Steganography.Crypto;
using Steganography.ImageManipulating;
using System;
using System.IO;
using System.Windows;

namespace Steganography
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Object to hide data into and retrieve data from medium.
        /// </summary>
        private readonly IManipulator _manipulator;
        /// <summary>
        /// Object to encrypt data using AES.
        /// </summary>
        private readonly IMyEncryptable _aesEncrypt;
        /// <summary>
        /// No encryption.
        /// </summary>
        private readonly IMyEncryptable _noEncrypt;

        public MainWindow()
        {
            _aesEncrypt = new RijndaelExample();
            _noEncrypt = new NoEncrypt();
            _manipulator = new ImageManipulator
            {
                Encryptor = _aesEncrypt, // Default encryption is AES.
                Hasher = new HashUtilities(),
                PictureEditor = new PictureEditor
                {
                    PixelEditor = new PixelEditor(),
                }
            };
            InitializeComponent();
        }

        private void BtnQuit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to quit?", "Hey!", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                e.Cancel = true;
                return;
            }
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Event handler for click event of Browse Picture button.
        /// Use to select medium file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPicture_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            var dlg = new OpenFileDialog
            {
                // Set filter for file extension and default file extension 
                DefaultExt = ".png",
                Filter = "PNG|*.png|BMP|*.bmp",
                ValidateNames = false,
            };

            // Display OpenFileDialog by calling ShowDialog method 
            var result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                TextPicture.Text = dlg.FileName;
                TextPicture.ToolTip = dlg.FileName;
            }
        }

        /// <summary>
        /// Event handler for click event of Browse File button.
        /// Use to select secret file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnFile_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            var dlg = new OpenFileDialog
            {
                // Set filter for file extension and default file extension 
                Filter = "All files|*.*",
                ValidateNames = false,
            };

            // Display OpenFileDialog by calling ShowDialog method 
            var result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                TextFile.Text = dlg.FileName;
                TextFile.ToolTip = dlg.FileName;
            }
        }

        /// <summary>
        /// Event handler for click event of Encode button.
        /// Hide secret file into medium then display save dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnEncode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DisplayStandByGraphic("Encoding, please wait...");

                if (string.IsNullOrEmpty(TextPicture.Text))
                {
                    throw new Exception("Please select a medium!");
                }

                if (string.IsNullOrEmpty(TextFile.Text))
                {
                    throw new Exception("Please select a file to encode!");
                }

                if (FileHelper.GetFileExtension(TextFile.Text).Length > 16)
                {
                    throw new Exception("File extension is too long!");
                }

                // Create and show save file dialog box
                var dlg = new SaveFileDialog
                {
                    FileName = "hidden", // Default file name
                    DefaultExt = "png", // Default file extension
                    Filter = "PNG|*.png|BMP|*.bmp", // Filter files by extension
                };
                var result = dlg.ShowDialog();

                // Process save file dialog box results
                if (result == true)
                {
                    // Read picture's info from disk.
                    var fileData = FileHelper.ReadFileFromDisk(TextPicture.Text).Item1;
                    // Read secret file's info from disk.
                    var secretTuple = FileHelper.ReadFileFromDisk(TextFile.Text);
 
                    // Hide file into picture
                    var outputExt = FileHelper.GetFileExtension(dlg.FileName);
                    var fileWithPictureData = await _manipulator.HideFileIntoMediumAsync(fileData, secretTuple.Item1, secretTuple.Item2, TextPassword.Password, outputExt);
                    File.WriteAllBytes(dlg.FileName, fileWithPictureData);
                    ResetTextBox();
                    MessageBox.Show("Completed.");
                }
            }
            catch (ArgumentNullException ex)
            {
                MessageBox.Show($"{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (FormatException ex)
            {
                MessageBox.Show($"{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {                
                MessageBox.Show($"Encoding failed{Environment.NewLine}{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                DisplayDefaultGraphic();
            }
        }

        /// <summary>
        /// Event handler for click event of Decode Picture button.
        /// If password and encryption method is correct, retrieve and decryption secret file from medium.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnDecode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DisplayStandByGraphic("Decoding, please wait...");

                if (string.IsNullOrEmpty(TextPicture.Text))
                {
                    throw new Exception("Please select a medium!");
                }

                var mediumData = FileHelper.ReadFileFromDisk(TextPicture.Text).Item1;
                var secret = await _manipulator.GetFileFromMediumAsync(mediumData, TextPassword.Password);

                var dlg = new SaveFileDialog
                {
                    FileName = "secret_file", // Default file name
                    DefaultExt = secret.Extension, // Default file extension
                    Filter = secret.Extension + " (*." + secret.Extension + ")|*." + secret.Extension, // Filter files by extension
                };

                // Show save file dialog box
                var result = dlg.ShowDialog();

                // Process save file dialog box results
                if (result == true)
                {
                    var filePath = dlg.FileName;
                    File.WriteAllBytes(filePath, secret.Data);
                    ResetTextBox();
                    MessageBox.Show("Completed.");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Decoding failed{Environment.NewLine}{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                DisplayDefaultGraphic();
            }
        }

        /// <summary>
        /// Reset the value of medium and secret file's text box.
        /// </summary>
        private void ResetTextBox()
        {
            TextPicture.Clear();
            TextFile.Clear();
        }

        /// <summary>
        /// Display animation gif when background task is running.
        /// </summary>
        /// <param name="message"></param>
        private void DisplayStandByGraphic(string message)
        {
            FrmMainWindow.Title = message;
            PnOverlay.Visibility = Visibility.Visible;
            GifCtrl.Visibility = Visibility.Visible;
            GifCtrl.StartAnimate();
        }

        /// <summary>
        /// Restore original layout of program.
        /// </summary>
        private void DisplayDefaultGraphic()
        {
            FrmMainWindow.Title = "Steganography GUI";
            PnOverlay.Visibility = Visibility.Hidden;
            GifCtrl.Visibility = Visibility.Hidden;
            GifCtrl.StopAnimate();
        }

        /// <summary>
        /// Change encryption method to AES when corresponding radio button is selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioAES_Checked(object sender, RoutedEventArgs e)
        {
            _manipulator.Encryptor = _aesEncrypt;
        }

        /// <summary>
        /// Change encryption method to no encryption when corresponding radio button is selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioNone_Checked(object sender, RoutedEventArgs e)
        {
            _manipulator.Encryptor = _noEncrypt;
        }
    }
}
