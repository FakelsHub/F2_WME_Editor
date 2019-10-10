Friend Class CityData

    Friend Enum LocateSize As Integer
        SmallSize = 18
        MediumSize = 30
        LargeSize = 46
    End Enum

    Private _cityname As String
    Private _locationXY As Point
    Private _size As LocateSize
    Private _lockstate As Boolean = False

    Friend Property ShowLoc As Boolean = True

    Friend ReadOnly Property Name As String
        Get
            Return _cityname
        End Get
    End Property

    Friend Property Location As Point
        Get
            Return _locationXY
        End Get
        Set(ByVal value As Point)
            _locationXY = value
        End Set
    End Property

    Friend Property Size As LocateSize
        Get
            Return _size
        End Get
        Set(ByVal value As LocateSize)
            _size = value
        End Set
    End Property

    Friend Property LockState As Boolean
        Get
            Return _lockstate
        End Get
        Set(ByVal value As Boolean)
            _lockstate = value
        End Set
    End Property

    Sub New(ByVal name As String, ByVal pos As String, ByVal size As String, ByVal state As String)

        Dim z As Integer = name.IndexOf(";")
        If z > 10 Then z -= 10 Else z = name.Length - 10
        _cityname = name.Substring(10, z).Trim()

        If pos.Length > 0 Then
            z = pos.IndexOf(";")
            If z > 10 Then z -= 10 Else z = pos.Length - 10
            Dim x_y As String() = pos.Substring(10, z).Trim().Split(",")
            _locationXY = New Point(x_y(0).Trim, x_y(1).Trim)
        Else
            _locationXY = New Point(0, 0)
        End If

        If size.Length > 0 Then
            z = size.IndexOf(";")
            If z > 5 Then z -= 5 Else z = size.Length - 5
            Select Case size.Substring(5, z).Trim().ToLower
                Case "small"
                    _size = LocateSize.SmallSize
                Case "medium"
                    _size = LocateSize.MediumSize
                Case Else '"large"
                    _size = LocateSize.LargeSize
            End Select
        Else
            _size = LocateSize.SmallSize
        End If

        If state.Length > 0 Then
            z = state.IndexOf(";")
            If z > 11 Then z -= 11 Else z = state.Length - 11
            _lockstate = String.Equals(state.Substring(11, z).Trim(), "on", StringComparison.OrdinalIgnoreCase)
            ShowLoc = Not _lockstate
        End If

    End Sub

End Class
