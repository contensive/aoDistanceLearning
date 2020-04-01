Imports Contensive.BaseClasses

Namespace Interfaces.Remotes
    '
    Public Class ProcessCSVFile
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
                Dim recordId As Integer = CP.Doc.GetInteger("recordId")
                '
                'CP.Utils.AppendLog("ProcessCSVFile.log","recordId: " & recordId)
                '
                If recordId<>0 Then
                    '

                    If Model.dbModels.QuizImports.existCsvFilename(CP, recordId) Then
                        Dim filename= Model.dbModels.QuizImports.getCsvFilename(CP, recordId)

                        Dim csvObjectList = Model.solutionModels.csvFile.GetCsvFileObjectList(CP, filename, remoteResponse.errors)
                        '
                        Dim QuizObject = Model.solutionModels.Quizzes.GetQuizzesObject(CP, filename, csvObjectList, remoteResponse.errors)
                        '

                        If remoteResponse.errors.Count > 0 Then
                            Call Model.dbModels.QuizImports.setProcessError(CP, recordId, remoteResponse.errors)
                        Else
                            ' 
                            ' Process / Create database records
                            '
                            Call Model.solutionModels.Quizzes.CreateDBRecords(CP, QuizObject)
                            '
                            'remoteResponse.data = QuizObject
                        End If
                        '
                        Call Model.dbModels.QuizImports.setEndProcess(CP, recordId)
                        '
                        Call Model.dbModels.QuizImports.SendNotificationEmail(CP, recordId)
                        '

                    Else 
                        'CP.Utils.AppendLog("ProcessCSVFile.log","ExistCsvFilename: False")
                    End If
                    '
                Else
                    remoteResponse.data = "ProcessCSVFile Error"
                    ' send message error
                    Dim oneError As New Model.architectureModels.errorClass
                    oneError.number = 200
                    oneError.userMsg = "User Is Not Authenticate."
                    remoteResponse.errors.Add(oneError)
                End If
                

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
                cp.Site.ErrorReport(ex, "Unexpected error in GetPatients." & method)
            Catch exLost As Exception
                '
                ' 
                '
            End Try
        End Sub
        '
        '
        '
    End Class
    '
End Namespace
