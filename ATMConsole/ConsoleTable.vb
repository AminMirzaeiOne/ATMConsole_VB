﻿Imports System.Text
Imports System.Text.RegularExpressions
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

    Private Function ToMarkDownString(delimiter As Char) As String
        Dim builder = New StringBuilder()

        ' find the longest column by searching each row
        Dim columnLengths = columnLengths()

        ' create the string format with padding
        __ = Format(columnLengths, delimiter)

        ' find the longest formatted line
        Dim columnHeaders = String.Format(Formats(CInt(0)).TrimStart(), Columns.ToArray())

        ' add each row
        Dim results = Rows.[Select](Function(row, i) String.Format(Formats(i + 1).TrimStart(), row)).ToList()

        ' create the divider
        Dim divider = Regex.Replace(columnHeaders, "[^|]", "-")

        builder.AppendLine(columnHeaders)
        builder.AppendLine(divider)
        results.ForEach(Sub(row) builder.AppendLine(row))

        Return builder.ToString()
    End Function


    Public Function ToMinimalString() As String
        Return ToMarkDownString(Char.MinValue)
    End Function

    Public Function ToStringAlternative() As String
        Dim builder = New StringBuilder()

        ' find the longest formatted line
        Dim columnHeaders = String.Format(Formats(CInt(0)).TrimStart(), Columns.ToArray())

        ' add each row
        Dim results = Rows.[Select](Function(row, i) String.Format(Formats(i + 1).TrimStart(), row)).ToList()

        ' create the divider
        Dim divider = Regex.Replace(columnHeaders, "[^| ]", "-")
        Dim dividerPlus = divider.Replace("|", "+")

        builder.AppendLine(dividerPlus)
        builder.AppendLine(columnHeaders)

        For Each row In results
            builder.AppendLine(dividerPlus)
            builder.AppendLine(row)
        Next
        builder.AppendLine(dividerPlus)

        Return builder.ToString()
    End Function


    Private Function Format(columnLengths As List(Of Integer), Optional delimiter As Char = "|"c) As String
        ' set right alignment if is a number
        Dim columnAlignment = Enumerable.Range(CInt(0), Columns.Count).[Select](GetNumberAlignment).ToList()

        SetFormats(columnLengths, columnAlignment)

        Dim delimiterStr = If(delimiter = Char.MinValue, String.Empty, delimiter.ToString())
        Dim lFormat = (Enumerable.Aggregate(Of String)(Enumerable.Select(Of Integer, Global.System.[String])(Enumerable.Range(CInt(0), Columns.Count), CType(Function(i) CStr(" " & delimiterStr & " {" & i.ToString() & "," & columnAlignment(CInt(i)).ToString() & columnLengths(CInt(i)).ToString() & "}"), Func(Of Integer, String))), CType(Function(s, a) CStr(s & a), Func(Of String, String, String))) & " ".ToString() & delimiterStr).Trim()
        Return lFormat
    End Function

    Private Function GetNumberAlignment(i As Integer) As String
        Return If(Options.NumberAlignment Is Alignment.Right AndAlso ColumnTypes IsNot Nothing AndAlso NumericTypes.Contains(ColumnTypes(i)), "", "-")
    End Function

    Private Function ColumnLengths() As List(Of Integer)
        Dim lColumnLengths = Columns.[Select](Function(t, i) Rows.[Select](Function(x) x(i)).Union({Columns(i)}).Where(Function(x) x IsNot Nothing).[Select](Function(x) x.ToString().ToCharArray().Sum(Function(c) CInt(If(c > 127, 2, 1)))).Max()).ToList()
        Return lColumnLengths
    End Function


    Public Sub Write(Optional format As Format = ConsoleTables.Format.Default)
        SetFormats(ColumnLengths(), Enumerable.Range(CInt(0), Columns.Count).[Select](GetNumberAlignment).ToList())

        Select Case format
            Case ConsoleTables.Format.[Default]
                Options.OutputTo.WriteLine(MyBase.ToString())
            Case ConsoleTables.Format.MarkDown
                Options.OutputTo.WriteLine(ToMarkDownString())
            Case ConsoleTables.Format.Alternative
                Options.OutputTo.WriteLine(ToStringAlternative())
            Case ConsoleTables.Format.Minimal
                Options.OutputTo.WriteLine(ToMinimalString())
            Case Else
                Throw New ArgumentOutOfRangeException(NameOf(format), format, Nothing)
        End Select
    End Sub

    Private Shared Function GetColumns(Of T)() As IEnumerable(Of String)
        Return GetType(T).GetProperties().[Select](Function(x) x.Name).ToArray()
    End Function



End Class
