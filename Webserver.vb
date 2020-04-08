Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading

Namespace Global.VBWebsocketServer
    Public Class Webserver
        Private Shared mMimes As IDictionary = New Dictionary(Of String, String) From
        {
            {".asf", "video/x-ms-asf"},
            {".asx", "video/x-ms-asf"},
            {".avi", "video/x-msvideo"},
            {".bin", "application/octet-stream"},
            {".cco", "application/x-cocoa"},
            {".crt", "application/x-x509-ca-cert"},
            {".css", "text/css"},
            {".deb", "application/octet-stream"},
            {".der", "application/x-x509-ca-cert"},
            {".dll", "application/octet-stream"},
            {".dmg", "application/octet-stream"},
            {".ear", "application/java-archive"},
            {".eot", "application/octet-stream"},
            {".exe", "application/octet-stream"},
            {".flv", "video/x-flv"},
            {".gif", "image/gif"},
            {".hqx", "application/mac-binhex40"},
            {".htc", "text/x-component"},
            {".htm", "text/html"},
            {".html", "text/html"},
            {".ico", "image/x-icon"},
            {".img", "application/octet-stream"},
            {".iso", "application/octet-stream"},
            {".jar", "application/java-archive"},
            {".jardiff", "application/x-java-archive-diff"},
            {".jng", "image/x-jng"},
            {".jnlp", "application/x-java-jnlp-file"},
            {".jpeg", "image/jpeg"},
            {".jpg", "image/jpeg"},
            {".js", "application/x-javascript"},
            {".mml", "text/mathml"},
            {".mng", "video/x-mng"},
            {".mov", "video/quicktime"},
            {".mp3", "audio/mpeg"},
            {".mpeg", "video/mpeg"},
            {".mpg", "video/mpeg"},
            {".msi", "application/octet-stream"},
            {".msm", "application/octet-stream"},
            {".msp", "application/octet-stream"},
            {".pdb", "application/x-pilot"},
            {".pdf", "application/pdf"},
            {".pem", "application/x-x509-ca-cert"},
            {".pl", "application/x-perl"},
            {".pm", "application/x-perl"},
            {".png", "image/png"},
            {".prc", "application/x-pilot"},
            {".ra", "audio/x-realaudio"},
            {".rar", "application/x-rar-compressed"},
            {".rpm", "application/x-redhat-package-manager"},
            {".rss", "text/xml"},
            {".run", "application/x-makeself"},
            {".sea", "application/x-sea"},
            {".shtml", "text/html"},
            {".sit", "application/x-stuffit"},
            {".swf", "application/x-shockwave-flash"},
            {".tcl", "application/x-tcl"},
            {".tk", "application/x-tcl"},
            {".txt", "text/plain"},
            {".war", "application/java-archive"},
            {".wbmp", "image/vnd.wap.wbmp"},
            {".wmv", "video/x-ms-wmv"},
            {".xml", "text/xml"},
            {".xpi", "application/x-xpinstall"},
            {".zip", "application/zip"}
        }
        Private mThread As Thread
        Private mThreadEvent As ManualResetEvent = New ManualResetEvent(True)
        Private mRootPath = Path.Combine(GetMyLocation(), "..\..\http\")
        Private mListener As HttpListener
        Private mResponse As HttpListenerResponse
        Private mPrefix = "http://localhost:9000/"

        Private Function GetMimeType(fileName As String) As String
            Dim mimeType = "application/unknown"
            Dim ext = System.IO.Path.GetExtension(fileName).ToLower()
            Dim regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext)
            If (regKey IsNot Nothing AndAlso regKey.GetValue("Content Type") IsNot Nothing) Then
                mimeType = regKey.GetValue("Content Type").ToString()
            End If
            Return mimeType
        End Function

        Public Sub New(Optional prefix = "")
            If (prefix <> "") Then
                mPrefix = prefix
            End If
            Try
                mListener = New HttpListener()
                mListener.Prefixes.Add(mPrefix)
                mListener.Start()
                Print($"web server is online: {mPrefix}")
                Print($"home path is: {mRootPath}")
                mThread = New Thread(AddressOf ServerLoop)
                mThread.IsBackground = True
                mThread.Start()
            Catch ex As Exception
                MessageBox("failed", "failed to start the webserver!")
            End Try
        End Sub
        Private Sub ServerLoop()
            While (True)
                'mThreadEvent.WaitOne()
                ThreadPool.QueueUserWorkItem(AddressOf ProcessLoop, mListener.GetContext())
            End While
        End Sub

        Private Sub ProcessLoop(context As HttpListenerContext)
            Dim request = context.Request
            mResponse = context.Response
            Dim filename = request.RawUrl.Substring(1)
            Dim filePath = Path.Combine(mRootPath, filename)
            If (Not File.Exists(filePath)) Then
                SendErrorResponse(404, "file not found " + filename)
                Exit Sub
            End If
            Print($"VERB:{request.HttpMethod} request file {filename}")
            Dim mime = GetMimeType(filename)
            Dim fileStreaming = File.OpenRead(filePath)
            mResponse.ContentType = mime
            mResponse.ContentLength64 = (New FileInfo(filePath)).Length
            fileStreaming.CopyTo(mResponse.OutputStream)
            mResponse.OutputStream.Close()
        End Sub

        Private Sub SendErrorResponse(statusCode As Integer, reason As String)
            mResponse.ContentLength64 = 0
            mResponse.StatusCode = statusCode
            mResponse.StatusDescription = reason
            mResponse.Close()
            Print($"warning: status code {statusCode}, {reason}")
        End Sub

        Public Sub Quit()
            mThread.Suspend()
        End Sub

        Public Sub [Resume]()
            mThread.Resume()
        End Sub

        Public Sub ShutDown()
            mThread.Abort()
        End Sub




    End Class
End Namespace
