using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsService;

namespace httplistener
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            HttpService service = new HttpService();
            service.Init();
            webBrowser1.Url = new Uri(Application.StartupPath + "\\test.html");
        }
      
    }
}

