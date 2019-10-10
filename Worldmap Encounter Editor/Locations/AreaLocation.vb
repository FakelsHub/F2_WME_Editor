Imports Worldmap_Encounter_Editor.CityData

Friend Class AreaLocation

    Private showReady As Boolean

    Sub New()
        InitializeComponent()

        For Each cd As CityData In CityList
            AreaList.Items.Add(cd.Name, cd.ShowLoc)
        Next

        AreaList.SelectedIndex = 0
    End Sub

    Private Sub Refresh_Loc(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Graph.BigPicbox.BackgroundImage = Graph.Worldmap_Pic.Clone
        Graph.ShowLayer()
        Graph.BigPicbox.Refresh()
    End Sub

    Private Sub AreaList_ItemCheck(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ItemCheckEventArgs) Handles AreaList.ItemCheck
        If Me.Visible Then CityList.Item(AreaList.SelectedIndex).ShowLoc = e.NewValue
    End Sub

    Private Sub AreaList_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AreaList.SelectedIndexChanged
        showReady = False

        NumericUpDown1.Value = CityList.Item(AreaList.SelectedIndex).Location.X
        NumericUpDown2.Value = CityList.Item(AreaList.SelectedIndex).Location.Y

        Select Case CityList.Item(AreaList.SelectedIndex).Size
            Case LocateSize.SmallSize
                ComboBox1.SelectedIndex = 0
            Case LocateSize.MediumSize
                ComboBox1.SelectedIndex = 1
            Case Else
                ComboBox1.SelectedIndex = 2
        End Select

        CheckBox1.Checked = CityList.Item(AreaList.SelectedIndex).LockState

        showReady = True
    End Sub

    Private Sub CheckBox1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox1.CheckedChanged
        If showReady Then CityList.Item(AreaList.SelectedIndex).LockState = CheckBox1.Checked
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox1.SelectedIndexChanged
        If Not showReady Then Return

        Select Case ComboBox1.SelectedIndex
            Case 0
                CityList.Item(AreaList.SelectedIndex).Size = LocateSize.SmallSize
            Case 1
                CityList.Item(AreaList.SelectedIndex).Size = LocateSize.MediumSize
            Case Else
                CityList.Item(AreaList.SelectedIndex).Size = LocateSize.LargeSize
        End Select
    End Sub

    Private Sub Location_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NumericUpDown1.ValueChanged, NumericUpDown2.ValueChanged
        If Not showReady Then Return
        Dim loc As Point = New Point(NumericUpDown1.Value, NumericUpDown2.Value)
        CityList.Item(AreaList.SelectedIndex).Location = loc
    End Sub

    Private Sub Save(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        'For Each cd As CityData In CityList
        '    AreaList.Items.Add(cd.Name, cd.ShowLoc)
        'Next

    End Sub

End Class