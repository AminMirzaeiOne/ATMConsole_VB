Public Class ConsoleTable
    Private _Rows As System.Collections.Generic.IList(Of Object()), _Options As ConsoleTableOptions
    Public Property Columns As IList(Of Object)

    Public Property Rows As IList(Of Object())
        Get
            Return _Rows
        End Get
        Protected Set(value As IList(Of Object()))
            _Rows = value
        End Set
    End Property

    Public Property Options As ConsoleTableOptions
        Get
            Return _Options
        End Get
        Protected Set(value As ConsoleTableOptions)
            _Options = value
        End Set
    End Property

    Public Sub New(ParamArray columns As String())
        Me.New(New ConsoleTableOptions With {
        .columns = New List(Of String)(columns)
    })
    End Sub
End Class
