using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TootStreamView.Entities;

namespace TootStreamView
{
    internal class Program
    {
        private const string DefaultDomain = "mstdn.maud.io";

        /// <summary>
        /// Send message buffer size.
        /// </summary>
        private const int MessageBufferSize = 4096;


        public static async Task Main(string[] args)
        {
            Console.WriteLine("Input instance domain [{0}]", DefaultDomain);
            var domain = Console.ReadLine();
            if (string.IsNullOrEmpty(domain))
            {
                domain = DefaultDomain;
            }

            string token;
            do
            {
                Console.WriteLine("Input your access token");
                token = Console.ReadLine();
            } while (string.IsNullOrEmpty(token));

            var param = new NameValueCollection {["stream"] = "public"};
            if (!string.IsNullOrEmpty(token))
            {
                param["access_token"] = token;
            }

            var uri = new UriBuilder("wss", domain, 443, "/api/v1/streaming", $"?{BuildQuery(param)}").Uri;

            Console.WriteLine(uri);

            var ws = new ClientWebSocket();
            try
            {
                await ws.ConnectAsync(uri, CancellationToken.None);
                while (ws.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult ret;
                    var bu = new byte[0];
                    do
                    {
                        var buff = new ArraySegment<byte>(new byte[MessageBufferSize]);

                        ret = await ws.ReceiveAsync(buff, CancellationToken.None);

                        bu = bu.Concat(buff.Take(ret.Count).ToArray()).ToArray();
                    } while (!ret.EndOfMessage);

                    var data = new UTF8Encoding().GetString(bu);

                    try
                    {
                        var ev = JsonConvert.DeserializeObject<StreamEvent>(data);
                        switch (ev.Event)
                        {
                            case "update":
                                PrintStatus(ParseStatus(ev.Payload));
                                break;
                            default:
                                Console.WriteLine("{0}: {1}", ev.Event, ev.Payload);
                                break;
                        }
                    }
                    catch (JsonReaderException ex)
                    {
                        Console.Error.WriteLine("JSON Parse Error: {0} {1} RAW:{2}", ex.Message, ex.StackTrace, data);
                    }
                }
                Console.WriteLine("Closed?");
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine("{0} {1} {2}", ex.Message, ex.ErrorCode.ToString(), ex.NativeErrorCode.ToString());
            }
        }

        private static Status ParseStatus(string payload)
        {
            return JsonConvert.DeserializeObject<Status>(payload);
        }

        private static void PrintStatus(Status status)
        {
            Console.WriteLine("@{0}: {1}", status.Account.AccountName, status.Content);
        }

        private static string BuildQuery(NameValueCollection nvc)
        {
            return string.Join("&",
                Array.ConvertAll(nvc.AllKeys, key => $"{WebUtility.UrlEncode(key)}={WebUtility.UrlEncode(nvc[key])}"));
        }
    }
}