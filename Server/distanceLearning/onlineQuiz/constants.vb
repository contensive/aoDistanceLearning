Imports System.Xml
Imports Contensive.BaseClasses


Namespace Contensive.Addons.OnlineQuiz
    '
    ' - rename namespace to the collection (to match the current project)
    ' - rename localLogName to create a custom trace log folder, like 'accounting'
    '
    Module constants
        '
        Public Const defaultTopVideoCopy = "<p>To complete this quiz, watch the video and submit your answers to the questions that follow.</p>"
        Public Const defaultTopNoVideoCopy = "<p>To complete this quiz, submit your answers to the questions that follow. </p>"
        Public Const defaultOneTime = "<p>You may submit this quiz only once.</p>"
        Public Const defaultButtonCopy = "<p>Click submit when you are finished with the quiz.</p>"
        Public Const defaultButtonWithSaveCopy = "<p>Click submit when you are finished with the quiz. If you are logged in, you may also save the quiz and return later.</p>"
        Public Const QuizAlreadyTakenDefault = "<p>You have already taken this quiz. Your previous answers are as follows.</p>"
        Public Const scoreCardAddon = "{55E3F437-B362-45D1-B793-A201D4D5C2C6}"
        '
        Public Structure subjectsStruc
            Dim SubjectID As Integer
            Dim SubjectCaption As String
            Dim TotalQuestions As Integer
            Dim CorrectAnswers As Integer
            Dim Score As Double
            Dim points As Integer
        End Structure        '
        Public Const codeBuildVersion As Integer = 5
        '
        Public Const cr = vbCrLf & vbTab
        Public Const cr2 = cr & vbTab
        Public Const cr3 = cr2 & vbTab
        '
        Public Const quizTypeIdGraded = 1
        Public Const quizTypeIdPoints = 2
        '
        ' site properties
        '
        Public Const spBatchProcessOnlyOne = "Account Billing Batch Process Only One"
        '
        ' messages
        '
        Public Const blockedMessage = "<h2>Blocked Content</h2><p>Your account must have administrator access to view this content.</p>"
        '
        ' guids
        '
        Public Const guidSample = "{B3FDBB50-1A40-42CA-81EC-EA825C55469A}"
        Public Const guidLogin = "{37E8E661-A1D9-4A61-9A07-F48A1053B1BB}"
        Public Const onlineQuizAddonGuid = "{4A5EBAD7-27ED-4AB4-BEF0-D42E01598FC5}"
        '
        ' buttons
        '
        Public Const buttonPrintAll = " Print All "
        Public Const buttonStartQuiz = "Start Quiz"
        '
        ' request Names
        '
        Public Const rnSrcFormId = "srcFormId"
        Public Const rnDstFormId = "dstFormId"
        Public Const rnButton = "button"
        Public Const rnPageNumber = "pageOrder"
        Public Const rnDstPageOrder = "dstPageOrder"
        '
        ' custom log path
        '
        Private traceLogPath As String = "sample"
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
        Friend Function encodeShortDateString(ByVal sourceDate As Date) As String
            Dim returnValue As String
            '
            If sourceDate < #1/1/1900# Then
                returnValue = ""
            Else
                returnValue = sourceDate.ToShortDateString
            End If
            Return returnValue

        End Function
        '
        '
        '
        Friend Function encodeBlankCurrency(ByVal source As Double) As String
            Dim returnValue As String = ""
            If source <> 0 Then
                returnValue = FormatCurrency(source, 2)
            End If
            Return returnValue
        End Function
        '
        '
        '
        Friend Sub appendLog(ByVal cp As CPBaseClass, ByVal logMessage As String)
            Dim nowDate As Date = Date.Now.Date()
            Dim logFilename As String = nowDate.Year & nowDate.Month.ToString("D2") & nowDate.Day.ToString("D2") & ".log"
            Call cp.File.CreateFolder(cp.Site.PhysicalInstallPath & "\logs\" & traceLogPath)
            Call cp.Utils.AppendLog(traceLogPath & "\" & logFilename, logMessage)
        End Sub
        '
        '
        '
        Private Sub localErrorReport(ByVal cp As CPBaseClass, ByVal ex As Exception, ByVal method As String)
            Call cp.Site.ErrorReport(ex, "error in aoAccountBilling.commonModule." & method)
        End Sub
    End Module
End Namespace