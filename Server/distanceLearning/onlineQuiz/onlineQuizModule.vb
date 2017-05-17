Module onlineQuizModule
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
    End Structure

End Module
