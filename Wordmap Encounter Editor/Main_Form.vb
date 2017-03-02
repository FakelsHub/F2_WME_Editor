Public Class Main_Form
    Private LocationCursor As Point
    Private LocationCursorOffset As Point
    Private LocationPaint As Point
    Private hover_tile As Integer

    Protected Overrides ReadOnly Property CreateParams() As System.Windows.Forms.CreateParams
        Get
            Dim cp As CreateParams = MyBase.CreateParams
            cp.ExStyle = cp.ExStyle Or &H2000000
            Return cp
        End Get
    End Property

    Private Sub EnableDoubleBuffering()
        Me.SetStyle(ControlStyles.DoubleBuffer Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.UserPaint Or ControlStyles.AllPaintingInWmPaint, True)
        Me.UpdateStyles()
    End Sub

    Private Sub Main_Form_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        EnableDoubleBuffering()
    End Sub

    Friend Sub PictureBox_MouseUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        If e.Button = Windows.Forms.MouseButtons.Middle And sender.Cursor = Cursors.SizeAll Then sender.Cursor = Cursors.Cross
    End Sub

    Friend Sub PictureBox_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        If e.Button = Windows.Forms.MouseButtons.Middle And sender.Cursor <> Cursors.SizeAll Then
            sender.Cursor = Cursors.SizeAll
            LocationCursor = Cursor.Position
        End If
    End Sub

    Friend Sub PictureBox_MouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        If sender.Cursor = Cursors.SizeAll Then
            LocationCursorOffset = (Cursor.Position - LocationCursor)
            LocationPaint -= LocationCursorOffset
            LocationCursor = Cursor.Position
            Panel1.AutoScrollPosition = LocationPaint
            If LocationPaint.X < Panel1.AutoScrollPosition.X Then LocationPaint.X = 0
            If LocationPaint.Y < Panel1.AutoScrollPosition.Y Then LocationPaint.Y = 0
            If 0 - LocationPaint.Y < Panel1.AutoScrollPosition.Y Then LocationPaint.Y = 1 + Not (Panel1.AutoScrollPosition.Y)
            If 0 - LocationPaint.X < Panel1.AutoScrollPosition.X Then LocationPaint.X = 1 + Not (Panel1.AutoScrollPosition.X)
        Else
            Dim x As Integer = e.X
            Dim y As Integer = e.Y
            'ToolStripStatusLabel4.Text = e.Location.ToString
            On Error GoTo Skip
            hover_tile = GetXY_Tiles(x, y)
            sender.Tag = hover_tile
            ToolStripStatusLabel2.Text = "X: " & x
            ToolStripStatusLabel3.Text = "Y: " & y
            'нужно вычислить тайл
            Dim str() As String = GetParamWordMap(x & "_" & y, "[Tile " & hover_tile & "]").Split(",")
            ToolTip1.ToolTipTitle = str(0) & " [" & str(3) & "]"
            ToolTip1.SetToolTip(sender, "Encounter: " & str(5))
            look_name = str(5)
            ToolStripStatusLabel1.Text = "Tile: " & hover_tile

            ToolTip1.Active = True
            Timer1.Enabled = True
            Timer1.Interval = 10000
Skip:
        End If
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        ToolTip1.Active = False
        Timer1.Enabled = False
    End Sub

    Friend Sub PictureBox_MouseDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        If e.Button = Windows.Forms.MouseButtons.Left Then
            ToolTip1.Hide(sender)
            CreateEncTableForm()
        End If
    End Sub

    Private Sub EditEncontrToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles EditEncontrToolStripMenuItem.Click
        CreateEncTableForm()
    End Sub

    Private Sub EditTableToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles EditTableToolStripMenuItem.Click
        CreateTileTableForm(hover_tile, True)
    End Sub

    Private Sub TileDataConfigToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TileDataConfigToolStripMenuItem.Click
        If TileConfig_Form.Visible = False Then TileConfig_Form.Show(Me)
    End Sub

    Private Sub ToolStripButton4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButton4.Click
        On Error Resume Next
        TileSel_Form.Show(Me)
    End Sub

    Private Sub ToolStripButton5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButton5.Click
        look_name = EncTable(0, 0)
        CreateEncTableForm()
    End Sub

    Private Sub ToolStripButton6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub ToolStripButton7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButton7.Click
        If Info_Frm.Visible = False Then
            Info_Frm.Show(Me)
            Me.Focus()
        Else
            Info_Frm.Hide()
        End If
    End Sub

    Private Sub ToolStripButton8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButton8.Click
        If LocShow Then
            LocShow = False
            BigPicbox.BackgroundImage = Worldmap_Pic.Clone
            ShowLayer()
        Else
            ViewAreaLoc()
            LocShow = True
        End If
        BigPicbox.Refresh()
    End Sub

    Private Sub ToolStripButton9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButton9.Click
        If TileShow Then
            TileShow = False
            BigPicbox.BackgroundImage = Worldmap_Pic.Clone
            ShowLayer()
        Else
            ViewTileNumber()
            TileShow = True
        End If
        BigPicbox.Refresh()
    End Sub

    Private Sub ToolStripButton10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButton10.Click
        If EncShow Then
            EncShow = False
            BigPicbox.BackgroundImage = Worldmap_Pic.Clone
            ShowLayer()
        Else
            ViewEncType()
            EncShow = True
        End If
        BigPicbox.Refresh()
    End Sub

    Private Sub ShowLayer()
        If TileShow Then ViewTileNumber()
        If LocShow Then ViewAreaLoc()
        If EncShow Then ViewEncType()
    End Sub

    Private Sub Main_Form_Move(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Move
        Info_Frm.Top = Me.Top + 65
        Info_Frm.Left = Me.Right - Info_Frm.Width - 40
    End Sub

    Private Sub Main_Form_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Resize
        Main_Form_Move(sender, e)
    End Sub

#Region "Map Size"
    '50%
    Private Sub ToolStripMenuItem6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem6.Click
        If ToolStripMenuItem6.CheckState = CheckState.Unchecked Then
            ToolStripMenuItem6.CheckState = CheckState.Checked
            ZoomMap = 2
            ClearCheckState(50)
        End If
    End Sub
    '65%
    Private Sub ToolStripMenuItem5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem5.Click
        If ToolStripMenuItem5.CheckState = CheckState.Unchecked Then
            ToolStripMenuItem5.CheckState = CheckState.Checked
            ZoomMap = 1.5
            ClearCheckState(65)
        End If
    End Sub
    '80%
    Private Sub ToolStripMenuItem4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem4.Click
        If ToolStripMenuItem4.CheckState = CheckState.Unchecked Then
            ToolStripMenuItem4.CheckState = CheckState.Checked
            ZoomMap = 1.25
            ClearCheckState(80)
        End If
    End Sub
    '90%
    Private Sub ToolStripMenuItem3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem3.Click
        If ToolStripMenuItem3.CheckState = CheckState.Unchecked Then
            ToolStripMenuItem3.CheckState = CheckState.Checked
            ZoomMap = 1.15
            ClearCheckState(90)
        End If
    End Sub
    '100%
    Private Sub ToolStripMenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem2.Click
        If ToolStripMenuItem2.CheckState = CheckState.Unchecked Then
            ToolStripMenuItem2.CheckState = CheckState.Checked
            ZoomMap = 1.0
            ClearCheckState(100)
        End If
    End Sub

    Private Sub ClearCheckState(ByVal zoom As Integer)
        If zoom <> 50 Then ToolStripMenuItem6.CheckState = CheckState.Unchecked
        If zoom <> 65 Then ToolStripMenuItem5.CheckState = CheckState.Unchecked
        If zoom <> 80 Then ToolStripMenuItem4.CheckState = CheckState.Unchecked
        If zoom <> 90 Then ToolStripMenuItem3.CheckState = CheckState.Unchecked
        If zoom <> 100 Then ToolStripMenuItem2.CheckState = CheckState.Unchecked
        'redraw map
        ReSizeMap()
    End Sub
#End Region

    Private Sub ToolStripButton1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButton1.Click
        'Panel1.Visible = Not Panel1.Visible
        BigPicbox.Visible = Not BigPicbox.Visible
    End Sub

    Private Sub ToolStrip1_MouseEnter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStrip1.MouseEnter
        Me.Focus()
    End Sub

    Friend Sub PictureBox_MouseHover(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.Focus()
    End Sub

    Private Sub ОткрытьToolStripButton_ButtonClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ОткрытьToolStripButton.ButtonClick
        FolderBrowserDialog1.Description = "Please select game folder:"
        FolderBrowserDialog1.ShowNewFolderButton = False
        FolderBrowserDialog1.RootFolder = Environment.SpecialFolder.MyComputer
        If FolderBrowserDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            Start_Main(FolderBrowserDialog1.SelectedPath)
            ToolStripStatusLabel4.Text = "Folder: " & FolderBrowserDialog1.SelectedPath
            If GlobalError > -1 Then Exit Sub
            ActiveMenu()
        End If
    End Sub

    Private Sub OpenfileToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenfileToolStripMenuItem.Click
        'Panel1.Visible = True
        'Start_Main()
        ActiveMenu()
        Info_Frm.Show(Me)
    End Sub

    Private Sub ActiveMenu()
        SaveToolStripButton.Enabled = True
        ToolStripButton1.Enabled = True
        ToolStripButton2.Enabled = True
        ToolStripButton3.Enabled = True
        ToolStripButton4.Enabled = True
        ToolStripButton5.Enabled = True
        ToolStripButton6.Enabled = True
        ToolStripButton7.Enabled = True
        ToolStripButton8.Enabled = True
        ToolStripButton9.Enabled = True
        ToolStripButton10.Enabled = True
        TileDataConfigToolStripMenuItem.Enabled = True
    End Sub


    Private Sub Main_Form_FormClosed(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
        On Error Resume Next
        My.Computer.FileSystem.DeleteDirectory(Application.StartupPath & "\wm_res\art", FileIO.DeleteDirectoryOption.DeleteAllContents)
        My.Computer.FileSystem.DeleteDirectory(Application.StartupPath & "\wm_res\data", FileIO.DeleteDirectoryOption.DeleteAllContents)
    End Sub
End Class
