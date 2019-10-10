Imports System.IO

Friend Class GetFunction

    Friend Shared WorldMap_TXT() As String

    Friend Shared Sub LoadWordMapFile(ByVal path)
        Dim fPath As String = Check_File("data\WORLDMAP.TXT")
        WorldMap_TXT = File.ReadAllLines(fPath & path & "\WORLDMAP.TXT")
    End Sub

    ''' <summary>
    ''' Получить значение заданного параметра
    ''' </summary>
    ''' <param name="e">размер секции в пределах которой искать параметр</param>
    Friend Shared Function GetParamWordMap(ByRef param As String, Optional ByRef section As String = Nothing, Optional ByVal e As Integer = 0) As String
        Dim s As Integer = 0
        If section <> Nothing Then s = 1 + FindLine(section)
        Return ParamGetValueWM(param, s, e)
    End Function

    ''' <summary>
    ''' Получить значение заданного параметра, начиная с указанной линии
    ''' </summary>
    ''' <param name="s">начало секции в пределах которой искать параметр</param>
    ''' <param name="e">конец секции в пределах которой искать параметр</param>
    Friend Shared Function ParamGetValueWM(ByRef param As String, ByRef s As Integer, Optional ByVal e As Integer = 0) As String
        Dim line() As String
        If e = 0 Then
            e = WorldMap_TXT.Length - 1
        Else
            e += s
        End If
        For i = s To e
            If WorldMap_TXT(i).IndexOf("=") > 0 Then
                line = WorldMap_TXT(i).Split("=", 2, StringSplitOptions.RemoveEmptyEntries)
                If line(0) = param Then Return line(1)
            End If
        Next
        Return Nothing
    End Function

    ''' <summary>
    ''' Получить индекс строки параметра
    ''' </summary>
    Friend Shared Function LineIndexWM(ByRef param As String, Optional ByRef section As String = Nothing) As Integer
        Dim line() As String
        Dim s As Integer = 0
        If section <> Nothing Then s = 1 + FindLine(section)
        For i = s To WorldMap_TXT.Length - 1
            If WorldMap_TXT(i).IndexOf("=") > 0 Then
                line = WorldMap_TXT(i).Split("=", 2, StringSplitOptions.RemoveEmptyEntries)
                If line(0) = param Then Return i
            End If
        Next
        Return -1
    End Function

    'Получить количеcтво используемых секций в массиве. only for maps.txt section=Map
    Friend Shared Function SectionCount(ByVal section As String, ByRef massive() As String) As UInteger
        Dim len As Integer = section.Length + 2
        For i = massive.Length - 1 To 0 Step -1
            If massive(i).StartsWith("[" & section & Chr(32)) Then
                Return CUInt(massive(i).Substring(len, massive(i).IndexOf("]") - len).Trim())
            End If
        Next
        Return 0
    End Function

    ''' <summary>
    ''' Получить имена всех lookup_name используемых карт из maps.txt
    ''' </summary>
    Friend Shared Sub MapsListName()
        Dim z As Integer
        Dim fPath = Check_File("data\MAPS.TXT")
        Dim MapsTxtData() As String = File.ReadAllLines(fPath & Data_path & "\MAPS.TXT")

        MapsName = New List(Of String)
        For n = 0 To MapsTxtData.Length - 1
            If MapsTxtData(n).StartsWith("lookup_name=", StringComparison.CurrentCultureIgnoreCase) Then
                z = MapsTxtData(n).IndexOf(";")
                If z > 12 Then z -= -12 Else z = MapsTxtData(n).Length - 12
                MapsName.Add(MapsTxtData(n).Substring(12, z).Trim())
            End If
        Next
        ' удалить повторы
        MapsName.Sort()
        For n = MapsName.Count - 1 To 1 Step -1
            If MapsName(n) = MapsName(n - 1) Then
                MapsName.RemoveAt(n)
            End If
        Next
    End Sub

    ''' <summary>
    ''' Получить данные всех используемых локаций из city.txt
    ''' </summary>
    Friend Shared Sub CitiesData()
        Dim area As Boolean
        Dim name = "", pos = "", size = "", state = "" ' As String

        Dim fPath As String = Check_File("data\CITY.TXT")
        Dim CityTxtData As String() = File.ReadAllLines(c_path & Data_path & "\CITY.TXT")

        CityList = New List(Of CityData)
        For n As Integer = 0 To CityTxtData.Length - 1
            If CityTxtData(n).StartsWith("area_name=", StringComparison.CurrentCultureIgnoreCase) Then
                If area Then
                    CityList.Add(New CityData(name, pos, size, state))
                    pos = String.Empty
                    size = String.Empty
                    state = String.Empty
                Else
                    area = True
                End If
                name = CityTxtData(n)
            ElseIf CityTxtData(n).StartsWith("world_pos=", StringComparison.CurrentCultureIgnoreCase) Then
                pos = CityTxtData(n)
            ElseIf CityTxtData(n).StartsWith("size=", StringComparison.CurrentCultureIgnoreCase) Then
                size = CityTxtData(n)
            ElseIf CityTxtData(n).StartsWith("lock_state=", StringComparison.CurrentCultureIgnoreCase) Then
                state = CityTxtData(n)
            End If
        Next
    End Sub

    Friend Shared Sub GlobalVars()
        Dim fPath = Check_File("data\VAULT13.GAM")
        Dim GlobalTxtData = File.ReadAllLines(c_path & Data_path & "\VAULT13.GAM")

        GlobalVar = New List(Of String)
        For n = 0 To GlobalTxtData.Length - 1
            If GlobalTxtData(n).StartsWith("GVAR_", StringComparison.CurrentCultureIgnoreCase) Then
                Dim z As Integer = GlobalTxtData(n).IndexOf(":")
                GlobalVar.Add(GlobalTxtData(n).Substring(5, z - 5).Trim)
            End If
        Next
    End Sub

    Friend Shared Function FindLine(ByVal str As String, Optional ByRef massive() As String = Nothing) As Integer
        If massive Is Nothing Then massive = WorldMap_TXT
        For z = 0 To massive.Length - 1
            If massive(z).StartsWith(str, StringComparison.CurrentCultureIgnoreCase) Then Return z
        Next
        Return -1
    End Function

    'узнать на каком столбце listview был сделан клик
    Friend Shared Function GetListViewColClick(ByRef lw As ListView, ByRef x As Integer) As Integer
        Dim w As Integer = lw.Items(0).Bounds.Left
        For n = 0 To lw.Columns.Count - 1 'lw.Items(lw.FocusedItem.Index).SubItems.Count 
            If x > w And x < (lw.Columns.Item(n).Width + w) Then
                x = lw.Items(lw.FocusedItem.Index).SubItems(n).Bounds.Location.X '+ 4
                Return n
            Else
                w += lw.Columns.Item(n).Width 'суммируем ширину столбцов
            End If
        Next
        Return -1
    End Function

    'узнать на какой строке listview был сделан клик
    Friend Shared Function GetListViewRowClick(ByRef lw As ListView, ByRef y As Integer) As Integer
        Dim h As Integer = lw.Items(0).Bounds.Top - 4
        For n = 0 To lw.Items.Count - 1
            If y > h And y < (lw.Items(n).Bounds.Height + h) Then
                lw.Items(n).Selected = True
                lw.Items(n).Focused = True
                Return n
            Else
                h += lw.Items(n).Bounds.Height 'суммируем высоту строки
            End If
        Next
        Return -1
    End Function

End Class
