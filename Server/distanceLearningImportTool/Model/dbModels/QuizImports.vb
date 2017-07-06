Imports Contensive.BaseClasses

Namespace Model.dbModels
    '
    Public Class QuizImports
        Inherits BasicRecord
        '
        '
        '
        Public Shared Function ExistCsvFilename(ByVal CP As CPBaseClass, recordId As Integer) As Boolean
            Dim result As Boolean = False
            Try
                Dim cs As CPCSBaseClass = CP.CSNew()
                If cs.Open(cnQuizCSVImports,"id=" & recordId) Then
                    '
                    'CP.Utils.AppendLog("ExistCsvFilename.log","Open record: " & recordId)

                    '
                    If Not String.IsNullOrEmpty(cs.GetText("csvFilename")) Then

                        dim filepath = CP.Site.PhysicalFilePath & cs.GetFilename("csvFilename").Replace("/","\")

                        If CP.File.fileExists(filepath) Then
                            result = True
                        End If

                    Else
                        'CP.Utils.AppendLog("ExistCsvFilename.log","csvFilename is empty")
                    End If
                    '
                End If
                Call cs.Close()
                '
            Catch ex As Exception
                CP.Site.ErrorReport(ex, "Unexpected error in QuizImports.ExistCsvFilename")
            End Try
            Return result
        End Function
        '
        '
        '
        Public Shared Function GetCsvFilename(ByVal CP As CPBaseClass, recordId As Integer) As String
            Dim result As String = ""
            Try
                Dim cs As CPCSBaseClass = CP.CSNew()
                If cs.Open(cnQuizCSVImports,"id=" & recordId) Then
                    '
                    result = CP.Site.PhysicalFilePath & cs.GetFilename("csvFilename").Replace("/","\")
                    '
                End If
                Call cs.Close()
                '
            Catch ex As Exception
                CP.Site.ErrorReport(ex, "Unexpected error in QuizImports.GetCsvFilename")
            End Try
            Return result
        End Function
        '
        '
        '
        Public Shared Function SetProcessError(ByVal CP As CPBaseClass, RecordId As Integer, ByRef errorList As List(Of Model.architectureModels.errorClass)) As Boolean
            Dim Result As Boolean =  False
            Try
                '
                Dim cs As CPCSBaseClass = CP.CSNew()
                Dim errorStr As String = "Process detect the follow errors:" & vbCrLf
                '
                For each oneError in errorList
                    errorStr &=  oneError.userMsg & vbCrLf
                Next
                '
                If cs.Open(cnQuizCSVImports,"id=" & recordId) Then
                    '
                    cs.SetField("importErrors", errorStr)
                    '
                End If
                Call cs.Close()
                '
            Catch ex As Exception
                CP.Site.ErrorReport(ex, "Unexpected error in QuizImports.SetProcessError")
            End Try
            Return Result
        End Function
        '
        '
        '
        Public Shared Function SetEndProcess(ByVal CP As CPBaseClass, RecordId As Integer) As Boolean
            Dim Result As Boolean =  False
            Try
                '
                Dim cs As CPCSBaseClass = CP.CSNew()
                '
                If cs.Open(cnQuizCSVImports,"id=" & recordId) Then
                    '
                    cs.SetField("dateimportcompleted", Now.ToString())
                    '
                End If
                Call cs.Close()
                '
            Catch ex As Exception
                CP.Site.ErrorReport(ex, "Unexpected error in QuizImports.SetEndProcess")
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
                CP.Site.ErrorReport(ex, "Unexpected error in QuizImports.SetEndProcess")
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
                CP.Site.ErrorReport(ex, "Unexpected error in Model.dbModels.Patient.SetPatientFile")
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

