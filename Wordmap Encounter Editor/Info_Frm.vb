Public Class Info_Frm

    Private Sub ToolStripMenuItem4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem4.Click
        Me.Opacity = 0.95
    End Sub

    Private Sub ToolStripMenuItem3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem3.Click
        Me.Opacity = 0.6
    End Sub

    Private Sub ToolStripMenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem2.Click
        Me.Opacity = 0.8
    End Sub

    Private Sub HideToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HideToolStripMenuItem.Click
        Me.Hide()
    End Sub

    Private Sub Info_Frm_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        Main_Form.Focus()
    End Sub

    Private Sub Info_Frm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Label1.Text &= Get_PercentEnc("Forced") & "%"
        Label2.Text &= Get_PercentEnc("Frequent") & "%"
        Label3.Text &= Get_PercentEnc("Common") & "%"
        Label4.Text &= Get_PercentEnc("Uncommon") & "%"
        Label5.Text &= Get_PercentEnc("Rare") & "%"
        Label6.Text &= Get_PercentEnc("None") & "%"
    End Sub
End Class