using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Authorization
{
    public class AuthorizationClient
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ServerAddress { get; set; }
        public AuthorizationToken AccessToken { get; set; }

        public AuthorizationClient(string username, string password, string serverAddress)
        {
            Username = username;
            Password = password;
            ServerAddress = serverAddress;
        }

        public async Task SendAuthRequest(HashAlgorithm algorithm)
        {
            AuthorizationRequest request = new AuthorizationRequest
            {
                Username = Username,
                PasswordHash = HashPassword(algorithm, Password)
            };

            await SendRequestAsync(request);
        }

        public async Task<bool> GetAnswerAsync()
        {
            bool answerRecieved = false;
            AuthorizationAnswer output = new AuthorizationAnswer();
            
            await Task.Run(() =>
            {
                while (answerRecieved == false)
                {
                    List<string> contents = GetAnswersAsync().Result;

                    for (int i = 0; i < contents.Count; i++)
                    {
                        AuthorizationAnswer answer = new AuthorizationAnswer(contents[i]);

                        if (answer.Username == Username)
                        {
                            output = answer;

                            if (output.Result == false)
                            {
                                AccessToken = AuthorizationToken.NullToken;
                            }
                            else
                            {
                                AccessToken = output.Token;
                            }

                            answerRecieved = true;
                        }
                    }

                    UpdateAnswersAsync(Username);
                    Thread.Sleep(500);
                }
            });

            return output.Result;
        }

        //-------------------------------------------------------------------//

        private string _requestsPath => ServerAddress + "requests.txt";
        private string _answersPath => ServerAddress + "answers.txt";

        private async Task SendRequestAsync(AuthorizationRequest req)
        {
            string request = $"{AuthorizationRequest.Header}-{req.Username}-{req.PasswordHash}";
            await AppendLinesAsync(_requestsPath, new List<string>() { request });
        }

        private async Task<List<string>> GetAnswersAsync()
        {
            return await ReadAllLinesAsync(_answersPath);
        }

        private async void UpdateAnswersAsync(string username)
        {
            List<string> contents = await ReadAllLinesAsync(_answersPath);
            List<AuthorizationAnswer> answers = contents.Select(x => new AuthorizationAnswer(x)).ToList();
            
            answers.RemoveAll(x => x.Username == username);
            contents = answers.Select(x => x.ToString()).ToList();

            await WriteAllLinesAsync(_answersPath, contents);
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

        //-------------------------------------------------------------------//

        private static async Task<List<string>> ReadAllLinesAsync(string filename)
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

        private static async Task WriteAllLinesAsync(string filename, List<string> lines)
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {
                foreach (string str in lines)
                {
                    await writer.WriteLineAsync(str);
                }
            }
        }
        
        private static async Task AppendLinesAsync(string filename, List<string> lines)
        {
            List<string> lst = await ReadAllLinesAsync(filename);
            lst.AddRange(lines);

            await WriteAllLinesAsync(filename, lst);
        }
    }
}
