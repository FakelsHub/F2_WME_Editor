Public Class TilesData_Form
    Private texbox As TextBox
    Private combox As ComboBox

    Private ItemSel As Boolean
    Private Type As Boolean = False

    'save todo
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Button1.Enabled = False

    End Sub

    Private Sub ListView1_MouseDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListView1.MouseDoubleClick
        Dim x, c As Integer
        x = e.X
        'узнать на каком столбце был клик
        c = GetListViewColClick(sender, x)
        'Or c = 0 запрет на редактирование первой колонки
        If c = -1 Then Exit Sub
        Select Case c
            Case 1 'terrain
                combox = New ComboBox
                combox.Name = "InputComBox"
                combox.DropDownStyle = ComboBoxStyle.DropDownList
                combox.Tag = c
                combox.BackColor = Color.LightYellow
                combox.Width = ListView1.Columns.Item(c).Width + 10
                combox.Location = New Point(x, ListView1.Items(ListView1.FocusedItem.Index).Bounds.Location.Y - 4)
                AddHandler combox.Leave, AddressOf Me.InputComBoxExit
                AddHandler combox.KeyDown, AddressOf Me.InputKeyDown
                For n = 0 To Terrain.GetLength(0) - 1
                    combox.Items.Add(Terrain(n, 0))
                Next
                combox.Text = ListView1.Items(ListView1.FocusedItem.Index).SubItems(c).Text
                Me.ListView1.Controls.Add(combox)
                combox.Focus()
            Case 3, 4, 5 'enc percent
                combox = New ComboBox
                combox.Name = "InputComBox"
                combox.Items.AddRange(EncPercent)
                combox.DropDownStyle = ComboBoxStyle.DropDownList
                combox.Tag = c
                combox.BackColor = Color.LightYellow
                combox.Width = ListView1.Columns.Item(c).Width
                combox.Location = New Point(x, ListView1.Items(ListView1.FocusedItem.Index).Bounds.Location.Y - 4)
                AddHandler combox.Leave, AddressOf Me.InputComBoxExit
                AddHandler combox.KeyDown, AddressOf Me.InputKeyDown
                combox.Text = ListView1.Items(ListView1.FocusedItem.Index).SubItems(c).Text
                Me.ListView1.Controls.Add(combox)
                combox.Focus()
            Case 6 'enc
                combox = New ComboBox
                combox.Name = "InputComBox"
                combox.DropDownStyle = ComboBoxStyle.DropDownList
                combox.MaxDropDownItems = 20
                combox.Tag = c
                combox.BackColor = Color.LightYellow
                combox.Width = ListView1.Columns.Item(c).Width
                combox.Location = New Point(x, ListView1.Items(ListView1.FocusedItem.Index).Bounds.Location.Y - 4)
                combox.Sorted = True
                AddHandler combox.Leave, AddressOf Me.InputComBoxExit
                For n = 0 To EncTable.GetLength(0) - 1
                    combox.Items.Add(EncTable(n, 0))
                Next
                combox.Text = ListView1.Items(ListView1.FocusedItem.Index).SubItems(c).Text
                Me.ListView1.Controls.Add(combox)
                combox.Focus()
            Case Else
                texbox = New TextBox
                texbox.Name = "InputTexBox"
                texbox.Tag = c
                texbox.BorderStyle = BorderStyle.FixedSingle
                texbox.BackColor = Color.LightYellow
                texbox.Width = ListView1.Columns.Item(c).Width '- 4
                texbox.Location = New Point(x, ListView1.Items(ListView1.FocusedItem.Index).Bounds.Location.Y - 4)
                AddHandler texbox.Leave, AddressOf Me.InputTexBoxExit
                AddHandler texbox.KeyUp, AddressOf Me.InputTexBoxKeyPress
                AddHandler texbox.KeyDown, AddressOf Me.InputKeyDown
                Me.ListView1.Controls.Add(texbox)
                texbox.Text = ListView1.Items(ListView1.FocusedItem.Index).SubItems(c).Text
                texbox.Focus()
                texbox.SelectionLength = 0
        End Select
    End Sub

    Private Sub InputTexBoxExit()
        texbox.Dispose()
    End Sub
    Private Sub InputTexBoxKeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs)
        If e.KeyData = Keys.Enter Then
            If texbox.Text <> ListView1.Items(ListView1.FocusedItem.Index).SubItems(texbox.Tag).Text Then
                ListView1.Items(ListView1.FocusedItem.Index).SubItems(texbox.Tag).Text = texbox.Text
                ChangeList(texbox.Tag)
            End If
            InputTexBoxExit()
        ElseIf e.KeyData = Keys.Escape Then
            InputTexBoxExit()
        End If
    End Sub

    Private Sub InputComBoxExit()
        If combox.Text <> ListView1.Items(ListView1.FocusedItem.Index).SubItems(combox.Tag).Text Then
            ListView1.Items(ListView1.FocusedItem.Index).SubItems(combox.Tag).Text = combox.Text
            ChangeList(combox.Tag)
        End If
        combox.Dispose()
    End Sub

    Private Sub InputKeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs)
        If e.KeyCode = Keys.Enter Or e.KeyData = Keys.Escape Then
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub ChangeList(ByVal i As Integer)
        If i = 0 Then
            ListView1.Items(ListView1.FocusedItem.Index).BackColor = Color.MistyRose
            ListView1.Sort()
        Else
            For n = 1 To ListView1.Columns.Count - 1
                If n = i Then
                    ListView1.Items(ListView1.FocusedItem.Index).SubItems(i).BackColor = Color.MistyRose
                Else
                    If ListView1.Items(ListView1.FocusedItem.Index).SubItems(n).BackColor <> Color.MistyRose Then
                        ListView1.Items(ListView1.FocusedItem.Index).SubItems(n).BackColor = Color.White
                    End If
                End If
            Next
        End If
        Button1.Enabled = True
    End Sub
    '
    Private Sub ViewSqard0X0ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ViewSqard0X0ToolStripMenuItem.Click
        look_name = ListView1.Items(ListView1.FocusedItem.Index).SubItems(6).Text
        CreateEncTableForm()
    End Sub

    Private Sub HighlightToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HighlightToolStripMenuItem.Click
        If HighlightToolStripMenuItem.CheckState = CheckState.Unchecked Then HideSquareShape()
    End Sub

    Private Sub ListView1_ItemSelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ListViewItemSelectionChangedEventArgs) Handles ListView1.ItemSelectionChanged
        If HighlightToolStripMenuItem.CheckState = CheckState.Checked AndAlso e.IsSelected Then 'AndAlso Not (Highlight)
            'If ItemSel Then
            HighlightSquare(ListView1.Items(e.ItemIndex).Text, Me.Tag) ' ListView1.FocusedItem.Index
            '    ItemSel = False
            'Else
            '    ItemSel = True
            '    If Highlight Then SquareShape.Dispose()
            'End If
        End If
    End Sub

    Private Sub ListView1_MouseUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListView1.MouseUp
        If e.Button = Windows.Forms.MouseButtons.Middle Then
            If look_name.Length > 1 Then CreateEncTableForm()
        End If
    End Sub

    Private Sub ListView1_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListView1.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Middle Then
            look_name = ""
            'узнать на какой строке был клик
            Dim row = GetListViewRowClick(sender, e.Y)
            If row > -1 Then look_name = ListView1.Items(row).SubItems(6).Text
        End If
    End Sub

    Private Sub TilesData_Form_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        HideSquareShape()
    End Sub

    Private Sub TilesData_Form_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Resize
        If sender.WindowState = FormWindowState.Minimized Then HideSquareShape()
    End Sub

    Private Sub HideSquareShape()
        If Highlight Then
            SquareShape.Dispose()
            Highlight = False
        End If
    End Sub

    Private Sub ListView1_ColumnClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) Handles ListView1.ColumnClick
        Type = Not Type
        ListView1.Items.Clear()
        CreateTileTable_sub(Me.Tag, Me, Type)
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox1.SelectedIndexChanged
        If Me.Visible Then Button1.Enabled = True
    End Sub

    Private Sub NumericUpDown1_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NumericUpDown1.ValueChanged
        If Me.Visible Then Button1.Enabled = True
    End Sub

    Private Sub TextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox1.TextChanged
        If Me.Visible Then Button1.Enabled = True
    End Sub

End Class