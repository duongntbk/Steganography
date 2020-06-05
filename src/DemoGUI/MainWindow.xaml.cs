using Microsoft.Win32;
using Steganography.Crypto;
using Steganography.ImageManipulating;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using IManipulator = Steganography.ImageManipulating.IManipulator;

namespace Steganography
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Binding context for main form.
        /// </summary>
        public MainWindowContext Context { get; set; }
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
            Context = new MainWindowContext();
            InitializeComponent();
            DataContext = Context;
        }

        private void ExitCommand_Execute(object sender, ExecutedRoutedEventArgs e)
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
        /// Command check for all commands which can run only when decoding/encoding process is not running.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotRunning_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !Context.IsProcessing;
        }

        /// <summary>
        /// Command for Browse Picture button.
        /// Use to select medium file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectMediumCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var defaultExt = ".png";
            var filter = "PNG|*.png|BMP|*.bmp";
            var filePath = SelectOpenFileDialog(filter, defaultExt);

            // If user has selected a file, display its name in a TextBox
            if (filePath != null)
            {
                Context.MediumFilePath = filePath;
            }
        }

        /// <summary>
        /// Command for Browse File button.
        /// Use to select secret file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectSecretCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var filePath = SelectOpenFileDialog();

            // If user has selected a file, display its name in a TextBox
            if (filePath != null)
            {
                Context.SecretFilePath = filePath;
            }
        }

        /// <summary>
        /// Check whether encode command can be execute
        /// then disable/enable Encode button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EncodeCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            // Cannot start encoding if:
            // - Medium file is not yet selected.
            // - Secret file is not yet selected.
            // - Another encoding/decoding process is already running.
            if (string.IsNullOrEmpty(Context.MediumFilePath) || 
                string.IsNullOrEmpty(Context.SecretFilePath) ||
                Context.IsProcessing)
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }       
        }

        /// <summary>
        /// Command for Encode button.
        /// Hide secret file into medium then display save dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void EncodeCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                if (FileHelper.GetFileExtension(Context.SecretFilePath).Length > 16)
                {
                    throw new Exception("File extension cannot be longer than 16 characters!");
                }

                DisplayStandByGraphic("Encoding, please wait...");

                var filename = "hidden";
                var defaultExt = "png";
                var filter = "PNG|*.png|BMP|*.bmp";
                var filePath = SelectSaveFileDialog(filter, defaultExt, filename);

                // Process save file dialog box results
                if (filePath != null)
                {
                    // Read picture's info from disk.
                    var fileData = FileHelper.ReadFileFromDisk(Context.MediumFilePath).Item1;
                    // Read secret file's info from disk.
                    var secretTuple = FileHelper.ReadFileFromDisk(Context.SecretFilePath);
                    // Set encryptor according to radio button's value
                    _manipulator.Encryptor = Context.Encryption == EncryptionOption.Aes ? _aesEncrypt : _noEncrypt;

                    // Hide file into picture
                    var outputExt = FileHelper.GetFileExtension(filePath);
                    var fileWithPictureData = await Task.Run(() =>
                            _manipulator.HideFileIntoMedium(fileData, secretTuple.Item1, secretTuple.Item2,
                                TextPassword.Password, outputExt));
                    File.WriteAllBytes(filePath, fileWithPictureData);
                    Context.MediumFilePath = null;
                    Context.SecretFilePath = null;
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
        /// Check whether decode command can be execute then
        /// disable/enable Decode button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DecodeCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            // Cannot start decoding if medium is not set nor another encode/decode process is already running.
            if (string.IsNullOrEmpty(Context.MediumFilePath) || Context.IsProcessing)
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }         
        }

        /// <summary>
        /// Command for Decode button.
        /// If password and encryption method is correct, retrieve and decryption secret file from medium.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DecodeCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                DisplayStandByGraphic("Decoding, please wait...");

                // Set encryptor according to radio button's value
                _manipulator.Encryptor = Context.Encryption == EncryptionOption.Aes ? _aesEncrypt : _noEncrypt;

                var mediumData = FileHelper.ReadFileFromDisk(Context.MediumFilePath).Item1;
                var secret = await Task.Run(() =>
                    _manipulator.GetFileFromMedium(mediumData, TextPassword.Password));

                var filename = "secret_file";
                var filter = $"{secret.Extension} (*.{secret.Extension})|*.{secret.Extension}";
                var filePath = SelectSaveFileDialog(filter, secret.Extension, filename);

                // Process save file dialog box results
                if (filePath != null)
                {
                    File.WriteAllBytes(filePath, secret.Data);
                    Context.MediumFilePath = null;
                    Context.SecretFilePath = null;
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
        /// Display animation gif when background task is running.
        /// </summary>
        /// <param name="message"></param>
        private void DisplayStandByGraphic(string message)
        {
            Context.IsProcessing = true;
            Context.FormTitle = message;
            GifCtrl.StartAnimate();
        }

        /// <summary>
        /// Restore original layout of program.
        /// </summary>
        private void DisplayDefaultGraphic()
        {            
            GifCtrl.StopAnimate();
            Context.FormTitle = "Steganography GUI";
            Context.IsProcessing = false;
        }

        /// <summary>
        /// Display an open file dialog so that user can select a file as input.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="defaultExtension"></param>
        /// <returns></returns>
        private string SelectOpenFileDialog(string filter = "All files|*.*", string defaultExtension = null)
        {
            // Create OpenFileDialog 
            var dlg = new OpenFileDialog
            {
                // Set filter for file extension and default file extension
                DefaultExt = defaultExtension ?? string.Empty,
                Filter = filter,
                ValidateNames = false,
            };

            // Display OpenFileDialog by calling ShowDialog method 
            var result = dlg.ShowDialog();

            // If a file was selected, return its name; return null otherwise
            return result == true ? dlg.FileName : null;
        }

        /// <summary>
        /// Display an save file dialog so that user can select path to save output.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="defaultExtension"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        private string SelectSaveFileDialog(string filter = "All files|*.*", string defaultExtension = null,
            string filename = null)
        {
            var dlg = new SaveFileDialog
            {
                FileName = filename ?? string.Empty, // Default file name
                DefaultExt = defaultExtension ?? string .Empty, // Default file extension
                Filter = filter, // Filter files by extension
            };

            // Show save file dialog box
            var result = dlg.ShowDialog();

            // Process save file dialog box results
            return result == true ? dlg.FileName : null;
        }
    }
}
