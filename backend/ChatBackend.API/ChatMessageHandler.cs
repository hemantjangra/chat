using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using ChatBackend.Handler;

namespace ChatBackend.API
{
    public class ChatMessageHandler : WebSocketHandler
    {
        public ChatMessageHandler(ConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {
        }

        public override void OnConnected(WebSocket socket, TaskCompletionSource<object> taskCompletionSource)
        {
            base.OnConnected(socket, taskCompletionSource);

            var socketId = WebSocketConnectionManager.GetId(socket);
            var task = Task.Run(() => SendMessageToAllAsync($"{socketId} is now connected"));
            if (task.IsFaulted || task.Exception != null)
            {
                //log message or exception
                return;
            }
        }

        public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var socketId = WebSocketConnectionManager.GetId(socket);
            var message = $"{socketId} said: {Encoding.UTF8.GetString(buffer, 0, result.Count)}";

            await SendMessageToAllAsync(message);
        }
    }
}