using System;
using System.Collections;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Numerics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using AutoItX3Lib;
using System.Reflection.Emit;
using System.Reflection;

namespace ConsoleApp7
{
	// Token: 0x02000002 RID: 2
	internal class balikci
	{

    // Token: 0x06000001 RID: 1 RVA: 0x000020A0 File Offset: 0x000002A0
    public void isev(string pencereadi, float value)
		{
			bool flag = pencereadi == "ragemp_game_ui" && balikci.oltalama == 0;
			if (flag)
			{
				Console.WriteLine(string.Format("[{0}] Oltaya balık takıldı.", balikci.deneme));
				balikci.deneme++;
				SendKeys.SendWait("x");
				Console.WriteLine("Balık çekildi.");
				balikci.oltayagelen++;
				Thread.Sleep(1700);
				SendKeys.SendWait("x");
				Console.WriteLine("Olta atıldı.");
            }
		}

		// Token: 0x04000001 RID: 1
		private static int oltayagelen;

		// Token: 0x04000002 RID: 2
		private static int deneme;

		// Token: 0x04000003 RID: 3
		public static int oltalama;
	}
}
