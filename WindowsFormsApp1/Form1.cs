using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.SimpleBarcodeHandling;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {

        public MySimpleBarcodeFunctionalityHandler theBarcodeScanHandler;
        public MySimpleNetworkHandler theNetworkHandler;


        public Form1()
        {
            InitializeComponent();
            theBarcodeScanHandler = new MySimpleBarcodeFunctionalityHandler();
            theBarcodeScanHandler.signSecondaryWindowHandlingPact(this);
            /// check command line arguments; http://www.howtogeek.com/howto/programming/get-command-line-arguments-in-a-windows-forms-application/
            string[] args = Environment.GetCommandLineArgs();
            if (args.Count<string>() == 1) {
                MessageBox.Show(this, MySimpleBarcodeFunctionalityHandler.messageMSGBOX_usage, MySimpleBarcodeFunctionalityHandler.messageMSGBOX_caption);
                theNetworkHandler = null;
            } else
            {
                theNetworkHandler = new MySimpleNetworkHandler(args[1]);
                
            }
            theBarcodeScanHandler.signNetworkHandlingPact(theNetworkHandler);

        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)  {

            TimeSpan elapsed = (DateTime.Now - theBarcodeScanHandler._lastKeystroke);
            if (elapsed.TotalMilliseconds > MySimpleBarcodeFunctionalityHandler.lastKeyStrokeTimeoutMs)
                theBarcodeScanHandler._barcode.Clear();

            // record keystroke & timestamp
            theBarcodeScanHandler._barcode.Add(e.KeyChar);
            theBarcodeScanHandler._lastKeystroke = DateTime.Now;

            // process barcode
            if (e.KeyChar == 13 && theBarcodeScanHandler._barcode.Count > 0)
            {
                string msg = new String(theBarcodeScanHandler._barcode.ToArray());
                //perform actions with recorded barcode
                /*
                theBarcodeScanHandler.renderWindowStatus();
                theBarcodeScanHandler.renderWindowStatusMessage(MySimpleBarcodeFunctionalityHandler.messageBarcodeObtainedBase+msg);
                */
                theBarcodeScanHandler.processBarcodeCompletely(theBarcodeScanHandler._barcode);
                theBarcodeScanHandler._barcode.Clear();
            }


        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Text = MySimpleBarcodeFunctionalityHandler.activatedMessage;
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            this.label1.ForeColor = System.Drawing.Color.DarkRed;
            this.label1.Text = MySimpleBarcodeFunctionalityHandler.deactivatedMessage;
        }
    }
}
