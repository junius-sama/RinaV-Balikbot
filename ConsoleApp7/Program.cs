using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;
using SoundLevelMonitor;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleApp7
{
	// Token: 0x02000003 RID: 3
	internal class Program
	{

        // Token: 0x06000004 RID: 4 RVA: 0x00002059 File Offset: 0x00000259
        private static void Main(string[] args)
		{
            new AudioLevelMonitor();
            System.Windows.Forms.Application.Run();
        }

		// Token: 0x04000004 RID: 4
		private static System.Threading.Timer timer;
	}
}
