using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Authorization
{
    public class AuthorizationServer
    {
        private const string USERS_PATH = "users.txt";
        private const string TOKENS_PATH = "tokens.txt";
        private const string REQUESTS_PATH = "requests.txt";
        private const string ANSWERS_PATH = "answers.txt";

        public async void RegisterNewUser(string username, string password, HashAlgorithm algorithm)
        {
            string request = $"{username}-{HashPassword(algorithm, password)}";
            await AppendLinesAsync(USERS_PATH, new List<string>() { request });
        }

        public async void ReplyAsync()
        {
            bool completed = false;

            while (!completed)
            {
                await Task.Run(() =>
                {
                    List<AuthorizationRequest> reqs = (GetRequests().Result).Select(x => new AuthorizationRequest(x)).ToList();
                    List<AuthorizationAnswer> answers = new List<AuthorizationAnswer>();

                    foreach (AuthorizationRequest req in reqs)
                    {
                        AuthorizationAnswer ans = new AuthorizationAnswer();

                        ans.Username = req.Username;
                        ans.Result = FindUserAsync(req.Username, req.PasswordHash).Result;

                        if (ans.Result == true)
                        {
                            ans.Token = AuthorizationToken.GenerateToken();
                            AddTokenAsync(ans.Token);
                        }
                        else
                        {
                            ans.Token = AuthorizationToken.NullToken;
                        }

                        SendAnswerAsync(ans);
                    }

                    ClearRequestsAsync();

                    Thread.Sleep(1000);
                });
            }
        }

        //---------------------------------------------------------------------//
        
        private async void ClearRequestsAsync()
        {
            await WriteAllLinesAsync(REQUESTS_PATH, new List<string> { });
        }

        private async Task<List<string>> GetRequests()
        {
            return await ReadAllLinesAsync(REQUESTS_PATH);
        }

        private async void SendAnswerAsync(AuthorizationAnswer ans)
        {
            string answer = $"{AuthorizationAnswer.Header}-{ans.Username}-{((ans.Result == false) ? 0 : 1)}-{ans.Token}";

            await AppendLinesAsync(ANSWERS_PATH, new List<string> { answer });
        }

        private string HashPassword(HashAlgorithm algorithm, string password)
        {
            byte[] bytes = algorithm.ComputeHash(Encoding.Unicode.GetBytes(password));
            string h = "";

            foreach (byte b in bytes)
            {
                h += b.ToString();
            }

            return h;
        }

        private async void AddTokenAsync(AuthorizationToken token)
        {
            await AppendLinesAsync(TOKENS_PATH, new List<string>() { token.Token });
        }

        private async Task<bool> FindUserAsync(string username, string password)
        {
            List<string> users = await ReadAllLinesAsync(USERS_PATH);

            return users.Any(x => x == $"{username}-{password}");
        }

        //---------------------------------------------------------------------//

        private async Task<List<string>> ReadAllLinesAsync(string filename)
        {
            string l;
            List<string> lines = new List<string>();

            using (StreamReader reader = new StreamReader(filename))
            {
                while ((l = await reader.ReadLineAsync()) != null)
                {
                    lines.Add(l);
                }
            }

            return lines;
        }

        private async Task WriteAllLinesAsync(string filename, List<string> lines)
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {
                foreach (string str in lines)
                {
                    await writer.WriteLineAsync(str);
                }
            }
        }

        private async Task AppendLinesAsync(string filename, List<string> lines)
        {
            List<string> lst = await ReadAllLinesAsync(filename);
            lst.AddRange(lines);

            await WriteAllLinesAsync(filename, lst);
        }
    }
}
