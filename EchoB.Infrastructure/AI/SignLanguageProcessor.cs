using EchoB.Domain.AI;
using EchoB.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace EchoB.Infrastructure.AI
{

    public class SignLanguageProcessor : ISignLanguageProcessor, IDisposable
    {
        private readonly ClientWebSocket _webSocket;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private bool _isConnected;
        private readonly string _webSocketUrl;
        public SignLanguageProcessor(IOptions<AISettings> options)
        {
            _webSocket = new ClientWebSocket();
            _cancellationTokenSource = new CancellationTokenSource();
            _isConnected = false;
            _webSocketUrl = options.Value.WebSocketUrl;
        }

        public async Task ConnectAsync()
        {
            if (!_isConnected)
            {
                try
                {
                    await _webSocket.ConnectAsync(new Uri("ws://localhost:8765"), CancellationToken.None);
                    _isConnected = true;
                }
                catch (Exception ex)
                {
                    Serilog.Log.Error(ex, "Failed to connect to WebSocket server");
                    throw;
                }
            }
        }

        public async Task<string> ProcessFrameAsync(string base64Frame)
        {
            try
            {
                await ConnectAsync(); // Ensure WebSocket is connected

                // Send frame to Python WebSocket server
                var message = JsonSerializer.Serialize(new { frame = base64Frame });
                var buffer = Encoding.UTF8.GetBytes(message);
                await _webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

                // Receive response
                var receiveBuffer = new byte[1024 * 4];
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
                var responseJson = Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);
                var response = JsonSerializer.Deserialize<Dictionary<string, string>>(responseJson);

                if (response.ContainsKey("error"))
                {
                    Serilog.Log.Error("WebSocket server error: {Error}", response["error"]);
                    throw new Exception(response["error"]);
                }

                return response["text"];
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Error processing frame with WebSocket");
                throw;
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            if (_isConnected)
            {
                _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None).GetAwaiter().GetResult();
            }
            _webSocket.Dispose();
        }
    }
}
