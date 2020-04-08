Imports System.IO
Imports System.Reflection
Imports TaleWorlds.CampaignSystem
Imports TaleWorlds.Core
Imports TaleWorlds.MountAndBlade

Namespace Global.VBWebsocketServer
    Public Class Main
        Inherits MBSubModuleBase
        Dim webServer As Webserver
        Dim webSocket As Websocket
        Protected Overrides Sub OnSubModuleLoad()
            MyBase.OnSubModuleLoad()
            webServer = New Webserver()
            webSocket = New Websocket()
        End Sub

        Protected Overrides Sub OnGameStart(game As Game, gameStarterObject As IGameStarter)
            Dim campaign = game.GameType
            If (campaign Is Nothing) Then
                Debug.WriteLine("oops!")
                Exit Sub
            End If

            Dim campaignStarter = CType(gameStarterObject, CampaignGameStarter)
            AddBehaviour(campaignStarter)
        End Sub

        Private Sub AddBehaviour(gameInit As CampaignGameStarter)
            'gameInit.AddBehavior(New SimpleDayCounter)
        End Sub

        Protected Overrides Sub OnApplicationTick(dt As Single)
            MyBase.OnApplicationTick(dt)

        End Sub

        Protected Overrides Sub OnBeforeInitialModuleScreenSetAsRoot()
            MyBase.OnBeforeInitialModuleScreenSetAsRoot()
            Dim location = GetMyLocation()
            MessageBox("my dll is located at", location)
        End Sub

    End Class

End Namespace