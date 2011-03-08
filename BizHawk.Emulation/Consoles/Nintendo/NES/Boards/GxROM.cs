using System;
using System.IO;
using System.Diagnostics;

namespace BizHawk.Emulation.Consoles.Nintendo
{
	//generally mapper66

	//Doraemon
	//Dragon Power
	//Gumshoe
	//Thunder & Lightning
	//Super Mario Bros. + Duck Hunt

	//TODO - bus conflicts

	public class GxROM : NES.NESBoardBase
	{
		//configuraton
		int prg_mask, chr_mask;

		//state
		int prg, chr;

		public override bool Configure()
		{
			//configure
			switch (Cart.board_type)
			{
				case "NES-GNROM": //thunder & lightning
				case "BANDAI-GNROM":
				case "HVC-GNROM":
				case "NES-MHROM": //Super Mario Bros. / Duck Hunt
					AssertPrg(Cart.board_type == "NES-MHROM" ? 64 : 128); AssertChr(8, 16, 32); AssertVram(0); AssertWram(0);
					break;

				default:
					return false;
			}

			prg_mask = (Cart.prg_size/8/2) - 1;
			chr_mask = (Cart.chr_size / 8) - 1;
			SetMirrorType(Cart.pad_h, Cart.pad_v);


			return true;
		}
		public override byte ReadPRG(int addr)
		{
			return ROM[addr + (prg<<15)];
		}

		public override byte ReadPPU(int addr)
		{
			if (addr < 0x2000)
			{
				return VROM[addr + (chr << 13)];
			}
			else return base.ReadPPU(addr);
		}

		public override void WritePRG(int addr, byte value)
		{
			chr = ((value & 3) & chr_mask);
			prg = (((value>>4) & 3) & prg_mask);
		}

		public override void SaveStateBinary(BinaryWriter bw)
		{
			base.SaveStateBinary(bw);
			bw.Write(chr);
			bw.Write(prg);
		}

		public override void LoadStateBinary(BinaryReader br)
		{
			base.LoadStateBinary(br);
			chr = br.ReadInt32();
			prg = br.ReadInt32();
		}
	}
}