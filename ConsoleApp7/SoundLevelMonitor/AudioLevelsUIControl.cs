using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SoundLevelMonitor
{
	// Token: 0x02000005 RID: 5
	internal class AudioLevelsUIControl : Control
	{
		// Token: 0x06000010 RID: 16 RVA: 0x000028FC File Offset: 0x00000AFC
		public AudioLevelsUIControl()
		{
			this.DoubleBuffered = true;
			this.dispatcherTimer = new Timer();
			this.dispatcherTimer.Tick += this.DispatcherTimer_Tick;
			this.dispatcherTimer.Interval = 100;
			this.dispatcherTimer.Start();
			this.pens.Add(new Pen(Brushes.Crimson, 1f));
			this.pens.Add(new Pen(Brushes.DarkKhaki, 1f));
			this.pens.Add(new Pen(Brushes.FloralWhite, 1f));
			this.pens.Add(new Pen(Brushes.HotPink, 1f));
			this.pens.Add(new Pen(Brushes.Yellow, 1f));
			this.pens.Add(new Pen(Brushes.Lavender, 1f));
			this.pens.Add(new Pen(Brushes.Cyan, 1f));
			this.pens.Add(new Pen(Brushes.Maroon, 1f));
			base.SetStyle(ControlStyles.ResizeRedraw, true);
			base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002085 File Offset: 0x00000285
		private void DispatcherTimer_Tick(object sender, EventArgs e)
		{
			this.dispatcherTimer.Stop();
			base.Invalidate();
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000012 RID: 18 RVA: 0x00002A78 File Offset: 0x00000C78
		// (set) Token: 0x06000013 RID: 19 RVA: 0x00002A90 File Offset: 0x00000C90
		public AudioLevelMonitor AudioMonitor
		{
			get
			{
				return this._audioMonitor;
			}
			set
			{
				this._audioMonitor = value;
				bool flag = this._audioMonitor != null;
				if (flag)
				{
				}
			}
		}

		// Token: 0x06000014 RID: 20 RVA: 0x0000209B File Offset: 0x0000029B
		private void _audioMonitor_NewAudioSamplesEventListeners(AudioLevelMonitor monitor)
		{
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002AB4 File Offset: 0x00000CB4
		private void RenderVUMeterGrid(Graphics g, double maxSample)
		{
			g.FillRectangle(Brushes.Black, 0, 0, base.Size.Width, base.Size.Height);
			double num = 0.01;
			double num2 = num * 2.0;
			for (double num3 = 0.0; num3 < maxSample; num3 += num)
			{
				int y = (int)((double)base.Size.Height - (double)base.Size.Height * (num3 / maxSample));
				g.DrawLine(this.greenPen, new Point(0, y), new Point(base.Size.Width, y));
				bool flag = num3 >= num2;
				if (flag)
				{
					num *= 2.0;
					num2 = num * 2.0;
				}
			}
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002B98 File Offset: 0x00000D98
		private double computeMaxSampleLastN(IDictionary<string, AudioLevelMonitor.SampleInfo> sampleMap, int lastN)
		{
			double num = 0.0;
			foreach (KeyValuePair<string, AudioLevelMonitor.SampleInfo> keyValuePair in sampleMap)
			{
				double[] samples = keyValuePair.Value.samples;
				for (int i = 1; i <= samples.Length; i++)
				{
					bool flag = i > lastN;
					if (!flag)
					{
						double val = samples[samples.Length - i];
						num = Math.Max(num, val);
						bool flag2 = num > 0.9;
						if (flag2)
						{
							return 1.0;
						}
					}
				}
			}
			return num;
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002C5C File Offset: 0x00000E5C
		private Pen penForSessionId(string sessionId)
		{
			bool flag = this.nextPenToAllocate < 0;
			if (flag)
			{
				this.nextPenToAllocate = Math.Abs((int)DateTime.Now.Ticks) % (this.pens.Count - 1);
			}
			bool flag2 = this._sessionIdToPen.ContainsKey(sessionId);
			Pen result;
			if (flag2)
			{
				result = this._sessionIdToPen[sessionId];
			}
			else
			{
				Pen pen = this._sessionIdToPen[sessionId] = this.pens[this.nextPenToAllocate];
				this.nextPenToAllocate = (this.nextPenToAllocate + 1) % (this.pens.Count - 1);
				result = pen;
			}
			return result;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002D08 File Offset: 0x00000F08
		protected override void OnPaint(PaintEventArgs pe)
		{
			base.OnPaint(pe);
			Graphics graphics = pe.Graphics;
			bool flag = this.AudioMonitor == null;
			if (flag)
			{
				this.RenderVUMeterGrid(graphics, 1.0);
			}
			else
			{
				IDictionary<string, AudioLevelMonitor.SampleInfo> activeSamples = this.AudioMonitor.GetActiveSamples();
				double num = this.computeMaxSampleLastN(activeSamples, base.Size.Width);
				num = Math.Max(num, 0.05);
				this.RenderVUMeterGrid(graphics, num);
				foreach (KeyValuePair<string, AudioLevelMonitor.SampleInfo> keyValuePair in activeSamples)
				{
					Pen pen = this.penForSessionId(keyValuePair.Value.sessionId);
					string sessionName = keyValuePair.Value.SessionName;
					double[] samples = keyValuePair.Value.samples;
					double num2 = samples[samples.Length - 1];
					for (int i = 0; i < samples.Length - 1; i++)
					{
						bool flag2 = i > base.Size.Width;
						if (flag2)
						{
							break;
						}
						double num3 = samples[samples.Length - (i + 1)];
						graphics.DrawLine(pen, new Point(base.Size.Width - i, (int)((double)base.Size.Height - (double)base.Size.Height * (num2 / num))), new Point(base.Size.Width - (i + 1), (int)((double)base.Size.Height - (double)base.Size.Height * (num3 / num))));
						num2 = num3;
					}
				}
				List<string> list = activeSamples.Keys.ToList<string>();
				list.Sort();
				Font defaultFont = SystemFonts.DefaultFont;
				float num4 = 5f;
				float num5 = 0f;
				foreach (string key in list)
				{
					string sessionName2 = activeSamples[key].SessionName;
					SizeF sizeF = graphics.MeasureString(sessionName2, defaultFont);
					num4 += sizeF.Height;
					num5 = Math.Max(num5, sizeF.Width);
					num4 += 10f;
				}
				graphics.FillRectangle(Brushes.Black, 5f, 10f, num5 + 10f, num4);
				graphics.DrawRectangle(this.greenPen, 5f, 10f, num5 + 10f, num4);
				float num6 = 5f;
				foreach (string text in list)
				{
					string sessionName3 = activeSamples[text].SessionName;
					Pen pen2 = this.penForSessionId(text);
					Brush brush = pen2.Brush;
					num6 += graphics.MeasureString(sessionName3, defaultFont).Height;
					graphics.DrawString(sessionName3, defaultFont, brush, new PointF(10f, num6));
					num6 += 10f;
				}
				this.dispatcherTimer.Start();
			}
		}

		// Token: 0x0400000B RID: 11
		private AudioLevelMonitor _audioMonitor;

		// Token: 0x0400000C RID: 12
		private List<Pen> pens = new List<Pen>();

		// Token: 0x0400000D RID: 13
		private Dictionary<string, Pen> _sessionIdToPen = new Dictionary<string, Pen>();

		// Token: 0x0400000E RID: 14
		private Timer dispatcherTimer;

		// Token: 0x0400000F RID: 15
		private Pen greenPen = new Pen(Brushes.Green, 0.5f);

		// Token: 0x04000010 RID: 16
		private int nextPenToAllocate = -1;
	}
}
