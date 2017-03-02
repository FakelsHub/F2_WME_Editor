Imports System.IO

Module Txt_Function

    Friend WorldMap_TXT() As String

    Friend Sub ReadWM_TXT(ByVal path)
        WorldMap_TXT = File.ReadAllLines(path & "\WORLDMAP.TXT")
    End Sub

    'Получить значение заданного параметра
    'e - размер сексии в пределах которой искать параметр
    Friend Function GetParamWordMap(ByRef param As String, Optional ByRef section As String = Nothing, Optional ByVal e As Integer = 0) As String
        Dim s As Integer = 0
        If section <> Nothing Then s = 1 + FindLine(WorldMap_TXT, section)
        Return GetParamWordMap_sub(param, s, e)
    End Function
    'Получить значение заданного параметра, начиная с указанной линии s  
    Friend Function GetParamWordMap_sub(ByRef param As String, ByRef s As Integer, Optional ByVal e As Integer = 0) As String
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

    'Получить индекс строки параметра
    Friend Function GetIndexWordMap(ByRef param As String, Optional ByRef section As String = Nothing) As Integer
        Dim line() As String
        Dim s As Integer = 0
        If section <> Nothing Then s = 1 + FindLine(WorldMap_TXT, section)
        For i = s To WorldMap_TXT.Length - 1
            If WorldMap_TXT(i).IndexOf("=") > 0 Then
                line = WorldMap_TXT(i).Split("=", 2, StringSplitOptions.RemoveEmptyEntries)
                If line(0) = param Then Return i
            End If
        Next
        Return -1
    End Function

    'Получить количеcтво используемых тайлов [Tile ##]
    Friend Function GetTileCountWM() As UInteger
        Dim s As Integer = FindLine(WorldMap_TXT, "[Tile 0]")
        For i = WorldMap_TXT.Length - 1 To s Step -1
            If WorldMap_TXT(i).StartsWith("[Tile ") Then
                Return CUInt(WorldMap_TXT(i).Substring(6, WorldMap_TXT(i).IndexOf("]") - 6).Trim())
            End If
        Next
        Return 0
    End Function

    'Получить количеcтво используемых [Encounter Table ##]
    Friend Function GetEncTCountWM() As UInteger
        Dim s As Integer = FindLine(WorldMap_TXT, "[Encounter Table 0]")
        For i = Array.IndexOf(WorldMap_TXT, "[Tile Data]") To s Step -1
            If WorldMap_TXT(i).StartsWith("[Encounter Table ") Then
                Return CUInt(WorldMap_TXT(i).Substring(17, WorldMap_TXT(i).IndexOf("]") - 17).Trim())
            End If
        Next
        Return 0
    End Function

    'Получить количеcтво секций в массиве и их индекс в массиве.
    Friend Function GetSectionCountWM(ByRef section As String, ByRef massive() As String) As UInteger
        Dim str_index As String = Nothing
        Dim z As UInteger = 0
        For i = 0 To massive.Length - 1
            If massive(i).StartsWith("[" & section & Chr(32)) Then
                z += 1
                str_index &= i.ToString & "|"
            End If
        Next
        section = str_index.Remove(str_index.Length - 1)
        Return z - 1
    End Function

    'Получить количеcтво используемых секций в массиве. only for maps.txt section=Map
    Friend Function GetSectionCount(ByVal section As String, ByRef massive() As String) As UInteger
        Dim len As Integer = section.Length + 2
        For i = massive.Length - 1 To 0 Step -1
            If massive(i).StartsWith("[" & section & Chr(32)) Then
                Return CUInt(massive(i).Substring(len, massive(i).IndexOf("]") - len).Trim())
            End If
        Next
        Return 0
    End Function

    'Получить имена всех lookup_name используемых карт из maps.txt
    Friend Sub GetMapsListName(ByRef massive() As String)
        Dim z, i As Integer
        Dim name() As String
        For n = 0 To massive.Length - 1
            If massive(n).StartsWith("lookup_name=", StringComparison.CurrentCultureIgnoreCase) Then
                z = massive(n).IndexOf(";")
                If z > 12 Then z -= -12 Else z = massive(n).Length - 12
                MapsName(i) = massive(n).Substring(12, z).Trim()
                i += 1
            End If
        Next
        ' удалить повторы 
        Array.Sort(MapsName)
        name = MapsName.Clone
        z = 0
        For n = 1 To i - 1
            If name(n) <> name(n - 1) Then
                MapsName(z) = name(n - 1)
                z += 1
            End If
        Next
        MapsName(z) = name(name.Length - 1)
        ReDim Preserve MapsName(z)
    End Sub

    'Получить данные всех используемых локаций из city.txt
    Friend Sub GetCityData(ByRef massive() As String)
        Dim z, i As Integer
        i = -1
        For n = 0 To massive.Length - 1
            If massive(n).StartsWith("area_name=", StringComparison.CurrentCultureIgnoreCase) Then
                z = massive(n).IndexOf(";")
                If z > 10 Then z -= 10 Else z = massive(n).Length - 10
                i += 1
                CityData(i, 0) = massive(n).Substring(10, z).Trim()
                CityData(i, 3) = ""
            End If
            If massive(n).StartsWith("world_pos=", StringComparison.CurrentCultureIgnoreCase) Then
                z = massive(n).IndexOf(";")
                If z > 10 Then z -= 10 Else z = massive(n).Length - 10
                CityData(i, 1) = massive(n).Substring(10, z).Trim()
            End If
            If massive(n).StartsWith("size=", StringComparison.CurrentCultureIgnoreCase) Then
                z = massive(n).IndexOf(";")
                If z > 5 Then z -= 5 Else z = massive(n).Length - 5
                CityData(i, 2) = massive(n).Substring(5, z).Trim()
            End If
            If massive(n).StartsWith("lock_state=", StringComparison.CurrentCultureIgnoreCase) Then
                z = massive(n).IndexOf(";")
                If z > 11 Then z -= 11 Else z = massive(n).Length - 11
                CityData(i, 3) = massive(n).Substring(11, z).Trim()
            End If
        Next
    End Sub

    Friend Function FindLine(ByRef massive() As String, ByVal str As String) As Integer
        For z = 0 To massive.Length - 1
            If massive(z).StartsWith(str, StringComparison.CurrentCultureIgnoreCase) Then Return z
        Next
        Return -1
    End Function

End Module
