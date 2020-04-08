Imports System.IO
Imports System.Reflection
Imports TaleWorlds.Core
Namespace Global.VBWebsocketServer
    Module Utils
        Public Sub MessageBox(title As String, input As String,
                          Optional callbackOk As Action = Nothing, Optional buttonOk As String = "")
            If (buttonOk = "") Then buttonOk = "Accept"
            If (callbackOk Is Nothing) Then callbackOk = Sub()
                                                         End Sub
            InformationManager.ShowInquiry(New InquiryData(title, input, True, False, buttonOk, "",
            callbackOk,
            Sub()
            End Sub))
        End Sub
        Public Sub MessageBox(title As String, input As String, callbackOk As Action, callbackCancel As Action,
                          Optional buttonOk As String = "", Optional buttonCancel As String = "")
            If (buttonOk = "") Then buttonOk = "Accept"
            If (buttonCancel = "") Then buttonOk = "Cancel"
            InformationManager.ShowInquiry(New InquiryData(title, input, True, False, buttonOk, buttonCancel,
            callbackOk, callbackCancel))
        End Sub
        Public Function GetMyLocation() As String
            Return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
        End Function
        Public Sub Print(title As String)
            InformationManager.DisplayMessage(New InformationMessage(title))
        End Sub
    End Module
End Namespace