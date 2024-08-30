Module Themes
    Public Sub QuestionTheme()
        System.Console.Write("Enter The Application Theme (Light - Dark) : ")
        Dim theme As String = Console.ReadLine().ToLower()
        If theme = "light" Then
            System.Console.BackgroundColor = ConsoleColor.White
            System.Console.Clear()
            System.Console.ForegroundColor = ConsoleColor.Black
        ElseIf theme = "dark" Then
            System.Console.BackgroundColor = ConsoleColor.Black
            System.Console.Clear()
            System.Console.ForegroundColor = ConsoleColor.White

        End If
    End Sub
End Module
