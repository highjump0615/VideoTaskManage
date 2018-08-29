namespace TrackbarLib
{
    partial class TrackbarControl
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrackbarControl));
            this.axEventTrackBarX1 = new AxEventTrackBarXLib.AxEventTrackBarX();
            ((System.ComponentModel.ISupportInitialize)(this.axEventTrackBarX1)).BeginInit();
            this.SuspendLayout();
            // 
            // axEventTrackBarX1
            // 
            this.axEventTrackBarX1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axEventTrackBarX1.Enabled = true;
            this.axEventTrackBarX1.Location = new System.Drawing.Point(0, 0);
            this.axEventTrackBarX1.Name = "axEventTrackBarX1";
            this.axEventTrackBarX1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axEventTrackBarX1.OcxState")));
            this.axEventTrackBarX1.Size = new System.Drawing.Size(800, 450);
            this.axEventTrackBarX1.TabIndex = 0;
            // 
            // TrackbarControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.axEventTrackBarX1);
            this.Name = "TrackbarControl";
            this.Size = new System.Drawing.Size(800, 450);
            ((System.ComponentModel.ISupportInitialize)(this.axEventTrackBarX1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxEventTrackBarXLib.AxEventTrackBarX axEventTrackBarX1;
    }
}
