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

    Public Sub New(options As ConsoleTableOptions)
        options = If(options, CSharpImpl.__Throw(Of Object)(New ArgumentNullException("options")))
        Rows = New List(Of Object())()
        Columns = New List(Of Object)(options.Columns)
    End Sub

    Public Function AddColumn(names As IEnumerable(Of String)) As ConsoleTable
        For Each name In names
            Columns.Add(name)
        Next
        Return Me
    End Function

    Public Function AddRow(ParamArray values As Object()) As ConsoleTable
        If values Is Nothing Then Throw New ArgumentNullException(NameOf(values))

        If Not Columns.Any() Then Throw New Exception("Please set the columns first")

        If Columns.Count <> values.Length Then Throw New Exception($"The number columns in the row ({Columns.Count}) does not match the values ({values.Length})")

        Rows.Add(values)
        Return Me
    End Function


    Public Function Configure(action As Action(Of ConsoleTableOptions)) As ConsoleTable
        action(Options)
        Return Me
    End Function


    Public Shared Function FromDictionary(values As Dictionary(Of String, Dictionary(Of String, Object))) As ConsoleTable
        Dim table = New ConsoleTable()

        Dim columNames = values.SelectMany(Function(x) x.Value.Keys).Distinct().ToList()
        columNames.Insert(0, "")
        table.AddColumn(columNames)
        Dim value As Object = Nothing
        For Each row In values
            Dim r = New List(Of Object) From {
                    row.Key
                }
            For Each columName In columNames.Skip(1)
                r.Add(If(row.Value.TryGetValue(columName, value), value, ""))
            Next

            table.AddRow(r.Cast(Of Object)().ToArray())
        Next

        Return table
    End Function


    Public Shared Function From(Of T)(values As IEnumerable(Of T)) As ConsoleTable
        Dim table = New ConsoleTable With {
        .ColumnTypes = GetColumnsType(Of T)().ToArray()
    }

        Dim columns = GetColumns(Of T)().ToList()

        table.AddColumn(columns)

        For Each propertyValues In values.[Select](Function(value) columns.[Select](Function(column) GetColumnValue(Of T)(value, column)))
            table.AddRow(propertyValues.ToArray())
        Next

        Return table
    End Function


End Class
