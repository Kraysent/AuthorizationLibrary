using System;
using System.Security.Cryptography;
using System.Windows.Forms;
using Authorization;

namespace AuthClient
{
    public partial class Main : Form
    {
        private AuthorizationClient _client;

        private const string SERVER_ADDRESS = @"D:\Coding\C#\AuthorizationServer\AuthorizationServer\bin\Debug\";

        public Main()
        {
            InitializeComponent();
        }
        
        private async void SendButton_Click(object sender, EventArgs e)
        {
            string username = UsernameTextbox.Text;
            string password = PasswordTextbox.Text;

            _client = new AuthorizationClient(username, password, SERVER_ADDRESS);
            await _client.SendAuthRequest(new SHA256Managed());
            bool Result = await _client.GetAnswerAsync();
            
            if (Result == true)
            {
                Log($"Access granted. Token: {_client.AccessToken}.");
            }
            else
            {
                Log("Access denied.");
            }
        }
        
        private void Log(string t) => LogListbox?.Invoke(new Action(() => LogListbox.Items.Add(t)));

        private void Main_Load(object sender, EventArgs e)
        {

        }
    }
}
