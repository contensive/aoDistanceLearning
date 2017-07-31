Imports Contensive.BaseClasses

Namespace Model.viewModels
    '
    Public Class QuizImportCSVHandler
        Inherits AddonBaseClass
        '
        Public Overrides Function Execute(ByVal CP As CPBaseClass) As Object
            Dim returnHtml As String = ""
            Try
                '
                Dim body As String = ""
                Dim form As formBaseClass
                Dim rqs As String = CP.Doc.GetProperty(jsMultiformAjaxVbFrameRqs)
                Dim rightNow As Date = getRightNow(CP)
                Dim srcFormId As Integer = CP.Utils.EncodeInteger(CP.Doc.GetProperty(rnSrcFormId))
                Dim dstFormId As Integer = CP.Utils.EncodeInteger(CP.Doc.GetProperty(rnDstFormId))
                Dim formHandler As QuizImportCSVHandler = New QuizImportCSVHandler
                Dim application As applicationClass
                '
                ' get previously started application
                '
                application = getApplication(CP, False)
                '
                ' if there is no application, only allow form one
                '
                'cp.Utils.AppendLog("QuizImportCSVHandler.log","application.id: " & application.id)

                'cp.Utils.AppendLog("QuizImportCSVHandler.log","rnSrcFormId: " & CP.Doc.GetProperty(rnSrcFormId))
                'cp.Utils.AppendLog("QuizImportCSVHandler.log","rnDstFormId: " & CP.Doc.GetProperty(rnDstFormId))

                If application.id = 0 Then
                    If srcFormId <> formIdFileSelection Then
                        srcFormId = 0
                    End If
                    dstFormId = formIdFileSelection
                End If
                '
                ' process forms
                '
                If (srcFormId <> 0) Then
                    Select Case srcFormId
                        Case formIdFileSelection
                            '
                            form = New QuizFormFileSelection
                            dstFormId = form.processForm(CP, srcFormId, rqs, rightNow, application)
                        Case formIdEmailNotification
                            '
                            form = New QuizFormEmailNotification
                            dstFormId = form.processForm(CP, srcFormId, rqs, rightNow, application)
                        Case formIdThankYou
                            '
                            form = New QuizFormThankYou
                            dstFormId = form.processForm(CP, srcFormId, rqs, rightNow, application)
                    End Select
                End If
                '
                '
                '
                Select Case dstFormId
                    Case formIdThankYou
                        '
                        '
                        '
                        form = New QuizFormThankYou
                        body = form.getForm(CP, dstFormId, rqs, rightNow, application)
                    Case formIdEmailNotification
                        '
                        '
                        '
                        form = New QuizFormEmailNotification
                        body = form.getForm(CP, dstFormId, rqs, rightNow, application)
                    Case Else
                        '
                        ' default form
                        '
                        dstFormId = formIdFileSelection
                        form = New QuizFormFileSelection
                        body = form.getForm(CP, dstFormId, rqs, rightNow, application)
                End Select
                Call saveApplication(CP, application, rightNow)
                '
                ' assemble body
                '
                returnHtml = body
            Catch ex As Exception
                CP.Site.ErrorReport(ex, "error in formHandlerClass.execute")
            End Try
            Return returnHtml
        End Function
    End Class
    '
End Namespace
