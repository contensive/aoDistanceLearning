Imports Contensive.BaseClasses

Namespace  Model.dbModels
    '
    Public Module common
        '
        Public Const cnPeople As String = "People"
        Public Const cnStates = "States"
        Public Const cnCountries As String = "Countries"
        Public Const cnMemberRules = "Member Rules"
        '
        Public Const cnPatient = "Patient"
        Public Const cnPatientStatus = "Patient Status"
        Public Const cnRolePatientStatusRules = "Role Patient Status Rules"
        '
        Public Const cnRoles As String = "tbl_user_roles"
        Public Const cnProducts As String = "tbl_product"
        Public Const cnDoctorCreditCards As String = "Doctor Credit Cards"
        '
        Public Const cnAccounts As String = "Accounts"
        Public Const cnItems As String = "Items"
        '
        '
        Public Function ValidateFilePath(ByVal CP As CPBaseClass, FilePath As String, Optional DefaultPath As String = "") As String
            dim result As String = ""
            Try
                If Not String.IsNullOrEmpty(FilePath) Then
                    result = cp.Site.FilePath & FilePath
                Else
                    result = defaultPath
                End If
            Catch ex As Exception

            End Try
            Return result
        End Function
        '
        '
        '
    End Module
    '
End Namespace