using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

// ToggleButton을 누르면 start/stop toggle

namespace ToggleButtonTestForm
{
    public partial class ToggleButton : CheckBox    // ToggleButton
    {
        public string CheckedText
        {
            get;
            set;
        }

        public string UncheckedText
        {
            get;
            set;
        }

        public Color CheckedColor
        {
            get;
            set;
        }

        public Color UncheckedColor
        {
            get;
            set;
        }

        public ToggleButton()
            : this(string.Empty)
        {
        }

        public ToggleButton(string imageName)
            : base()
        {
            InitializeComponent();

            if (!string.IsNullOrEmpty(imageName))
            {
                // bitmap & size
                Image image = Image.FromFile(imageName);
                int cxImage = image.Width;
                int cyImage = image.Height;
                SizeF sizef = new SizeF(cxImage / image.HorizontalResolution, cyImage / image.VerticalResolution);
                float fScale = Math.Min(this.Width / sizef.Width, this.Height / sizef.Height);
                sizef.Width *= fScale;
                sizef.Height *= fScale;
                Size sizec = Size.Ceiling(sizef);
                Bitmap bitmap = new Bitmap(image, sizec);
                this.Image = image;
                image.Dispose();
            }

            this.Appearance = Appearance.Button;
            this.TextAlign = ContentAlignment.BottomCenter;
            this.TextImageRelation = TextImageRelation.ImageAboveText;
            this.CheckedColor = Color.Gray;
            this.UncheckedColor = this.BackColor;
        }

        protected override void OnClick(EventArgs e) 
        {
            base.OnClick(e);    // call the checkbox base class
            if (this.Checked)   // start recording when checked
            {
                this.Text = this.CheckedText;
                this.BackColor = this.CheckedColor;
            }
            else                // stop recording when unchecked
            {
                this.Text = this.UncheckedText;
                this.BackColor = this.UncheckedColor;
            }
        }
    }
}
