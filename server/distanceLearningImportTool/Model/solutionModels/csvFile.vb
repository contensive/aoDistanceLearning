Imports Contensive.BaseClasses
Imports Microsoft.VisualBasic.FileIO

Namespace Model.solutionModels
    '
    Public Class csvFile
        '
        Public Shared Function GetCsvFileObjectList(ByVal CP As CPBaseClass, csv_file_path As String, ByRef errorList As List(Of Model.architectureModels.errorClass)) As List(Of csvFileRow)
            Dim csvFileRowList As New List(Of csvFileRow)
            Try
                '
                Dim Quizzes As New List(Of Quizzes)
                Dim rowNumber As Integer = 1
                Dim columnCount As Integer = 0
                Dim maxColumnCount As Integer = 0
                '
                Dim tfp As New TextFieldParser(csv_file_path)
                tfp.Delimiters = New String() {","}
                tfp.TextFieldType = FieldType.Delimited
                '
                Dim header As String() = tfp.ReadFields() ' skip header

                If header.Length = 10 Then
                    '
                    While tfp.EndOfData = False
                        rowNumber +=1
                        Dim fields As String() = tfp.ReadFields()
                        columnCount = fields.Length
                        '
                        ' Validate Number of columns
                        '
                        If columnCount >= 4 Then

                            Select Case fields.Length
                                Case 4
                                    csvFileRowList.Add(Model.solutionModels.csvFileRow.GetRow(CP, fields(0),fields(1),fields(2),columnCount-3,rowNumber,fields(3),"","","","","","" ))
                                Case 5
                                    csvFileRowList.Add(Model.solutionModels.csvFileRow.GetRow(CP, fields(0),fields(1),fields(2),columnCount-3,rowNumber,fields(3),fields(4),"","","","","" ))
                                Case 6
                                    csvFileRowList.Add(Model.solutionModels.csvFileRow.GetRow(CP, fields(0),fields(1),fields(2),columnCount-3,rowNumber,fields(3),fields(4),fields(5),"","","","" ))
                                Case 7
                                    csvFileRowList.Add(Model.solutionModels.csvFileRow.GetRow(CP, fields(0),fields(1),fields(2),columnCount-3,rowNumber,fields(3),fields(4),fields(5),fields(6),"","","" ))
                                Case 8
                                    csvFileRowList.Add(Model.solutionModels.csvFileRow.GetRow(CP, fields(0),fields(1),fields(2),columnCount-3,rowNumber,fields(3),fields(4),fields(5),fields(6),fields(7),"","" ))
                                Case 9
                                    csvFileRowList.Add(Model.solutionModels.csvFileRow.GetRow(CP, fields(0),fields(1),fields(2),columnCount-3,rowNumber,fields(3),fields(4),fields(5),fields(6),fields(7),fields(8),"" ))
                                Case 10
                                    csvFileRowList.Add(Model.solutionModels.csvFileRow.GetRow(CP, fields(0),fields(1),fields(2),columnCount-3,rowNumber,fields(3),fields(4),fields(5),fields(6),fields(7),fields(8),fields(9) ))
                            End Select
                            '
                            ' Order de list by Subject, rowNumber
                            '
                            '
                        Else
                            ' Number of columns are <> 10
                            CP.Utils.AppendLog("ProcessFile.log", "error in number of colums in row # " & rowNumber)
                        End If
                        '
                    End While
                    '
                    Dim SortedList = (csvFileRowList.OrderBy(Function(x) x.Subject).ThenBy(Function(x) x.rowNumber)).ToList()
                    csvFileRowList = SortedList
                    '
                Else
                    '
                    Dim oneError As New Model.architectureModels.errorClass
                    oneError.number = 100
                    oneError.userMsg = "Number of columns is different to 10."
                    errorList.Add(oneError)
                    '
                End If
                '
            Catch ex As Exception
                cp.Site.ErrorReport(ex, "Unexpected error in csvFile.ProcessFile")
                Dim oneError As New Model.architectureModels.errorClass
                oneError.number = 100
                oneError.userMsg = "Unexpected error in csvFile.ProcessFile."
                errorList.Add(oneError)
            End Try
            Return csvFileRowList
        End Function

    End Class
    '
End Namespace
