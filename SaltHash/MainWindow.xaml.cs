using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace SaltHash
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public Tuple<string, string, string> GetHashPass(string plainText)
        {
            byte[] salt;
            Random random = new Random();
            int saltSize = random.Next(4, 8);
            salt = new byte[saltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            string text = plainText;
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            byte[] textBytesSalt = new byte[textBytes.Length + salt.Length];

            for (int i = 0; i < textBytes.Length; i++)
                textBytesSalt[i] = textBytes[i];

            for (int i = 0; i < salt.Length; i++)
                textBytesSalt[textBytes.Length + i] = salt[i];

            HashAlgorithm hash = new SHA256Managed();

            byte[] hashSaltBytes = hash.ComputeHash(textBytesSalt);
            byte[] hashBytes = hash.ComputeHash(textBytes);
            string hashSaltValue = Convert.ToBase64String(hashSaltBytes);
            string saltValue = Convert.ToBase64String(salt);
            string hashValue = Convert.ToBase64String(hashBytes);

            return Tuple.Create(hashSaltValue, saltValue, hashValue);
        }

        public string HashPass(string text, byte[] salt)
        {
            HashAlgorithm hash = new SHA256Managed();

            byte[] saltBytes = salt;
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            byte[] textBytesSalt = new byte[textBytes.Length + saltBytes.Length];

            for (int i = 0; i < textBytes.Length; i++)
                textBytesSalt[i] = textBytes[i];

            for (int i = 0; i < saltBytes.Length; i++)
                textBytesSalt[textBytes.Length + i] = saltBytes[i];

            byte[] hashSaltBytes = hash.ComputeHash(textBytesSalt);

            string hastValue = Convert.ToBase64String(hashSaltBytes);
            return hastValue;
        }

        private void bGenerateHash_Click(object sender, RoutedEventArgs e)
        {
            Tuple<string, string, string> hashPassword = GetHashPass(tbPasswordHash.Text);
            tbHashSalt.Text = hashPassword.Item1;
            tbSalt.Text = hashPassword.Item2;
            tbOnlyHash.Text = hashPassword.Item3;
        }

        private void bDecryptHash_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                tbHashDecrypt.Text = HashPass(tbPasswordHash.Text, Convert.FromBase64String(tbSalt.Text));
                if (tbHashDecrypt.Text == tbHashSalt.Text)
                    lError.Content = "Пароль верен";
                else
                    lError.Content = "Пароль не верен";
            }
            catch
            {
                MessageBox.Show("Данная соль невозможна", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
