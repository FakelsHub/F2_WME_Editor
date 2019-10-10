Friend Class WMData

    Friend Const AMBUSH As String = "AMBUSH"
    Friend Const FIGHTING As String = "FIGHTING"

    Private total_tiles As Integer

    Friend Property x_tiles As Integer
    Friend Property y_tiles As Integer

    Friend TilesData As List(Of Tile)
    Friend EncTable As Dictionary(Of String, Integer)  'key = look name / value = index in WorldMap_TXT
    Friend Encounter As Dictionary(Of String, Integer) 'key = name / value = index in WorldMap_TXT 

    Friend TerrainList As List(Of Terrain) = New List(Of Terrain)

    Friend ReadOnly EncPercent As String() = {"Forced", "Frequent", "Common", "Uncommon", "Rare", "None"} 'default

    Sub New()
        GetFunction.LoadWordMapFile(Data_path)

        'получаем количетво тайлов
        x_tiles = GetFunction.GetParamWordMap("num_horizontal_tiles", "[Tile Data]")
        total_tiles = GetTileCountWM()
        y_tiles = ((1 + total_tiles) / x_tiles)

        Main_Form.ToolStripProgressBar1.Maximum = total_tiles

        CreateTileData()
        CreateEncTable()
        CreateEncounter()

        'Enc Percent & Terrain
        For Each tr As String In GetFunction.GetParamWordMap("terrain_types", "[Data]").Split(",")
            TerrainList.Add(New Terrain(tr))
        Next

    End Sub

    Private Sub CreateTileData()
        'создаем массив с именами тайлов [Tile ###]
        TilesData = New List(Of Tile)

        Dim frmName, tileName As String
        For n As Integer = 0 To total_tiles
            tileName = "[Tile " & n & "]"
            TilesData.Add(New Tile(tileName))
            TilesData(n).TileFrmInx = GetFunction.GetParamWordMap("art_idx", tileName)

            'преобразовать frm > gif
            frmName = INTRFACE_LST(TilesData(n).TileFrmInx)
            TilesData(n).TileFrmName = frmName.Remove(frmName.IndexOf(Chr(32)))
            TilesData(n).TileFrmName = WMFrmGif(TilesData(n).TileFrmName) 'конвертим и возвращаем изменненое имя

            Main_Form.ToolStripProgressBar1.Value = n
            Application.DoEvents()
        Next
    End Sub

    'создать [Encounter Table] массив
    Private Sub CreateEncTable()
        Dim z As Integer

        EncTable = New Dictionary(Of String, Integer)

        For n As Integer = 0 To GetEncTCountWM()
            Dim tmp As String = GetFunction.GetParamWordMap("lookup_name", "[Encounter Table " & n & "]")
            z = tmp.IndexOf(";")
            If z > 0 Then tmp = tmp.Remove(z)
            EncTable.Add(tmp.Trim, GetFunction.FindLine("[Encounter Table " & n & "]"))
        Next
    End Sub

    'создать [Encounter: Name] массив
    Private Sub CreateEncounter()

        Encounter = New Dictionary(Of String, Integer)

        Dim tmp As String = "Encounter:"
        Dim m As Integer = GetSectionCountWM(tmp)
        Dim index As String() = tmp.Split("|")
        tmp = Nothing

        Dim s As Integer
        For n = 0 To m
            tmp = GetFunction.WorldMap_TXT(CInt(index(n))).Trim
            Dim z = tmp.IndexOf("]")
            s = tmp.IndexOf(":") + 1
            Encounter.Add(tmp.Substring(s, z - s).Trim, CInt(index(n)))
        Next

    End Sub

    'Получить количеcтво используемых тайлов [Tile ##]
    Friend Function GetTileCountWM() As UInteger
        Dim s As Integer = GetFunction.FindLine("[Tile 0]")

        For i = GetFunction.WorldMap_TXT.Length - 1 To s Step -1
            If GetFunction.WorldMap_TXT(i).StartsWith("[Tile ") Then
                Return CUInt(GetFunction.WorldMap_TXT(i).Substring(6, GetFunction.WorldMap_TXT(i).IndexOf("]") - 6).Trim())
            End If
        Next

        Return 0
    End Function

    'Получить количеcтво используемых [Encounter Table ##]
    Friend Function GetEncTCountWM() As UInteger
        Dim s As Integer = GetFunction.FindLine("[Encounter Table 0]")

        For i = Array.IndexOf(GetFunction.WorldMap_TXT, "[Tile Data]") To s Step -1
            If GetFunction.WorldMap_TXT(i).StartsWith("[Encounter Table ") Then
                Return CUInt(GetFunction.WorldMap_TXT(i).Substring(17, GetFunction.WorldMap_TXT(i).IndexOf("]") - 17).Trim())
            End If
        Next

        Return 0
    End Function

    'Получить количеcтво секций в массиве и их индекс в массиве.
    Friend Function GetSectionCountWM(ByRef section As String, Optional ByRef massive() As String = Nothing) As UInteger
        Dim str_index As String = Nothing
        Dim z As UInteger = 0

        If massive Is Nothing Then massive = GetFunction.WorldMap_TXT

        For i = 0 To massive.Length - 1
            If massive(i).StartsWith("[" & section & Chr(32)) Then
                z += 1
                str_index &= i.ToString & "|"
            End If
        Next
        section = str_index.Remove(str_index.Length - 1)

        Return z - 1
    End Function

    Friend Function GetPercentEnc(ByVal param As String) As String
        param = GetFunction.GetParamWordMap(param, "[Data]")
        param = param.Remove(param.IndexOf("%")).Trim

        Return param
    End Function

    Friend Sub ShowEncTableData(ByVal index As Integer, ByVal frm As EncTable_Form)
        Dim sLine As String()

        Dim startLine As Integer = WorldMapData.EncTable.Values(index)
        Dim endLine As Integer = WorldMapData.EncTable.Values(index + 1) - startLine 'размер поиска

        look_name = GetFunction.ParamGetValueWM("lookup_name", startLine)
        Dim i As Integer = look_name.IndexOf(";")
        With frm
            If i > 0 Then
                .TextBox1.Text = look_name.Remove(i).Trim 'EncTable(n, 0)
                .TextBox3.Text = look_name.Substring(i).Trim
            Else
                .TextBox1.Text = look_name.Trim
                .TextBox3.Text = Nothing
            End If
            .TextBox2.Text = GetFunction.ParamGetValueWM("maps", startLine)


            For m = 0 To 100
                Dim enc As String = IIf(m < 10, "enc_0" & m, "enc_" & m)
                look_name = GetFunction.ParamGetValueWM(enc, startLine, endLine)
                If look_name Is Nothing Then Exit For
                sLine = look_name.Split(",")

                .ListView1.Items.Add(enc)
                .ListView1.Items(m).SubItems.Add(sLine(0).Substring(sLine(0).IndexOf(":") + 1))
                If sLine(1).StartsWith("Counter:") Then
                    'for special
                    .ListView1.Items(m).SubItems.Add(sLine(4).Substring(sLine(4).IndexOf(":") + 1).Trim) 'enc
                    .ListView1.Items(m).SubItems.Add("-")
                    .ListView1.Items(m).SubItems.Add("-")
                    .ListView1.Items(m).SubItems.Add(sLine(5).Trim) 'if
                    .ListView1.Items(m).SubItems.Add(sLine(3).Substring(sLine(3).IndexOf(":") + 1).Trim) 'map
                    .ListView1.Items(m).SubItems.Add(sLine(1).Substring(sLine(1).IndexOf(":") + 1).Trim) 'countr
                    .ListView1.Items(m).SubItems.Add(sLine(2).Trim) 'flag
                    .ListView1.Items(m).BackColor = Color.Beige
                    .ListView1.Items(m).Tag = Color.Beige
                Else
                    sLine(1) = sLine(1).Substring(sLine(1).IndexOf(":") + 1).Trim
                    i = sLine(1).IndexOf(AMBUSH) 'len=6
                    If i > 0 Then 'AMBUSH
                        .ListView1.Items(m).SubItems.Add(sLine(1).Remove(i).Trim)
                        .ListView1.Items(m).SubItems.Add(AMBUSH)
                        .ListView1.Items(m).SubItems.Add(sLine(1).Substring(i + 6).Trim)
                    Else
                        i = sLine(1).IndexOf(FIGHTING) 'len=8
                        If i > 0 Then 'FIGHTING
                            .ListView1.Items(m).SubItems.Add(sLine(1).Remove(i).Trim)
                            .ListView1.Items(m).SubItems.Add(FIGHTING)
                            .ListView1.Items(m).SubItems.Add(sLine(1).Substring(i + 8).Trim)
                        Else 'None
                            .ListView1.Items(m).SubItems.Add(sLine(1).Trim)
                            .ListView1.Items(m).SubItems.Add("-")
                            .ListView1.Items(m).SubItems.Add("-")
                        End If
                    End If
                    If sLine.Length > 2 Then .ListView1.Items(m).SubItems.Add(sLine(2).Trim) 'if
                    'добавить столбы
                    For n = 1 To 9 - .ListView1.Items(m).SubItems.Count()
                        .ListView1.Items(m).SubItems.Add("")
                    Next
                    .ListView1.Items(m).BackColor = Color.Ivory
                    .ListView1.Items(m).Tag = Color.Ivory
                End If
                startLine += 1
            Next
        End With
    End Sub

End Class
