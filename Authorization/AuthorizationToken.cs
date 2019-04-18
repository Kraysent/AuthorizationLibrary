using System;

namespace Authorization
{
    public class AuthorizationToken
    {
        public string Token { get; set; }

        public static AuthorizationToken NullToken => new AuthorizationToken("0");

        public static AuthorizationToken GenerateToken()
        {
            Random rnd = new Random();
            AuthorizationToken token = new AuthorizationToken(rnd.Next(10000).ToString());

            return token;
        }

        public AuthorizationToken(string token)
        {
            Token = token;
        }

        public override string ToString()
        {
            return Token.ToString();
        }
    }
}
