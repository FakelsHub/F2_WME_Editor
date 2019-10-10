Imports System.IO

Module Main

    'Friend Declare Function SetParent Lib "user32" (ByVal hWndChild As Integer, ByVal hWndNewParent As Integer) As Integer

    Friend Game_path As String = Application.StartupPath 'для отладки
    Friend c_path As String

    Friend Const Temp_path As String = "\wm_res"
    Friend Const Data_path As String = "\data"
    Private Const MasterDat As String = "master.dat"
    Friend Const Art_path As String = "\art\intrface\"

    Friend WorldMapData As WMData
    Friend CityList As List(Of CityData) 'данные о городах / название, координата на карте, размер, lock_state
    Friend GlobalVar As List(Of String)
    Friend MapsName As List(Of String)   'имена карт из maps.txt
    Friend INTRFACE_LST As List(Of String)

    Friend look_name As String
    Friend GlobalError As SByte = -1

    Friend Sub LoadData(ByVal SelectPath As String)

        'On Error Resume Next
        If Directory.Exists(Application.StartupPath & Temp_path & "\art") Then
            Directory.Delete(Application.StartupPath & Temp_path & "\art", True)
        End If

        Game_path = SelectPath

        GlobalError = 0
        Try
            GlobalError += 1
            c_path = Check_File("art\intrface\intrface.lst")
            INTRFACE_LST = File.ReadAllLines(c_path & "\art\intrface\intrface.lst").ToList

            GlobalError += 1
            WorldMapData = New WMData()

            Directory.Delete(Application.StartupPath & "\art", True)
            Graph.CreatePictureMap()

            GlobalError += 1
            'получить  список карт
            GetFunction.MapsListName()

            GlobalError += 1
            'получить список городов/локаций
            GetFunction.CitiesData()

            GlobalError += 1
            'получить список имен глобальных переменных
            GetFunction.GlobalVars()

            GlobalError = -1
        Catch ex As Exception
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
        End Try

        Main_Form.ToolStripProgressBar1.Value = 0

    End Sub

    Friend Sub CreateTileTableForm(ByVal tile As Integer, Optional ByVal sel As Boolean = False)
        Dim TileTableFrm As New TilesData_Form
        Dim indx As Integer

        'EncTableFrm.MdiParent = Main_Form
        'SetParent(EncTableFrm.Handle.ToInt32, Main_Form.Panel1.Handle.ToInt32)

        With TileTableFrm
            .Text &= tile
            .Tag = tile
            .ComboBox1.Items.AddRange(INTRFACE_LST.ToArray)
            .ComboBox1.SelectedIndex = WorldMapData.TilesData(tile).TileFrmInx
            .TextBox1.Text = GetFunction.GetParamWordMap("walk_mask_name", "[Tile " & tile & "]", 46)
            .NumericUpDown1.Value = GetFunction.GetParamWordMap("encounter_difficulty", "[Tile " & tile & "]")
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
        Dim i, indx As Integer
        indx = GetFunction.LineIndexWM("0_0", "[Tile " & tile & "]")

        If Not type Then
            For x = 0 To 6
                For y = 0 To 5
                    Frm.TilesAddList(x & "_" & y, indx, i)
                Next
            Next
        Else
            For y = 0 To 5
                For x = 0 To 6
                    Frm.TilesAddList(x & "_" & y, indx, i)
                Next
            Next
        End If
    End Sub

    Friend Sub CreateEncTableForm()
        Dim EncTableFrm As New EncTable_Form

        'EncTableFrm.MdiParent = Main_Form
        'SetParent(EncTableFrm.Handle.ToInt32, Main_Form.Panel1.Handle.ToInt32)

        With EncTableFrm
            For n = 0 To WorldMapData.EncTable.Count - 1
                Dim key As String = WorldMapData.EncTable.Keys(n).ToString
                .ComboBox1.Items.Add(String.Format("Encounter Table {0} ({1})", n, key))
                If StrComp(key, look_name, CompareMethod.Text) = 0 Then
                    .ComboBox1.SelectedIndex = n
                    WorldMapData.ShowEncTableData(n, EncTableFrm)
                End If
            Next
            .Show(Main_Form)
        End With
    End Sub

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

End Module
