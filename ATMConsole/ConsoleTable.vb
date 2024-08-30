Public Class ConsoleTable
    Public ReadOnly Property Columns As IList(Of Object)
    Public ReadOnly Property Rows As IList(Of Object())

    Private _ColumnTypes As System.Type(), _Formats As System.Collections.Generic.IList(Of String)
    Public ReadOnly Property Options As ConsoleTableOptions

    Public Property ColumnTypes As Type()
        Get
            Return _ColumnTypes
        End Get
        Private Set(value As Type())
            _ColumnTypes = value
        End Set

    Public Shared ReadOnly NumericTypes As HashSet(Of Type) = New HashSet(Of Type) From {
    GetType(Integer),
        GetType(Double),
        GetType(Decimal),
    GetType(Long),
        GetType(Short),
        GetType(SByte),
    GetType(Byte),
        GetType(ULong),
        GetType(UShort),
    GetType(UInteger),
        GetType(Single)
    }

    Public Sub New(ParamArray columns As String())
        Me.New(New ConsoleTableOptions With {
                .columns = New List(Of String)(columns)
            })
    End Sub



End Class
