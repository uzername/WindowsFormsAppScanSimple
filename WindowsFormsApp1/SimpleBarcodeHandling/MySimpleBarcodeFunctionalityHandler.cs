using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// the simplest way to handle barcode input in C# WinForms https://stackoverflow.com/a/10850102
namespace WindowsFormsApp1.SimpleBarcodeHandling
{
    public class JSONresponseFromAPI
    {
        public String status { get; set; }
        public String time { get; set; }
    }
    public class MyInfoWindow: System.Windows.Forms.Form
    {
        protected override void OnShown(EventArgs e)
        { //https://stackoverflow.com/a/6901015
            base.OnShown(e);
            if (Owner != null && StartPosition == System.Windows.Forms.FormStartPosition.Manual)
            {
                int offset = Owner.OwnedForms.Length * 38;  
                System.Drawing.Point p = new System.Drawing.Point(Owner.Left + offset, Owner.Top + offset);
                this.Location = p;
            }
        }
        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }
        public MyInfoWindow()  {
            InitializeComponent();
        }
        protected override System.Windows.Forms.CreateParams CreateParams
        {
            get
            {
                System.Windows.Forms.CreateParams baseParams = base.CreateParams;
                const int WS_EX_NOACTIVATE = 0x08000000;
                const int WS_EX_TOOLWINDOW = 0x00000080;
                baseParams.ExStyle |= (int)(WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW);
                return baseParams;
            }
        }
        public void setInfoText(String in_infoDescription, bool critical) {
            if (critical == true) { this.labelDescription.ForeColor = System.Drawing.Color.DarkRed; } else { this.labelDescription.ForeColor = System.Drawing.Color.Black; }
            this.labelDescription.Text = in_infoDescription;
        }

        public void setInfoText1(string in_infoDescription, bool critical)
        {
            if (critical == true) { this.labelDescription2.ForeColor = System.Drawing.Color.DarkRed; } else { this.labelDescription2.ForeColor = System.Drawing.Color.Black; }
            this.labelDescription2.Text = in_infoDescription;
        }

        public void setInfoText2(string in_infoDescription, bool critical)
        {
            if (critical == true) { this.labelDescription3.ForeColor = System.Drawing.Color.DarkRed; } else { this.labelDescription3.ForeColor = System.Drawing.Color.Black; }
            this.labelDescription3.Text = in_infoDescription;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()  {
            //http://www.philosophicalgeek.com/2008/12/12/an-easy-stack-layout-panel-for-winforms/
            this.labelDescription = new System.Windows.Forms.Label();
            this.labelDescription2 = new System.Windows.Forms.Label();
            this.labelDescription3 = new System.Windows.Forms.Label();
            panelRoot = new System.Windows.Forms.FlowLayoutPanel();
            this.panelRoot.SuspendLayout();
            this.SuspendLayout();
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.AutoSize = true;
            this.labelDescription2.Name = "labelDescription2";
            this.labelDescription2.AutoSize = true;
            this.labelDescription3.Name = "labelDescription3";
            this.labelDescription3.AutoSize = true;

            panelRoot.AutoScroll = true;
            panelRoot.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            panelRoot.WrapContents = false;
            this.panelRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRoot.Controls.Add(this.labelDescription);
            this.panelRoot.Controls.Add(this.labelDescription2);
            this.panelRoot.Controls.Add(this.labelDescription3);
            this.Controls.Add(this.panelRoot);
            this.panelRoot.ResumeLayout(false);
            this.panelRoot.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.FlowLayoutPanel panelRoot;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.Label labelDescription2;
        private System.Windows.Forms.Label labelDescription3;

        
    }

    public class MySimpleBarcodeFunctionalityHandler {
        public DateTime _lastKeystroke = new DateTime(0);
        public List<char> _barcode = new List<char>(10);

        public static uint lastKeyStrokeTimeoutMs = 50;


        public static String activatedMessage = "Отсканируйте штрихкод";
        public static String deactivatedMessage = "Окно неактивно, штрихкоды не принимаются... \n Активируйте окно.";
        public static String messageBarcodeObtainedBase = "Штрихкод получен: ";
        public static String messageNetworkNotObtained = "Не указано хранилище данных";
        // https://stackoverflow.com/q/616584
        public static String messageMSGBOX_usage = "Запускайте с cmd с указанием сервера, где хранится информация \n" +
            "Например: \n " + System.AppDomain.CurrentDomain.FriendlyName + " http://localhost:80/";
        public static String messageMSGBOX_caption = "Использование";
        public static String messageBarcodeRecorded = "Отсканировано!";

        public System.Windows.Forms.Form recordedOwner;
        private MyInfoWindow serviceForm;
        public MySimpleNetworkHandler recordedHandlerOfRequests;
        //required for displaying status window.
        public void signSecondaryWindowHandlingPact(System.Windows.Forms.Form in_wndParent) {
            recordedOwner = in_wndParent;
        }
        public void renderWindowStatusMessage(String in_Status, int index, bool critical) {
            if (serviceForm != null) {
                switch (index)
                {
                    case 0: {
                            (serviceForm as MyInfoWindow).setInfoText(in_Status, critical);
                            break;
                    }
                    case 1: {
                            (serviceForm as MyInfoWindow).setInfoText1(in_Status, critical);
                            break;
                    }
                    case 2: {
                            (serviceForm as MyInfoWindow).setInfoText2(in_Status, critical);
                            break;
                    }
                    default: { break; }
                }
                
            }
        }
        public void renderWindowStatus() {
            // https://stackoverflow.com/a/428517
            //If you run Form B on a separate thread from A and C, the ShowDialog call will only block that thread. Clearly, that's not a trivial investment of work of course.
            if (serviceForm == null)
            {
                serviceForm = new MyInfoWindow();
                serviceForm.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                serviceForm.Width = 300;
                serviceForm.Height = 100;
                serviceForm.Show(this.recordedOwner);
            } else {
                //cleanup messages
                (serviceForm as MyInfoWindow).setInfoText("", false);
                (serviceForm as MyInfoWindow).setInfoText1("", false);
                (serviceForm as MyInfoWindow).setInfoText2("", false);
            }
            
               
        }

        internal void processBarcodeCompletely(List<char> barcode)
        {
            String barcode2 = new String(barcode.ToArray());
            barcode2 = barcode2.Substring(0, barcode2.Length-1);
            this.renderWindowStatus();
            this.renderWindowStatusMessage(MySimpleBarcodeFunctionalityHandler.messageBarcodeObtainedBase + barcode2, 0, false);
            if (this.recordedHandlerOfRequests != null)
            {
                Tuple<List<String>, Boolean> netResponse = this.recordedHandlerOfRequests.sendNetworkBarcodeDataToServer(barcode2);
                if ((netResponse.Item2 == true))
                {
                    switch (netResponse.Item1[0])
                    {
                        case "OK":
                            {
                                this.renderWindowStatusMessage(messageBarcodeRecorded, 1, false);
                                this.renderWindowStatusMessage(netResponse.Item1[1], 2, false);
                                break;
                            }
                        case "FAIL_NOTALREADYEXIST":
                            {
                                this.renderWindowStatusMessage("Такого штрихкода нету в базе!", 1, true);
                                break;
                            }
                        default:
                            {
                                this.renderWindowStatusMessage(netResponse.Item1[0], 1, true);
                                break;
                            }
                    }

                }
                else {
                    this.renderWindowStatusMessage(netResponse.Item1[0], 1, true);
                }

                
            }
            else {
                this.renderWindowStatusMessage(MySimpleBarcodeFunctionalityHandler.messageNetworkNotObtained, 1, true);
            }
        }

        internal void signNetworkHandlingPact(MySimpleNetworkHandler theNetworkHandler)
        {
            recordedHandlerOfRequests = theNetworkHandler;
        }
    }

    public class MySimpleNetworkHandler
    {
        public String remoteServerName;
        public static String REST_address= "recvbarcode";

        public MySimpleNetworkHandler(string v)        {
            if (v[v.Length-1]!='/')
            {
                remoteServerName =v+'/';
            } else 
            remoteServerName = v;
        }
        public Tuple<List<String>, Boolean> sendNetworkBarcodeDataToServer(String in_barcode)
        {
            List<String> formalResponse = new List<string>();
            Boolean successResult = false;
            //second argument shows correctness of response
            Tuple<List<String>, Boolean> result = null;

            String json = "{ \"scannedbarcode\":" + "\""+in_barcode+"\"" + "}";

            System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
            if (client == null)
            {
                formalResponse.Add("Не получилось соединиться с сервером"); successResult = false;
                result = new Tuple<List<string>, bool>(formalResponse, false);
                return result;
            }
            client.MaxResponseContentBufferSize = 256000;
            int _TimeoutSec = 90;
            client.Timeout = new TimeSpan(0, 0, _TimeoutSec);
            string _ContentType = "application/json";
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(_ContentType));
            var uri = new Uri(String.Format(remoteServerName +"{0}", REST_address));
            System.Net.Http.HttpRequestMessage msgToSend = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Post, uri);

            var content = new System.Net.Http.StringContent(json, Encoding.UTF8, "application/json");
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(_ContentType);
            msgToSend.Content = content;
            System.Net.Http.HttpResponseMessage response;
            // response = client.PostAsync(uri, content).Result;
            String execResult = ""; String responseContent = "";
            try
            {
                response = client.SendAsync(msgToSend).Result;
                responseContent = response.Content.ReadAsStringAsync().Result;
                successResult = true;
            }
            catch (System.Net.Http.HttpRequestException ex1)
            {
                execResult = "Http Request Failure: " + ex1.Message;
                successResult = false;
            }
            catch (TimeoutException ex2)
            {
                execResult = "Http Request Timeout: " + ex2.Message;
                successResult = false;
            }
            catch (Exception ex)
            {
                execResult = "Network Failure: " + ex.Message;
                successResult = false;
            }
            if (successResult == false) {
                formalResponse.Add(execResult);
            } else {
                List<JSONresponseFromAPI> responseParsed = Newtonsoft.Json.JsonConvert.DeserializeObject<List<JSONresponseFromAPI>>(responseContent);
                successResult = true;
                formalResponse.Add(responseParsed[0].status);
                formalResponse.Add(responseParsed[0].time);
            }
            result = new Tuple<List<string>, bool>(formalResponse, successResult);
            return result;
        }
    }

}
