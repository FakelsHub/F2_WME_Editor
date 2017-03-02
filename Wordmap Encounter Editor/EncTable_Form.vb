Public Class EncTable_Form
    Private texbox As TextBox
    Private combox As ComboBox
    Private numbox As NumericUpDown
    Private numFrm As Byte

    Dim col As Integer

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub EditConditionToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles EditConditionToolStripMenuItem.Click
        Dim EncCondFrm As New EncCond_Form
        ComboBox1.Enabled = False
        OK_Button.Visible = False
        numFrm += 1
        EncCondFrm.Ini_Load(ListView1.Items(ListView1.FocusedItem.Index).SubItems(5).Text)
        EncCondFrm.Text &= ListView1.Items(ListView1.FocusedItem.Index).Text
        EncCondFrm.Show(Me)
    End Sub

    Private Sub EditToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles EditToolStripMenuItem.Click
        Dim EncEditFrm As New EncEdit_Form
        EncEditFrm.Tag = ListView1.FocusedItem.Index
        Dim attack As String = ListView1.Items(ListView1.FocusedItem.Index).SubItems(2).Text
        Dim target As String = ListView1.Items(ListView1.FocusedItem.Index).SubItems(4).Text
        Dim vs As String = ListView1.Items(ListView1.FocusedItem.Index).SubItems(3).Text
        ComboBox1.Enabled = False
        OK_Button.Visible = False
        numFrm += 1
        EncEditFrm.Ini_Load(attack, target, vs, Me)
        'For Each cnt As Windows.Forms.Control In Me.Controls
        'cnt.Enabled = False
        'Next
        EncEditFrm.Text &= ListView1.Items(ListView1.FocusedItem.Index).Text
        EncEditFrm.Show(Me)
    End Sub

    Friend Sub EncEditReturn(ByVal frm As EncEdit_Form, Optional ByVal change As Boolean = False)
        'For Each cnt As Windows.Forms.Control In Me.Controls
        'cnt.Enabled = True
        'Next
        numFrm -= 1
        If numFrm <= 0 Then
            ComboBox1.Enabled = True
            OK_Button.Visible = True
            numFrm = 0
        End If
        If change Then
            ListView1.Items(frm.Tag).SubItems(2).Text = frm.attack
            ListView1.Items(frm.Tag).SubItems(4).Text = frm.target
            ListView1.Items(frm.Tag).SubItems(3).Text = frm.vs
            OK_Button.Enabled = True
        End If
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox1.SelectedIndexChanged
        If Me.Visible Then
            ListView1.Items.Clear()
            ShowEncTableData(ComboBox1.SelectedIndex, Me)
        End If
    End Sub

    Private Sub ListView1_MouseDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListView1.MouseDoubleClick
        Dim x, c As Integer
        x = e.X
        'узнать на каком столбце был клик
        c = GetListViewColClick(sender, x)
        'Or c = 0 запрет на редактирование первой колонки
        If c = -1 Then Exit Sub
        Select Case c
            Case 1 '%
                numbox = New NumericUpDown
                numbox.Name = "InputNumBox"
                numbox.Tag = c
                numbox.BackColor = Color.LightYellow
                numbox.TextAlign = HorizontalAlignment.Right
                numbox.Width = ListView1.Columns.Item(c).Width '+ 10
                numbox.Location = New Point(x, ListView1.Items(ListView1.FocusedItem.Index).Bounds.Location.Y - 4)
                AddHandler numbox.Leave, AddressOf Me.InputNumBoxExit
                AddHandler numbox.KeyUp, AddressOf Me.InputNumBoxKeyPress
                AddHandler numbox.KeyDown, AddressOf Me.InputKeyDown
                numbox.Value = ListView1.Items(ListView1.FocusedItem.Index).SubItems(c).Text.Replace("%", "")
                Me.ListView1.Controls.Add(numbox)
                numbox.Focus()
            Case 3 ' VS
                combox = New ComboBox
                combox.Name = "InputComBox"
                combox.Items.AddRange({"-", "AMBUSH", "FIGHTING"})
                combox.DropDownStyle = ComboBoxStyle.DropDownList
                combox.Tag = c
                combox.BackColor = Color.LightYellow
                combox.Width = ListView1.Columns.Item(c).Width + 10
                combox.Location = New Point(x, ListView1.Items(ListView1.FocusedItem.Index).Bounds.Location.Y - 4)
                AddHandler combox.Leave, AddressOf Me.InputComBoxExit
                AddHandler combox.KeyDown, AddressOf Me.InputKeyDown
                combox.Text = ListView1.Items(ListView1.FocusedItem.Index).SubItems(c).Text
                Me.ListView1.Controls.Add(combox)
                combox.Focus()
            Case 6 'spec map
                combox = New ComboBox
                combox.Name = "InputComBox"
                combox.Items.Add("")
                combox.Items.AddRange(MapsName)
                combox.DropDownStyle = ComboBoxStyle.DropDownList
                combox.MaxDropDownItems = 20
                combox.DropDownWidth = 175
                combox.Tag = c
                combox.BackColor = Color.LightYellow
                combox.Width = ListView1.Columns.Item(c).Width + 10
                combox.Location = New Point(x, ListView1.Items(ListView1.FocusedItem.Index).Bounds.Location.Y - 4)
                AddHandler combox.Leave, AddressOf Me.InputComBoxExit
                'AddHandler combox.SelectedIndexChanged, AddressOf Me.InputComBoxChangeIndex
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

    Private Sub ChangeList(ByVal i As Integer)
        ListView1.Items(ListView1.FocusedItem.Index).UseItemStyleForSubItems = False
        If i = 0 Then
            ListView1.Items(ListView1.FocusedItem.Index).BackColor = Color.MistyRose
            ListView1.Sort()
        Else
            For n = 0 To ListView1.Columns.Count - 1
                If n = i Then
                    ListView1.Items(ListView1.FocusedItem.Index).SubItems(i).BackColor = Color.MistyRose
                Else
                    If ListView1.Items(ListView1.FocusedItem.Index).SubItems(n).BackColor <> Color.MistyRose Then
                        ListView1.Items(ListView1.FocusedItem.Index).SubItems(n).BackColor = ListView1.Items(ListView1.FocusedItem.Index).Tag
                    End If
                End If
            Next
        End If

    End Sub

    Private Sub InputNumBoxExit()
        If numbox.Text.Length > 0 Then
            If numbox.Text & "%" <> ListView1.Items(ListView1.FocusedItem.Index).SubItems(numbox.Tag).Text Then
                ListView1.Items(ListView1.FocusedItem.Index).SubItems(numbox.Tag).Text = numbox.Value & "%"
                ChangeList(numbox.Tag)
            End If
        End If
        numbox.Dispose()
        ListView1.Focus()
    End Sub

    Private Sub InputNumBoxKeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs)
        If e.KeyData = Keys.Enter Then
            InputNumBoxExit()
        ElseIf e.KeyData = Keys.Escape Then
            numbox.Dispose()
        End If
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

    Private Sub EncTable_Form_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.SetStyle(ControlStyles.DoubleBuffer Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.UserPaint Or ControlStyles.AllPaintingInWmPaint, True)
        Me.UpdateStyles()
    End Sub

    Private Sub ListView1_MouseUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListView1.MouseUp
        If e.Button = Windows.Forms.MouseButtons.Middle Then
            If col > 0 Then
                If col > 1 And col < 5 Then
                    EditToolStripMenuItem.PerformClick()
                ElseIf col = 5 Then
                    EditConditionToolStripMenuItem.PerformClick()
                End If
            End If
        End If
    End Sub

    Private Sub ListView1_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListView1.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Middle Then
            Dim row As Integer = GetListViewRowClick(sender, e.Y)
            col = -1
            If row = -1 Then Exit Sub
            col = GetListViewColClick(sender, e.X)
        End If
    End Sub

End Class
