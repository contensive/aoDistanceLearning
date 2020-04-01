Imports Contensive.BaseClasses

Namespace Model.dbModels
    '
    Public Class QuizImports
        Inherits BasicRecord
        '
        '
        '
        Public Shared Function existCsvFilename(ByVal CP As CPBaseClass, recordId As Integer) As Boolean
            Try
                Using cs As CPCSBaseClass = CP.CSNew()
                    If cs.Open(cnQuizCSVImports, "id=" & recordId) Then
                        If Not String.IsNullOrEmpty(cs.GetText("csvFilename")) Then
                            Return CP.CdnFiles.FileExists(cs.GetFilename("csvFilename").Replace("/", "\"))
                        End If
                    End If
                    Return False
                End Using
            Catch ex As Exception
                cp.Site.ErrorReport(ex, "Unexpected error in QuizImports.ExistCsvFilename")
                Return False
            End Try
        End Function
        '
        '
        '
        Public Shared Function getCsvFilename(ByVal CP As CPBaseClass, recordId As Integer) As String
            Try
                Using cs As CPCSBaseClass = CP.CSNew()
                    If cs.Open(cnQuizCSVImports, "id=" & recordId) Then
                        Return CP.CdnFiles.PhysicalFilePath & cs.GetFilename("csvFilename").Replace("/", "\")
                    End If
                    Return ""
                End Using
            Catch ex As Exception
                cp.Site.ErrorReport(ex, "Unexpected error in QuizImports.GetCsvFilename")
                Return ""
            End Try
        End Function
        '
        '
        '
        Public Shared Sub setProcessError(ByVal CP As CPBaseClass, RecordId As Integer, ByRef errorList As List(Of Model.architectureModels.errorClass))
            Try
                Using cs As CPCSBaseClass = CP.CSNew()
                    Dim errorStr As String = "Process detect the follow errors:" & vbCrLf
                    For Each oneError In errorList
                        errorStr &= oneError.userMsg & vbCrLf
                    Next
                    If cs.Open(cnQuizCSVImports, "id=" & RecordId) Then
                        cs.SetField("importErrors", errorStr)
                    End If
                End Using
            Catch ex As Exception
                cp.Site.ErrorReport(ex, "Unexpected error in QuizImports.SetProcessError")
            End Try
        End Sub
        '
        '
        '
        Public Shared Function setEndProcess(ByVal CP As CPBaseClass, RecordId As Integer) As Boolean
            Dim Result As Boolean = False
            Try
                '
                Dim cs As CPCSBaseClass = CP.CSNew()
                '
                If cs.Open(cnQuizCSVImports, "id=" & RecordId) Then
                    '
                    cs.SetField("dateimportcompleted", Now.ToString())
                    '
                End If
                Call cs.Close()
                '
            Catch ex As Exception
                cp.Site.ErrorReport(ex, "Unexpected error in QuizImports.SetEndProcess")
            End Try
            Return Result
        End Function
        '
        ' 
        '
        Public Shared Function SendNotificationEmail(ByVal CP As CPBaseClass, RecordId As Integer) As Boolean
            Dim Result As Boolean =  False
            Try
                '
                Dim cs As CPCSBaseClass = CP.CSNew()
                Dim email As String = ""
                '
                If cs.Open(cnQuizCSVImports,"id=" & recordId) Then
                    '
                    email = cs.GetText("notificationEmail")
                    '
                End If
                Call cs.Close()

                CP.Email.sendSystem("Quiz Import Notification Task")

                If Not String.IsNullOrEmpty(email) Then
                    Dim htmlBody As String = CP.Content.GetCopy("Quiz Import Process Task Content")
                    Dim fromEmail As String = cp.Site.GetText("Quiz Import Process From Email")
                    Dim subjectEmail As String = cp.Site.GetText("Quiz Import Process Subject Email")
                    CP.Email.send(email, fromEmail, subjectEmail, htmlBody)
                End If

                '
            Catch ex As Exception
                cp.Site.ErrorReport(ex, "Unexpected error in QuizImports.SetEndProcess")
            End Try
            Return Result
        End Function
        '
        '
        '
        Public Shared Function SetCSVFile(ByVal CP As CPBaseClass, ByRef RecordId As Integer, ByRef errorList As List(Of Model.architectureModels.errorClass)) As Boolean
            Dim result As Boolean = False
            Try
                Dim cs As CPCSBaseClass = CP.CSNew()

                If RecordId <> 0 Then
                    If cs.Open(cnQuizCSVImports,"id=" & recordId) Then
                        '
                        cs.SetFormInput("csvFilename", "csvFilename")
                        '
                    End If
                    Call cs.Close()
                    '
                Else
                    '
                    Dim oneError As New Model.architectureModels.errorClass
                    oneError.number = 320
                    oneError.userMsg = "App Id is not Valid."
                    errorList.Add(oneError)
                End If
                '
            Catch ex As Exception
                cp.Site.ErrorReport(ex, "Unexpected error in Model.dbModels.Patient.SetPatientFile")
                Dim oneError As New Model.architectureModels.errorClass
                oneError.number = 300
                oneError.userMsg = "Error in Set CSV File."
                errorList.Add(oneError)
            End Try
            Return result
        End Function
        '
        '
        '
    End Class
    '
End Namespace

