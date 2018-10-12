// Generated on: 2018-10-10 14:53:26

using System.IO;
using System.Collections.Generic;



using System;

/*
    Generated portion of the Parser

        Generated from Parser.frame and HackerCalc.atg

    Keith Fletcher
    Oct 2018

    This file is Unlicensed.
    See the foot of the file, or refer to <http://unlicense.org>
*/

namespace HisRoyalRedness.com {



public partial class Parser {
	public const int _EOF = 0;
	public const int _func_name = 1;
	public const int _i4 = 2;
	public const int _i8 = 3;
	public const int _i16 = 4;
	public const int _i32 = 5;
	public const int _i64 = 6;
	public const int _i128 = 7;
	public const int _u4 = 8;
	public const int _u8 = 9;
	public const int _u16 = 10;
	public const int _u32 = 11;
	public const int _u64 = 12;
	public const int _u128 = 13;
	public const int _bin_limited_int = 14;
	public const int _oct_limited_int = 15;
	public const int _dec_limited_int = 16;
	public const int _hex_limited_int = 17;
	public const int _bin_unlimited_int = 18;
	public const int _oct_unlimited_int = 19;
	public const int _dec_unlimited_int = 20;
	public const int _hex_unlimited_int = 21;
	public const int _true_float = 22;
	public const int _typed_float = 23;
	public const int _float_type = 24;
	public const int _typed_ts_seconds = 25;
	public const int _timespan_type = 26;
	public const int _time_type = 27;
	public const int _date_type = 28;
	public const int _ts_seconds_type = 29;
	public const int _ts_minutes_type = 30;
	public const int _ts_hours_type = 31;
	public const int _ts_days_type = 32;
	public const int _date = 33;
	public const int _date_rev = 34;
	public const int _time = 35;
	public const int _now = 36;
	public const int _notToken = 37;
	public const int _multToken = 38;
	public const int _exponentToken = 39;
	public const int _addToken = 40;
	public const int _subToken = 41;
	public const int _shiftToken = 42;
	public const int _bitToken = 43;
	public const int _unlimited_cast = 44;
	public const int _signed_cast = 45;
	public const int _unsigned_cast = 46;
	public const int _float_cast = 47;
	public const int _timespan_cast = 48;
	public const int _time_cast = 49;
	public const int _date_cast = 50;
	public const int _openBracket = 51;
	public const int _closeBracket = 52;
	public const int _paramDelim = 53;

	public enum TokenKinds {
		_EOF = 0,
		_func_name = 1,
		_i4 = 2,
		_i8 = 3,
		_i16 = 4,
		_i32 = 5,
		_i64 = 6,
		_i128 = 7,
		_u4 = 8,
		_u8 = 9,
		_u16 = 10,
		_u32 = 11,
		_u64 = 12,
		_u128 = 13,
		_bin_limited_int = 14,
		_oct_limited_int = 15,
		_dec_limited_int = 16,
		_hex_limited_int = 17,
		_bin_unlimited_int = 18,
		_oct_unlimited_int = 19,
		_dec_unlimited_int = 20,
		_hex_unlimited_int = 21,
		_true_float = 22,
		_typed_float = 23,
		_float_type = 24,
		_typed_ts_seconds = 25,
		_timespan_type = 26,
		_time_type = 27,
		_date_type = 28,
		_ts_seconds_type = 29,
		_ts_minutes_type = 30,
		_ts_hours_type = 31,
		_ts_days_type = 32,
		_date = 33,
		_date_rev = 34,
		_time = 35,
		_now = 36,
		_notToken = 37,
		_multToken = 38,
		_exponentToken = 39,
		_addToken = 40,
		_subToken = 41,
		_shiftToken = 42,
		_bitToken = 43,
		_unlimited_cast = 44,
		_signed_cast = 45,
		_unsigned_cast = 46,
		_float_cast = 47,
		_timespan_cast = 48,
		_time_cast = 49,
		_date_cast = 50,
		_openBracket = 51,
		_closeBracket = 52,
		_paramDelim = 53,
	}

	public const int maxT = 54;

	const bool _T = true;
	const bool _x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;



	public Parser(Scanner scanner) {
		this.scanner = scanner;
		errors = new Errors();
	}

	void SynErr (int n) {
		if (errDist >= minErrDist) errors.SynErr(la.line, la.col, n);
		errDist = 0;
	}

	public void SemErr (string msg) {
		if (errDist >= minErrDist) errors.SemErr(t.line, t.col, msg);
		errDist = 0;
	}
	
	void Get () {
		for (;;) {
			t = la;
			la = scanner.Scan();
			if (la.kind <= maxT) { ++errDist; break; }

			la = t;
		}
	}
	
	void Expect (int n) {
		if (la.kind==n) Get(); else { SynErr(n); }
	}
	
	bool StartOf (int s) {
		return set[s, la.kind];
	}
	
	void ExpectWeak (int n, int follow) {
		if (la.kind == n) Get();
		else {
			SynErr(n);
			while (!StartOf(follow)) Get();
		}
	}


	bool WeakSeparator(int n, int syFol, int repFol) {
		int kind = la.kind;
		if (kind == n) {Get(); return true;}
		else if (StartOf(repFol)) {return false;}
		else {
			SynErr(n);
			while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind])) {
				Get();
				kind = la.kind;
			}
			return StartOf(syFol);
		}
	}

	
	void HackerCalc() {
		IToken token = null; 
		Expr(out token);
		RootToken = token; 
	}

	void Expr(out IToken token) {
		BitExpr(out token);
	}

	void BitExpr(out IToken token) {
		IToken tk = null; OperatorToken op = null; 
		ShiftExpr(out tk);
		while (la.kind == 43 /* bitToken */ ) {
			Get();
			if (op == null) { op = OperatorToken.Parse(t.val).Tap(o => o.Left = tk); } 
			else { op = OperatorToken.Parse(t.val).Tap(o => o.Left = op); } 
			ShiftExpr(out tk);
			op.Right = tk; 
		}
		token = (IToken)op ?? (IToken)tk; 
	}

	void ShiftExpr(out IToken token) {
		IToken tk = null; OperatorToken op = null; 
		AddExpr(out tk);
		while (la.kind == 42 /* shiftToken */ ) {
			Get();
			if (op == null) { op = OperatorToken.Parse(t.val).Tap(o => o.Left = tk); } 
			else { op = OperatorToken.Parse(t.val).Tap(o => o.Left = op); } 
			AddExpr(out tk);
			op.Right = tk; 
		}
		token = (IToken)op ?? (IToken)tk; 
	}

	void AddExpr(out IToken token) {
		IToken tk = null; OperatorToken op = null; 
		MulExpr(out tk);
		while (la.kind == 40 /* addToken */  || la.kind == 41 /* subToken */ ) {
			if (la.kind == 40 /* addToken */ ) {
				Get();
			} else {
				Get();
			}
			if (op == null) { op = OperatorToken.Parse(t.val).Tap(o => o.Left = tk); } 
			else { op = OperatorToken.Parse(t.val).Tap(o => o.Left = op); } 
			MulExpr(out tk);
			op.Right = tk; 
		}
		token = (IToken)op ?? (IToken)tk; 
	}

	void MulExpr(out IToken token) {
		IToken tk = null; OperatorToken op = null; 
		ExponentExpr(out tk);
		while (la.kind == 38 /* multToken */ ) {
			Get();
			if (op == null) { op = OperatorToken.Parse(t.val).Tap(o => o.Left = tk); } 
			else { op = OperatorToken.Parse(t.val).Tap(o => o.Left = op); } 
			ExponentExpr(out tk);
			op.Right = tk; 
		}
		token = (IToken)op ?? (IToken)tk; 
	}

	void ExponentExpr(out IToken token) {
		IToken tk = null; OperatorToken op = null; 
		NotExpr(out tk);
		while (la.kind == 39 /* exponentToken */ ) {
			Get();
			if (op == null) { op = OperatorToken.Parse(t.val).Tap(o => o.Left = tk); } 
			else { op = OperatorToken.Parse(t.val).Tap(o => o.Left = op); } 
			NotExpr(out tk);
			op.Right = tk; 
		}
		token = (IToken)op ?? (IToken)tk; 
	}

	void NotExpr(out IToken token) {
		IToken tk = null; OperatorToken op = null; 
		if (la.kind == 37 /* notToken */ ) {
			Get();
			op = OperatorToken.ParseNegate(t.val); 
		}
		Func(out tk);
		token = op == null ? tk : op.Tap(o => o.Left = tk); 
	}

	void Func(out IToken token) {
		token = null; IToken tk = null; 
		if (la.kind == 1 /* func_name */ ) {
			Get();
			var name = t.val; 
			Expect(51); /* openBracket */
			var parameters = new List<IToken>(); 
			Expr(out tk);
			parameters.Add(tk); 
			while (la.kind == 53 /* paramDelim */ ) {
				Get();
				Expr(out tk);
				parameters.Add(tk); 
			}
			Expect(52); /* closeBracket */
			token = FunctionToken.Parse(name, parameters); 
		} else if (StartOf(1)) {
			Term(out token);
		} else SynErr(55);
	}

	void Term(out IToken token) {
		token = null; 
		if (IsPartialEquation()) {
			Expect(0); /* EOF */
		} else if (IsBracket()) {
			Bracket(out token);
		} else if (IsTimespanNumber()) {
			Timespan(out token);
		} else if (IsDateTime()) {
			DateTime(out token);
		} else if (la.kind == 33 /* date */  || la.kind == 34 /* date_rev */ ) {
			Date(out token);
		} else if (la.kind == 35 /* time */ ) {
			Time(out token);
		} else if (StartOf(2)) {
			Numeric(out token);
		} else if (la.kind == 36 /* now */ ) {
			Now(out token);
		} else SynErr(56);
	}

	void Bracket(out IToken token) {
		IToken tk = null; OperatorToken op = null; 
		if (la.kind == 41 /* subToken */ ) {
			Get();
			op = OperatorToken.ParseNegate(t.val); 
		}
		Expect(51); /* openBracket */
		Expr(out tk);
		IToken grp = new GroupingToken(tk); 
		Expect(52); /* closeBracket */
		token = op == null ? grp : ((OperatorToken)op).Tap(o => o.Left = grp); 
	}

	void Timespan(out IToken token) {
		TimespanToken tk = null; 
		CompoundTimespanPortion(out tk);
		token = tk; 
	}

	void DateTime(out IToken token) {
		DateToken dt = null; 
		if (la.kind == 33 /* date */ ) {
			Get();
			dt = DateToken.Parse(t.val); 
		} else if (la.kind == 34 /* date_rev */ ) {
			Get();
			dt = DateToken.Parse(t.val, true); 
		} else SynErr(57);
		Expect(35); /* time */
		token = DateToken.CreateDateTime(dt, TimeToken.Parse(t.val)); 
	}

	void Date(out IToken token) {
		token = null; 
		if (la.kind == 33 /* date */ ) {
			Get();
			token = DateToken.Parse(t.val); 
		} else if (la.kind == 34 /* date_rev */ ) {
			Get();
			token = DateToken.Parse(t.val, true); 
		} else SynErr(58);
	}

	void Time(out IToken token) {
		token = null; 
		Expect(35); /* time */
		token = TimeToken.Parse(t.val); 
	}

	void Numeric(out IToken token) {
		token = null; bool isNeg = false; 
		if (la.kind == 41 /* subToken */ ) {
			Get();
			isNeg = true; 
		}
		if (la.kind == 22 /* true_float */  || la.kind == 23 /* typed_float */ ) {
			Float(out token, ref isNeg);
		} else if (StartOf(3)) {
			Integer(out token, ref isNeg);
		} else SynErr(59);
	}

	void Now(out IToken token) {
		Expect(36); /* now */
		token = new DateToken(); 
	}

	void Float(out IToken token, ref bool isNeg) {
		if (la.kind == 22 /* true_float */ ) {
			Get();
		} else if (la.kind == 23 /* typed_float */ ) {
			Get();
		} else SynErr(60);
		var fltVal = t.val; var rawToken = t.val; 
		if (la.kind == 24 /* float_type */ ) {
			Get();
			rawToken += t.val; 
		}
		token = FloatToken.Parse(fltVal, isNeg, rawToken); 
	}

	void Integer(out IToken token, ref bool isNeg) {
		token = null; 
		if (StartOf(4)) {
			UnlimitedInteger(out token, ref isNeg);
		} else if (StartOf(5)) {
			LimitedInteger(out token, ref isNeg);
		} else SynErr(61);
	}

	void UnlimitedInteger(out IToken token, ref bool isNeg) {
		var intBase = IntegerBase.Decimal; 
		if (la.kind == 20 /* dec_unlimited_int */ ) {
			Get();
		} else if (la.kind == 21 /* hex_unlimited_int */ ) {
			Get();
			intBase = IntegerBase.Hexadecimal; 
		} else if (la.kind == 18 /* bin_unlimited_int */ ) {
			Get();
			intBase = IntegerBase.Binary; 
		} else if (la.kind == 19 /* oct_unlimited_int */ ) {
			Get();
			intBase = IntegerBase.Octal; 
		} else SynErr(62);
		token = UnlimitedIntegerToken.Parse(t.val, intBase, isNeg); 
	}

	void LimitedInteger(out IToken token, ref bool isNeg) {
		var intBase = IntegerBase.Decimal; 
		if (la.kind == 16 /* dec_limited_int */ ) {
			Get();
		} else if (la.kind == 17 /* hex_limited_int */ ) {
			Get();
			intBase = IntegerBase.Hexadecimal; 
		} else if (la.kind == 14 /* bin_limited_int */ ) {
			Get();
			intBase = IntegerBase.Binary; 
		} else if (la.kind == 15 /* oct_limited_int */ ) {
			Get();
			intBase = IntegerBase.Octal; 
		} else SynErr(63);
		var intVal = t.val; var isSigned = true; var signAndBitWidth = ""; 
		if (StartOf(6)) {
			switch (la.kind) {
			case 2: /* i4*/{
				Get();
				break;
			}
			case 3: /* i8*/{
				Get();
				break;
			}
			case 4: /* i16*/{
				Get();
				break;
			}
			case 5: /* i32*/{
				Get();
				break;
			}
			case 6: /* i64*/{
				Get();
				break;
			}
			case 7: /* i128*/{
				Get();
				break;
			}
			}
			signAndBitWidth = t.val; 
		} else if (StartOf(7)) {
			switch (la.kind) {
			case 8: /* u4*/{
				Get();
				break;
			}
			case 9: /* u8*/{
				Get();
				break;
			}
			case 10: /* u16*/{
				Get();
				break;
			}
			case 11: /* u32*/{
				Get();
				break;
			}
			case 12: /* u64*/{
				Get();
				break;
			}
			case 13: /* u128*/{
				Get();
				break;
			}
			}
			signAndBitWidth = t.val; isSigned = false; 
		} else SynErr(64);
		var bitWidth = LimitedIntegerToken.ParseBitWidth(signAndBitWidth.Substring(1)); 
		var rawToken = intVal + signAndBitWidth; 
		token = LimitedIntegerToken.Parse(intVal, intBase, bitWidth, isSigned, isNeg, rawToken); 
	}

	void CompoundTimespanPortion(out TimespanToken token) {
		token = new TimespanToken(); 
		TimespanToken tk = null; 
		if (IsTimespanDays()) {
			TimespanDays(out tk);
			token = token.AddCompoundPortions(tk); 
			if (la.kind == 20 /* dec_unlimited_int */  || la.kind == 22 /* true_float */  || la.kind == 25 /* typed_ts_seconds */ ) {
				if (IsTimespanHours()) {
					TimespanHours(out tk);
					token = token.AddCompoundPortions(tk); 
					if (la.kind == 20 /* dec_unlimited_int */  || la.kind == 22 /* true_float */  || la.kind == 25 /* typed_ts_seconds */ ) {
						if (IsTimespanMinutes()) {
							TimespanMinutes(out tk);
							token = token.AddCompoundPortions(tk); 
							if (la.kind == 20 /* dec_unlimited_int */  || la.kind == 22 /* true_float */  || la.kind == 25 /* typed_ts_seconds */ ) {
								TimespanSeconds(out tk);
								token = token.AddCompoundPortions(tk); 
							}
						} else {
							TimespanSeconds(out tk);
							token = token.AddCompoundPortions(tk); 
						}
					}
				} else if (IsTimespanMinutes()) {
					TimespanMinutes(out tk);
					token = token.AddCompoundPortions(tk); 
					if (la.kind == 20 /* dec_unlimited_int */  || la.kind == 22 /* true_float */  || la.kind == 25 /* typed_ts_seconds */ ) {
						TimespanSeconds(out tk);
						token = token.AddCompoundPortions(tk); 
					}
				} else {
					TimespanSeconds(out tk);
					token = token.AddCompoundPortions(tk); 
				}
			}
		} else if (IsTimespanHours()) {
			TimespanHours(out tk);
			token = token.AddCompoundPortions(tk); 
			if (la.kind == 20 /* dec_unlimited_int */  || la.kind == 22 /* true_float */  || la.kind == 25 /* typed_ts_seconds */ ) {
				if (IsTimespanMinutes()) {
					TimespanMinutes(out tk);
					token = token.AddCompoundPortions(tk); 
					if (la.kind == 20 /* dec_unlimited_int */  || la.kind == 22 /* true_float */  || la.kind == 25 /* typed_ts_seconds */ ) {
						TimespanSeconds(out tk);
						token = token.AddCompoundPortions(tk); 
					}
				} else {
					TimespanSeconds(out tk);
					token = token.AddCompoundPortions(tk); 
				}
			}
		} else if (IsTimespanMinutes()) {
			TimespanMinutes(out tk);
			token = token.AddCompoundPortions(tk); 
			if (la.kind == 20 /* dec_unlimited_int */  || la.kind == 22 /* true_float */  || la.kind == 25 /* typed_ts_seconds */ ) {
				TimespanSeconds(out tk);
				token = token.AddCompoundPortions(tk); 
			}
		} else if (la.kind == 20 /* dec_unlimited_int */  || la.kind == 22 /* true_float */  || la.kind == 25 /* typed_ts_seconds */ ) {
			TimespanSeconds(out tk);
			token = token.AddCompoundPortions(tk); 
		} else SynErr(65);
	}

	void TimespanDays(out TimespanToken token) {
		token = null; 
		var days = ""; var tsType = ""; 
		if (la.kind == 22 /* true_float */ ) {
			Get();
			days = t.val; 
			Expect(32); /* ts_days_type */
			tsType = t.val; 
		} else if (la.kind == 20 /* dec_unlimited_int */ ) {
			Get();
			days = t.val; 
			Expect(32); /* ts_days_type */
			tsType = t.val; 
		} else SynErr(66);
		token = TimespanToken.Parse(TimeSpan.FromDays(double.Parse(days)), $"{days} {tsType}"); 
	}

	void TimespanHours(out TimespanToken token) {
		token = null; 
		var hours = ""; var tsType = ""; 
		if (la.kind == 22 /* true_float */ ) {
			Get();
			hours = t.val; 
			Expect(31); /* ts_hours_type */
			tsType = t.val; 
		} else if (la.kind == 20 /* dec_unlimited_int */ ) {
			Get();
			hours = t.val; 
			Expect(31); /* ts_hours_type */
			tsType = t.val; 
		} else SynErr(67);
		token = TimespanToken.Parse(TimeSpan.FromHours(double.Parse(hours)), $"{hours} {tsType}"); 
	}

	void TimespanMinutes(out TimespanToken token) {
		token = null; 
		var minutes = ""; var tsType = ""; 
		if (la.kind == 22 /* true_float */ ) {
			Get();
			minutes = t.val; 
			Expect(30); /* ts_minutes_type */
			tsType = t.val; 
		} else if (la.kind == 20 /* dec_unlimited_int */ ) {
			Get();
			minutes = t.val; 
			Expect(30); /* ts_minutes_type */
			tsType = t.val; 
		} else SynErr(68);
		token = TimespanToken.Parse(TimeSpan.FromMinutes(double.Parse(minutes)), $"{minutes} {tsType}"); 
	}

	void TimespanSeconds(out TimespanToken token) {
		var seconds = ""; var tsType = ""; 
		if (la.kind == 25 /* typed_ts_seconds */ ) {
			Get();
			seconds = t.val; 
			Expect(26); /* timespan_type */
			tsType = t.val; 
		} else if (la.kind == 22 /* true_float */ ) {
			Get();
			seconds = t.val; 
			Expect(29); /* ts_seconds_type */
			tsType = t.val; 
		} else if (la.kind == 20 /* dec_unlimited_int */ ) {
			Get();
			seconds = t.val; 
			Expect(29); /* ts_seconds_type */
			tsType = t.val; 
		} else SynErr(69);
		token = TimespanToken.Parse(TimeSpan.FromSeconds(double.Parse(seconds)), $"{seconds} {tsType}"); 
	}



	public bool Parse() {
		la = new Token();
		la.val = "";		
		Get();
		HackerCalc();
		Expect(0);

		return errors.count == 0;
	}
	
	static readonly bool[,] set = {
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x},
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _x,_T,_x,_x, _x,_x,_x,_x, _x,_T,_T,_T, _T,_x,_x,_x, _x,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _x,_x,_x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_T,_T, _T,_T,_T,_T, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_T,_T, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x},
		{_x,_x,_T,_T, _T,_T,_T,_T, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _T,_T,_T,_T, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x}

	};
}

#region Errors
public partial class Errors {
	public int count = 0;                                    // number of errors detected
	public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text

	public virtual void SynErr (int line, int col, int n) {
		string s;
		switch (n) {
			case 0: s = "EOF expected"; break;
			case 1: s = "func_name expected"; break;
			case 2: s = "i4 expected"; break;
			case 3: s = "i8 expected"; break;
			case 4: s = "i16 expected"; break;
			case 5: s = "i32 expected"; break;
			case 6: s = "i64 expected"; break;
			case 7: s = "i128 expected"; break;
			case 8: s = "u4 expected"; break;
			case 9: s = "u8 expected"; break;
			case 10: s = "u16 expected"; break;
			case 11: s = "u32 expected"; break;
			case 12: s = "u64 expected"; break;
			case 13: s = "u128 expected"; break;
			case 14: s = "bin_limited_int expected"; break;
			case 15: s = "oct_limited_int expected"; break;
			case 16: s = "dec_limited_int expected"; break;
			case 17: s = "hex_limited_int expected"; break;
			case 18: s = "bin_unlimited_int expected"; break;
			case 19: s = "oct_unlimited_int expected"; break;
			case 20: s = "dec_unlimited_int expected"; break;
			case 21: s = "hex_unlimited_int expected"; break;
			case 22: s = "true_float expected"; break;
			case 23: s = "typed_float expected"; break;
			case 24: s = "float_type expected"; break;
			case 25: s = "typed_ts_seconds expected"; break;
			case 26: s = "timespan_type expected"; break;
			case 27: s = "time_type expected"; break;
			case 28: s = "date_type expected"; break;
			case 29: s = "ts_seconds_type expected"; break;
			case 30: s = "ts_minutes_type expected"; break;
			case 31: s = "ts_hours_type expected"; break;
			case 32: s = "ts_days_type expected"; break;
			case 33: s = "date expected"; break;
			case 34: s = "date_rev expected"; break;
			case 35: s = "time expected"; break;
			case 36: s = "now expected"; break;
			case 37: s = "notToken expected"; break;
			case 38: s = "multToken expected"; break;
			case 39: s = "exponentToken expected"; break;
			case 40: s = "addToken expected"; break;
			case 41: s = "subToken expected"; break;
			case 42: s = "shiftToken expected"; break;
			case 43: s = "bitToken expected"; break;
			case 44: s = "unlimited_cast expected"; break;
			case 45: s = "signed_cast expected"; break;
			case 46: s = "unsigned_cast expected"; break;
			case 47: s = "float_cast expected"; break;
			case 48: s = "timespan_cast expected"; break;
			case 49: s = "time_cast expected"; break;
			case 50: s = "date_cast expected"; break;
			case 51: s = "openBracket expected"; break;
			case 52: s = "closeBracket expected"; break;
			case 53: s = "paramDelim expected"; break;
			case 54: s = "??? expected"; break;
			case 55: s = "invalid Func"; break;
			case 56: s = "invalid Term"; break;
			case 57: s = "invalid DateTime"; break;
			case 58: s = "invalid Date"; break;
			case 59: s = "invalid Numeric"; break;
			case 60: s = "invalid Float"; break;
			case 61: s = "invalid Integer"; break;
			case 62: s = "invalid UnlimitedInteger"; break;
			case 63: s = "invalid LimitedInteger"; break;
			case 64: s = "invalid LimitedInteger"; break;
			case 65: s = "invalid CompoundTimespanPortion"; break;
			case 66: s = "invalid TimespanDays"; break;
			case 67: s = "invalid TimespanHours"; break;
			case 68: s = "invalid TimespanMinutes"; break;
			case 69: s = "invalid TimespanSeconds"; break;

			default: s = "error " + n; break;
		}
		WriteLine(errMsgFormat, line, col, s);
		count++;
	}
}
#endregion Errors


public class FatalError: Exception {
	public FatalError(string m): base(m) {}
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