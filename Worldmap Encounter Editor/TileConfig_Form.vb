'Imports System.Windows.Forms

Public Class TileConfig_Form

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub TileConfig_Form_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        NumericUpDown1.Value = WorldMapData.x_tiles
        For Each tl As Terrain In WorldMapData.TerrainList
            ListView2.Items.Add(New ListViewItem({tl.type, tl.speed}))
        Next
        Dim n As Integer
        For Each ep As String In WorldMapData.EncPercent
            ListView1.Items(n).SubItems.Add(WorldMapData.GetPercentEnc(ep))
            n += 1
        Next

    End Sub

End Class
