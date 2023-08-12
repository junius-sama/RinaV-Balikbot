using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Timers;
using ConsoleApp7;
using CSCore.CoreAudioAPI;

namespace SoundLevelMonitor
{
	// Token: 0x02000004 RID: 4
	public class AudioLevelMonitor
	{
		// Token: 0x06000006 RID: 6 RVA: 0x000021F4 File Offset: 0x000003F4
		public AudioLevelMonitor()
		{
			this.dispatchingTimer = new System.Timers.Timer(this.interval_ms);
			this.dispatchingTimer.Elapsed += this.DispatchingTimer_Elapsed;
			this.dispatchingTimer.AutoReset = false;
			this.dispatchingTimer.Start();
			Console.Title = "junius";
			Console.WriteLine("junius");
			Console.WriteLine("discord.gg/5hTq2FWEgJ");
        }

		// Token: 0x06000007 RID: 7 RVA: 0x00002060 File Offset: 0x00000260
		public void Stop()
		{
			this.dispatchingTimer.Stop();
		}

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000008 RID: 8 RVA: 0x0000229C File Offset: 0x0000049C
		// (remove) Token: 0x06000009 RID: 9 RVA: 0x000022D4 File Offset: 0x000004D4
		public event AudioLevelMonitor.NewAudioSamplesEvent NewAudioSamplesEventListeners;

		// Token: 0x0600000A RID: 10 RVA: 0x0000206F File Offset: 0x0000026F
		private void DispatchingTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
            this.CheckAudioLevels();
			this.dispatchingTimer.Start();
		}

		// Token: 0x0600000B RID: 11 RVA: 0x0000230C File Offset: 0x0000050C
		private void truncateSamples(List<double> samples)
		{
			int num = samples.Count - this.maxSamplesToKeep;
			while (num-- > 0)
			{
				samples.RemoveAt(0);
			}
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002340 File Offset: 0x00000540
		private bool areSamplesEmpty(List<double> samples)
		{
			foreach (double num in samples)
			{
				bool flag = num != 0.0;
				if (flag)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000023AC File Offset: 0x000005AC
		public IDictionary<string, AudioLevelMonitor.SampleInfo> GetActiveSamples()
		{
			Dictionary<string, AudioLevelMonitor.SampleInfo> dictionary = new Dictionary<string, AudioLevelMonitor.SampleInfo>();
			lock (this)
			{
				foreach (KeyValuePair<string, AudioLevelMonitor.SampleInfo> keyValuePair in this.sessionIdToInfo)
				{
					AudioLevelMonitor.SampleInfo value = keyValuePair.Value;
					bool flag2 = this.sessionIdToAudioSamples.ContainsKey(value.sessionId);
					if (flag2)
					{
						value.samples = this.sessionIdToAudioSamples[value.sessionId].ToArray();
						dictionary[value.sessionId] = value;
					}
				}
			}
			return dictionary;
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002480 File Offset: 0x00000680
		public void CheckAudioLevels()
		{
			lock (this)
			{
				HashSet<string> hashSet = new HashSet<string>();
				try
				{
					using (AudioSessionManager2 defaultAudioSessionManager = AudioLevelMonitor.GetDefaultAudioSessionManager2(DataFlow.Render))
					{
						using (AudioSessionEnumerator sessionEnumerator = defaultAudioSessionManager.GetSessionEnumerator())
						{
							foreach (AudioSessionControl audioSessionControl in sessionEnumerator)
							{
								using (AudioSessionControl2 audioSessionControl2 = audioSessionControl.QueryInterface<AudioSessionControl2>())
								{
									Process process = audioSessionControl2.Process;
									string sessionIdentifier = audioSessionControl2.SessionIdentifier;
									int processID = audioSessionControl2.ProcessID;
									string text = audioSessionControl2.DisplayName;
									bool flag2 = process != null;
									if (flag2)
									{
										bool flag3 = text == "";
										if (flag3)
										{
											text = process.MainWindowTitle;
										}
										bool flag4 = text == "";
										if (flag4)
										{
											text = process.ProcessName;
										}
									}
									bool flag5 = text == "";
									if (flag5)
									{
										text = "--unnamed--";
									}
									AudioLevelMonitor.SampleInfo value = default(AudioLevelMonitor.SampleInfo);
									value.sessionId = sessionIdentifier;
									value.pid = processID;
									value.SessionName = text;
									this.sessionIdToInfo[sessionIdentifier] = value;
									using (AudioMeterInformation audioMeterInformation = audioSessionControl.QueryInterface<AudioMeterInformation>())
									{
										float peakValue = audioMeterInformation.GetPeakValue();
										bool flag6 = peakValue != 0f;
										if (flag6)
										{
											bool flag7 = process != null;
											if (flag7)
											{
												hashSet.Add(sessionIdentifier);
												List<double> list;
												bool flag8 = !this.sessionIdToAudioSamples.TryGetValue(sessionIdentifier, out list);
												if (flag8)
												{
													list = new List<double>();
													this.sessionIdToAudioSamples[sessionIdentifier] = list;
												}
												float peakValue2 = audioMeterInformation.GetPeakValue();
												list.Add((double)peakValue2);
												this.truncateSamples(list);
												balikci balikci = new balikci();
												balikci.isev(process.ProcessName.ToString(), peakValue);
											}
										}
									}
								}
							}
						}
					}
				}
				catch (CoreAudioAPIException ex)
				{
					Console.WriteLine("Okumayla alakalı bir problem meydana geldi.");
					Thread.Sleep(1000);
					Environment.Exit(0);
					return;
				}
				HashSet<string> hashSet2 = new HashSet<string>();
				foreach (KeyValuePair<string, List<double>> keyValuePair in this.sessionIdToAudioSamples)
				{
					bool flag9 = !hashSet.Contains(keyValuePair.Key);
					if (flag9)
					{
						keyValuePair.Value.Add(0.0);
						this.truncateSamples(keyValuePair.Value);
						bool flag10 = this.areSamplesEmpty(keyValuePair.Value);
						if (flag10)
						{
							hashSet2.Add(keyValuePair.Key);
						}
					}
				}
				foreach (string key in hashSet2)
				{
					this.sessionIdToAudioSamples.Remove(key);
				}
			}
			GC.Collect();
			AudioLevelMonitor.NewAudioSamplesEvent newAudioSamplesEventListeners = this.NewAudioSamplesEventListeners;
			if (newAudioSamplesEventListeners != null)
			{
				newAudioSamplesEventListeners(this);
			}
		}

		// Token: 0x0600000F RID: 15 RVA: 0x0000289C File Offset: 0x00000A9C
		private static AudioSessionManager2 GetDefaultAudioSessionManager2(DataFlow dataFlow)
		{
			AudioSessionManager2 result;
			using (MMDeviceEnumerator mmdeviceEnumerator = new MMDeviceEnumerator())
			{
				using (MMDevice defaultAudioEndpoint = mmdeviceEnumerator.GetDefaultAudioEndpoint(dataFlow, Role.Multimedia))
				{
					AudioSessionManager2 audioSessionManager = AudioSessionManager2.FromMMDevice(defaultAudioEndpoint);
					result = audioSessionManager;
				}
			}
			return result;
		}

		// Token: 0x04000005 RID: 5
		private System.Timers.Timer dispatchingTimer;

		// Token: 0x04000006 RID: 6
		public double interval_ms = 50.0;

		// Token: 0x04000007 RID: 7
		private IDictionary<string, AudioLevelMonitor.SampleInfo> sessionIdToInfo = new Dictionary<string, AudioLevelMonitor.SampleInfo>();

		// Token: 0x04000008 RID: 8
		private IDictionary<string, List<double>> sessionIdToAudioSamples = new Dictionary<string, List<double>>();

		// Token: 0x04000009 RID: 9
		private int maxSamplesToKeep = 1000;

		// Token: 0x02000006 RID: 6
		// (Invoke) Token: 0x0600001A RID: 26
		public delegate void NewAudioSamplesEvent(AudioLevelMonitor monitor);

		// Token: 0x02000007 RID: 7
		public struct SampleInfo
		{
			// Token: 0x04000011 RID: 17
			public string sessionId;

			// Token: 0x04000012 RID: 18
			public int pid;

			// Token: 0x04000013 RID: 19
			public string SessionName;

			// Token: 0x04000014 RID: 20
			public double[] samples;
		}
	}
}
