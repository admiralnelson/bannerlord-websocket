Imports System.Dynamic
Imports System.Net
Imports System.Threading
Imports TaleWorlds.CampaignSystem
Imports WebSocketSharp
Imports WebSocketSharp.Server
Imports Newtonsoft.Json


Namespace Global.VBWebsocketServer

    Public Class Websocket
        Dim mServer As WebSocketServer
        Dim mThread As Thread
        Friend Class MyHub
            Inherits WebSocketBehavior
            Dim connected = False
            Protected Overrides Sub OnClose(e As CloseEventArgs)
                connected = False
                MyBase.OnClose(e)
            End Sub

            Protected Overrides Sub OnError(e As ErrorEventArgs)
                MyBase.OnError(e)
            End Sub

            Protected Overrides Sub OnMessage(e As MessageEventArgs)
                MyBase.OnMessage(e)
                Dim clientIPAddress = Context.UserEndPoint.Address
                Print($"got msg from {clientIPAddress} : {e.Data}")
                Send($"reply {e.Data}")
            End Sub

            Protected Overrides Sub OnOpen()
                MyBase.OnOpen()
                connected = True
                Dim clientIPAddress = Context.UserEndPoint.Address
                Print($"connected {clientIPAddress}")
                While connected
                    Try
                        For Each party In Campaign.Current.Parties

                            If (party.IsMobile) Then
                                Dim obj As Object = New ExpandoObject()
                                obj.position = party.Position2D
                                obj.name = party.Name
                                If (party.Owner IsNot Nothing) Then
                                    obj.hero = party.Owner.Name
                                End If
                                obj.hero = party.Id
                                Dim data = JsonConvert.SerializeObject(obj)
                                Send(data)
                            End If
                        Next
                    Catch ex As Exception
                        Print($"an exception while sending data: {ex.Message}")
                    End Try
                End While
            End Sub
        End Class

        Public Sub New(Optional ip As String = "localhost",
                       Optional port As Integer = 9001)
            Dim url = $"ws://{ip}:{port}"
            mServer = New WebSocketServer(port)
            mServer.AddWebSocketService(Of MyHub)("/main")
            Debug.Print(mServer.IsListening)
            For Each x In mServer.WebSocketServices.Paths
                Debug.WriteLine(x)
            Next

        End Sub
        Public Sub Start()
            mServer.Start()
            Debug.WriteLine("Listening on port {0}, and providing WebSocket services:", mServer.Port)
        End Sub

        Public Sub [Stop]()
            mServer.Stop()
            Debug.WriteLine("Stopping services...")
        End Sub

    End Class

End Namespace