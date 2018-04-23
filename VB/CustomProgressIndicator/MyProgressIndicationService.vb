Imports Microsoft.VisualBasic
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

		#Region "ISnapMailMergeProgressIndicationService Members"

        Public ReadOnly Property CancellationToken() As CancellationToken Implements ISnapMailMergeProgressIndicationService.CancellationToken
            Get
                Return token
            End Get
        End Property

        Public Sub Reset() Implements ISnapMailMergeProgressIndicationService.Reset
            If (Not Object.ReferenceEquals(Me.cts, Nothing)) Then
                Me.cts.Dispose()
                Me.cts = Nothing
            End If
            Me.cts = New CancellationTokenSource()
            Me.token = Me.cts.Token
        End Sub

		#End Region

		#Region "IProgressIndicationService Members"

        Public Sub Begin(ByVal displayName As String, ByVal minProgress As Integer, ByVal maxProgress As Integer, ByVal currentProgress As Integer) Implements ISnapMailMergeProgressIndicationService.Begin
            Dim begin As Action = Function() AnonymousMethod1(minProgress, maxProgress, currentProgress)
            Me.snapControl.BeginInvoke(begin)
        End Sub
		
		Private Function AnonymousMethod1(ByVal minProgress As Integer, ByVal maxProgress As Integer, ByVal currentProgress As Integer) As Boolean
			Me.frm = New MyProgressIndicationForm()
			SubscribeFormEvents()
			Me.frm.Begin(minProgress, maxProgress, currentProgress)
			Me.frm.ShowDialog(Me.snapControl)
			Return True
		End Function

        Public Sub [End]() Implements ISnapMailMergeProgressIndicationService.End
            If Object.ReferenceEquals(Me.frm, Nothing) Then
                Return
            End If
            Dim [end] As Action = Function() AnonymousMethod2()
            Me.snapControl.BeginInvoke([end])
        End Sub
		
		Private Function AnonymousMethod2() As Boolean
			UnsubscribeFormEvents()
			Me.frm.Dispose()
			Me.frm = Nothing
			Return True
		End Function

        Public Sub SetProgress(ByVal currentProgress As Integer) Implements ISnapMailMergeProgressIndicationService.SetProgress
            If Object.ReferenceEquals(Me.frm, Nothing) Then
                Return
            End If
            Dim setProgress As Action = Function() AnonymousMethod3(currentProgress)
            Me.snapControl.BeginInvoke(setProgress)
        End Sub
		
		Private Function AnonymousMethod3(ByVal currentProgress As Integer) As Boolean
			Me.frm.SetProgress(currentProgress)
			Return True
		End Function

		#End Region

		Private Sub SubscribeFormEvents()
			AddHandler Me.frm.FormClosing, AddressOf OnProgressIndicationFormClosing
		End Sub

		Private Sub UnsubscribeFormEvents()
			If Object.ReferenceEquals(Me.frm, Nothing) Then
				Return
			End If
			RemoveHandler Me.frm.FormClosing, AddressOf OnProgressIndicationFormClosing
		End Sub

		Private Sub OnProgressIndicationFormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs)
			If XtraMessageBox.Show(Me.frm.LookAndFeel, Me.frm, "Do you really want to interrupt mail merge?", Application.ProductName, MessageBoxButtons.YesNo) = DialogResult.Yes Then
				Me.cts.Cancel()
			Else
				e.Cancel = True
			End If
		End Sub

		#Region "IDisposable Members"

		Public Sub Dispose() Implements IDisposable.Dispose
			Dispose(True)
			GC.SuppressFinalize(Me)
		End Sub
		Protected Overrides Sub Finalize()
			Dispose(False)
		End Sub
		Protected Overridable Sub Dispose(ByVal disposing As Boolean)
			If disposing Then
				If (Not Object.ReferenceEquals(Me.frm, Nothing)) Then
					Me.frm.Dispose()
					Me.frm = Nothing
				End If
				If (Not Object.ReferenceEquals(Me.cts, Nothing)) Then
					Me.cts.Dispose()
					Me.cts = Nothing
				End If
			End If
		End Sub

		#End Region
	End Class
End Namespace
