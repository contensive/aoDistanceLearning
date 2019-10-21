Imports Contensive.BaseClasses

Namespace Model.viewModels
    '
    Public Class QuizFormThankYou
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
                If Not String.IsNullOrEmpty(cp.Doc.GetText("csvFilename")) Then
                    If application.id = 0 Then
                        ' New App
                        ' create record and set site property
                        If cs.Insert(cnMultiFormAjaxApplications) Then
                            application.id = cs.GetInteger("id")
                            Call cp.Visit.SetProperty(cnMultiFormAjaxApplications & " ApplicationId", application.id.ToString())

                            Call cs.SetFormInput("csvFilename", "csvFilename")

                        End If
                        Call cs.Close()
                        '
                    Else 
                        ' Existing App
                        ' Open record and set site property
                        If cs.Open(cnMultiFormAjaxApplications, "id=" & application.id ) Then
                            Call cs.SetFormInput("csvFilename", "csvFilename")
                        End If
                        Call cs.Close()
                        '
                    End If
                    isInputOK = True
                    '
                Else
                    '
                    application.errorMsg = "CSV file is required."
                    isInputOK = False
                    '
                End If
                '
                If isInputOK Then
                    '
                    Select Case button
                        Case buttonNext
                            nextFormId = formIdEmailNotification
                    End Select

                Else
                    '
                    nextFormId = formIdEmailNotification
                    '
                End If
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
                Dim firstName As String = ""
                '
                Call layout.OpenLayout("{4d840be4-ba54-4328-8262-66c7427827a1}") ' 
                '
                Call layout.SetInner("#mfaContent", cp.Content.GetCopy("Import Quiz Tool Thank You"))
                '
                ' wrap it in a form for the javascript to use during submit
                '
                body = layout.GetHtml()
                body &= cp.Html.Hidden(rnAppId, application.id)
                body &= cp.Html.Hidden(rnSrcFormId, dstFormId.ToString)
                returnHtml = cp.Html.Form(body, "", "", "mfaForm3", rqs)
            Catch ex As Exception
                errorReport(ex, cp, "getForm")
            End Try
            Return returnHtml
        End Function
        '
        '
        '
        Private Sub errorReport(ByVal ex As Exception, ByVal cp As CPBaseClass, ByVal method As String)
            cp.Site.ErrorReport(ex, "error in QuizFormThankYou." & method)
        End Sub

    End Class
    '
End Namespace
