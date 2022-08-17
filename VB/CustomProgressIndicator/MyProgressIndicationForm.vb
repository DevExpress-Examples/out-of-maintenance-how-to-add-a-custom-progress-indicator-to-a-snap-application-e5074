Imports DevExpress.XtraEditors

' ...
Namespace CustomProgressIndicator

    Public Partial Class MyProgressIndicationForm
        Inherits XtraForm

        Public Sub New()
            InitializeComponent()
        End Sub

        Private minValue As Integer

        Private maxValue As Integer

        Private Overloads ReadOnly Property Scale As DevExpress.XtraGauges.Win.Gauges.Linear.LinearScaleComponent
            Get
                Return linearGauge1.Scales(0)
            End Get
        End Property

        Private ReadOnly Property Label As DevExpress.XtraGauges.Win.Base.LabelComponent
            Get
                Return linearGauge1.Labels(0)
            End Get
        End Property

        Public Sub Begin(ByVal maxValue As Integer)
            Begin(0, maxValue, 0)
        End Sub

        Public Sub Begin(ByVal minValue As Integer, ByVal maxValue As Integer, ByVal currentValue As Integer)
            Me.minValue = minValue
            Me.maxValue = maxValue
            SetProgress(currentValue)
        End Sub

        Public Sub SetProgress(ByVal currentValue As Integer)
            Dim percent As Single = 100F * (currentValue - minValue) / (maxValue - minValue)
            Scale.Value = percent
            Label.Text = String.Format("{0}", CInt(percent))
        End Sub
    End Class
End Namespace
