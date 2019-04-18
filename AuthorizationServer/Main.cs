using System;
using System.Security.Cryptography;
using System.Windows.Forms;
using Authorization;

namespace AuthServer
{
    public partial class Main : Form
    {
        AuthorizationServer _server;

        public Main()
        {
            InitializeComponent();

            _server = new AuthorizationServer();
            _server.ReplyAsync();
        }
        
        private void RegisterButton_Click(object sender, EventArgs e)
        {
            string username = UsernameTextbox.Text;
            string password = PasswordTextbox.Text;

            _server.RegisterNewUser(username, password, new SHA256Managed());
        }
    }
}
