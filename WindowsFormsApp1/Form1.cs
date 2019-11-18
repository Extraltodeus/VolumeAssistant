using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.CoreAudioApi;


namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        MMDevice device;
        private Dictionary<int, float> smoothLoops;
        private int currentLoop;
        private int refreshRate = 240;

        protected float ValueSmoother(float result)
        {
            int smoothFactor = getSmoothLength();
            smoothLoops[currentLoop] = result;
            int divider = 0;
            float total = 0;
            for (int i = 0; i < smoothFactor; i++)
            {
                if (!smoothLoops.ContainsKey(i))
                {
                    smoothLoops[i] = result;
                }
                total = total + smoothLoops[i];
                divider++;
            }
            if (currentLoop < smoothFactor)
            {
                currentLoop++;
            }
            else
            {
                currentLoop = 0;
            }
            return total / divider;
        }

        public static MMDevice GetDefaultRenderDevice()
        {
            using (var enumerator = new MMDeviceEnumerator())
            {
                return enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
            }
        }

        public Form1()
        {
            InitializeComponent();
            smoothLoops = new Dictionary<int, float>();
            currentLoop = 0;
        }

        private void setVolume(float vol)
        {
            device.AudioEndpointVolume.MasterVolumeLevelScalar = vol;
        }

        private void InitializeTimer()
        {
            float t1f = 1000 / refreshRate;
            timer1.Interval = (int)t1f;
            timer1.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            device = GetDefaultRenderDevice();
            InitializeTimer();

            refreshLabels();
        }

        private void volumeControle()
        {
            var peaks = device.AudioMeterInformation.PeakValues;
            var peak = (peaks[0] + peaks[1]) / 2;

            float correction = peak * ((float)trackBar1.Value / 10);

            if (correction > 1)
                correction = 1;

            float vol = 1 - correction;

            vol = ValueSmoother(vol);

            int h = pictureBox1.Size.Height - (int)(pictureBox1.Size.Height * vol);
            pictureBox2.Size = new Size(pictureBox1.Size.Width, h);



            float endResult = vol * peak;
            if (endResult > 1)
                endResult = 1;
            
            int h2 = pictureBox4.Size.Height - (int)(pictureBox4.Size.Height * endResult);
            pictureBox3.Size = new Size(pictureBox4.Size.Width, h2);

            int h3 = pictureBox4.Size.Height - (int)(pictureBox6.Size.Height * peak);
            pictureBox5.Size = new Size(pictureBox6.Size.Width, h3);

            setVolume(vol);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            volumeControle();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            refreshLabels();
        }


        private int getSmoothLength()
        {
            if (trackBar3.Value == 0)
                return 1;
            
            int sml = refreshRate * trackBar3.Value / 10;

            return sml;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            refreshLabels();
        }

        private void refreshLabels()
        {
            label2.Text = ((float)trackBar1.Value / 10).ToString();
            label6.Text = ((float)trackBar3.Value / 10).ToString();
            if (trackBar3.Value == 0)
                label6.Text = "Instant.";
        }

        private void setPreset(int c, int l)
        {
            trackBar1.Value = c;
            trackBar3.Value = l;
            refreshLabels();
        }

        private void actionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setPreset(13, 1);
        }

        private void movieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setPreset(6, 10);
        }

        private void gamingToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            setPreset(7, 1);
        }

        private void musicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setPreset(5,3);
        }

        private void loudYoutuberToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setPreset(9, 2);
        }

        private void instantCorrectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setPreset(6, 0);
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void onToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled == true)
            {
                timer1.Enabled = false;
                onToolStripMenuItem.Text = "OFF";
            }
            else
            {
                timer1.Enabled = true;
                onToolStripMenuItem.Text = "ON";
            }
        }
    }
}
