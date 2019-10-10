Public Class EncEdit_Form

    Friend attack As String
    Friend target As String
    Friend vs As String

    Private frmParent As EncTable_Form
    Private OK As Boolean = False

    Friend Sub Ini_Load(ByVal attck As String, ByVal trgt As String, ByVal v_s As String, ByVal frm As EncTable_Form)
        Dim sline() As String
        Dim tmp() As String

        frmParent = frm
        attack = attck.Trim
        target = trgt.Trim
        vs = v_s
        Select Case vs
            Case WMData.AMBUSH
                ComboBox1.SelectedIndex = 1
            Case WMData.FIGHTING
                ComboBox1.SelectedIndex = 2
            Case Else
                ComboBox1.SelectedIndex = 0
                ListView2.Items.Add(vbNullString)
                ListView2.Items(0).SubItems.Add(vbNullString)
                ListView2.Items.Add(vbNullString)
                ListView2.Items(1).SubItems.Add(vbNullString)
                GoTo SkipTarget
        End Select
        'target
        sline = Split(target, "AND", -1, CompareMethod.Text)
        If sline.Length > 1 Then
            tmp = sline(0).Trim.Split(" ")
            tmp(0) = tmp(0).Replace("(", vbNullString)
            ListView2.Items.Add(tmp(0).Replace(")", vbNullString))
            ListView2.Items(0).SubItems.Add(tmp(1))

            tmp = sline(1).Trim.Split(" ")
            If tmp.Length > 1 Then
                tmp(0) = tmp(0).Replace("(", vbNullString)
                ListView2.Items.Add(tmp(0).Replace(")", vbNullString))
                ListView2.Items(1).SubItems.Add(tmp(1))
            Else
                ListView2.Items.Add(vbNullString)
                ListView2.Items(1).SubItems.Add(sline(1))
            End If
        Else
            sline = target.Split(" ")
            If sline.Length > 1 Then
                sline(0) = sline(0).Replace("(", vbNullString)
                ListView2.Items.Add(sline(0).Replace(")", vbNullString))
                ListView2.Items(0).SubItems.Add(sline(1))
            Else
                ListView2.Items.Add(vbNullString)
                ListView2.Items(0).SubItems.Add(target)
            End If
            ListView2.Items.Add(vbNullString)
            ListView2.Items(1).SubItems.Add(vbNullString)
        End If
        ListView2.Items(0).Selected = True
SkipTarget:
        'attack
        sline = Split(attack, "AND", -1, CompareMethod.Text)
        If sline.Length > 1 Then
            tmp = sline(0).Trim.Split(" ")
            tmp(0) = tmp(0).Replace("(", vbNullString)
            ListView1.Items.Add(tmp(0).Replace(")", vbNullString))
            ListView1.Items(0).SubItems.Add(tmp(1))

            tmp = sline(1).Trim.Split(" ")
            tmp(0) = tmp(0).Replace("(", vbNullString)
            ListView1.Items.Add(tmp(0).Replace(")", vbNullString))
            ListView1.Items(1).SubItems.Add(tmp(1))
        Else
            sline = attack.Split(" ")
            If sline.Length > 1 Then
                sline(0) = sline(0).Replace("(", vbNullString)
                ListView1.Items.Add(sline(0).Replace(")", vbNullString))
                ListView1.Items(0).SubItems.Add(sline(1))
            Else
                ListView1.Items.Add(vbNullString)
                ListView1.Items(0).SubItems.Add(attack)
            End If
            ListView1.Items.Add(vbNullString)
            ListView1.Items(1).SubItems.Add(vbNullString)
        End If

        'For n = 0 To WorldMapData.Encounter.Count - 1
        ListBox1.Items.AddRange(WorldMapData.Encounter.Keys.ToArray)
        'Next
        ListBox1.SelectedIndex = 0
        ListView1.Items(0).Selected = True
        ListBox1.Focus()
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        OK = True
        attack = ""
        'attack
        If ListView1.Items(0).Text.Trim <> vbNullString Then
            attack = String.Format("({0}) {1}", ListView1.Items(0).Text.Trim, ListView1.Items(0).SubItems(1).Text.Trim)
            'attack = "(" & ListView1.Items(0).Text.Trim & ") "
            'attack &= ListView1.Items(0).SubItems(1).Text.Trim
        End If
        If ListView1.Items(1).Text.Trim <> vbNullString Then
            If attack.Length > 0 Then attack &= " AND "
            attack &= String.Format("({0}) {1}", ListView1.Items(1).Text.Trim, ListView1.Items(1).SubItems(1).Text.Trim)
            'attack &= "(" & ListView1.Items(1).Text.Trim & ") "
            'attack &= ListView1.Items(1).SubItems(1).Text.Trim
        End If
        'target
        target = ""
        If ComboBox1.SelectedIndex <> 0 Then
            If ListView2.Items(0).SubItems(1).Text.ToLower = "player".ToLower Then
                target = "Player"
            Else
                If ListView2.Items(0).Text.Trim <> vbNullString Then
                    target = String.Format("({0}) {1}", ListView2.Items(0).Text.Trim, ListView2.Items(0).SubItems(1).Text.Trim)
                    'target = "(" & ListView2.Items(0).Text.Trim & ") "
                    'target &= ListView2.Items(0).SubItems(1).Text.Trim
                End If
                If ListView2.Items(1).SubItems(1).Text.Trim <> vbNullString Then
                    If target.Length > 0 Then target &= " AND "
                    If ListView2.Items(1).SubItems(1).Text.ToLower = "player".ToLower Then
                        target &= "Player"
                    Else
                        target &= String.Format("({0}) {1}", ListView2.Items(1).Text.Trim, ListView2.Items(1).SubItems(1).Text.Trim)
                        'target &= "(" & ListView2.Items(1).Text.Trim & ") "
                        'target &= ListView2.Items(1).SubItems(1).Text.Trim
                    End If
                End If
            End If
            vs = ComboBox1.Text
        Else
            target = "-"
            vs = "-"
        End If
        Me.Close()
    End Sub

    Private Sub EncEdit_Form_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        frmParent.EncEditReturn(Me, OK)
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox1.SelectedIndexChanged
        Button2.Enabled = True
        Button5.Enabled = True
        ListView2.Enabled = True
        Select Case ComboBox1.SelectedIndex
            Case 1

            Case 2

            Case Else
                ListView2.Enabled = False
                Button2.Enabled = False
                Button5.Enabled = False
        End Select
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        ListView1.Focus()
        Dim i As Integer = ListView1.FocusedItem.Index
        If ListView1.Items(i).Text = vbNullString Then ListView1.Items(i).Text = "1-1"
        ListView1.Items(i).SubItems(1).Text = ListBox1.Text
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        ListView2.Focus()
        Dim i As Integer = ListView2.FocusedItem.Index
        If ListView2.Items(i).Text = vbNullString Then ListView2.Items(i).Text = "1-1"
        ListView2.Items(i).SubItems(1).Text = ListBox1.Text
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        ListView1.Focus()
        Dim i As Integer = ListView1.FocusedItem.Index
        ListView1.Items(i).Text = vbNullString
        ListView1.Items(i).SubItems(1).Text = vbNullString
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        ListView2.Focus()
        Dim i As Integer = ListView2.FocusedItem.Index
        ListView2.Items(i).Text = vbNullString
        ListView2.Items(i).SubItems(1).Text = vbNullString
    End Sub

End Class