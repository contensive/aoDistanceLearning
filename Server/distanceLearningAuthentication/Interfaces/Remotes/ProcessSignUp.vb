Imports Contensive.BaseClasses

Namespace Interfaces.Remotes
    '
    Public Class ProcessSignUp
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
                Dim SignUp = deserializeSignUpObject(cp, CP.Doc.GetText("jsonData"))
                '
                Call  Model.dbModels.User.CreateUser(CP, SignUp, remoteResponse.errors)
                '
                If remoteResponse.errors.Count > 0 Then
                    remoteResponse.data =   "Sign Up Process Errors" 
                Else
                    remoteResponse.data =   Model.dbModels.User.getUser(CP, SignUp.id)
                End If
                '
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
        Private Function deserializeSignUpObject(ByVal CP As CPBaseClass, ByVal value As String) As Model.solutionModels.SignUp
            Dim SignUpObject As New Model.solutionModels.SignUp
            Dim jsonSerializer As New Web.Script.Serialization.JavaScriptSerializer
            '
            Try
                '
                '
                '
                SignUpObject = jsonSerializer.Deserialize(Of Model.solutionModels.SignUp)(value)

            Catch ex As Exception
                CP.Site.ErrorReport(ex, "error in deserializeUserObject")
            End Try
            '
            Return SignUpObject
        End Function
        '
        '
        '
        Private Sub errorReport(ByVal cp As CPBaseClass, ByVal ex As Exception, ByVal method As String)
            Try
                cp.Site.ErrorReport(ex, "Unexpected error in ProcessSignUp." & method)
            Catch exLost As Exception
                '
                ' 
                '
            End Try
        End Sub
    End Class
    '
End Namespace