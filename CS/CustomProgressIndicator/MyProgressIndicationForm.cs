using System.Windows.Forms;
using DevExpress.XtraEditors;
// ...

namespace CustomProgressIndicator {
    public partial class MyProgressIndicationForm : XtraForm {
        public MyProgressIndicationForm() {
            InitializeComponent();
        }

        int minValue;
        int maxValue;

        private new DevExpress.XtraGauges.Win.Gauges.Linear.LinearScaleComponent Scale { get { return this.linearGauge1.Scales[0]; } }
        private DevExpress.XtraGauges.Win.Base.LabelComponent Label { get { return linearGauge1.Labels[0]; } }

        public void Begin(int maxValue) { Begin(0, maxValue, 0); }
        public void Begin(int minValue, int maxValue, int currentValue) {
            this.minValue = minValue;
            this.maxValue = maxValue;
            SetProgress(currentValue);
        }

        public void SetProgress(int currentValue) {
            float percent =
                100f * (currentValue - this.minValue) / (this.maxValue - this.minValue);
            Scale.Value = percent;
            Label.Text = string.Format("{0}", (int)percent);
        }
    }
}
