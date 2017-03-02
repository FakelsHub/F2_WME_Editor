Public Class TileSel_Form

    Private Sub TileSel_Form_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ListBox1.Items.AddRange(TileName)
        ListBox1.SelectedIndex = 0
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        OpenTileFrm()
    End Sub

    Private Sub ListBox1_MouseDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListBox1.MouseDoubleClick
        OpenTileFrm()
    End Sub

    Private Sub OpenTileFrm()
        CreateTileTableForm(ListBox1.SelectedIndex)
        Me.Close()
    End Sub

    Private Sub TileSel_Form_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        Me.Opacity = 1
    End Sub

    Private Sub TileSel_Form_Deactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
        If BigPicbox.Visible Then Me.Opacity = 0.5
    End Sub
End Class