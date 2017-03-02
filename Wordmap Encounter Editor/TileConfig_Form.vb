'Imports System.Windows.Forms

Public Class TileConfig_Form

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub TileConfig_Form_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        NumericUpDown1.Value = x_tiles
        For n = 0 To Terrain.GetLength(0) - 1
            ListView2.Items.Add(Terrain(n, 0))
            ListView2.Items(n).SubItems.Add(Terrain(n, 1))
        Next
        ListView1.Items(0).SubItems.Add(Get_PercentEnc("Forced"))
        ListView1.Items(1).SubItems.Add(Get_PercentEnc("Frequent"))
        ListView1.Items(2).SubItems.Add(Get_PercentEnc("Common"))
        ListView1.Items(3).SubItems.Add(Get_PercentEnc("Uncommon"))
        ListView1.Items(4).SubItems.Add(Get_PercentEnc("Rare"))
        ListView1.Items(5).SubItems.Add(Get_PercentEnc("None"))

    End Sub

End Class
