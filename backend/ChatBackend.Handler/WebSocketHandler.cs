using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatBackend.Handler
{
    public abstract class WebSocketHandler
    {
        protected ConnectionManager WebSocketConnectionManager { get; set; }

        public WebSocketHandler(ConnectionManager webSocketConnectionManager)
        {
            WebSocketConnectionManager = webSocketConnectionManager;
        }

        public virtual void OnConnected(WebSocket socket, TaskCompletionSource<object> taskCompletionSource) =>
            WebSocketConnectionManager.AddSocket(socket, taskCompletionSource);

        public virtual async Task OnDisconnected(WebSocket socket) =>
            await WebSocketConnectionManager.RemoveSocket(WebSocketConnectionManager.GetId(socket));

        public async Task SendMessageAsync(WebSocket socket, string message)
        {
            if (socket.State != WebSocketState.Open) return;
            try
            {
                await socket.SendAsync(
                    buffer: new ArraySegment<byte>(array: Encoding.ASCII.GetBytes(message), offset: 0,
                        count: message.Length), messageType: WebSocketMessageType.Text, endOfMessage: true,
                    cancellationToken: CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public async Task SendMessageAsync(string socketId, string message) =>
            await SendMessageAsync(WebSocketConnectionManager.GetSocketById(socketId), message);

        public async Task SendMessageToAllAsync(string message)
        {
            foreach (var keyValuePair in WebSocketConnectionManager.GetAll())
            {
                if (keyValuePair.Value.State == WebSocketState.Open)
                {
                    await SendMessageAsync(keyValuePair.Value, message);
                }
            }
        }

        public abstract Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer);
    }
}