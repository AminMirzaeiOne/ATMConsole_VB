Imports System.Text
Imports Microsoft.SqlServer.Server

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

    Public Shared Function From(dataTable As DataTable) As ConsoleTable
        Dim table = New ConsoleTable()

        Dim columns = dataTable.Columns.Cast(Of DataColumn)().[Select](Function(x) x.ColumnName).ToList()

        table.AddColumn(columns)
        Dim data As Byte() = Nothing

        For Each row As DataRow In dataTable.Rows
            Dim items = row.ItemArray.[Select](Function(x)
                                                   Dim data As Byte() = Nothing
                                                   Return If(CSharpImpl.__Assign(data, TryCast(x, Byte())) IsNot Nothing, Convert.ToBase64String(data), x.ToString())
                                               End Function).ToArray()
            table.AddRow(items)
        Next

        Return table
    End Function


    Public Overrides Function ToString() As String
        Dim builder = New StringBuilder()

        ' find the longest column by searching each row
        Dim columnLengths = columnLengths()

        ' set right alinment if is a number
        Dim columnAlignment = Enumerable.Range(CInt(0), Columns.Count).[Select](GetNumberAlignment).ToList()

        ' create the string format with padding ; just use for maxRowLength
        Dim format = Enumerable.Range(0, Columns.Count).[Select](Function(i) " | {" & i.ToString() & "," & columnAlignment(i).ToString() & columnLengths(i).ToString().ToString() & "}").Aggregate(Function(s, a) s + a).ToString() & " |"

        SetFormats(columnLengths(), columnAlignment)

        ' find the longest formatted line
        Dim maxRowLength = Math.Max(0, If(Rows.Any(), Rows.Max(Function(row) String.Format(format, row).Length), 0))
        Dim columnHeaders = String.Format(Formats(0), Columns.ToArray())

        ' longest line is greater of formatted columnHeader and longest row
        Dim longestLine = Math.Max(maxRowLength, columnHeaders.Length)

        ' add each row
        Dim results = Rows.[Select](Function(row, i) String.Format(Formats(i + 1), row)).ToList()

        ' create the divider
        Dim divider = " " & String.Join("", Enumerable.Repeat("-", longestLine - 1)).ToString() & " "

        builder.AppendLine(divider)
        builder.AppendLine(columnHeaders)

        For Each row In results
            builder.AppendLine(divider)
            builder.AppendLine(row)
        Next

        builder.AppendLine(divider)

        If Options.EnableCount Then
            builder.AppendLine("")
            builder.AppendFormat(" Count: {0}", Rows.Count)
        End If

        Return builder.ToString()
    End Function

    Private Sub SetFormats(columnLengths As List(Of Integer), columnAlignment As List(Of String))
        Dim allLines = New List(Of Object())()
        allLines.Add(Columns.ToArray())
        allLines.AddRange(Rows)
        Formats = allLines.[Select](Function(d) Enumerable.Range(CInt(0), Columns.Count).[Select](Function(i)                                                                                       Return " | {" & i.ToString() & "," & columnAlignment(CInt(i)) & length.ToString().ToString() & "}"
                                                                                                  End Function).Aggregate(Function(s, a) s + a).ToString() & " |").ToList()
    End Sub

    Public Shared Function GetTextWidth(value As String) As Integer
        If Equals(value, Nothing) Then Return 0

        Dim length = value.ToCharArray().Sum(Function(c) If(c > 127, 2, 1))
        Return length
    End Function

    Public Function ToMarkDownString() As String
        Return ToMarkDownString("|"c)
    End Function



End Class
