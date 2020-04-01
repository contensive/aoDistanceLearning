Imports Contensive.BaseClasses

Namespace Model.dbModels
    '
    <Serializable>
    Public Class User
        Inherits BasicRecord
        '
        Public firstname As String 
        Public lastname As String
        Public email As String
        Public username As String
        'Public password As String
        Public visitId As Integer
        '
        '
        '
        '
        Public Function VerifyActualUsernameIsValid(ByVal CP As CPBaseClass) As Boolean
            Dim result As Boolean = False
            Try
                Dim usernameBase As String = ""
                If String.IsNullOrEmpty(username) Then
                    usernameBase = (firstName & lastName).replace(" ","").ToLower()
                    If verifyUsernameAlreadyExist(cp,usernameBase) Then
                        for i = 1 To 100 
                            If Not verifyUsernameAlreadyExist(cp, usernameBase & i) Then
                                username = usernameBase & i
                                ' set username value
                                setFieldWithValue(CP, id, "username", username)
                                Exit For
                            End If
                        Next
                    Else
                        ' set username value
                        username = usernameBase
                        setFieldWithValue(CP, id, "username", username)
                        result = true
                    End If
                Else
                    result= True
                End If
            Catch ex As Exception
                cp.Site.ErrorReport(ex, "Unexpected error in Member.VerifyActualUsername")
            End Try
            Return result
        End Function
        '
        Public Function GenerateUserNewPassword(ByVal CP As CPBaseClass, userId As Integer) As String
            Dim newPassword As String = ""
            Try
                Dim cs As CPCSBaseClass = CP.CSNew()
                newPassword = (Guid.NewGuid().ToString("N")).Substring(0,8)
                '
                Call setFieldWithValue(CP, userId, "password", newPassword)
                '
            Catch ex As Exception
                cp.Site.ErrorReport(ex, "Unexpected error in Member.GenerateUserNewPassword")
            End Try
            Return newPassword
        End Function
        '
        ' *****************
        ' Shared Functions
        ' *****************
        '
        Public Shared Function setFieldWithValue(ByVal CP As CPBaseClass, userId As Integer,fieldName As String, value As String)
            Dim result As Boolean = False
            Try
                Dim cs As CPCSBaseClass = CP.CSNew()
                If cs.Open(cnPeople,"id=" & userId) Then
                    Call cs.SetField(fieldName, value)
                End If
                Call cs.Close()
            Catch ex As Exception
                cp.Site.ErrorReport(ex, "Unexpected error in Member.verifyUsernameAlreadyExist")
            End Try
            Return result
        End Function
        '
        Public Shared Function verifyUsernameAlreadyExist(ByVal CP As CPBaseClass, username As String)
            Dim result As Boolean = False
            Try
                Dim cs As CPCSBaseClass = CP.CSNew()
                result = cs.Open(cnPeople,"username=" &  CP.Db.EncodeSQLText(username))
                Call cs.Close()
            Catch ex As Exception
                cp.Site.ErrorReport(ex, "Unexpected error in Member.verifyUsernameAlreadyExist")
            End Try
            Return result
        End Function
        '
        Public Shared Function CreateUser(ByVal CP As CPBaseClass, ByRef ActualUser As solutionModels.SignUp, ByRef errorList As List(Of architectureModels.errorClass)) As Boolean
            Dim Result As Boolean
            Dim errorStep as String = "00"
            '
            Try
                '
                Dim cs As CPCSBaseClass = CP.CSNew()
                'Dim emailExist As Boolean = False
                Dim usernameExist As Boolean = False
                '
                '
                Dim strHtml As String = ""
                '
                errorStep = "01"
                ' check if email exist
                'emailExist = cs.Open(cnPeople, "email = " & CP.Db.EncodeSQLText(ActualUser.email))
                'Call cs.Close()

                errorStep = "02"
                ' check if username exist
                usernameExist = cs.Open(cnPeople, "username = " & CP.Db.EncodeSQLText(ActualUser.username))
                Call cs.Close()

                errorStep = "03"
                '
                If Not String.IsNullOrEmpty(ActualUser.username) And Not String.IsNullOrEmpty(ActualUser.password) Then

                    If Not usernameExist Then
                        '
                        If cs.Insert(cnPeople) Then
                            '
                            ActualUser.id = cs.GetInteger("id")
                            '
                            errorStep = "04"
                            cs.SetField("Name", ActualUser.firstname & " " & ActualUser.lastname)
                            cs.SetField("FirstName", ActualUser.firstname)
                            cs.SetField("LastName", ActualUser.lastname)
                            cs.SetField("Username", ActualUser.username)
                            cs.SetField("Email", ActualUser.username)

                            errorStep = "07"
                            cs.SetField("password", ActualUser.password)
                            '
                            Call cs.Save()
                            '
                            ' Login the user
                            '
                            'CP.User.LoginByID(ActualUser.id)
                            '
                        End If
                        Call cs.Close()
                        '
                    Else
                        '
                        Dim oneError As New architectureModels.errorClass
                        oneError.number = 720
                        oneError.userMsg = ActualUser.username & " already exists in the system. Please login or use a different email to continue."
                        errorList.Add(oneError)
                        '
                    End If
                    '
                Else
                    '
                    Dim oneError As New architectureModels.errorClass
                    oneError.number = 700
                    oneError.userMsg = "email and/or password can't be empty."
                    errorList.Add(oneError)
                    '
                End If                     
                '
            Catch ex As Exception
                cp.Site.ErrorReport(ex, "Unexpected error in User.CreateUser - errorStep:" & errorStep)
            End Try

            '
            Return Result
        End Function
        '
        Public Shared Function getRecordFromEmail(ByVal CP As CPBaseClass, email As String ) As User
            Dim oMember As New User
            Try
                '
                Dim cs As CPCSBaseClass = CP.CSNew()
                If cs.Open(cnPeople,"email = " & CP.Db.EncodeSQLText(email))
                    oMember = getUser(CP, cs.GetInteger("id"))
                End If
                Call cs.Close()
                '
            Catch ex As Exception
                cp.Site.ErrorReport(ex, "Unexpected error in Member.getRecordFromEmail")
            End Try
            Return oMember
        End Function
        '
        Public Shared Function getUser(ByVal CP As CPBaseClass, UserId As Integer) As User
            Dim oUser As New User
            '
            Try
                Dim cs As CPCSBaseClass = CP.CSNew()

                If cs.Open(cnPeople, "id=" & UserId) Then
                    '
                    oUser.id = cs.GetInteger("id")
                    '
                    oUser.name = cs.GetText("Name")
                    oUser.firstname = cs.GetText("FirstName")
                    oUser.lastname = cs.GetText("LastName")
                    oUser.email = cs.GetText("Email")
                    oUser.username = cs.GetText("username")
                    '
                    oUser.visitId = CP.Visit.Id
                    '
                End If
                Call cs.Close()
                '
            Catch ex As Exception
                cp.Site.ErrorReport(ex, "Unexpected error in User.getUser")
            End Try
            '
            Return oUser
        End Function
        '
        '
        '
        Public Shared Function ActualUserIsInGroup(ByVal CP As CPBaseClass, groupId As Integer) As Boolean
            Dim result As Boolean = False
            Try
                '
                Dim cs As CPCSBaseClass = CP.CSNew()
                If cs.Open(cnMemberRules, "MemberID=" & CP.User.Id & " and GroupID=" & groupId) Then
                    result = True
                End If
                Call cs.Close()
                '
            Catch ex As Exception
                cp.Site.ErrorReport(ex, "Unexpected error in User.ActualUserIsInGroup")
            End Try
            Return result
        End Function
        '
        '
        '
        Public Shared Function AddUserToGroup(ByVal CP As CPBaseClass, groupId As Integer) As Boolean
            Dim result As Boolean = False
            Try

                Dim csGroup As CPCSBaseClass = CP.CSNew()
                Dim UserExistInGroupId As Boolean = False
                If groupId<>0 Then
                    UserExistInGroupId = csGroup.Open("Member Rules", "MemberID=" & CP.User.id & " And GroupID=" & groupId)
                    Call csGroup.Close()
                    If Not UserExistInGroupId Then
                        If csGroup.Insert("Member Rules") Then
                            Call csGroup.SetField("MemberID", CP.User.id)
                            Call csGroup.SetField("GroupID", groupId)
                            Call csGroup.SetField("active", "1")
                            result = True
                        End If
                        Call csGroup.Close()
                    End If
                End If
            Catch ex As Exception
                cp.Site.ErrorReport(ex, "Unexpected error in User.AddUserToGroup")
            End Try
            Return result
        End Function
        '
        '
        '
        Public Shared Function getAccountId(ByVal CP As CPBaseClass, userId As Integer) As Integer
            Dim accountId As Integer = 0
            Try
                '
                Dim cs As CPCSBaseClass = CP.CSNew()
                Dim csAccount As CPCSBaseClass = CP.CSNew()
                If userId <> 0 Then
                    If cs.Open(cnPeople, "id=" & userId) Then
                        accountId = cs.GetInteger("accountId")

                        '
                        '  Check if really exit the account
                        '
                        If Not csAccount.Open(cnAccounts, "id=" & accountId) Then
                            accountId = 0
                        End If
                        Call csAccount.Close()
                        '
                    End If
                    Call cs.Close()
                End If

            Catch ex As Exception
                cp.Site.ErrorReport(ex, "Unexpected error in User.getAccountId")
            End Try
            Return accountId
        End Function
        '
        '
        '
    End Class
    '
End Namespace



