Imports Contensive.BaseClasses

Namespace Model.viewModels
    '
    Public Class QuizFormEmailNotification
        Inherits formBaseClass
        '
        '
        '
        Friend Overrides Function processForm(ByVal cp As CPBaseClass, ByVal srcFormId As Integer, ByVal rqs As String, ByVal rightnow As Date, ByRef application As applicationClass) As Integer
            Dim nextFormId As Integer = srcFormId
            Try
                Dim button As String
                Dim cs As CPCSBaseClass = cp.CSNew
                Dim isInputOK As Boolean = True
                '
                ' ajax routines return a different name for button
                '
                button = cp.Doc.GetText("ajaxButton")
                If button = "" Then
                    button = cp.Doc.GetText(rnButton)
                End If

                ' Check if exist a file to upload 
                If Not String.IsNullOrEmpty(cp.Doc.GetText("notificationEmail")) Then
                    '
                    application.notificationEmail = cp.Doc.GetText("notificationEmail")
                    '
                End If
                '
                Select Case button
                    Case buttonNext, buttonFinish
                        application.changed = true
                        application.Completed = True

                        ' ************
                        ' call the addon to process the csv upload

                        cp.Doc.SetProperty("recordId", application.id)
                        cp.Utils.ExecuteAddonAsProcess("{53588d6f-9915-4712-a9f8-f69624a5a0ec}")

                        ' ************

                        nextFormId = formIdThankYou
                End Select
                '
            Catch ex As Exception
                errorReport(ex, cp, "QuizFormFileSelection.processForm")
            End Try
            Return nextFormId
        End Function
        '
        '
        '
        Friend Overrides Function getForm(ByVal cp As CPBaseClass, ByVal dstFormId As Integer, ByVal rqs As String, ByVal rightNow As Date, ByRef application As applicationClass) As String
            Dim returnHtml As String = ""
            Try
                Dim layout As CPBlockBaseClass = cp.BlockNew
                Dim cs As CPCSBaseClass = cp.CSNew
                Dim body As String
                '
                Call layout.OpenLayout("{92a6d8a6-b7f6-49d5-b59f-54ced0de0e70}")
                '

                Call layout.SetInner("#mfaContent", cp.Content.GetCopy("Import Quiz Tool email Notification"))
                If Not String.IsNullOrEmpty(application.errorMsg)
                    Call layout.SetInner("#mfaErrorWrapper", application.errorMsg)
                End If

                '
                ' wrap it in a form for the javascript to use during submit
                '
                body = layout.GetHtml()
                body &= cp.Html.Hidden(rnAppId, application.id)
                body &= cp.Html.Hidden(rnSrcFormId, dstFormId.ToString)
                returnHtml = cp.Html.Form(body, , , "mfaForm2", rqs)
            Catch ex As Exception
                errorReport(ex, cp, "getForm")
            End Try
            Return returnHtml
        End Function
        '
        '
        '
        Private Sub errorReport(ByVal ex As Exception, ByVal cp As CPBaseClass, ByVal method As String)
            cp.Site.ErrorReport(ex, "error in QuizFormEmailNotification." & method)
        End Sub
    End Class
    '
End Namespace
