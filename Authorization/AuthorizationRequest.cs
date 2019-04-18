namespace Authorization
{
    public class AuthorizationRequest
    {
        public const string Header = "AUTHORIZATION";
        public string Username { get; set; }
        public string PasswordHash { get; set; }

        public AuthorizationRequest()
        {

        }
        public AuthorizationRequest(string str, char delimeter = '-')
        {
            string[] split = str.Split(delimeter);

            if (split[0] == Header)
            {
                Username = split[1];
                PasswordHash = split[2];
            }
        }

        public override string ToString()
        {
            return $"{Username}-{PasswordHash}";
        }
    }
}
