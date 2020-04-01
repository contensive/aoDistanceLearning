Imports Contensive.BaseClasses

Namespace Interfaces.Remotes
    '
    Public Class ProcessForgetPassword
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
                '
                dim email As String = cp.Doc.GetText("email")
                If Not String.IsNullOrEmpty(email) Then
                    '
                    Call Model.solutionModels.Login.ForgetPassword(cp, email)
                    '
                    remoteResponse.data = "Process Forget Password Ok"
                Else
                    remoteResponse.data = "Process Forget Password Error"
                    ' send message error
                    Dim oneError As New Model.architectureModels.errorClass
                    oneError.number = 240
                    oneError.userMsg = "Email Information is required."
                    remoteResponse.errors.Add(oneError)
                End If
                '
            Catch ex As Exception
                cp.Site.ErrorReport( ex, "execute")
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
