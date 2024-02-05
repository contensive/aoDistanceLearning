Imports Contensive.BaseClasses

Namespace Model.viewModels
    '
    Public Class QuizImportCSVForm
        Inherits AddonBaseClass
        '
        Public Overrides Function Execute(ByVal CP As CPBaseClass) As Object
            Dim returnHtml As String = ""
            '
            Try
                '
                '
                '
                Dim body As String = ""
                Dim rqs As String = CP.Doc.RefreshQueryString
                Dim formHandler As QuizImportCSVHandler = New QuizImportCSVHandler
                '
                '
                '
                Call CP.Doc.SetProperty("multiformAjaxVbFrameRqs", rqs)
                body = formHandler.Execute(CP)
                '
                '
                '
                Call CP.Doc.AddHeadJavascript("var " & jsMultiformAjaxVbFrameRqs & "='" & rqs & "';")
                returnHtml = CP.Html.div(body, "", "", divMultiFormAjaxFrameDiv)
            Catch ex As Exception
                cp.Site.ErrorReport(ex, "error in QuizImportCSVForm.execute")
            End Try
            Return returnHtml
        End Function
    End Class
    '
End Namespace
