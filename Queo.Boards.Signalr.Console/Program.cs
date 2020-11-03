using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using Queo.Boards.Hubs;
using RestSharp;
using RestSharp.Extensions;

namespace Queo.Boards.Signalr.Console {
    class Program {
        // private const string APP_URL = "https://localhost:44376";
        private const string APP_URL = "https://demobereich03-cs-appsrv02.dev.queo-group.com";

        private const string AUTH_URL = APP_URL + "/Token";

        private const string BOARD_CHANNEL_HUB = "BoardChannelHub";
        private const string SIGNAL_R_URL = APP_URL + "/signalr";

        public static string ReadPassword() {
            StringBuilder pwdBuilder = new StringBuilder();
            while (true) {
                ConsoleKeyInfo i = System.Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter) {
                    break;
                } else if (i.Key == ConsoleKey.Backspace) {
                    if (pwdBuilder.Length > 0) {
                        pwdBuilder.Remove(pwdBuilder.Length - 1, 1);
                        System.Console.Write("\b \b");
                    }
                } else {
                    pwdBuilder.Append(i.KeyChar);
                    System.Console.Write("*");
                }
            }
            return pwdBuilder.ToString();
        }

        private static HubConnection Connect() {
            System.Console.Write("To: ");
            string to = System.Console.ReadLine();

            System.Console.Write("Username: ");
            string username = System.Console.ReadLine();

            System.Console.Write("Passwort: ");
            string password = ReadPassword();

            IDictionary<string, string> queryParameterCollection = new Dictionary<string, string>();
            string queryParameterString = null;
            System.Console.WriteLine();
            System.Console.Write("QueryParameter: ");
            queryParameterString = System.Console.ReadLine();
            if (!string.IsNullOrEmpty(queryParameterString)) {
                string[] queryParameters = queryParameterString.Split(new[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string queryParameter in queryParameters) {
                    string[] parameter = queryParameter.Split(new[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (parameter.Length == 2) {
                        queryParameterCollection.Add(parameter[0].Trim(), parameter[1]);
                    } else {
                        queryParameterCollection.Add(parameter[0].Trim(), "");
                    }
                }
            }

            try {
                HubConnection connection = new HubConnection(SIGNAL_R_URL, queryParameterCollection);
                connection.Headers.Add("Authorization", "bearer " + GetAuthToken(username, password));
                IHubProxy hubProxy = connection.CreateHubProxy(to);
                hubProxy.On("Execute",
                    delegate(ChannelEvent data) {
                        System.Console.WriteLine("Connection '{0}' => Notification: Execute", connection.ConnectionId);
                        System.Console.WriteLine("Data: " + Environment.NewLine + " Command: " + Environment.NewLine + data.Command + Environment.NewLine + " with payload: " + Environment.NewLine + data.Payload);
                    });

                hubProxy.On("Info",
                    delegate (string message) {
                        System.Console.WriteLine("Connection '{0}' => Info: {1}", connection.ConnectionId, message);
                    });

                connection.StateChanged += delegate {
                    System.Console.WriteLine("Connection '{0}' => Status geändert zu '{1}'", connection.ConnectionId, connection.State);
                };

                connection.Start();

                return connection;
            } catch (Exception e) {
                System.Console.WriteLine(e.Message);
            }

            return null;
        }

        private static string GetAuthToken(string username, string password) {
            RestClient client = new RestClient(AUTH_URL);
            RestRequest request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", string.Format("grant_type=password&userName={0}&password={1}", username.UrlEncode(), password.UrlEncode()), ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK) {
                throw new Exception("Ungültige Anmeldedaten");
            }

            dynamic responseJSon = JsonConvert.DeserializeObject(response.Content);

            return responseJSon.access_token;
        }

        static void Main(string[] args) {
            ServicePointManager.ServerCertificateValidationCallback +=
                    (sender, certificate, chain, sslPolicyErrors) => true;

            string input;
            do {
                input = System.Console.ReadLine();
                if (input == "connect") {
                    Connect();
                }
            }
            while (input != "exit");
        }
    }
}