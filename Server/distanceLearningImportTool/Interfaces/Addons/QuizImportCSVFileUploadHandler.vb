Imports Contensive.BaseClasses

Namespace Interfaces.Addons
    '
    Public Class QuizImportCSVFileUploadHandler
        Inherits AddonBaseClass
        '
        '
        '
        Public Overrides Function Execute(ByVal CP As CPBaseClass) As Object
            Dim returnHtml As String
            Dim jsonSerializer As New System.Web.Script.Serialization.JavaScriptSerializer
            Dim remoteResponse As New Model.architectureModels.remoteResponseObject
            Try
                '
                Dim recordId As Integer = CP.Doc.GetInteger("appId")

                'CP.Utils.AppendLog("QuizImportCSVFileUploadHandler.log","recordId: " & recordId)
                '
                 Call  Model.dbModels.QuizImports.SetCSVFile(cp, recordId, remoteResponse.errors)

                remoteResponse.data = "ok"

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
