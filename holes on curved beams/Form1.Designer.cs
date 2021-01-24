
namespace holes_on_curved_beams
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.tx_dis = new System.Windows.Forms.TextBox();
            this.tx_size = new System.Windows.Forms.TextBox();
            this.dd = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cm_rotation = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(61, 163);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Insert";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tx_dis
            // 
            this.tx_dis.Location = new System.Drawing.Point(93, 92);
            this.tx_dis.Name = "tx_dis";
            this.tx_dis.Size = new System.Drawing.Size(43, 20);
            this.tx_dis.TabIndex = 1;
            this.tx_dis.Text = "500";
            // 
            // tx_size
            // 
            this.tx_size.Location = new System.Drawing.Point(93, 55);
            this.tx_size.Name = "tx_size";
            this.tx_size.Size = new System.Drawing.Size(43, 20);
            this.tx_size.TabIndex = 1;
            this.tx_size.Text = "25";
            // 
            // dd
            // 
            this.dd.AutoSize = true;
            this.dd.Location = new System.Drawing.Point(27, 58);
            this.dd.Name = "dd";
            this.dd.Size = new System.Drawing.Size(52, 13);
            this.dd.TabIndex = 2;
            this.dd.Text = "Hole Size";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 95);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Distance";
            // 
            // cm_rotation
            // 
            this.cm_rotation.FormattingEnabled = true;
            this.cm_rotation.Items.AddRange(new object[] {
            "Left",
            "Right"});
            this.cm_rotation.Location = new System.Drawing.Point(153, 91);
            this.cm_rotation.Name = "cm_rotation";
            this.cm_rotation.Size = new System.Drawing.Size(97, 21);
            this.cm_rotation.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(262, 243);
            this.Controls.Add(this.cm_rotation);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dd);
            this.Controls.Add(this.tx_size);
            this.Controls.Add(this.tx_dis);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Holes in Curved Beam";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox tx_dis;
        private System.Windows.Forms.TextBox tx_size;
        private System.Windows.Forms.Label dd;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cm_rotation;
    }
}

