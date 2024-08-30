Module Themes
    Public Sub QuestionTheme()
        System.Console.Write("Enter The Application Theme (Light - Dark) : ")
        Dim theme As String = Console.ReadLine().ToLower()
        If theme = "light" Then
            System.Console.BackgroundColor = ConsoleColor.White
            System.Console.Clear()
        ElseIf theme = "dark" Then
            System.Console.BackgroundColor = ConsoleColor.Black
            System.Console.Clear()
        End If
    End Sub
End Module
