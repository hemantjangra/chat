using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace ChatBackend.Handler
{
    public class ConnectionManager
    {
        private ConcurrentDictionary<string, WebSocket> _socket = new ConcurrentDictionary<string, WebSocket>();

        public WebSocket GetSocketById(string id) =>
             _socket.FirstOrDefault(socket => socket.Key == id).Value;

        public ConcurrentDictionary<string, WebSocket> GetAll() => 
            _socket;

        public string GetId(WebSocket socket) => 
            _socket.FirstOrDefault(soc=> soc.Value == socket).Key;
        
        public void AddSocket(WebSocket socket, TaskCompletionSource<object> taskCompletionSource){
            _socket.TryAdd(_createConnectionId(), socket);
        }

        public async Task RemoveSocket(string id)
        {
            _socket.TryRemove(id, out var socket);

            await socket.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure,
                statusDescription: "Closed By Connection Manager", cancellationToken: CancellationToken.None);
        }

        private readonly Func<string> _createConnectionId = () => Guid.NewGuid().ToString();
    }
}