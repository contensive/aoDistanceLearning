Imports Contensive.BaseClasses

Namespace Model.solutionModels
    '
    Public Class csvFileRow
        '
        Public Subject As String
        '
        Public rowNumber As Integer
        '
        Public Question As String
        Public Answer As String
        '
        Public OptionCount As Integer
        '
        Public OptionA As String 
        Public OptionB As String 
        Public OptionC As String 
        Public OptionD As String 
        Public OptionE As String 
        Public OptionF As String 
        Public OptionG As String 
        '
        Public Shared Function GetRow(ByVal CP As CPBaseClass,
                                      Subject As String, Answer As String, Question As String, OptionCount As Integer, rowNumber As Integer,
                                      OptionA As String, Optional OptionB As String = "", Optional OptionC As String = "",
                                      Optional OptionD As String = "", Optional OptionE As String = "", Optional OptionF As String = "", 
                                      Optional OptionG As String = "") As csvFileRow
            '
            Dim oCsvFileRow As New csvFileRow
            '
            Try
                '
                ' **************************************
                ' Validate the number of Answer/Options
                ' **************************************
                '
                If String.IsNullOrEmpty(OptionG) And OptionCount = 7 Then
                    OptionCount = 6
                End If
                If String.IsNullOrEmpty(OptionF) And OptionCount >= 6 Then
                    OptionCount = 5
                End If
                If String.IsNullOrEmpty(OptionE) And OptionCount >= 5 Then
                    OptionCount = 4
                End If
                If String.IsNullOrEmpty(OptionD) And OptionCount >= 4 Then
                    OptionCount = 3
                End If
                If String.IsNullOrEmpty(OptionC) And OptionCount >= 3 Then
                    OptionCount = 2
                End If
                If String.IsNullOrEmpty(OptionB) And OptionCount >= 2 Then
                    OptionCount = 1
                End If
                If String.IsNullOrEmpty(OptionA) And OptionCount >= 1 Then
                    OptionCount = 0
                End If
                '
                ' **************************************
                '
                oCsvFileRow.Subject = Subject
                oCsvFileRow.Answer = Answer
                oCsvFileRow.Question = Question
                '
                oCsvFileRow.OptionCount = OptionCount
                oCsvFileRow.rowNumber = rowNumber
                '
                oCsvFileRow.OptionA = OptionA
                oCsvFileRow.OptionB = OptionB
                oCsvFileRow.OptionC = OptionC
                oCsvFileRow.OptionD = OptionD
                oCsvFileRow.OptionE = OptionE
                oCsvFileRow.OptionF = OptionF
                oCsvFileRow.OptionG = OptionG
                '
            Catch ex As Exception
                CP.Site.ErrorReport(ex, "Unexpected error in csvFileRow.GetRow")
            End Try
            '
            Return oCsvFileRow
        End Function
        '
    End Class
    '
End Namespace
