// Generated on: 2018-10-10 14:53:26


using System;
using System.IO;
using System.Collections.Generic;

/*
    Generated portion of the Scanner

        Generated from Scanner.frame and HackerCalc.atg

    Keith Fletcher
    Oct 2018

    This file is Unlicensed.
    See the foot of the file, or refer to <http://unlicense.org>
*/

namespace HisRoyalRedness.com {

public partial class Scanner : IDisposable {
	const char EOL = '\n';
	const int eofSym = 0; /* pdt */
	const int maxT = 54;
	const int noSym = 54;
	char valCh;       // current input character (for token.val)

	static Scanner() {
		start = new Dictionary<int, int>(128);
		for (int i = 95; i <= 95; ++i) start[i] = 1;
		for (int i = 98; i <= 98; ++i) start[i] = 169;
		for (int i = 111; i <= 111; ++i) start[i] = 170;
		for (int i = 52; i <= 57; ++i) start[i] = 171;
		for (int i = 102; i <= 102; ++i) start[i] = 95;
		start[50] = 172; 
		start[49] = 173; 
		for (int i = 33; i <= 33; ++i) start[i] = 133;
		for (int i = 126; i <= 126; ++i) start[i] = 133;
		for (int i = 42; i <= 42; ++i) start[i] = 174;
		for (int i = 47; i <= 47; ++i) start[i] = 175;
		for (int i = 92; i <= 92; ++i) start[i] = 175;
		for (int i = 37; i <= 37; ++i) start[i] = 134;
		for (int i = 43; i <= 43; ++i) start[i] = 136;
		for (int i = 45; i <= 45; ++i) start[i] = 137;
		for (int i = 60; i <= 60; ++i) start[i] = 138;
		for (int i = 62; i <= 62; ++i) start[i] = 139;
		for (int i = 38; i <= 38; ++i) start[i] = 141;
		for (int i = 94; i <= 94; ++i) start[i] = 141;
		for (int i = 124; i <= 124; ++i) start[i] = 141;
		for (int i = 40; i <= 40; ++i) start[i] = 176;
		for (int i = 41; i <= 41; ++i) start[i] = 167;
		for (int i = 44; i <= 44; ++i) start[i] = 168;
		start[105] = 177; 
		start[117] = 178; 
		start[48] = 179; 
		start[46] = 180; 
		start[116] = 181; 
		start[100] = 182; 
		start[115] = 183; 
		start[109] = 184; 
		start[104] = 185; 
		start[51] = 186; 
		start[110] = 130; 
		start[Buffer.EOF] = -1;

	}
	
	void NextCh() {
		if (oldEols > 0) { ch = EOL; oldEols--; } 
		else {
			pos = buffer.Pos;
			// buffer reads unicode chars, if UTF8 has been detected
			ch = buffer.Read(); col++; charPos++;
			// replace isolated '\r' by '\n' in order to make
			// eol handling uniform across Windows, Unix and Mac
			if (ch == '\r' && buffer.Peek() != '\n') ch = EOL;
			if (ch == EOL) { line++; col = 0; }
		}
		if (ch != Buffer.EOF) {
			valCh = (char) ch;
			ch = char.ToLower((char) ch);
		}

	}

	void AddCh() {
		if (tlen >= tval.Length) {
			char[] newBuf = new char[2 * tval.Length];
			Array.Copy(tval, 0, newBuf, 0, tval.Length);
			tval = newBuf;
		}
		if (ch != Buffer.EOF) {
			tval[tlen++] = valCh;
			NextCh();
		}
	}



	void CheckLiteral() {
		switch (t.val.ToLower()) {
			//Terminals
			//Pragmas
			default: break;
		}
	}

	Token NextToken() {
		while (ch == ' ' ||
			ch >= 9 && ch <= 10 || ch == 13
		) NextCh();

		int apx = 0;
		int recKind = noSym;
		int recEnd = pos;
		t = new Token();
		t.pos = pos; t.col = col; t.line = line; t.charPos = charPos;
		int state;
		state = start.ContainsKey(ch) ? start[ch] : 0;
		tlen = 0; AddCh();
		
		switch (state) {
			case -1: { t.kind = eofSym; break; } // NextCh already done
			case 0: {
				if (recKind != noSym) {
					tlen = recEnd - t.pos;
					SetScannerBehindT();
				}
				t.kind = recKind; break;
			} // NextCh already done
			case 1:
				if (ch == '_' || ch >= 'a' && ch <= 'z') {AddCh(); goto case 2;}
				else {goto case 0;}
			case 2:
				if (ch == '(') {apx++; AddCh(); goto case 3;}
				else if (ch >= '0' && ch <= '9' || ch == '_' || ch >= 'a' && ch <= 'z') {AddCh(); goto case 2;}
				else {goto case 0;}
			case 3:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 1; /* func_name */ break;}
			case 4:
				{t.kind = 2; /* i4 */ break;}
			case 5:
				{t.kind = 3; /* i8 */ break;}
			case 6:
				{t.kind = 4; /* i16 */ break;}
			case 7:
				if (ch == '2') {AddCh(); goto case 8;}
				else {goto case 0;}
			case 8:
				{t.kind = 5; /* i32 */ break;}
			case 9:
				if (ch == '4') {AddCh(); goto case 10;}
				else {goto case 0;}
			case 10:
				{t.kind = 6; /* i64 */ break;}
			case 11:
				if (ch == '8') {AddCh(); goto case 12;}
				else {goto case 0;}
			case 12:
				{t.kind = 7; /* i128 */ break;}
			case 13:
				{t.kind = 8; /* u4 */ break;}
			case 14:
				{t.kind = 9; /* u8 */ break;}
			case 15:
				{t.kind = 10; /* u16 */ break;}
			case 16:
				if (ch == '2') {AddCh(); goto case 17;}
				else {goto case 0;}
			case 17:
				{t.kind = 11; /* u32 */ break;}
			case 18:
				if (ch == '4') {AddCh(); goto case 19;}
				else {goto case 0;}
			case 19:
				{t.kind = 12; /* u64 */ break;}
			case 20:
				if (ch == '8') {AddCh(); goto case 21;}
				else {goto case 0;}
			case 21:
				{t.kind = 13; /* u128 */ break;}
			case 22:
				if (ch == '2') {apx++; AddCh(); goto case 31;}
				else {goto case 0;}
			case 23:
				if (ch == '4') {apx++; AddCh(); goto case 32;}
				else {goto case 0;}
			case 24:
				if (ch == '8') {apx++; AddCh(); goto case 33;}
				else {goto case 0;}
			case 25:
				if (ch == '2') {apx++; AddCh(); goto case 37;}
				else {goto case 0;}
			case 26:
				if (ch == '4') {apx++; AddCh(); goto case 38;}
				else {goto case 0;}
			case 27:
				if (ch == '8') {apx++; AddCh(); goto case 39;}
				else {goto case 0;}
			case 28:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 14; /* bin_limited_int */ break;}
			case 29:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 14; /* bin_limited_int */ break;}
			case 30:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 14; /* bin_limited_int */ break;}
			case 31:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 14; /* bin_limited_int */ break;}
			case 32:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 14; /* bin_limited_int */ break;}
			case 33:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 14; /* bin_limited_int */ break;}
			case 34:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 14; /* bin_limited_int */ break;}
			case 35:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 14; /* bin_limited_int */ break;}
			case 36:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 14; /* bin_limited_int */ break;}
			case 37:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 14; /* bin_limited_int */ break;}
			case 38:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 14; /* bin_limited_int */ break;}
			case 39:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 14; /* bin_limited_int */ break;}
			case 40:
				if (ch == '2') {apx++; AddCh(); goto case 49;}
				else {goto case 0;}
			case 41:
				if (ch == '4') {apx++; AddCh(); goto case 50;}
				else {goto case 0;}
			case 42:
				if (ch == '8') {apx++; AddCh(); goto case 51;}
				else {goto case 0;}
			case 43:
				if (ch == '2') {apx++; AddCh(); goto case 55;}
				else {goto case 0;}
			case 44:
				if (ch == '4') {apx++; AddCh(); goto case 56;}
				else {goto case 0;}
			case 45:
				if (ch == '8') {apx++; AddCh(); goto case 57;}
				else {goto case 0;}
			case 46:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 15; /* oct_limited_int */ break;}
			case 47:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 15; /* oct_limited_int */ break;}
			case 48:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 15; /* oct_limited_int */ break;}
			case 49:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 15; /* oct_limited_int */ break;}
			case 50:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 15; /* oct_limited_int */ break;}
			case 51:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 15; /* oct_limited_int */ break;}
			case 52:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 15; /* oct_limited_int */ break;}
			case 53:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 15; /* oct_limited_int */ break;}
			case 54:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 15; /* oct_limited_int */ break;}
			case 55:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 15; /* oct_limited_int */ break;}
			case 56:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 15; /* oct_limited_int */ break;}
			case 57:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 15; /* oct_limited_int */ break;}
			case 58:
				if (ch == '2') {apx++; AddCh(); goto case 67;}
				else {goto case 0;}
			case 59:
				if (ch == '4') {apx++; AddCh(); goto case 68;}
				else {goto case 0;}
			case 60:
				if (ch == '8') {apx++; AddCh(); goto case 69;}
				else {goto case 0;}
			case 61:
				if (ch == '2') {apx++; AddCh(); goto case 73;}
				else {goto case 0;}
			case 62:
				if (ch == '4') {apx++; AddCh(); goto case 74;}
				else {goto case 0;}
			case 63:
				if (ch == '8') {apx++; AddCh(); goto case 75;}
				else {goto case 0;}
			case 64:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 16; /* dec_limited_int */ break;}
			case 65:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 16; /* dec_limited_int */ break;}
			case 66:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 16; /* dec_limited_int */ break;}
			case 67:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 16; /* dec_limited_int */ break;}
			case 68:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 16; /* dec_limited_int */ break;}
			case 69:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 16; /* dec_limited_int */ break;}
			case 70:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 16; /* dec_limited_int */ break;}
			case 71:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 16; /* dec_limited_int */ break;}
			case 72:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 16; /* dec_limited_int */ break;}
			case 73:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 16; /* dec_limited_int */ break;}
			case 74:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 16; /* dec_limited_int */ break;}
			case 75:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 16; /* dec_limited_int */ break;}
			case 76:
				if (ch == '2') {apx++; AddCh(); goto case 85;}
				else {goto case 0;}
			case 77:
				if (ch == '4') {apx++; AddCh(); goto case 86;}
				else {goto case 0;}
			case 78:
				if (ch == '8') {apx++; AddCh(); goto case 87;}
				else {goto case 0;}
			case 79:
				if (ch == '2') {apx++; AddCh(); goto case 91;}
				else {goto case 0;}
			case 80:
				if (ch == '4') {apx++; AddCh(); goto case 92;}
				else {goto case 0;}
			case 81:
				if (ch == '8') {apx++; AddCh(); goto case 93;}
				else {goto case 0;}
			case 82:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 17; /* hex_limited_int */ break;}
			case 83:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 17; /* hex_limited_int */ break;}
			case 84:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 17; /* hex_limited_int */ break;}
			case 85:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 17; /* hex_limited_int */ break;}
			case 86:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 17; /* hex_limited_int */ break;}
			case 87:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 17; /* hex_limited_int */ break;}
			case 88:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 17; /* hex_limited_int */ break;}
			case 89:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 17; /* hex_limited_int */ break;}
			case 90:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 17; /* hex_limited_int */ break;}
			case 91:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 17; /* hex_limited_int */ break;}
			case 92:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 17; /* hex_limited_int */ break;}
			case 93:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 17; /* hex_limited_int */ break;}
			case 94:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 23; /* typed_float */ break;}
			case 95:
				{t.kind = 24; /* float_type */ break;}
			case 96:
				if (ch == 's') {apx++; AddCh(); goto case 97;}
				else {goto case 0;}
			case 97:
				{
					tlen -= apx;
					SetScannerBehindT();
					t.kind = 25; /* typed_ts_seconds */ break;}
			case 98:
				{t.kind = 26; /* timespan_type */ break;}
			case 99:
				{t.kind = 27; /* time_type */ break;}
			case 100:
				{t.kind = 28; /* date_type */ break;}
			case 101:
				{t.kind = 29; /* ts_seconds_type */ break;}
			case 102:
				{t.kind = 30; /* ts_minutes_type */ break;}
			case 103:
				{t.kind = 31; /* ts_hours_type */ break;}
			case 104:
				{t.kind = 32; /* ts_days_type */ break;}
			case 105:
				if (ch == '0') {AddCh(); goto case 110;}
				else if (ch == '1') {AddCh(); goto case 111;}
				else {goto case 0;}
			case 106:
				if (ch == '-') {AddCh(); goto case 107;}
				else {goto case 0;}
			case 107:
				if (ch >= '0' && ch <= '2') {AddCh(); goto case 108;}
				else if (ch == '3') {AddCh(); goto case 109;}
				else {goto case 0;}
			case 108:
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 119;}
				else {goto case 0;}
			case 109:
				if (ch >= '0' && ch <= '1') {AddCh(); goto case 119;}
				else {goto case 0;}
			case 110:
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 106;}
				else {goto case 0;}
			case 111:
				if (ch >= '0' && ch <= '2') {AddCh(); goto case 106;}
				else {goto case 0;}
			case 112:
				if (ch == '0') {AddCh(); goto case 117;}
				else if (ch == '1') {AddCh(); goto case 118;}
				else {goto case 0;}
			case 113:
				if (ch == '/') {AddCh(); goto case 114;}
				else {goto case 0;}
			case 114:
				if (ch >= '0' && ch <= '2') {AddCh(); goto case 115;}
				else if (ch == '3') {AddCh(); goto case 116;}
				else {goto case 0;}
			case 115:
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 119;}
				else {goto case 0;}
			case 116:
				if (ch >= '0' && ch <= '1') {AddCh(); goto case 119;}
				else {goto case 0;}
			case 117:
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 113;}
				else {goto case 0;}
			case 118:
				if (ch >= '0' && ch <= '2') {AddCh(); goto case 113;}
				else {goto case 0;}
			case 119:
				{t.kind = 33; /* date */ break;}
			case 120:
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 121;}
				else {goto case 0;}
			case 121:
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 122;}
				else {goto case 0;}
			case 122:
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 126;}
				else {goto case 0;}
			case 123:
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 124;}
				else {goto case 0;}
			case 124:
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 125;}
				else {goto case 0;}
			case 125:
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 126;}
				else {goto case 0;}
			case 126:
				{t.kind = 34; /* date_rev */ break;}
			case 127:
				if (ch >= '0' && ch <= '5') {AddCh(); goto case 128;}
				else {goto case 0;}
			case 128:
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 129;}
				else {goto case 0;}
			case 129:
				{t.kind = 35; /* time */ break;}
			case 130:
				if (ch == 'o') {AddCh(); goto case 131;}
				else {goto case 0;}
			case 131:
				if (ch == 'w') {AddCh(); goto case 132;}
				else {goto case 0;}
			case 132:
				{t.kind = 36; /* now */ break;}
			case 133:
				{t.kind = 37; /* notToken */ break;}
			case 134:
				{t.kind = 38; /* multToken */ break;}
			case 135:
				{t.kind = 39; /* exponentToken */ break;}
			case 136:
				{t.kind = 40; /* addToken */ break;}
			case 137:
				{t.kind = 41; /* subToken */ break;}
			case 138:
				if (ch == '<') {AddCh(); goto case 140;}
				else {goto case 0;}
			case 139:
				if (ch == '>') {AddCh(); goto case 140;}
				else {goto case 0;}
			case 140:
				{t.kind = 42; /* shiftToken */ break;}
			case 141:
				{t.kind = 43; /* bitToken */ break;}
			case 142:
				{t.kind = 44; /* unlimited_cast */ break;}
			case 143:
				if (ch == ')') {AddCh(); goto case 147;}
				else {goto case 0;}
			case 144:
				if (ch == '2') {AddCh(); goto case 143;}
				else {goto case 0;}
			case 145:
				if (ch == '4') {AddCh(); goto case 143;}
				else {goto case 0;}
			case 146:
				if (ch == '8') {AddCh(); goto case 143;}
				else {goto case 0;}
			case 147:
				{t.kind = 45; /* signed_cast */ break;}
			case 148:
				if (ch == ')') {AddCh(); goto case 152;}
				else {goto case 0;}
			case 149:
				if (ch == '2') {AddCh(); goto case 148;}
				else {goto case 0;}
			case 150:
				if (ch == '4') {AddCh(); goto case 148;}
				else {goto case 0;}
			case 151:
				if (ch == '8') {AddCh(); goto case 148;}
				else {goto case 0;}
			case 152:
				{t.kind = 46; /* unsigned_cast */ break;}
			case 153:
				if (ch == ')') {AddCh(); goto case 156;}
				else {goto case 0;}
			case 154:
				if (ch == 'a') {AddCh(); goto case 155;}
				else {goto case 0;}
			case 155:
				if (ch == 't') {AddCh(); goto case 153;}
				else {goto case 0;}
			case 156:
				{t.kind = 47; /* float_cast */ break;}
			case 157:
				if (ch == ')') {AddCh(); goto case 161;}
				else {goto case 0;}
			case 158:
				if (ch == 'p') {AddCh(); goto case 159;}
				else {goto case 0;}
			case 159:
				if (ch == 'a') {AddCh(); goto case 160;}
				else {goto case 0;}
			case 160:
				if (ch == 'n') {AddCh(); goto case 157;}
				else {goto case 0;}
			case 161:
				{t.kind = 48; /* timespan_cast */ break;}
			case 162:
				{t.kind = 49; /* time_cast */ break;}
			case 163:
				if (ch == ')') {AddCh(); goto case 166;}
				else {goto case 0;}
			case 164:
				if (ch == 't') {AddCh(); goto case 165;}
				else {goto case 0;}
			case 165:
				if (ch == 'e') {AddCh(); goto case 163;}
				else {goto case 0;}
			case 166:
				{t.kind = 50; /* date_cast */ break;}
			case 167:
				{t.kind = 52; /* closeBracket */ break;}
			case 168:
				{t.kind = 53; /* paramDelim */ break;}
			case 169:
				if (ch >= '0' && ch <= '1') {AddCh(); goto case 198;}
				else {goto case 0;}
			case 170:
				if (ch >= '0' && ch <= '7') {AddCh(); goto case 199;}
				else {goto case 0;}
			case 171:
				recEnd = pos; recKind = 20;
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 200;}
				else if (ch == 'f') {apx++; AddCh(); goto case 94;}
				else if (ch == 'i') {apx++; AddCh(); goto case 191;}
				else if (ch == 'u') {apx++; AddCh(); goto case 192;}
				else if (ch == '.') {AddCh(); goto case 180;}
				else if (ch == 't') {apx++; AddCh(); goto case 96;}
				else if (ch == ':') {AddCh(); goto case 201;}
				else {t.kind = 20; /* dec_unlimited_int */ break;}
			case 172:
				recEnd = pos; recKind = 20;
				if (ch >= '4' && ch <= '9') {AddCh(); goto case 202;}
				else if (ch == 'f') {apx++; AddCh(); goto case 94;}
				else if (ch >= '0' && ch <= '3') {AddCh(); goto case 203;}
				else if (ch == 'i') {apx++; AddCh(); goto case 191;}
				else if (ch == 'u') {apx++; AddCh(); goto case 192;}
				else if (ch == '.') {AddCh(); goto case 180;}
				else if (ch == 't') {apx++; AddCh(); goto case 96;}
				else if (ch == ':') {AddCh(); goto case 201;}
				else {t.kind = 20; /* dec_unlimited_int */ break;}
			case 173:
				recEnd = pos; recKind = 20;
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 203;}
				else if (ch == 'f') {apx++; AddCh(); goto case 94;}
				else if (ch == 'i') {apx++; AddCh(); goto case 191;}
				else if (ch == 'u') {apx++; AddCh(); goto case 192;}
				else if (ch == '.') {AddCh(); goto case 180;}
				else if (ch == 't') {apx++; AddCh(); goto case 96;}
				else if (ch == ':') {AddCh(); goto case 201;}
				else {t.kind = 20; /* dec_unlimited_int */ break;}
			case 174:
				recEnd = pos; recKind = 38;
				if (ch == '*') {AddCh(); goto case 135;}
				else {t.kind = 38; /* multToken */ break;}
			case 175:
				recEnd = pos; recKind = 38;
				if (ch == '/' || ch == 92) {AddCh(); goto case 135;}
				else {t.kind = 38; /* multToken */ break;}
			case 176:
				recEnd = pos; recKind = 51;
				if (ch == 'i') {AddCh(); goto case 204;}
				else if (ch == 'u') {AddCh(); goto case 195;}
				else if (ch == 'f') {AddCh(); goto case 196;}
				else if (ch == 't') {AddCh(); goto case 205;}
				else if (ch == 'd') {AddCh(); goto case 197;}
				else {t.kind = 51; /* openBracket */ break;}
			case 177:
				if (ch == '4') {AddCh(); goto case 4;}
				else if (ch == '8') {AddCh(); goto case 5;}
				else if (ch == '1') {AddCh(); goto case 206;}
				else if (ch == '3') {AddCh(); goto case 7;}
				else if (ch == '6') {AddCh(); goto case 9;}
				else {goto case 0;}
			case 178:
				if (ch == '4') {AddCh(); goto case 13;}
				else if (ch == '8') {AddCh(); goto case 14;}
				else if (ch == '1') {AddCh(); goto case 207;}
				else if (ch == '3') {AddCh(); goto case 16;}
				else if (ch == '6') {AddCh(); goto case 18;}
				else {goto case 0;}
			case 179:
				recEnd = pos; recKind = 20;
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 203;}
				else if (ch == 'f') {apx++; AddCh(); goto case 94;}
				else if (ch == 'i') {apx++; AddCh(); goto case 191;}
				else if (ch == 'u') {apx++; AddCh(); goto case 192;}
				else if (ch == 'x') {AddCh(); goto case 208;}
				else if (ch == '.') {AddCh(); goto case 180;}
				else if (ch == 't') {apx++; AddCh(); goto case 96;}
				else if (ch == ':') {AddCh(); goto case 201;}
				else {t.kind = 20; /* dec_unlimited_int */ break;}
			case 180:
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 209;}
				else {goto case 0;}
			case 181:
				if (ch == 's') {AddCh(); goto case 98;}
				else if (ch == 't') {AddCh(); goto case 99;}
				else {goto case 0;}
			case 182:
				recEnd = pos; recKind = 32;
				if (ch == 't') {AddCh(); goto case 100;}
				else if (ch == 'a') {AddCh(); goto case 210;}
				else {t.kind = 32; /* ts_days_type */ break;}
			case 183:
				recEnd = pos; recKind = 29;
				if (ch == 'e') {AddCh(); goto case 211;}
				else {t.kind = 29; /* ts_seconds_type */ break;}
			case 184:
				recEnd = pos; recKind = 30;
				if (ch == 'i') {AddCh(); goto case 212;}
				else {t.kind = 30; /* ts_minutes_type */ break;}
			case 185:
				recEnd = pos; recKind = 31;
				if (ch == 'o') {AddCh(); goto case 213;}
				else if (ch == 'r') {AddCh(); goto case 214;}
				else {t.kind = 31; /* ts_hours_type */ break;}
			case 186:
				recEnd = pos; recKind = 20;
				if (ch >= '2' && ch <= '9') {AddCh(); goto case 200;}
				else if (ch == 'f') {apx++; AddCh(); goto case 94;}
				else if (ch >= '0' && ch <= '1') {AddCh(); goto case 202;}
				else if (ch == 'i') {apx++; AddCh(); goto case 191;}
				else if (ch == 'u') {apx++; AddCh(); goto case 192;}
				else if (ch == '.') {AddCh(); goto case 180;}
				else if (ch == 't') {apx++; AddCh(); goto case 96;}
				else if (ch == ':') {AddCh(); goto case 201;}
				else {t.kind = 20; /* dec_unlimited_int */ break;}
			case 187:
				if (ch == '4') {apx++; AddCh(); goto case 28;}
				else if (ch == '8') {apx++; AddCh(); goto case 29;}
				else if (ch == '1') {apx++; AddCh(); goto case 215;}
				else if (ch == '3') {apx++; AddCh(); goto case 22;}
				else if (ch == '6') {apx++; AddCh(); goto case 23;}
				else {goto case 0;}
			case 188:
				if (ch == '4') {apx++; AddCh(); goto case 34;}
				else if (ch == '8') {apx++; AddCh(); goto case 35;}
				else if (ch == '1') {apx++; AddCh(); goto case 216;}
				else if (ch == '3') {apx++; AddCh(); goto case 25;}
				else if (ch == '6') {apx++; AddCh(); goto case 26;}
				else {goto case 0;}
			case 189:
				if (ch == '4') {apx++; AddCh(); goto case 46;}
				else if (ch == '8') {apx++; AddCh(); goto case 47;}
				else if (ch == '1') {apx++; AddCh(); goto case 217;}
				else if (ch == '3') {apx++; AddCh(); goto case 40;}
				else if (ch == '6') {apx++; AddCh(); goto case 41;}
				else {goto case 0;}
			case 190:
				if (ch == '4') {apx++; AddCh(); goto case 52;}
				else if (ch == '8') {apx++; AddCh(); goto case 53;}
				else if (ch == '1') {apx++; AddCh(); goto case 218;}
				else if (ch == '3') {apx++; AddCh(); goto case 43;}
				else if (ch == '6') {apx++; AddCh(); goto case 44;}
				else {goto case 0;}
			case 191:
				if (ch == '4') {apx++; AddCh(); goto case 64;}
				else if (ch == '8') {apx++; AddCh(); goto case 65;}
				else if (ch == '1') {apx++; AddCh(); goto case 219;}
				else if (ch == '3') {apx++; AddCh(); goto case 58;}
				else if (ch == '6') {apx++; AddCh(); goto case 59;}
				else {goto case 0;}
			case 192:
				if (ch == '4') {apx++; AddCh(); goto case 70;}
				else if (ch == '8') {apx++; AddCh(); goto case 71;}
				else if (ch == '1') {apx++; AddCh(); goto case 220;}
				else if (ch == '3') {apx++; AddCh(); goto case 61;}
				else if (ch == '6') {apx++; AddCh(); goto case 62;}
				else {goto case 0;}
			case 193:
				if (ch == '4') {apx++; AddCh(); goto case 82;}
				else if (ch == '8') {apx++; AddCh(); goto case 83;}
				else if (ch == '1') {apx++; AddCh(); goto case 221;}
				else if (ch == '3') {apx++; AddCh(); goto case 76;}
				else if (ch == '6') {apx++; AddCh(); goto case 77;}
				else {goto case 0;}
			case 194:
				if (ch == '4') {apx++; AddCh(); goto case 88;}
				else if (ch == '8') {apx++; AddCh(); goto case 89;}
				else if (ch == '1') {apx++; AddCh(); goto case 222;}
				else if (ch == '3') {apx++; AddCh(); goto case 79;}
				else if (ch == '6') {apx++; AddCh(); goto case 80;}
				else {goto case 0;}
			case 195:
				if (ch == '4' || ch == '8') {AddCh(); goto case 148;}
				else if (ch == '1') {AddCh(); goto case 224;}
				else if (ch == '3') {AddCh(); goto case 149;}
				else if (ch == '6') {AddCh(); goto case 150;}
				else {goto case 0;}
			case 196:
				if (ch == ')') {AddCh(); goto case 156;}
				else if (ch == 'l') {AddCh(); goto case 225;}
				else {goto case 0;}
			case 197:
				if (ch == ')') {AddCh(); goto case 166;}
				else if (ch == 't') {AddCh(); goto case 163;}
				else if (ch == 'a') {AddCh(); goto case 164;}
				else {goto case 0;}
			case 198:
				recEnd = pos; recKind = 18;
				if (ch >= '0' && ch <= '1') {AddCh(); goto case 198;}
				else if (ch == 'i') {apx++; AddCh(); goto case 187;}
				else if (ch == 'u') {apx++; AddCh(); goto case 188;}
				else {t.kind = 18; /* bin_unlimited_int */ break;}
			case 199:
				recEnd = pos; recKind = 19;
				if (ch >= '0' && ch <= '7') {AddCh(); goto case 199;}
				else if (ch == 'i') {apx++; AddCh(); goto case 189;}
				else if (ch == 'u') {apx++; AddCh(); goto case 190;}
				else {t.kind = 19; /* oct_unlimited_int */ break;}
			case 200:
				recEnd = pos; recKind = 20;
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 226;}
				else if (ch == 'f') {apx++; AddCh(); goto case 94;}
				else if (ch == 'i') {apx++; AddCh(); goto case 191;}
				else if (ch == 'u') {apx++; AddCh(); goto case 192;}
				else if (ch == '.') {AddCh(); goto case 180;}
				else if (ch == 't') {apx++; AddCh(); goto case 96;}
				else if (ch == '-') {AddCh(); goto case 105;}
				else if (ch == '/') {AddCh(); goto case 112;}
				else {t.kind = 20; /* dec_unlimited_int */ break;}
			case 201:
				if (ch >= '0' && ch <= '5') {AddCh(); goto case 227;}
				else {goto case 0;}
			case 202:
				recEnd = pos; recKind = 20;
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 226;}
				else if (ch == 'f') {apx++; AddCh(); goto case 94;}
				else if (ch == 'i') {apx++; AddCh(); goto case 191;}
				else if (ch == 'u') {apx++; AddCh(); goto case 192;}
				else if (ch == '.') {AddCh(); goto case 180;}
				else if (ch == 't') {apx++; AddCh(); goto case 96;}
				else if (ch == '-') {AddCh(); goto case 228;}
				else if (ch == '/') {AddCh(); goto case 229;}
				else {t.kind = 20; /* dec_unlimited_int */ break;}
			case 203:
				recEnd = pos; recKind = 20;
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 226;}
				else if (ch == 'f') {apx++; AddCh(); goto case 94;}
				else if (ch == 'i') {apx++; AddCh(); goto case 191;}
				else if (ch == 'u') {apx++; AddCh(); goto case 192;}
				else if (ch == '.') {AddCh(); goto case 180;}
				else if (ch == 't') {apx++; AddCh(); goto case 96;}
				else if (ch == '-') {AddCh(); goto case 228;}
				else if (ch == '/') {AddCh(); goto case 229;}
				else if (ch == ':') {AddCh(); goto case 201;}
				else {t.kind = 20; /* dec_unlimited_int */ break;}
			case 204:
				if (ch == ')') {AddCh(); goto case 142;}
				else if (ch == '4' || ch == '8') {AddCh(); goto case 143;}
				else if (ch == '1') {AddCh(); goto case 223;}
				else if (ch == '3') {AddCh(); goto case 144;}
				else if (ch == '6') {AddCh(); goto case 145;}
				else {goto case 0;}
			case 205:
				if (ch == ')') {AddCh(); goto case 162;}
				else if (ch == 's') {AddCh(); goto case 157;}
				else if (ch == 'i') {AddCh(); goto case 230;}
				else {goto case 0;}
			case 206:
				if (ch == '6') {AddCh(); goto case 6;}
				else if (ch == '2') {AddCh(); goto case 11;}
				else {goto case 0;}
			case 207:
				if (ch == '6') {AddCh(); goto case 15;}
				else if (ch == '2') {AddCh(); goto case 20;}
				else {goto case 0;}
			case 208:
				if (ch >= '0' && ch <= '9' || ch >= 'a' && ch <= 'f') {AddCh(); goto case 231;}
				else {goto case 0;}
			case 209:
				recEnd = pos; recKind = 22;
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 209;}
				else if (ch == 't') {apx++; AddCh(); goto case 96;}
				else {t.kind = 22; /* true_float */ break;}
			case 210:
				recEnd = pos; recKind = 32;
				if (ch == 'y') {AddCh(); goto case 232;}
				else {t.kind = 32; /* ts_days_type */ break;}
			case 211:
				recEnd = pos; recKind = 29;
				if (ch == 'c') {AddCh(); goto case 233;}
				else {t.kind = 29; /* ts_seconds_type */ break;}
			case 212:
				recEnd = pos; recKind = 30;
				if (ch == 'n') {AddCh(); goto case 234;}
				else {t.kind = 30; /* ts_minutes_type */ break;}
			case 213:
				recEnd = pos; recKind = 31;
				if (ch == 'u') {AddCh(); goto case 235;}
				else {t.kind = 31; /* ts_hours_type */ break;}
			case 214:
				recEnd = pos; recKind = 31;
				if (ch == 's') {AddCh(); goto case 103;}
				else {t.kind = 31; /* ts_hours_type */ break;}
			case 215:
				if (ch == '6') {apx++; AddCh(); goto case 30;}
				else if (ch == '2') {apx++; AddCh(); goto case 24;}
				else {goto case 0;}
			case 216:
				if (ch == '6') {apx++; AddCh(); goto case 36;}
				else if (ch == '2') {apx++; AddCh(); goto case 27;}
				else {goto case 0;}
			case 217:
				if (ch == '6') {apx++; AddCh(); goto case 48;}
				else if (ch == '2') {apx++; AddCh(); goto case 42;}
				else {goto case 0;}
			case 218:
				if (ch == '6') {apx++; AddCh(); goto case 54;}
				else if (ch == '2') {apx++; AddCh(); goto case 45;}
				else {goto case 0;}
			case 219:
				if (ch == '6') {apx++; AddCh(); goto case 66;}
				else if (ch == '2') {apx++; AddCh(); goto case 60;}
				else {goto case 0;}
			case 220:
				if (ch == '6') {apx++; AddCh(); goto case 72;}
				else if (ch == '2') {apx++; AddCh(); goto case 63;}
				else {goto case 0;}
			case 221:
				if (ch == '6') {apx++; AddCh(); goto case 84;}
				else if (ch == '2') {apx++; AddCh(); goto case 78;}
				else {goto case 0;}
			case 222:
				if (ch == '6') {apx++; AddCh(); goto case 90;}
				else if (ch == '2') {apx++; AddCh(); goto case 81;}
				else {goto case 0;}
			case 223:
				if (ch == '6') {AddCh(); goto case 143;}
				else if (ch == '2') {AddCh(); goto case 146;}
				else {goto case 0;}
			case 224:
				if (ch == '6') {AddCh(); goto case 148;}
				else if (ch == '2') {AddCh(); goto case 151;}
				else {goto case 0;}
			case 225:
				if (ch == ')') {AddCh(); goto case 156;}
				else if (ch == 't') {AddCh(); goto case 153;}
				else if (ch == 'o') {AddCh(); goto case 154;}
				else {goto case 0;}
			case 226:
				recEnd = pos; recKind = 20;
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 236;}
				else if (ch == 'f') {apx++; AddCh(); goto case 94;}
				else if (ch == 'i') {apx++; AddCh(); goto case 191;}
				else if (ch == 'u') {apx++; AddCh(); goto case 192;}
				else if (ch == '.') {AddCh(); goto case 180;}
				else if (ch == 't') {apx++; AddCh(); goto case 96;}
				else {t.kind = 20; /* dec_unlimited_int */ break;}
			case 227:
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 237;}
				else {goto case 0;}
			case 228:
				if (ch == '0') {AddCh(); goto case 238;}
				else if (ch == '1') {AddCh(); goto case 239;}
				else {goto case 0;}
			case 229:
				if (ch == '0') {AddCh(); goto case 240;}
				else if (ch == '1') {AddCh(); goto case 241;}
				else {goto case 0;}
			case 230:
				if (ch == ')') {AddCh(); goto case 162;}
				else if (ch == 'm') {AddCh(); goto case 242;}
				else {goto case 0;}
			case 231:
				recEnd = pos; recKind = 21;
				if (ch >= '0' && ch <= '9' || ch >= 'a' && ch <= 'f') {AddCh(); goto case 231;}
				else if (ch == 'i') {apx++; AddCh(); goto case 193;}
				else if (ch == 'u') {apx++; AddCh(); goto case 194;}
				else {t.kind = 21; /* hex_unlimited_int */ break;}
			case 232:
				recEnd = pos; recKind = 32;
				if (ch == 's') {AddCh(); goto case 104;}
				else {t.kind = 32; /* ts_days_type */ break;}
			case 233:
				recEnd = pos; recKind = 29;
				if (ch == 's') {AddCh(); goto case 101;}
				else if (ch == 'o') {AddCh(); goto case 243;}
				else {t.kind = 29; /* ts_seconds_type */ break;}
			case 234:
				recEnd = pos; recKind = 30;
				if (ch == 's') {AddCh(); goto case 102;}
				else if (ch == 'u') {AddCh(); goto case 244;}
				else {t.kind = 30; /* ts_minutes_type */ break;}
			case 235:
				recEnd = pos; recKind = 31;
				if (ch == 'r') {AddCh(); goto case 245;}
				else {t.kind = 31; /* ts_hours_type */ break;}
			case 236:
				recEnd = pos; recKind = 20;
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 246;}
				else if (ch == 'f') {apx++; AddCh(); goto case 94;}
				else if (ch == 'i') {apx++; AddCh(); goto case 191;}
				else if (ch == 'u') {apx++; AddCh(); goto case 192;}
				else if (ch == '.') {AddCh(); goto case 180;}
				else if (ch == 't') {apx++; AddCh(); goto case 96;}
				else if (ch == '-') {AddCh(); goto case 105;}
				else if (ch == '/') {AddCh(); goto case 112;}
				else {t.kind = 20; /* dec_unlimited_int */ break;}
			case 237:
				recEnd = pos; recKind = 35;
				if (ch == ':') {AddCh(); goto case 127;}
				else {t.kind = 35; /* time */ break;}
			case 238:
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 247;}
				else {goto case 0;}
			case 239:
				if (ch >= '0' && ch <= '2') {AddCh(); goto case 247;}
				else {goto case 0;}
			case 240:
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 248;}
				else {goto case 0;}
			case 241:
				if (ch >= '0' && ch <= '2') {AddCh(); goto case 248;}
				else {goto case 0;}
			case 242:
				if (ch == 'e') {AddCh(); goto case 249;}
				else {goto case 0;}
			case 243:
				recEnd = pos; recKind = 29;
				if (ch == 'n') {AddCh(); goto case 250;}
				else {t.kind = 29; /* ts_seconds_type */ break;}
			case 244:
				recEnd = pos; recKind = 30;
				if (ch == 't') {AddCh(); goto case 251;}
				else {t.kind = 30; /* ts_minutes_type */ break;}
			case 245:
				recEnd = pos; recKind = 31;
				if (ch == 's') {AddCh(); goto case 103;}
				else {t.kind = 31; /* ts_hours_type */ break;}
			case 246:
				recEnd = pos; recKind = 20;
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 246;}
				else if (ch == 'f') {apx++; AddCh(); goto case 94;}
				else if (ch == 'i') {apx++; AddCh(); goto case 191;}
				else if (ch == 'u') {apx++; AddCh(); goto case 192;}
				else if (ch == '.') {AddCh(); goto case 180;}
				else if (ch == 't') {apx++; AddCh(); goto case 96;}
				else {t.kind = 20; /* dec_unlimited_int */ break;}
			case 247:
				if (ch == '-') {AddCh(); goto case 252;}
				else {goto case 0;}
			case 248:
				if (ch == '/') {AddCh(); goto case 253;}
				else {goto case 0;}
			case 249:
				if (ch == ')') {AddCh(); goto case 162;}
				else if (ch == 's') {AddCh(); goto case 158;}
				else {goto case 0;}
			case 250:
				recEnd = pos; recKind = 29;
				if (ch == 'd') {AddCh(); goto case 254;}
				else {t.kind = 29; /* ts_seconds_type */ break;}
			case 251:
				recEnd = pos; recKind = 30;
				if (ch == 'e') {AddCh(); goto case 255;}
				else {t.kind = 30; /* ts_minutes_type */ break;}
			case 252:
				if (ch >= '0' && ch <= '2') {AddCh(); goto case 256;}
				else if (ch >= '4' && ch <= '9') {AddCh(); goto case 120;}
				else if (ch == '3') {AddCh(); goto case 257;}
				else {goto case 0;}
			case 253:
				if (ch >= '0' && ch <= '2') {AddCh(); goto case 258;}
				else if (ch >= '4' && ch <= '9') {AddCh(); goto case 123;}
				else if (ch == '3') {AddCh(); goto case 259;}
				else {goto case 0;}
			case 254:
				recEnd = pos; recKind = 29;
				if (ch == 's') {AddCh(); goto case 101;}
				else {t.kind = 29; /* ts_seconds_type */ break;}
			case 255:
				recEnd = pos; recKind = 30;
				if (ch == 's') {AddCh(); goto case 102;}
				else {t.kind = 30; /* ts_minutes_type */ break;}
			case 256:
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 260;}
				else {goto case 0;}
			case 257:
				if (ch >= '0' && ch <= '1') {AddCh(); goto case 261;}
				else if (ch >= '2' && ch <= '9') {AddCh(); goto case 121;}
				else {goto case 0;}
			case 258:
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 262;}
				else {goto case 0;}
			case 259:
				if (ch >= '0' && ch <= '1') {AddCh(); goto case 263;}
				else if (ch >= '2' && ch <= '9') {AddCh(); goto case 124;}
				else {goto case 0;}
			case 260:
				recEnd = pos; recKind = 33;
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 122;}
				else {t.kind = 33; /* date */ break;}
			case 261:
				recEnd = pos; recKind = 33;
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 122;}
				else {t.kind = 33; /* date */ break;}
			case 262:
				recEnd = pos; recKind = 33;
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 125;}
				else {t.kind = 33; /* date */ break;}
			case 263:
				recEnd = pos; recKind = 33;
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 125;}
				else {t.kind = 33; /* date */ break;}

		}
		t.val = new String(tval, 0, tlen);
		return t;
	}
}

/*
This is free and unencumbered software released into the public domain.

Anyone is free to copy, modify, publish, use, compile, sell, or
distribute this software, either in source code form or as a compiled
binary, for any purpose, commercial or non-commercial, and by any
means.

In jurisdictions that recognize copyright laws, the author or authors
of this software dedicate any and all copyright interest in the
software to the public domain. We make this dedication for the benefit
of the public at large and to the detriment of our heirs and
successors. We intend this dedication to be an overt act of
relinquishment in perpetuity of all present and future rights to this
software under copyright law.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.

For more information, please refer to <http://unlicense.org>
*/}