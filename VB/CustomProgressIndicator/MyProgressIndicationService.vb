Imports System
Imports System.Threading
Imports System.Windows.Forms
Imports DevExpress.Snap
Imports DevExpress.Snap.Core.Services
Imports DevExpress.Utils
Imports DevExpress.XtraEditors

' ...
Namespace CustomProgressIndicator

    Friend Class MyProgressIndicationService
        Implements ISnapMailMergeProgressIndicationService, IDisposable

        Private ReadOnly snapControl As SnapControl

        Private frm As MyProgressIndicationForm

        Private cts As CancellationTokenSource

        Private token As CancellationToken

        Public Sub New(ByVal snapControl As SnapControl)
            Guard.ArgumentNotNull(snapControl, "snapControl")
            Me.snapControl = snapControl
        End Sub

'#Region "ISnapMailMergeProgressIndicationService Members"
        Public ReadOnly Property CancellationToken As CancellationToken Implements ISnapMailMergeProgressIndicationService.CancellationToken
            Get
                Return token
            End Get
        End Property

        Public Sub Reset() Implements ISnapMailMergeProgressIndicationService.Reset
            If Not ReferenceEquals(cts, Nothing) Then
                cts.Dispose()
                cts = Nothing
            End If

            cts = New CancellationTokenSource()
            token = cts.Token
        End Sub

'#End Region
'#Region "IProgressIndicationService Members"
        Public Sub Begin(ByVal displayName As String, ByVal minProgress As Integer, ByVal maxProgress As Integer, ByVal currentProgress As Integer) Implements DevExpress.Services.IProgressIndicationService.Begin
            Dim lBegin As Action = Sub()
                frm = New MyProgressIndicationForm()
                SubscribeFormEvents()
                frm.Begin(minProgress, maxProgress, currentProgress)
                frm.ShowDialog(snapControl)
            End Sub
            snapControl.BeginInvoke(lBegin)
        End Sub

        Public Sub [End]() Implements DevExpress.Services.IProgressIndicationService.End
            If ReferenceEquals(frm, Nothing) Then Return
            Dim lEnd As Action = Sub()
                UnsubscribeFormEvents()
                frm.Dispose()
                frm = Nothing
            End Sub
            snapControl.BeginInvoke(lEnd)
        End Sub

        Public Sub SetProgress(ByVal currentProgress As Integer) Implements DevExpress.Services.IProgressIndicationService.SetProgress
            If ReferenceEquals(frm, Nothing) Then Return
            Dim lSetProgress As Action = Sub() frm.SetProgress(currentProgress)
            snapControl.BeginInvoke(lSetProgress)
        End Sub

'#End Region
        Private Sub SubscribeFormEvents()
            AddHandler frm.FormClosing, AddressOf OnProgressIndicationFormClosing
        End Sub

        Private Sub UnsubscribeFormEvents()
            If ReferenceEquals(frm, Nothing) Then Return
            RemoveHandler frm.FormClosing, AddressOf OnProgressIndicationFormClosing
        End Sub

        Private Sub OnProgressIndicationFormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs)
            If XtraMessageBox.Show(frm.LookAndFeel, frm, "Do you really want to interrupt mail merge?", Application.ProductName, MessageBoxButtons.YesNo) = DialogResult.Yes Then
                cts.Cancel()
            Else
                e.Cancel = True
            End If
        End Sub

'#Region "IDisposable Members"
        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

        Protected Overrides Sub Finalize()
            Dispose(False)
        End Sub

        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If disposing Then
                If Not ReferenceEquals(frm, Nothing) Then
                    frm.Dispose()
                    frm = Nothing
                End If

                If Not ReferenceEquals(cts, Nothing) Then
                    cts.Dispose()
                    cts = Nothing
                End If
            End If
        End Sub
'#End Region
    End Class
End Namespace
