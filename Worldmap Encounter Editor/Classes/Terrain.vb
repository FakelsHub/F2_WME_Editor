Friend Class Terrain

    Friend type As String
    Friend speed As Integer

    Sub New(ByVal line As String)
        Dim z As Integer = line.IndexOf(":")
        type = line.Remove(z).Trim
        speed = line.Substring(z + 1).Trim
    End Sub

End Class
