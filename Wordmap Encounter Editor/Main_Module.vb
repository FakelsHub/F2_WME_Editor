Imports System.IO

Module Main_Module

    Friend Declare Function SetParent Lib "user32" (ByVal hWndChild As Integer, ByVal hWndNewParent As Integer) As Integer

    Friend Game_path As String = Application.StartupPath 'для отладки
    Friend c_path As String

    Const Temp_path As String = "\wm_res"
    Const Data_path As String = "\data"
    Const MasterDat As String = "master.dat"
    Const Art_path As String = "\art\intrface\"

    Friend SquareShape As PictureBox
    Friend BigPicbox As PictureBox
    Friend Worldmap_Pic As Bitmap 'готовое изображение карты + сетка
    Friend grid As New Bitmap(350, 300, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
    Friend tilepic As Bitmap

    Friend INTRFACE_LST() As String
    Friend x_tiles As Integer
    Friend y_tiles As Integer
    Friend total_tiles As UInteger

    Friend TileName(0) As String
    Friend TileFrmInx(0) As String
    Friend TileFrmName(0) As String

    Friend EncTable(0, 1) As String ' 0=look name / 1=index in WorldMap_TXT 
    Friend Encounter(0, 1) As String '0=name / 1= index in WorldMap_TXT 
    Friend GlobalVar() As String

    Friend Terrain(0, 1) As String
    'Friend EncPercent(0, 1) As Percent '
    Friend EncPercent() As String = {"Forced", "Frequent", "Common", "Uncommon", "Rare", "None"} 'default

    Friend MapsName() As String 'имена карт из maps.txt
    Friend CityData(0, 0) As String 'данные о городах 0-название, 1-координата на карте, 2-размер, 3-lock_state

    Friend look_name As String

    Friend ZoomMap As Double = 1.25
    Friend wm_X As Integer 'размер карты по Х в пикселях
    Friend wm_Y As Integer 'размер карты по Y в пикселях

    Friend LocShow As Boolean
    Friend TileShow As Boolean
    Friend EncShow As Boolean
    Friend Highlight As Boolean 'подсветка квадрата

    Private TileReady As Boolean

    Friend GlobalError As SByte = -1

    Private Const LargeSize As Integer = 40
    Private Const MediumSize As Integer = 25
    Private Const SmallSize As Integer = 15

    Friend Sub Start_Main(ByVal SelectPath As String)
        Dim tmp As String
        On Error Resume Next
        My.Computer.FileSystem.DeleteDirectory(Application.StartupPath & Temp_path & "\art", FileIO.DeleteDirectoryOption.DeleteAllContents)

        'Game_path = SelectPath
        On Error GoTo ErrorPath
        GlobalError = 0
        c_path = Check_File("data\WORLDMAP.TXT")
        ReadWM_TXT(c_path & Data_path)

        GlobalError += 1
        c_path = Check_File("art\intrface\intrface.lst")
        'On Error GoTo LstBadFile
        INTRFACE_LST = IO.File.ReadAllLines(c_path & "\art\intrface\intrface.lst")
        'ClearTrimLine(INTRFACE_LST)
        'ClearEmptyLine(INTRFACE_LST)
        'On Error GoTo 0

        'получаем количетво тайлов
        x_tiles = GetParamWordMap("num_horizontal_tiles", "[Tile Data]")
        total_tiles = GetTileCountWM()
        y_tiles = ((1 + total_tiles) / x_tiles)

        'создаем массив с именами тайлов [Tile ###]
        ReDim TileName(total_tiles)
        ReDim TileFrmInx(total_tiles)
        For n As UInteger = 0 To total_tiles
            TileName(n) = "[Tile " & n & "]"
            TileFrmInx(n) = GetParamWordMap("art_idx", TileName(n))
        Next

        GlobalError += 1
        Main_Form.ToolStripProgressBar1.Maximum = TileFrmInx.Length
        ReDim TileFrmName(total_tiles)
        'преобразовать frm > gif
        For n = 0 To TileFrmInx.Length - 1
            tmp = INTRFACE_LST(TileFrmInx(n))
            TileFrmName(n) = tmp.Remove(tmp.IndexOf(Chr(32)))
            TileFrmName(n) = WMFrmGif(TileFrmName(n)) 'конвертим и возвращаем изменненое имя
            Main_Form.ToolStripProgressBar1.Value = n
            Application.DoEvents()
        Next
        My.Computer.FileSystem.DeleteDirectory(Application.StartupPath & "\art", FileIO.DeleteDirectoryOption.DeleteAllContents)
        '
        New_CreateMap()
        '
        'создать [Encounter Table] массив
        ReDim EncTable(GetEncTCountWM(), 1)
        Dim z As Integer
        For n = 0 To EncTable.GetLength(0) - 1
            tmp = GetParamWordMap("lookup_name", "[Encounter Table " & n & "]")
            z = tmp.IndexOf(";")
            If z > 0 Then tmp = tmp.Remove(z)
            EncTable(n, 0) = tmp.Trim
            EncTable(n, 1) = FindLine(WorldMap_TXT, "[Encounter Table " & n & "]")
        Next

        'создать [Encounter: Name] массив
        Dim s As Integer
        tmp = "Encounter:"
        ReDim Encounter(GetSectionCountWM(tmp, WorldMap_TXT), 1)
        Dim index() As String = tmp.Split("|")
        tmp = Nothing
        For n = 0 To Encounter.GetLength(0) - 1
            tmp = WorldMap_TXT(CInt(index(n))).Trim
            z = tmp.IndexOf("]")
            s = tmp.IndexOf(":") + 1
            Encounter(n, 0) = tmp.Substring(s, z - s).Trim
            Encounter(n, 1) = CInt(index(n))
        Next

        'Enc Percent & Terrain
        tmp = GetParamWordMap("terrain_types", "[Data]")
        Dim sLine() As String = tmp.Split(",")
        ReDim Terrain(sLine.Length - 1, 1)
        For n = 0 To sLine.Length - 1
            z = sLine(n).IndexOf(":")
            Terrain(n, 0) = sLine(n).Remove(z).Trim
            Terrain(n, 1) = sLine(n).Substring(z + 1).Trim
        Next

        GlobalError += 1
        'получить  список карт
        c_path = Check_File("data\MAPS.TXT")
        Dim tMassive() As String = File.ReadAllLines(c_path & Data_path & "\MAPS.TXT")
        ReDim MapsName(GetSectionCount("Map", tMassive))
        GetMapsListName(tMassive)

        GlobalError += 1
        'получить список городов/локаций
        c_path = Check_File("data\CITY.TXT")
        tMassive = File.ReadAllLines(c_path & Data_path & "\CITY.TXT")
        ReDim CityData(GetSectionCount("Area", tMassive), 3)
        GetCityData(tMassive)

        GlobalError += 1
        'получить список имен глобальных переменных
        c_path = Check_File("data\VAULT13.GAM")
        tMassive = File.ReadAllLines(c_path & Data_path & "\VAULT13.GAM")
        s = 0
        For n = 0 To tMassive.Length - 1
            If tMassive(n).StartsWith("GVAR_", StringComparison.CurrentCultureIgnoreCase) Then
                z = tMassive(n).IndexOf(":")
                ReDim Preserve GlobalVar(s)
                GlobalVar(s) = tMassive(n).Substring(5, z - 5).Trim
                s += 1
            End If
        Next
        GlobalError = -1
        Main_Form.ToolStripProgressBar1.Value = 0
        Exit Sub
ErrorPath:
        Select Case GlobalError
            Case 0
                MsgBox("Not found WORLDMAP.TXT file.", MsgBoxStyle.Critical, "Incorect game folder")
            Case 1
                MsgBox("Not found INTRFACE.LST file.", MsgBoxStyle.Critical, "Incorect game folder")
            Case 2
                MsgBox("Error when creating image maps.", MsgBoxStyle.Critical, "Worldmap Editor")
            Case 3
                MsgBox("Not found MAPS.TXT file.", MsgBoxStyle.Critical, "Incorect game folder")
            Case 4
                MsgBox("Not found CITY.TXT file.", MsgBoxStyle.Critical, "Incorect game folder")
            Case 5
                MsgBox("Not found VAULT13.GAM file.", MsgBoxStyle.Critical, "Incorect game folder")
            Case Else
                MsgBox("Incorect game folder.", MsgBoxStyle.Critical, "Worldmap Editor")
        End Select
    End Sub

    Friend Sub New_CreateMap()
        Dim G As Graphics
        Dim n As Integer
        Dim wm_gif As Bitmap 'для изображения gif 300x350

        'создаем сетку
        G = Graphics.FromImage(grid)
        G.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
        For x = 0 To 6
            If x = 0 Then
                G.DrawLine(Pens.Gold, 0, 0, 0, 300) 'x
            Else
                G.DrawLine(Pens.Gray, 50 * x, 0, 50 * x, 300) 'x
            End If
        Next
        For y = 0 To 5
            If y = 0 Then
                G.DrawLine(Pens.Gold, 0, 0, 350, 0) 'y
            Else
                G.DrawLine(Pens.Gray, 0, 50 * y, 350, 50 * y) 'y
            End If
        Next

        wm_X = (350 * x_tiles)
        wm_Y = (300 * y_tiles)
        Worldmap_Pic = New Bitmap(wm_X, wm_Y, System.Drawing.Imaging.PixelFormat.Format16bppRgb555)

        Main_Form.ToolStripProgressBar1.Maximum = TileFrmInx.Length
        n = 0
        G = Graphics.FromImage(Worldmap_Pic)
        G.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
        ' Строим карту в строчку слева на право 
        For y = 0 To y_tiles - 1
            For x = 0 To x_tiles - 1
                wm_gif = New Bitmap(Image.FromFile(Application.StartupPath & Temp_path & Art_path & TileFrmName(n)))
                G.DrawImage(wm_gif, 350 * x, 300 * y) 'накладываем чистое изображение
                G.DrawImage(grid, 350 * x, 300 * y) 'накладываем сетку
                n += 1
                Main_Form.ToolStripProgressBar1.Value = n
                Application.DoEvents()
            Next
        Next
        'досоздаем желтую сетку
        Dim GoldPen As New System.Drawing.Pen(Color.Gold, 3)
        G.DrawLine(GoldPen, 1, 0, 1, wm_Y) 'y
        G.DrawLine(GoldPen, wm_X - 2, 0, wm_X - 2, wm_Y) 'y
        '
        G.DrawLine(GoldPen, 0, 1, wm_X, 1) 'x
        G.DrawLine(GoldPen, 0, wm_Y - 2, wm_X, wm_Y - 2) 'x

        'задел на будующее
        'Dim wp As Integer = Main_Form.Panel1.Width - 10
        'Dim hp As Integer = Main_Form.Panel1.Height - 10
        'Dim Pic = New Bitmap(wp, hp, System.Drawing.Imaging.PixelFormat.Format16bppRgb555)
        'G = Graphics.FromImage(Pic)
        'G.DrawImageUnscaledAndClipped(Worldmap_Pic, New Rectangle(-150, -150, wp + 150, hp + 150))

        BigPicbox = New PictureBox
        With BigPicbox
            .Name = "PictureBox_WM"
            .Width = wm_X / ZoomMap '350 * tiles_x 
            .Height = wm_Y / ZoomMap '300 * tiles_y
            .Cursor = Cursors.Cross
            .Location = New Point(5, 5)
            .BackgroundImage = Worldmap_Pic.Clone
            .BackgroundImageLayout = ImageLayout.Zoom
            .ContextMenuStrip = Main_Form.ContextMenuStrip1
            AddHandler .MouseMove, AddressOf Main_Form.PictureBox_MouseMove
            AddHandler .MouseHover, AddressOf Main_Form.PictureBox_MouseHover
            AddHandler .MouseDown, AddressOf Main_Form.PictureBox_MouseDown
            AddHandler .MouseDoubleClick, AddressOf Main_Form.PictureBox_MouseDoubleClick
            AddHandler .MouseUp, AddressOf Main_Form.PictureBox_MouseUp
        End With
        Main_Form.Panel1.Controls.Add(BigPicbox)
    End Sub

    Friend Sub ReSizeMap()
        BigPicbox.Width = wm_X / ZoomMap
        BigPicbox.Height = wm_Y / ZoomMap
        If Highlight Then
            SquareShape.Dispose()
            Highlight = False
        End If
        BigPicbox.BackgroundImage = Worldmap_Pic.Clone
        If LocShow Then ViewAreaLoc()
        If TileShow Then ViewTileNumber()
        'BigPicbox.Refresh()
    End Sub

    Friend Sub ViewEncType()
        Dim EncFonts As New Font("Arial", 9, FontStyle.Bold)
        Dim G As Graphics
        'Dim z As UInteger

        G = Graphics.FromImage(BigPicbox.BackgroundImage)
        G.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
        'G.TextContrast = 1
        For n_y = 0 To y_tiles
            For n_x = 0 To x_tiles
                'отрисовка для текущего n тайла 
                For y = 0 To 5
                    For x = 0 To 6
                        G.DrawString("TEST" & y & x, EncFonts, Brushes.LightGray, (350 * n_x) + (50 * x), (300 * n_y) + ((50 * y) + 15))
                    Next
                Next
            Next
        Next
        'BigPicbox.Refresh()
    End Sub

    Friend Sub ViewAreaLoc()
        Dim sz As Integer = Math.Ceiling(11 * (ZoomMap - 0.25))
        If sz > 17 Then sz = 17
        Dim LocFonts As New Font("Arial", sz, FontStyle.Bold)
        Dim G As Graphics
        Dim text_sz As Size
        Dim loc As New Bitmap(My.Resources.city)
        Dim XYLocCord, Size As Point
        Dim rect As Rectangle
        Dim lsz As Integer
        Dim x_y() As String

        Dim drawFormat As New StringFormat
        drawFormat.LineAlignment = StringAlignment.Center
        'drawFormat.Alignment = StringAlignment.Center
        'drawFormat.FormatFlags = StringFormatFlags.NoWrap
        drawFormat.FormatFlags = StringFormatFlags.NoClip

        For n = 0 To CityData.GetLength(0) - 1
            If CityData(n, 3).ToLower = "on" Then GoTo no_loc
            x_y = CityData(n, 1).Split(",")
            XYLocCord = New Point(x_y(0).Trim, x_y(1).Trim)
            'корректируем смещение координат
            XYLocCord -= New Point(20, 20)
            Select Case CityData(n, 2).ToLower
                Case "small"
                    lsz = SmallSize
                Case "medium"
                    lsz = MediumSize
                Case "large"
                    lsz = LargeSize
                Case Else
                    Exit Sub
            End Select
            Size = New Point(lsz, lsz)
            rect = New Rectangle(XYLocCord, Size)
            'circle
            G = Graphics.FromImage(BigPicbox.BackgroundImage)
            G.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBilinear
            G.CompositingMode = Drawing2D.CompositingMode.SourceOver
            G.CompositingQuality = Drawing2D.CompositingQuality.GammaCorrected
            G.DrawImage(loc, rect)
            'text
            G.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
            G.TextContrast = 2
            text_sz = G.MeasureString(CityData(n, 0), LocFonts).ToSize
            rect.X -= (text_sz.Width - rect.Width) / 2
            rect.Y += rect.Height
            rect.Height = 20
            rect.Width = text_sz.Width * 2
            G.DrawString(CityData(n, 0), LocFonts, Brushes.Lime, rect, drawFormat)
no_loc:
        Next
    End Sub

    ' Показать номера тайлов
    Friend Sub ViewTileNumber()
        Dim G As Graphics
        If Not TileReady Then
            Dim n As Integer = 0
            Dim offset As Integer = 0
            Const x_offset As Integer = 220
            Const y_offset As Integer = 205
            tilepic = New Bitmap(wm_X, wm_Y, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            Dim NumFonts As New Font("Arial", 75, FontStyle.Bold)
            Dim drawBrush As New SolidBrush(Color.FromArgb(100, Color.WhiteSmoke))
            'получить координату тайла
            For y = 1 To y_tiles
                For x = 1 To x_tiles
                    If offset = 0 AndAlso n > 9 Then offset = 30
                    G = Graphics.FromImage(tilepic)
                    'G = Graphics.FromImage(BigPicbox.BackgroundImage)
                    G.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
                    G.DrawString(n.ToString, NumFonts, drawBrush, (x * 350) - (x_offset + offset), (y * 300) - y_offset)
                    n += 1
                Next
            Next
            TileReady = True
        End If
        G = Graphics.FromImage(BigPicbox.BackgroundImage)
        G.DrawImage(tilepic, 0, 0)
    End Sub

    Friend Sub HighlightSquare(ByVal x_y As String, ByVal tile As Integer)
        Dim xy() As String = x_y.Split("_")
        Dim XYSquare As Point
        Dim x, y As Integer

        'Координаты квадрата на тайле.
        tile += 1
        y = Math.Ceiling(tile / x_tiles)
        x = (tile - (x_tiles * (y - 1))) - 1
        y -= 1
        XYSquare.X = (((350 / ZoomMap) * x) + (xy(0).Trim * (50 / ZoomMap))) + 1
        XYSquare.Y = (((300 / ZoomMap) * y) + (xy(1).Trim * (50 / ZoomMap))) + 1

        If Highlight Then SquareShape.Dispose()
        SquareShape = New PictureBox
        With SquareShape
            .Name = "SquareShape"
            .Size = New Point((50 / ZoomMap), (50 / ZoomMap))
            .BorderStyle = BorderStyle.None
            .Location = XYSquare
            .Image = My.Resources.shape
            .SizeMode = PictureBoxSizeMode.Zoom
        End With
        Main_Form.Panel1.Controls.Add(SquareShape)
        SquareShape.Parent = BigPicbox
        'SquareShape.BringToFront()

        Highlight = True
    End Sub

    Friend Function Get_PercentEnc(ByVal param As String) As String
        param = GetParamWordMap(param, "[Data]")
        param = param.Remove(param.IndexOf("%")).Trim
        Return param
    End Function

    Friend Sub CreateTileTableForm(ByVal tile As Integer, Optional ByVal sel As Boolean = False)
        Dim TileTableFrm As New TilesData_Form
        Dim indx As Integer
        'EncTableFrm.MdiParent = Main_Form
        'SetParent(EncTableFrm.Handle.ToInt32, Main_Form.Panel1.Handle.ToInt32)
        With TileTableFrm
            .Text &= tile
            .Tag = tile
            .ComboBox1.Items.AddRange(INTRFACE_LST)
            .ComboBox1.SelectedIndex = TileFrmInx(tile)
            .TextBox1.Text = GetParamWordMap("walk_mask_name", "[Tile " & tile & "]", 46)
            .NumericUpDown1.Value = GetParamWordMap("encounter_difficulty", "[Tile " & tile & "]")
            CreateTileTable_sub(tile, TileTableFrm)
            If sel Then
                indx = (CInt(Main_Form.ToolStripStatusLabel2.Text.Substring(3, 1)) * 6) + CInt(Main_Form.ToolStripStatusLabel3.Text.Substring(3, 1))
            Else
                indx = 0
            End If
            .ListView1.Items(indx).Selected = True
            .Show(Main_Form)
        End With
    End Sub

    Friend Sub CreateTileTable_sub(ByRef tile As Integer, ByRef Frm As TilesData_Form, Optional ByVal type As Boolean = False)
        Dim sLine() As String
        Dim indx, i As Integer
        indx = GetIndexWordMap("0_0", "[Tile " & tile & "]")
        With Frm
            If Not type Then
                For x = 0 To 6
                    For y = 0 To 5
                        .ListView1.Items.Add(x & "_" & y)
                        sLine = GetParamWordMap_sub(x & "_" & y, indx).Split(",")
                        .ListView1.Items(i).SubItems.AddRange(sLine)
                        .ListView1.Items(i).UseItemStyleForSubItems = False
                        .ListView1.Items(i).BackColor = Color.LemonChiffon 'Color.FromArgb(230, 230, 230)
                        i += 1
                    Next
                Next
            Else
                For y = 0 To 5
                    For x = 0 To 6
                        .ListView1.Items.Add(x & "_" & y)
                        sLine = GetParamWordMap_sub(x & "_" & y, indx).Split(",")
                        .ListView1.Items(i).SubItems.AddRange(sLine)
                        .ListView1.Items(i).UseItemStyleForSubItems = False
                        .ListView1.Items(i).BackColor = Color.LemonChiffon 'Color.FromArgb(230, 230, 230)
                        i += 1
                    Next
                Next
            End If
        End With
    End Sub

    Friend Sub CreateEncTableForm()
        Dim EncTableFrm As New EncTable_Form
        'EncTableFrm.MdiParent = Main_Form
        'SetParent(EncTableFrm.Handle.ToInt32, Main_Form.Panel1.Handle.ToInt32)
        With EncTableFrm
            For n = 0 To EncTable.GetLength(0) - 1
                .ComboBox1.Items.Add("Encounter Table " & n & "  (" & EncTable(n, 0) & ")")
                If StrComp(EncTable(n, 0), look_name, CompareMethod.Text) = 0 Then
                    .ComboBox1.SelectedIndex = n
                    ShowEncTableData(n, EncTableFrm)
                End If
            Next
            .Show(Main_Form)
        End With
    End Sub

    Friend Sub ShowEncTableData(ByRef NumEncTable As Integer, ByRef frm As EncTable_Form)
        Dim i, l, StopLine As Integer
        Dim sLine() As String

        look_name = GetParamWordMap_sub("lookup_name", EncTable(NumEncTable, 1))
        i = look_name.IndexOf(";")
        With frm
            If i > 0 Then
                .TextBox1.Text = look_name.Remove(i).Trim 'EncTable(n, 0)
                .TextBox3.Text = look_name.Substring(i).Trim
            Else
                .TextBox1.Text = look_name.Trim
                .TextBox3.Text = Nothing
            End If
            .TextBox2.Text = GetParamWordMap_sub("maps", EncTable(NumEncTable, 1))
            l = EncTable(NumEncTable, 1)
            StopLine = EncTable(NumEncTable + 1, 1) - l 'размер поиска
            For m = 0 To 100
                If m < 10 Then look_name = GetParamWordMap_sub("enc_0" & m, l, StopLine) Else look_name = GetParamWordMap_sub("enc_" & m, l, StopLine)
                l += 1
                If look_name <> Nothing Then
                    sLine = look_name.Split(",")
                    If m < 10 Then .ListView1.Items.Add("enc_0" & m) Else .ListView1.Items.Add("enc_" & m)
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
                        i = sLine(1).IndexOf("AMBUSH") 'len=6
                        If i > 0 Then 'AMBUSH
                            .ListView1.Items(m).SubItems.Add(sLine(1).Remove(i).Trim)
                            .ListView1.Items(m).SubItems.Add("AMBUSH")
                            .ListView1.Items(m).SubItems.Add(sLine(1).Substring(i + 6).Trim)
                        Else
                            i = sLine(1).IndexOf("FIGHTING") 'len=8
                            If i > 0 Then 'FIGHTING
                                .ListView1.Items(m).SubItems.Add(sLine(1).Remove(i).Trim)
                                .ListView1.Items(m).SubItems.Add("FIGHTING")
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
                Else : Exit For : End If
            Next
        End With
    End Sub

    Friend Function GetXY_Tiles(ByRef x As Integer, ByRef y As Integer) As Integer
        'узнаем # тайл по кооридинатам
        Dim x_tile As Integer = Math.Floor(x / (350 / ZoomMap))
        Dim y_tile As Integer = Math.Floor(y / (300 / ZoomMap))

        'узнаем квадрат x/y на тайле
        x = Math.Floor((x - ((350 / ZoomMap) * x_tile)) / (50 / ZoomMap))
        y = Math.Floor((y - ((300 / ZoomMap) * y_tile)) / (50 / ZoomMap))

        Return ((y_tile + 1) * x_tiles) - (x_tiles - x_tile) 'возвращаем номер тайла
    End Function

    'Проверить наличее файла если его нет то извлеч из Dat
    Friend Function Check_File(ByRef pfile As String, Optional ByRef unpack As Boolean = True) As String
        If My.Computer.FileSystem.FileExists(Game_path & Data_path & "\" & pfile) = True Then
            Return Game_path & Data_path
        ElseIf My.Computer.FileSystem.FileExists(Game_path & "\" & MasterDat & "\" & pfile) = True Then
            Return Game_path & "\" & MasterDat  'папка
        Else
            If My.Computer.FileSystem.FileExists(Application.StartupPath & Temp_path & "\" & pfile) = False And unpack Then
                'Извлечь требуемый файл в кэш
                'Main_Form.TextBox1.Text = "Извлечение: " & cFile & vbCrLf & Main_Form.TextBox1.Text
                Shell(Application.StartupPath & "\dat2.exe x -d wm_res """ & Game_path & "\master.dat"" " & pfile, AppWinStyle.Hide, True)
            End If
            Return Application.StartupPath & Temp_path
        End If
    End Function

    Friend Function WMFrmGif(ByVal FrmFile As String) As String
        Dim pfile As String = Art_path & FrmFile
        On Error Resume Next
        If My.Computer.FileSystem.FileExists(Game_path & Data_path & pfile) = True Then
            My.Computer.FileSystem.CopyFile(Game_path & Data_path & pfile, Application.StartupPath & pfile)
            Shell(Application.StartupPath & "\frm2gif.exe -p color.pal ." & pfile, AppWinStyle.Hide, True)
        ElseIf My.Computer.FileSystem.FileExists(Game_path & "\" & MasterDat & pfile) = True Then
            My.Computer.FileSystem.CopyFile(Game_path & "\" & MasterDat & pfile, Application.StartupPath & pfile)
            Shell(Application.StartupPath & "\frm2gif.exe -p color.pal ." & pfile, AppWinStyle.Hide, True)
        Else 'Извлечь требуемый файл
            Shell(Application.StartupPath & "\frm2gif.exe -d -f """ & Game_path & "\master.dat"" -p color.pal " & FrmFile, AppWinStyle.Hide, True)
        End If
        'меняем разрешение файла на gif
        FrmFile = FrmFile.Substring(0, FrmFile.IndexOf(".")) & ".gif"
        My.Computer.FileSystem.MoveFile(Application.StartupPath & Art_path & FrmFile, Application.StartupPath & Temp_path & Art_path & FrmFile)
        Return FrmFile
    End Function

    'узнать на каком столбце listview был сделан клик
    Friend Function GetListViewColClick(ByRef lw As ListView, ByRef x As Integer) As Integer
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
    Friend Function GetListViewRowClick(ByRef lw As ListView, ByRef y As Integer) As Integer
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

End Module
