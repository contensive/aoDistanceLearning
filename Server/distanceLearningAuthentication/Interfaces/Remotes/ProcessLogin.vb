Imports Contensive.BaseClasses

Namespace Interfaces.Remotes
    '
    Public Class ProcessLogin
        Inherits AddonBaseClass
        '
        '
        '
        Public Overrides Function Execute(ByVal CP As CPBaseClass) As Object
            Dim returnHtml As String
            Dim jsonSerializer As New Web.Script.Serialization.JavaScriptSerializer
            Dim remoteResponse As New Model.architectureModels.remoteResponseObject
            Try
                '
                Call Model.solutionModels.Login.ProcessLogin(CP, deserializeLoginObject(CP, CP.Doc.GetText("jsonData")), remoteResponse.errors)
                If remoteResponse.errors.Count > 0 Then
                    remoteResponse.data = "Process Login Result Error"
                    CP.User.Logout()
                Else
                    remoteResponse.data = Model.dbModels.User.getUser(CP, CP.User.id)
                End If
                

            Catch ex As Exception
                errorReport(CP, ex, "execute")
                remoteResponse = New Model.architectureModels.remoteResponseObject With {.data = New Object, .errors = New List(Of Model.architectureModels.errorClass) From {New Model.architectureModels.errorClass With {.number = 1, .userMsg = "Internal Error"}}}
                ' http error
                CP.Response.SetStatus("500")
                '
            Finally
                returnHtml = jsonSerializer.Serialize(remoteResponse)
            End Try
            Return returnHtml
        End Function
        '
        '
        '
        Private Function deserializeLoginObject(ByVal CP As CPBaseClass, ByVal value As String) As Model.solutionModels.Login
            Dim SignUpObject As New Model.solutionModels.Login
            Dim jsonSerializer As New Web.Script.Serialization.JavaScriptSerializer
            '
            Try
                '
                '
                '
                SignUpObject = jsonSerializer.Deserialize(Of Model.solutionModels.Login)(value)

            Catch ex As Exception
                Try
                    CP.Site.ErrorReport(ex, "error in deserializeLoginObject")
                Catch errObj As Exception
                End Try
            End Try
            '
            Return SignUpObject
        End Function
        '
        '
        '
        Private Sub errorReport(ByVal cp As CPBaseClass, ByVal ex As Exception, ByVal method As String)
            Try
                cp.Site.ErrorReport(ex, "Unexpected error in Login." & method)
            Catch exLost As Exception
                '
                ' 
                '
            End Try
        End Sub
    End Class
    '
End Namespace
