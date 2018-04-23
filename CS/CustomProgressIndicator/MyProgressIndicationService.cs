using System;
using System.Threading;
using System.Windows.Forms;
using DevExpress.Snap;
using DevExpress.Snap.Core.Services;
using DevExpress.Utils;
using DevExpress.XtraEditors;
// ...

namespace CustomProgressIndicator {
    class MyProgressIndicationService : ISnapMailMergeProgressIndicationService, IDisposable {
        readonly SnapControl snapControl;
        MyProgressIndicationForm frm;
        CancellationTokenSource cts;
        CancellationToken token;

        public MyProgressIndicationService(SnapControl snapControl) {
            Guard.ArgumentNotNull(snapControl, "snapControl");
            this.snapControl = snapControl;
        }

        #region ISnapMailMergeProgressIndicationService Members

        public CancellationToken CancellationToken { get { return token; } }

        public void Reset() {
            if (!object.ReferenceEquals(this.cts, null)) {
                this.cts.Dispose();
                this.cts = null;
            }
            this.cts = new CancellationTokenSource();
            this.token = this.cts.Token;
        }

        #endregion

        #region IProgressIndicationService Members

        public void Begin(string displayName, int minProgress, int maxProgress,
            int currentProgress) {
            Action begin = delegate {
                this.frm = new MyProgressIndicationForm();
                SubscribeFormEvents();
                this.frm.Begin(minProgress, maxProgress, currentProgress);
                this.frm.ShowDialog(this.snapControl);
            };
            this.snapControl.BeginInvoke(begin);
        }

        public void End() {
            if (object.ReferenceEquals(this.frm, null))
                return;
            Action end = delegate {
                UnsubscribeFormEvents();
                this.frm.Dispose();
                this.frm = null;
            };
            this.snapControl.BeginInvoke(end);
        }

        public void SetProgress(int currentProgress) {
            if (object.ReferenceEquals(this.frm, null))
                return;
            Action setProgress = delegate {
                this.frm.SetProgress(currentProgress);
            };
            this.snapControl.BeginInvoke(setProgress);
        }

        #endregion

        void SubscribeFormEvents() {
            this.frm.FormClosing += OnProgressIndicationFormClosing;
        }

        void UnsubscribeFormEvents() {
            if (object.ReferenceEquals(this.frm, null))
                return;
            this.frm.FormClosing -= OnProgressIndicationFormClosing;
        }

        void OnProgressIndicationFormClosing(object sender, FormClosingEventArgs e) {
            if (XtraMessageBox.Show(this.frm.LookAndFeel, this.frm,
                "Do you really want to interrupt mail merge?", Application.ProductName,
                MessageBoxButtons.YesNo) == DialogResult.Yes)
                this.cts.Cancel();
            else
                e.Cancel = true;
        }

        #region IDisposable Members

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~MyProgressIndicationService() {
            Dispose(false);
        }
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                if (!object.ReferenceEquals(this.frm, null)) {
                    this.frm.Dispose();
                    this.frm = null;
                }
                if (!object.ReferenceEquals(this.cts, null)) {
                    this.cts.Dispose();
                    this.cts = null;
                }
            }
        }

        #endregion
    }
}
