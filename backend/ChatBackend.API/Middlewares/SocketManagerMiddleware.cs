using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using ChatBackend.Handler;
using Microsoft.AspNetCore.Http;

namespace ChatBackend.Middleware
{
    public class SocketManagerMiddleware
    {
        private readonly RequestDelegate _next;
        private WebSocketHandler _webSocketHandler { get; set; }

        public SocketManagerMiddleware(RequestDelegate next, WebSocketHandler webSocketHandler)
        {
            _next = next;
            _webSocketHandler = webSocketHandler;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest) return;

            using (var socket = await context.WebSockets.AcceptWebSocketAsync())
            {
                //solve handshake problems line 28
                var socketFinishedTcs = new TaskCompletionSource<object>();
                _webSocketHandler.OnConnected(socket, socketFinishedTcs);
                await socketFinishedTcs.Task;
            }
        }

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024*4];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                handleMessage(result, buffer);
                
                //recommendation
                if (result.CloseStatus != null)
                    await socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription,
                        CancellationToken.None);
            }
            
        }
    }
}