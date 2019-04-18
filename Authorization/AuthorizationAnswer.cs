using System;

namespace Authorization
{
    public class AuthorizationAnswer
    {
        public const string Header = "AUTHORIZATION";
        public string Username { get; set; }
        public bool Result { get; set; }
        public AuthorizationToken Token { get; set; }

        public AuthorizationAnswer()
        {

        }
        public AuthorizationAnswer(string str, char delimeter = '-')
        {
            string[] contents = str.Split(delimeter);

            if (contents[0] == Header)
            {
                Username = contents[1];
                Result = (contents[2] == "0") ? false : true;
                Token = new AuthorizationToken((Result == true) ? contents[3] : "0");
            }
            else
            {
                throw new ArgumentException("Bad header.");
            }
        }

        public override string ToString()
        {
            return $"{Header}-{Username}-{((Result == true) ? 1 : 0)}-{Token}";
        }
    }
}
