using System;
using System.Collections.Generic;
using System.Linq;
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
        const string DefaultDomain = "mstdn.maud.io";

        /// <summary>
        /// Send message buffer size.
        /// </summary>
        const int MessageBufferSize = 4096;


        async public static Task Main(string[] args)
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

            var uri = new UriBuilder("wss", domain, 443, "/api/v1/streaming", $"?access_token={token}&stream=public")
                .Uri;

            Console.WriteLine(uri);

            var ws = new ClientWebSocket();
            try
            {
                await ws.ConnectAsync(uri, CancellationToken.None);
                var buff = new ArraySegment<byte>(new byte[MessageBufferSize]);
                while (ws.State == WebSocketState.Open)
                {
                    var ret = await ws.ReceiveAsync(buff, CancellationToken.None);

                    var data = new UTF8Encoding().GetString(buff.Take(ret.Count).ToArray());

                    var ev = JsonConvert.DeserializeObject<StreamEvent>(data);
                    switch (ev.Event)
                    {
                        case "update":
                            printStatus(parseStatus(ev.Payload));
                            break;
                        default:
                            Console.WriteLine("{0}: {1}", ev.Event, ev.Payload);
                            break;
                    }
                }
                Console.WriteLine("Closed?");
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine("{0} {1} {2}", ex.Message, ex.ErrorCode.ToString(), ex.NativeErrorCode.ToString());
            }
        }

        static Status parseStatus(string payload)
        {
            return JsonConvert.DeserializeObject<Status>(payload);
        }

        static void printStatus(Status status)
        {
            Console.WriteLine("@{0}: {1}", status.Account.AccountName, status.Content);
        }
    }
}