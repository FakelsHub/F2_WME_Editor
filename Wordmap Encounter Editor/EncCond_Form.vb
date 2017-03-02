Public Class EncCond_Form
    Private texbox As TextBox
    Private combox As ComboBox

    Private IfType() As String = {"Global", "Player", "Time Day"}
    Private OprCon() As String = {"==", "<=", ">=", "<", ">", "&"}
    Private PlStats() As String = {"Level", "Age"}

    Friend Sub Ini_Load(ByVal IfCond As String)
        Dim z, x, y As Integer
        Dim tmp() As String = Nothing
        Dim str() As String = Split(IfCond, "And", -1, CompareMethod.Text)
        For n = 0 To str.Length - 1
            str(n) = str(n).Trim
            tmp = str(n).Split("(") ' в 0-if 1-type 2-gvar+oper+value 
            z = tmp(2).IndexOf(")")
            If str(n).Contains(IfType(0)) Then
                ListView1.Items.Add(IfType(0))
                x = CInt(tmp(2).Remove(z).Trim) 'получаем номер gvar
                ListView1.Items(n).SubItems.Add(GlobalVar(x)) 'имя gvar
            ElseIf str(n).Contains(IfType(1)) Then
                ListView1.Items.Add(IfType(1))
                ListView1.Items(n).SubItems.Add(tmp(2).Remove(z).Trim) 'имя stat
            Else
                ListView1.Items.Add(IfType(2))
                ListView1.Items(n).SubItems.Add(vbNullString) 'ничего
            End If
            'operator
            x = -1
            y = 0
            Do While x = -1
                x = tmp(2).LastIndexOf(OprCon(y))
                y += 1
            Loop ' в х позиция последнего символа оператора
            If y <= 3 Then x += 1 'если двойной оператор
            ListView1.Items(n).SubItems.Add(tmp(2).Substring(z + 1, x - z).Trim)
            'value
            z = tmp(2).LastIndexOf(")")
            ListView1.Items(n).SubItems.Add(tmp(2).Substring(x + 1, z - 1 - x).Trim)
        Next

    End Sub

    Private Sub ListView1_MouseDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListView1.MouseDoubleClick
        Dim x, c As Integer
        x = e.X
        'узнать на каком столбце был клик
        c = GetListViewColClick(sender, x)
        If c = -1 Then Exit Sub
        Select Case c
            Case 0 'type
                combox = New ComboBox
                combox.Name = "InputComBox"
                combox.Items.AddRange(IfType)
                combox.DropDownStyle = ComboBoxStyle.DropDownList
                'combox.MaxDropDownItems = 20
                'combox.DropDownWidth = 175
                combox.Tag = c
                combox.BackColor = Color.LightYellow
                combox.Width = ListView1.Columns.Item(c).Width + 10
                combox.Location = New Point(x, ListView1.Items(ListView1.FocusedItem.Index).Bounds.Location.Y - 4)
                AddHandler combox.Leave, AddressOf Me.InputComBoxExit
                combox.Text = ListView1.Items(ListView1.FocusedItem.Index).SubItems(c).Text
                Me.ListView1.Controls.Add(combox)
                combox.Focus()
            Case 1 '
                combox = New ComboBox
                combox.Name = "InputComBox"
                If ListView1.FocusedItem.Text = IfType(0) Then
                    combox.Items.AddRange(GlobalVar)
                ElseIf ListView1.FocusedItem.Text = IfType(1) Then
                    combox.Items.AddRange(PlStats)
                Else
                    combox.Dispose()
                    Exit Select
                End If
                combox.DropDownStyle = ComboBoxStyle.DropDownList
                combox.MaxDropDownItems = 20
                combox.DropDownWidth = 175
                combox.Tag = c
                combox.BackColor = Color.LightYellow
                combox.Width = ListView1.Columns.Item(c).Width + 10
                combox.Location = New Point(x, ListView1.Items(ListView1.FocusedItem.Index).Bounds.Location.Y - 4)
                AddHandler combox.Leave, AddressOf Me.InputComBoxExit
                combox.Text = ListView1.Items(ListView1.FocusedItem.Index).SubItems(c).Text
                Me.ListView1.Controls.Add(combox)
                combox.Focus()
            Case 2 'operator
                combox = New ComboBox
                combox.Name = "InputComBox"
                combox.Items.AddRange(OprCon)
                combox.DropDownStyle = ComboBoxStyle.DropDownList
                combox.Tag = c
                combox.BackColor = Color.LightYellow
                combox.Width = ListView1.Columns.Item(c).Width + 10
                combox.Location = New Point(x, ListView1.Items(ListView1.FocusedItem.Index).Bounds.Location.Y - 4)
                AddHandler combox.Leave, AddressOf Me.InputComBoxExit
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
                AddHandler texbox.KeyUp, AddressOf Me.InputTexBoxKeyUp
                AddHandler texbox.KeyDown, AddressOf Me.InputKeyDown
                AddHandler texbox.KeyPress, AddressOf Me.InputTexBoxKeyPress
                Me.ListView1.Controls.Add(texbox)
                texbox.Text = ListView1.Items(ListView1.FocusedItem.Index).SubItems(c).Text
                texbox.Focus()
                texbox.SelectionLength = 0
        End Select
    End Sub

    Private Sub InputComBoxExit()
        If combox.Text <> ListView1.Items(ListView1.FocusedItem.Index).SubItems(combox.Tag).Text Then
            ListView1.Items(ListView1.FocusedItem.Index).SubItems(combox.Tag).Text = combox.Text
        End If
        combox.Dispose()
    End Sub

    Private Sub InputTexBoxExit()
        texbox.Dispose()
    End Sub
    Private Sub InputTexBoxKeyUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs)
        If e.KeyData = Keys.Enter Then
            If texbox.Text <> ListView1.Items(ListView1.FocusedItem.Index).SubItems(texbox.Tag).Text Then
                ListView1.Items(ListView1.FocusedItem.Index).SubItems(texbox.Tag).Text = texbox.Text
            End If
            InputTexBoxExit()
        ElseIf e.KeyData = Keys.Escape Then
            InputTexBoxExit()
        End If
    End Sub

    Private Sub InputTexBoxKeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
        If Not IsNumeric(e.KeyChar) AndAlso Asc(e.KeyChar) <> 8 AndAlso Asc(e.KeyChar) <> 45 Then e.Handled = True
    End Sub

    Private Sub InputKeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs)
        If e.KeyCode = Keys.Enter Or e.KeyData = Keys.Escape Then
            e.SuppressKeyPress = True
        End If
    End Sub

End Class