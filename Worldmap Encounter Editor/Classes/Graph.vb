Friend Class Graph

    Friend Shared BigPicbox As PictureBox
    Friend Shared Worldmap_Pic As Bitmap 'готовое изображение карты + сетка
    Private Shared SquareShape As PictureBox
    Private Shared grid As New Bitmap(350, 300, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
    Private Shared tilepic As Bitmap

    Friend Shared LocShow As Boolean
    Friend Shared TileShow As Boolean
    Friend Shared EncShow As Boolean
    Private Shared _highlight As Boolean 'подсветка квадрата

    Friend Shared ZoomMap As Double = 1.25

    Private Shared wm_X As Integer 'размер карты по Х в пикселях
    Private Shared wm_Y As Integer 'размер карты по Y в пикселях
    Private Shared tileReady As Boolean

    Friend Shared Property Highlight As Boolean
        Get
            Return _highlight
        End Get
        Set(ByVal value As Boolean)
            _highlight = value
            If Not (value) Then SquareShape.Dispose()
        End Set
    End Property

    Friend Shared Sub CreatePictureMap()
        Dim G As Graphics
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

        wm_X = (350 * WorldMapData.x_tiles)
        wm_Y = (300 * WorldMapData.y_tiles)
        Worldmap_Pic = New Bitmap(wm_X, wm_Y, System.Drawing.Imaging.PixelFormat.Format16bppRgb555)

        Main_Form.ToolStripProgressBar1.Maximum = WorldMapData.TilesData.Count
        Dim n As Integer = 0
        G = Graphics.FromImage(Worldmap_Pic)
        G.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
        ' Строим карту в строчку слева на право 
        For y = 0 To WorldMapData.y_tiles - 1
            For x = 0 To WorldMapData.x_tiles - 1
                wm_gif = New Bitmap(Image.FromFile(Application.StartupPath & Temp_path & Art_path & WorldMapData.TilesData(n).TileFrmName))
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

    Friend Shared Sub ReSizeMap()
        BigPicbox.Width = wm_X / ZoomMap
        BigPicbox.Height = wm_Y / ZoomMap
        If Graph.Highlight Then
            Graph.Highlight = False
        End If
        BigPicbox.BackgroundImage = Worldmap_Pic.Clone
        If Graph.LocShow Then Graph.ViewAreaLoc()
        If Graph.TileShow Then Graph.ViewTileNumber()
        'BigPicbox.Refresh()
    End Sub

    Friend Shared Sub ShowLayer()
        If Graph.TileShow Then Graph.ViewTileNumber()
        If Graph.LocShow Then Graph.ViewAreaLoc()
        If Graph.EncShow Then Graph.ViewEncType()
    End Sub

    Friend Shared Sub ViewAreaLoc()
        Dim G As Graphics
        Dim loc As New Bitmap(My.Resources.city)
        Dim XYLocCord As Point
        Dim rect As Rectangle
        Dim text_sz As Size
        Dim sz As Integer = Math.Ceiling(11 * (ZoomMap - 0.25))
        If sz > 17 Then sz = 17
        Dim LocFonts As New Font("Arial", sz, FontStyle.Bold)

        Dim drawFormat As New StringFormat
        drawFormat.LineAlignment = StringAlignment.Center
        'drawFormat.Alignment = StringAlignment.Center
        'drawFormat.FormatFlags = StringFormatFlags.NoWrap
        drawFormat.FormatFlags = StringFormatFlags.NoClip

        For n As Integer = 0 To CityList.Count - 1
            If CityList(n).LockState OrElse (Not CityList(n).ShowLoc) Then Continue For
            XYLocCord = CityList(n).Location
            XYLocCord.Offset(New Point(-20, -20)) ' корректируем координату
            rect = New Rectangle(XYLocCord, New Point(CityList(n).Size, CityList(n).Size))

            'circle
            G = Graphics.FromImage(BigPicbox.BackgroundImage)
            G.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBilinear
            G.CompositingMode = Drawing2D.CompositingMode.SourceOver
            G.CompositingQuality = Drawing2D.CompositingQuality.GammaCorrected
            G.DrawImage(loc, rect)

            ' print text
            G.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
            G.TextContrast = 2
            text_sz = G.MeasureString(CityList(n).Name, LocFonts).ToSize
            rect.X -= (text_sz.Width - rect.Width) / 2
            rect.Y += rect.Height
            rect.Height = 20
            rect.Width = text_sz.Width * 2
            G.DrawString(CityList(n).Name, LocFonts, Brushes.Lime, rect, drawFormat)
        Next
    End Sub

    ' Показать номера тайлов
    Friend Shared Sub ViewTileNumber()
        Dim G As Graphics
        If Not tileReady Then
            Dim n As Integer = 0
            Dim offset As Integer = 0
            Const x_offset As Integer = 220
            Const y_offset As Integer = 205
            tilepic = New Bitmap(wm_X, wm_Y, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            Dim NumFonts As New Font("Arial", 75, FontStyle.Bold)
            Dim drawBrush As New SolidBrush(Color.FromArgb(100, Color.WhiteSmoke))
            'получить координату тайла
            For y = 1 To WorldMapData.y_tiles
                For x = 1 To WorldMapData.x_tiles
                    If offset = 0 AndAlso n > 9 Then offset = 30
                    G = Graphics.FromImage(tilepic)
                    'G = Graphics.FromImage(BigPicbox.BackgroundImage)
                    G.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
                    G.DrawString(n.ToString, NumFonts, drawBrush, (x * 350) - (x_offset + offset), (y * 300) - y_offset)
                    n += 1
                Next
            Next
            tileReady = True
        End If
        G = Graphics.FromImage(BigPicbox.BackgroundImage)
        G.DrawImage(tilepic, 0, 0)
    End Sub

    Friend Shared Sub ViewEncType()
        Dim EncFonts As New Font("Arial", 9, FontStyle.Bold)
        Dim G As Graphics
        'Dim z As UInteger

        G = Graphics.FromImage(BigPicbox.BackgroundImage)
        G.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
        'G.TextContrast = 1
        For n_y = 0 To WorldMapData.y_tiles
            For n_x = 0 To WorldMapData.x_tiles
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

    Friend Shared Sub HighlightSquare(ByVal x_y As String, ByVal tile As Integer)
        Dim xy() As String = x_y.Split("_")
        Dim XYSquare As Point
        Dim x, y As Integer

        'Координаты квадрата на тайле.
        tile += 1
        y = Math.Ceiling(tile / WorldMapData.x_tiles)
        x = (tile - (WorldMapData.x_tiles * (y - 1))) - 1
        y -= 1
        XYSquare.X = (((350 / ZoomMap) * x) + (xy(0).Trim * (50 / ZoomMap))) + 1
        XYSquare.Y = (((300 / ZoomMap) * y) + (xy(1).Trim * (50 / ZoomMap))) + 1

        If Graph.Highlight Then SquareShape.Dispose()

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

        Graph.Highlight = True
    End Sub

    Friend Shared Function GetXY_Tiles(ByRef x As Integer, ByRef y As Integer) As Integer
        'узнаем # тайл по кооридинатам
        Dim x_tile As Integer = Math.Floor(x / (350 / Graph.ZoomMap))
        Dim y_tile As Integer = Math.Floor(y / (300 / Graph.ZoomMap))

        'узнаем квадрат x/y на тайле
        x = Math.Floor((x - ((350 / Graph.ZoomMap) * x_tile)) / (50 / Graph.ZoomMap))
        y = Math.Floor((y - ((300 / Graph.ZoomMap) * y_tile)) / (50 / Graph.ZoomMap))

        Return ((y_tile + 1) * WorldMapData.x_tiles) - (WorldMapData.x_tiles - x_tile) 'возвращаем номер тайла
    End Function

End Class
