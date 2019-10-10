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
        Label1.Text &= WorldMapData.GetPercentEnc(WorldMapData.EncPercent(0)) & "%"
        Label2.Text &= WorldMapData.GetPercentEnc(WorldMapData.EncPercent(1)) & "%"
        Label3.Text &= WorldMapData.GetPercentEnc(WorldMapData.EncPercent(2)) & "%"
        Label4.Text &= WorldMapData.GetPercentEnc(WorldMapData.EncPercent(3)) & "%"
        Label5.Text &= WorldMapData.GetPercentEnc(WorldMapData.EncPercent(4)) & "%"
        Label6.Text &= WorldMapData.GetPercentEnc(WorldMapData.EncPercent(5)) & "%"
    End Sub
End Class