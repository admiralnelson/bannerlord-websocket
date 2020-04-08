Imports System.Net
Imports System.Net.Sockets
Imports System.Net.WebSockets
Imports System.Threading

Namespace Global.VBWebsocketServer



    Public Class Websocket
        Dim mIp = "127.0.0.1"
        Dim mPort = 9001
        Dim mConnectedClient = 0
        Dim mWebsocketListener As HttpListener
        Dim mListening = True
        Dim mThread As Thread


        Public Sub New(Optional ip As String = "localhost", Optional port As Integer = 9001)
            mIp = ip
            mPort = port
            mWebsocketListener = New HttpListener()
            mWebsocketListener.Prefixes.Add($"http://{mIp}:{mPort}/ws/")
            mWebsocketListener.Start()
            Print($"websocket is started at {port}:{ip}")
            mThread = New Thread(AddressOf ServerLoop)
            mThread.IsBackground = True
            mThread.Start()
        End Sub

        Private Sub ServerLoop()
            While (True)
                'mThreadEvent.WaitOne()
                ThreadPool.QueueUserWorkItem(
                    Sub(httpContext As HttpListenerContext)
                        If (httpContext.Request.IsWebSocketRequest) Then
                            ProcessLoop(httpContext)
                        Else
                            Print($"request from {httpContext.Request.RemoteEndPoint} is not a websocket")
                            httpContext.Response.StatusCode = 400
                            httpContext.Response.Close()
                        End If
                    End Sub, mWebsocketListener.GetContext())
            End While
        End Sub

        Private Async Sub ProcessLoop(httpContext As HttpListenerContext)
            Dim webSocketContext As WebSocketContext
            Try
                webSocketContext = Await httpContext.AcceptWebSocketAsync(subProtocol:=Nothing)
                Interlocked.Increment(mConnectedClient)
                Print($"processed {mConnectedClient}")
            Catch ex As Exception
                httpContext.Response.StatusCode = 500
                httpContext.Response.Close()
                Print($"websocket exception! {ex}")
                Exit Sub
            End Try

            Dim webSocketServer = webSocketContext.WebSocket

            Try
                Dim buffer(2048) As Byte
                While (webSocketServer.State = WebSocketState.Open)
                    Dim receivedResult = Await webSocketServer.ReceiveAsync(
                        New ArraySegment(Of Byte)(buffer),
                        CancellationToken.None)

                    If (receivedResult.MessageType = WebSocketMessageType.Close) Then
                        Await webSocketServer.CloseAsync(WebSocketCloseStatus.NormalClosure,
                                                         "",
                                                         CancellationToken.None)
                    ElseIf (receivedResult.MessageType = WebSocketMessageType.Text) Then
                        Dim receivedMsg = Text.Encoding.UTF8.GetString(buffer, 0,
                                                                       receivedResult.Count)
                        receivedMsg = $"bannerlord reply: {receivedMsg}"
                        Print($"client sends:{receivedMsg}")
                        Dim sendBuffer = Text.Encoding.UTF8.GetBytes(receivedMsg)
                        Await webSocketServer.SendAsync(New ArraySegment(Of Byte)(sendBuffer),
                                                        WebSocketMessageType.Text,
                                                        receivedResult.EndOfMessage,
                                                        CancellationToken.None)
                    End If
                End While

            Catch ex As Exception
                Print($"websocket exception {ex}")
            Finally
                If (webSocketServer IsNot Nothing) Then
                    webSocketServer.Dispose()
                End If
            End Try

        End Sub

        Public Sub ShutDown()
            mThread.Abort()
        End Sub


    End Class

End Namespace