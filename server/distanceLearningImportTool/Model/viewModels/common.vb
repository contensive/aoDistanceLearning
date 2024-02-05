Imports Contensive.BaseClasses

Namespace Model.viewModels
    '
    Public Module common
        '
        Public Const divMultiFormAjaxFrameDiv = "multiFormAjaxFrame"
        Public Const jsMultiformAjaxVbFrameRqs = "multiformAjaxVbFrameRqs"
        '
        '
        ' request names 
        '
        Public Const rnAppId As String = "appId"
        Public Const rnSrcFormId As String = "srcFormId"
        Public Const rnDstFormId As String = "dstFormId"
        Public Const rnButton As String = "button"
        '
        '
        '
        Public Const cnMultiFormAjaxApplications = "Quiz CSV Imports"
        '
        '
        '
        Public Const buttonOK As String = "OK"
        Public Const buttonSave As String = "Save"
        Public Const buttonCancel As String = "Cancel"
        Public Const buttonNext As String = "Next"
        Public Const buttonPrevious As String = "Previous"
        Public Const buttonContinue As String = "Continue"
        Public Const buttonRestart As String = "Restart"
        Public Const buttonFinish As String = "Finish"
        '
        '
        Public Const formIdFileSelection As Integer = 1
        Public Const formIdEmailNotification As Integer = 2
        Public Const formIdThankYou As Integer = 3
        '
        '
        Friend Function encodeMinDate(ByVal sourceDate As Date) As Date
            Dim returnValue As Date = sourceDate
            If returnValue < #1/1/1900# Then
                returnValue = Date.MinValue
            End If
            Return returnValue
        End Function
        '
        '
        '
        Friend Function getRightNow(ByVal cp As Contensive.BaseClasses.CPBaseClass) As Date
            Dim returnValue As Date = Date.Now()
            Try
                Dim testString As String = cp.Site.GetProperty("Sample Manager Test Mode Date", "")
                '
                ' change 'sample' to the name of this collection
                '
                If testString <> "" Then
                    returnValue = encodeMinDate(cp.Utils.EncodeDate(testString))
                    If returnValue = Date.MinValue Then
                        returnValue = Date.Now()
                    End If
                End If
            Catch ex As Exception
                Call cp.Site.ErrorReport(ex, "Error in getRightNow")
            End Try
            Return returnValue
        End Function
        '
        '
        '

        Friend Function getApplication(ByVal cp As CPBaseClass, ByVal createRecordIfMissing As Boolean) As applicationClass
            Dim application As applicationClass = New applicationClass
            Try
                Dim cs As CPCSBaseClass = cp.CSNew()
                '
                application.completed = False
                application.changed = False
                application.id = cp.Visit.GetInteger(cnMultiFormAjaxApplications & " ApplicationId")
                '
                If cs.Open(cnMultiFormAjaxApplications,"id=" & application.id) Then
                    '
                    application.notificationEmail = cs.GetText("notificationEmail")
                    '
                Else
                    '
                    Call cs.Close()
                    If Not cs.Open(cnMultiFormAjaxApplications, "(dateCompleted is null)") Then
                        Call cs.Close()
                        Call cs.Insert(cnMultiFormAjaxApplications)
                    End If
                    '
                    application.id = cs.GetInteger("id")
                    application.notificationEmail = ""
                    Call cp.Visit.SetProperty(cnMultiFormAjaxApplications & " ApplicationId", application.id)
                    '
                End If
                Call cs.Close()
                '
            Catch ex As Exception
                Call cp.Site.ErrorReport(ex, "Error in getApplication")
            End Try
            Return application
        End Function
        '
        '
        '
        Public Sub saveApplication(ByVal cp As CPBaseClass, ByVal application As applicationClass, ByVal rightNow As Date)
            Try
                Dim cs As CPCSBaseClass = cp.CSNew()
                If application.changed Then
                    If application.id > 0 Then
                        Call cs.Open(cnMultiFormAjaxApplications, "(id=" & application.id & ")")
                    End If
                    If Not cs.OK Then
                        If cs.Insert(cnMultiFormAjaxApplications) Then
                            '
                            ' create a new record. 
                            ' Set application Id in case needed later
                            ' Set visit property to save the application Id
                            '
                            application.id = cs.GetInteger("id")
                            Call cp.Visit.SetProperty(cnMultiFormAjaxApplications & " ApplicationId", application.id.ToString())
                        End If
                    End If
                    If cs.OK Then
                        Call cs.SetField("notificationEmail", application.notificationEmail)
                        If application.completed Then
                            Call cs.SetField("datecompleted", rightNow.ToString)
                            Call cp.Visit.SetProperty(cnMultiFormAjaxApplications & " ApplicationId", "0")
                        End If
                    End If
                    Call cs.Close()
                End If
            Catch ex As Exception
                Call cp.Site.ErrorReport(ex, "Error in getApplication")
            End Try
        End Sub
        '
        '
        '
    End Module
    '
    Public Class applicationClass
        Public id As Integer
        Public changed As Boolean
        Public Completed As Boolean
        'Public csvFilename As String
        Public notificationEmail As String
        Public errorMsg As String
    End Class
    '
    ' base class for the form classes. Having this means in he form handler you can only create one object, not one per form
    '
    Public MustInherit Class formBaseClass
        Friend MustOverride Function processForm(ByVal cp As CPBaseClass, ByVal srcFormId As Integer, ByVal rqs As String, ByVal rightNow As Date, ByRef application As applicationClass) As Integer

        Friend MustOverride Function getForm(ByVal cp As CPBaseClass, ByVal dstFormId As Integer, ByVal rqs As String, ByVal rightNow As Date, ByRef application As applicationClass) As String
    End Class

    '
End Namespace

