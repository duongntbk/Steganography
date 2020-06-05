using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Steganography
{
    /// <summary>
    /// Context class for main window form.
    /// </summary>
    public class MainWindowContext : INotifyPropertyChanged
    {
        /// <summary>
        /// Backing field for MediumFilePath.
        /// </summary>
        private string _mediumFilePath;

        /// <summary>
        /// Backing field for SecretFilePath.
        /// </summary>
        private string _secretFilePath;

        /// <summary>
        /// Backing field for IsProcessing.
        /// </summary>
        private bool _isProcessing;

        /// <summary>
        /// Backing field for FormTitle.
        /// </summary>
        private string _formTitle;

        public MainWindowContext()
        {
            FormTitle = "Steganography GUI";
        }

        /// <summary>
        /// Binding path for medium file.
        /// </summary>
        public string MediumFilePath
        {
            get => _mediumFilePath;
            set
            {
                _mediumFilePath = value;
                // Notify UI of change in value of MediumFilePath.
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Binding path for secret file.
        /// </summary>
        public string SecretFilePath
        {
            get => _secretFilePath;
            set
            {
                _secretFilePath = value;
                // Notify UI of change in value of SecretFilePath.
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Flag to indicate whether an encode/decode process is currently running.
        /// </summary>
        public bool IsProcessing
        {
            get => _isProcessing;
            set
            {
                _isProcessing = value;
                // Notify UI of change in value of SecretFilePath.
                NotifyPropertyChanged();
            }
        }

        public string FormTitle
        {
            get => _formTitle;
            set
            {
                _formTitle = value;
                // Notify UI of change in value of SecretFilePath.
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Binding path for 2 radio buttons used to select encryption mode.
        /// The default encrpytion mode is AES.
        /// </summary>
        public EncryptionOption Encryption { get; set; } = EncryptionOption.Aes;

        /// <summary>
        /// Implement PropertyChanged of INotifyPropertyChanged.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Handler to notify UI of change in binding data.
        /// Use CallerMemberName to avoid hard coding UI elements' in code behind.
        /// </summary>
        /// <param name="propName"></param>
        public void NotifyPropertyChanged([CallerMemberName]string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
