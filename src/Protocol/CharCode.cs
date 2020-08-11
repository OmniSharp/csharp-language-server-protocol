namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    internal static class CharCode
    {
        public const int Null = 0;

        /// <summary>
        /// The `\t` character.
        /// </summary>
        public const int Tab = 9;

        /// <summary>
        /// The `\n` character.
        /// </summary>
        public const int LineFeed = 10;

        /// <summary>
        /// The `\r` character.
        /// </summary>
        public const int CarriageReturn = 13;

        public const int Space = 32;

        /// <summary>
        /// The `!` character.
        /// </summary>
        public const int ExclamationMark = 33;

        /// <summary>
        /// The `"` character.
        /// </summary>
        public const int DoubleQuote = 34;

        /// <summary>
        /// The `#` character.
        /// </summary>
        public const int Hash = 35;

        /// <summary>
        /// The `$` character.
        /// </summary>
        public const int DollarSign = 36;

        /// <summary>
        /// The `%` character.
        /// </summary>
        public const int PercentSign = 37;

        /// <summary>
        /// The `&` character.
        /// </summary>
        public const int Ampersand = 38;

        /// <summary>
        /// The `'` character.
        /// </summary>
        public const int SingleQuote = 39;

        /// <summary>
        /// The `(` character.
        /// </summary>
        public const int OpenParen = 40;

        /// <summary>
        /// The `)` character.
        /// </summary>
        public const int CloseParen = 41;

        /// <summary>
        /// The `*` character.
        /// </summary>
        public const int Asterisk = 42;

        /// <summary>
        /// The `+` character.
        /// </summary>
        public const int Plus = 43;

        /// <summary>
        /// The `,` character.
        /// </summary>
        public const int Comma = 44;

        /// <summary>
        /// The `-` character.
        /// </summary>
        public const int Dash = 45;

        /// <summary>
        /// The `.` character.
        /// </summary>
        public const int Period = 46;

        /// <summary>
        /// The `/` character.
        /// </summary>
        public const int Slash = 47;

        public const int Digit0 = 48;
        public const int Digit1 = 49;
        public const int Digit2 = 50;
        public const int Digit3 = 51;
        public const int Digit4 = 52;
        public const int Digit5 = 53;
        public const int Digit6 = 54;
        public const int Digit7 = 55;
        public const int Digit8 = 56;
        public const int Digit9 = 57;

        /// <summary>
        /// The `:` character.
        /// </summary>
        public const int Colon = 58;

        /// <summary>
        /// The `;` character.
        /// </summary>
        public const int Semicolon = 59;

        /// <summary>
        /// The `<` character.
        /// </summary>
        public const int LessThan = 60;

        /// <summary>
        /// The `=` character.
        /// </summary>
        public const int Equals = 61;

        /// <summary>
        /// The `>` character.
        /// </summary>
        public const int GreaterThan = 62;

        /// <summary>
        /// The `?` character.
        /// </summary>
        public const int QuestionMark = 63;

        /// <summary>
        /// The `@` character.
        /// </summary>
        public const int AtSign = 64;

        public const int A = 65;
        public const int B = 66;
        public const int C = 67;
        public const int D = 68;
        public const int E = 69;
        public const int F = 70;
        public const int G = 71;
        public const int H = 72;
        public const int I = 73;
        public const int J = 74;
        public const int K = 75;
        public const int L = 76;
        public const int M = 77;
        public const int N = 78;
        public const int O = 79;
        public const int P = 80;
        public const int Q = 81;
        public const int R = 82;
        public const int S = 83;
        public const int T = 84;
        public const int U = 85;
        public const int V = 86;
        public const int W = 87;
        public const int X = 88;
        public const int Y = 89;
        public const int Z = 90;

        /// <summary>
        /// The `[` character.
        /// </summary>
        public const int OpenSquareBracket = 91;

        /// <summary>
        /// The `\` character.
        /// </summary>
        public const int Backslash = 92;

        /// <summary>
        /// The `]` character.
        /// </summary>
        public const int CloseSquareBracket = 93;

        /// <summary>
        /// The `^` character.
        /// </summary>
        public const int Caret = 94;

        /// <summary>
        /// The `_` character.
        /// </summary>
        public const int Underline = 95;

        /// <summary>
        /// The ``(`)`` character.
        /// </summary>
        public const int BackTick = 96;

        public const int a = 97;
        public const int b = 98;
        public const int c = 99;
        public const int d = 100;
        public const int e = 101;
        public const int f = 102;
        public const int g = 103;
        public const int h = 104;
        public const int i = 105;
        public const int j = 106;
        public const int k = 107;
        public const int l = 108;
        public const int m = 109;
        public const int n = 110;
        public const int o = 111;
        public const int p = 112;
        public const int q = 113;
        public const int r = 114;
        public const int s = 115;
        public const int t = 116;
        public const int u = 117;
        public const int v = 118;
        public const int w = 119;
        public const int x = 120;
        public const int y = 121;
        public const int z = 122;

        /// <summary>
        /// The `{` character.
        /// </summary>
        public const int OpenCurlyBrace = 123;

        /// <summary>
        /// The `|` character.
        /// </summary>
        public const int Pipe = 124;

        /// <summary>
        /// The `}` character.
        /// </summary>
        public const int CloseCurlyBrace = 125;

        /// <summary>
        /// The `~` character.
        /// </summary>
        public const int Tilde = 126;

        public const int U_Combining_Grave_Accent = 0x0300; //	U+0300	Combining Grave Accent
        public const int U_Combining_Acute_Accent = 0x0301; //	U+0301	Combining Acute Accent
        public const int U_Combining_Circumflex_Accent = 0x0302; //	U+0302	Combining Circumflex Accent
        public const int U_Combining_Tilde = 0x0303; //	U+0303	Combining Tilde
        public const int U_Combining_Macron = 0x0304; //	U+0304	Combining Macron
        public const int U_Combining_Overline = 0x0305; //	U+0305	Combining Overline
        public const int U_Combining_Breve = 0x0306; //	U+0306	Combining Breve
        public const int U_Combining_Dot_Above = 0x0307; //	U+0307	Combining Dot Above
        public const int U_Combining_Diaeresis = 0x0308; //	U+0308	Combining Diaeresis
        public const int U_Combining_Hook_Above = 0x0309; //	U+0309	Combining Hook Above
        public const int U_Combining_Ring_Above = 0x030A; //	U+030A	Combining Ring Above
        public const int U_Combining_Double_Acute_Accent = 0x030B; //	U+030B	Combining Double Acute Accent
        public const int U_Combining_Caron = 0x030C; //	U+030C	Combining Caron
        public const int U_Combining_Vertical_Line_Above = 0x030D; //	U+030D	Combining Vertical Line Above
        public const int U_Combining_Double_Vertical_Line_Above = 0x030E; //	U+030E	Combining Double Vertical Line Above
        public const int U_Combining_Double_Grave_Accent = 0x030F; //	U+030F	Combining Double Grave Accent
        public const int U_Combining_Candrabindu = 0x0310; //	U+0310	Combining Candrabindu
        public const int U_Combining_Inverted_Breve = 0x0311; //	U+0311	Combining Inverted Breve
        public const int U_Combining_Turned_Comma_Above = 0x0312; //	U+0312	Combining Turned Comma Above
        public const int U_Combining_Comma_Above = 0x0313; //	U+0313	Combining Comma Above
        public const int U_Combining_Reversed_Comma_Above = 0x0314; //	U+0314	Combining Reversed Comma Above
        public const int U_Combining_Comma_Above_Right = 0x0315; //	U+0315	Combining Comma Above Right
        public const int U_Combining_Grave_Accent_Below = 0x0316; //	U+0316	Combining Grave Accent Below
        public const int U_Combining_Acute_Accent_Below = 0x0317; //	U+0317	Combining Acute Accent Below
        public const int U_Combining_Left_Tack_Below = 0x0318; //	U+0318	Combining Left Tack Below
        public const int U_Combining_Right_Tack_Below = 0x0319; //	U+0319	Combining Right Tack Below
        public const int U_Combining_Left_Angle_Above = 0x031A; //	U+031A	Combining Left Angle Above
        public const int U_Combining_Horn = 0x031B; //	U+031B	Combining Horn
        public const int U_Combining_Left_Half_Ring_Below = 0x031C; //	U+031C	Combining Left Half Ring Below
        public const int U_Combining_Up_Tack_Below = 0x031D; //	U+031D	Combining Up Tack Below
        public const int U_Combining_Down_Tack_Below = 0x031E; //	U+031E	Combining Down Tack Below
        public const int U_Combining_Plus_Sign_Below = 0x031F; //	U+031F	Combining Plus Sign Below
        public const int U_Combining_Minus_Sign_Below = 0x0320; //	U+0320	Combining Minus Sign Below
        public const int U_Combining_Palatalized_Hook_Below = 0x0321; //	U+0321	Combining Palatalized Hook Below
        public const int U_Combining_Retroflex_Hook_Below = 0x0322; //	U+0322	Combining Retroflex Hook Below
        public const int U_Combining_Dot_Below = 0x0323; //	U+0323	Combining Dot Below
        public const int U_Combining_Diaeresis_Below = 0x0324; //	U+0324	Combining Diaeresis Below
        public const int U_Combining_Ring_Below = 0x0325; //	U+0325	Combining Ring Below
        public const int U_Combining_Comma_Below = 0x0326; //	U+0326	Combining Comma Below
        public const int U_Combining_Cedilla = 0x0327; //	U+0327	Combining Cedilla
        public const int U_Combining_Ogonek = 0x0328; //	U+0328	Combining Ogonek
        public const int U_Combining_Vertical_Line_Below = 0x0329; //	U+0329	Combining Vertical Line Below
        public const int U_Combining_Bridge_Below = 0x032A; //	U+032A	Combining Bridge Below
        public const int U_Combining_Inverted_Double_Arch_Below = 0x032B; //	U+032B	Combining Inverted Double Arch Below
        public const int U_Combining_Caron_Below = 0x032C; //	U+032C	Combining Caron Below
        public const int U_Combining_Circumflex_Accent_Below = 0x032D; //	U+032D	Combining Circumflex Accent Below
        public const int U_Combining_Breve_Below = 0x032E; //	U+032E	Combining Breve Below
        public const int U_Combining_Inverted_Breve_Below = 0x032F; //	U+032F	Combining Inverted Breve Below
        public const int U_Combining_Tilde_Below = 0x0330; //	U+0330	Combining Tilde Below
        public const int U_Combining_Macron_Below = 0x0331; //	U+0331	Combining Macron Below
        public const int U_Combining_Low_Line = 0x0332; //	U+0332	Combining Low Line
        public const int U_Combining_Double_Low_Line = 0x0333; //	U+0333	Combining Double Low Line
        public const int U_Combining_Tilde_Overlay = 0x0334; //	U+0334	Combining Tilde Overlay
        public const int U_Combining_Short_Stroke_Overlay = 0x0335; //	U+0335	Combining Short Stroke Overlay
        public const int U_Combining_Long_Stroke_Overlay = 0x0336; //	U+0336	Combining Long Stroke Overlay
        public const int U_Combining_Short_Solidus_Overlay = 0x0337; //	U+0337	Combining Short Solidus Overlay
        public const int U_Combining_Long_Solidus_Overlay = 0x0338; //	U+0338	Combining Long Solidus Overlay
        public const int U_Combining_Right_Half_Ring_Below = 0x0339; //	U+0339	Combining Right Half Ring Below
        public const int U_Combining_Inverted_Bridge_Below = 0x033A; //	U+033A	Combining Inverted Bridge Below
        public const int U_Combining_Square_Below = 0x033B; //	U+033B	Combining Square Below
        public const int U_Combining_Seagull_Below = 0x033C; //	U+033C	Combining Seagull Below
        public const int U_Combining_X_Above = 0x033D; //	U+033D	Combining X Above
        public const int U_Combining_Vertical_Tilde = 0x033E; //	U+033E	Combining Vertical Tilde
        public const int U_Combining_Double_Overline = 0x033F; //	U+033F	Combining Double Overline
        public const int U_Combining_Grave_Tone_Mark = 0x0340; //	U+0340	Combining Grave Tone Mark
        public const int U_Combining_Acute_Tone_Mark = 0x0341; //	U+0341	Combining Acute Tone Mark
        public const int U_Combining_Greek_Perispomeni = 0x0342; //	U+0342	Combining Greek Perispomeni
        public const int U_Combining_Greek_Koronis = 0x0343; //	U+0343	Combining Greek Koronis
        public const int U_Combining_Greek_Dialytika_Tonos = 0x0344; //	U+0344	Combining Greek Dialytika Tonos
        public const int U_Combining_Greek_Ypogegrammeni = 0x0345; //	U+0345	Combining Greek Ypogegrammeni
        public const int U_Combining_Bridge_Above = 0x0346; //	U+0346	Combining Bridge Above
        public const int U_Combining_Equals_Sign_Below = 0x0347; //	U+0347	Combining Equals Sign Below
        public const int U_Combining_Double_Vertical_Line_Below = 0x0348; //	U+0348	Combining Double Vertical Line Below
        public const int U_Combining_Left_Angle_Below = 0x0349; //	U+0349	Combining Left Angle Below
        public const int U_Combining_Not_Tilde_Above = 0x034A; //	U+034A	Combining Not Tilde Above
        public const int U_Combining_Homothetic_Above = 0x034B; //	U+034B	Combining Homothetic Above
        public const int U_Combining_Almost_Equal_To_Above = 0x034C; //	U+034C	Combining Almost Equal To Above
        public const int U_Combining_Left_Right_Arrow_Below = 0x034D; //	U+034D	Combining Left Right Arrow Below
        public const int U_Combining_Upwards_Arrow_Below = 0x034E; //	U+034E	Combining Upwards Arrow Below
        public const int U_Combining_Grapheme_Joiner = 0x034F; //	U+034F	Combining Grapheme Joiner
        public const int U_Combining_Right_Arrowhead_Above = 0x0350; //	U+0350	Combining Right Arrowhead Above
        public const int U_Combining_Left_Half_Ring_Above = 0x0351; //	U+0351	Combining Left Half Ring Above
        public const int U_Combining_Fermata = 0x0352; //	U+0352	Combining Fermata
        public const int U_Combining_X_Below = 0x0353; //	U+0353	Combining X Below
        public const int U_Combining_Left_Arrowhead_Below = 0x0354; //	U+0354	Combining Left Arrowhead Below
        public const int U_Combining_Right_Arrowhead_Below = 0x0355; //	U+0355	Combining Right Arrowhead Below
        public const int U_Combining_Right_Arrowhead_And_Up_Arrowhead_Below = 0x0356; //	U+0356	Combining Right Arrowhead And Up Arrowhead Below
        public const int U_Combining_Right_Half_Ring_Above = 0x0357; //	U+0357	Combining Right Half Ring Above
        public const int U_Combining_Dot_Above_Right = 0x0358; //	U+0358	Combining Dot Above Right
        public const int U_Combining_Asterisk_Below = 0x0359; //	U+0359	Combining Asterisk Below
        public const int U_Combining_Double_Ring_Below = 0x035A; //	U+035A	Combining Double Ring Below
        public const int U_Combining_Zigzag_Above = 0x035B; //	U+035B	Combining Zigzag Above
        public const int U_Combining_Double_Breve_Below = 0x035C; //	U+035C	Combining Double Breve Below
        public const int U_Combining_Double_Breve = 0x035D; //	U+035D	Combining Double Breve
        public const int U_Combining_Double_Macron = 0x035E; //	U+035E	Combining Double Macron
        public const int U_Combining_Double_Macron_Below = 0x035F; //	U+035F	Combining Double Macron Below
        public const int U_Combining_Double_Tilde = 0x0360; //	U+0360	Combining Double Tilde
        public const int U_Combining_Double_Inverted_Breve = 0x0361; //	U+0361	Combining Double Inverted Breve
        public const int U_Combining_Double_Rightwards_Arrow_Below = 0x0362; //	U+0362	Combining Double Rightwards Arrow Below
        public const int U_Combining_Latin_Small_Letter_A = 0x0363; //	U+0363	Combining Latin Small Letter A
        public const int U_Combining_Latin_Small_Letter_E = 0x0364; //	U+0364	Combining Latin Small Letter E
        public const int U_Combining_Latin_Small_Letter_I = 0x0365; //	U+0365	Combining Latin Small Letter I
        public const int U_Combining_Latin_Small_Letter_O = 0x0366; //	U+0366	Combining Latin Small Letter O
        public const int U_Combining_Latin_Small_Letter_U = 0x0367; //	U+0367	Combining Latin Small Letter U
        public const int U_Combining_Latin_Small_Letter_C = 0x0368; //	U+0368	Combining Latin Small Letter C
        public const int U_Combining_Latin_Small_Letter_D = 0x0369; //	U+0369	Combining Latin Small Letter D
        public const int U_Combining_Latin_Small_Letter_H = 0x036A; //	U+036A	Combining Latin Small Letter H
        public const int U_Combining_Latin_Small_Letter_M = 0x036B; //	U+036B	Combining Latin Small Letter M
        public const int U_Combining_Latin_Small_Letter_R = 0x036C; //	U+036C	Combining Latin Small Letter R
        public const int U_Combining_Latin_Small_Letter_T = 0x036D; //	U+036D	Combining Latin Small Letter T
        public const int U_Combining_Latin_Small_Letter_V = 0x036E; //	U+036E	Combining Latin Small Letter V
        public const int U_Combining_Latin_Small_Letter_X = 0x036F; //	U+036F	Combining Latin Small Letter X

        /// <summary>
        /// Unicode Character "LINE SEPARATOR" (U+2028)
        /// http://www.fileformat.info/info/unicode/char/2028/index.htm
        /// </summary>
        public const int LINE_SEPARATOR_2028 = 8232;

        // http://www.fileformat.info/info/unicode/category/Sk/list.htm
        public const int U_CIRCUMFLEX = 0x005E; // U+005E	CIRCUMFLEX
        public const int U_GRAVE_ACCENT = 0x0060; // U+0060	GRAVE ACCENT
        public const int U_DIAERESIS = 0x00A8; // U+00A8	DIAERESIS
        public const int U_MACRON = 0x00AF; // U+00AF	MACRON
        public const int U_ACUTE_ACCENT = 0x00B4; // U+00B4	ACUTE ACCENT
        public const int U_CEDILLA = 0x00B8; // U+00B8	CEDILLA
        public const int U_MODIFIER_LETTER_LEFT_ARROWHEAD = 0x02C2; // U+02C2	MODIFIER LETTER LEFT ARROWHEAD
        public const int U_MODIFIER_LETTER_RIGHT_ARROWHEAD = 0x02C3; // U+02C3	MODIFIER LETTER RIGHT ARROWHEAD
        public const int U_MODIFIER_LETTER_UP_ARROWHEAD = 0x02C4; // U+02C4	MODIFIER LETTER UP ARROWHEAD
        public const int U_MODIFIER_LETTER_DOWN_ARROWHEAD = 0x02C5; // U+02C5	MODIFIER LETTER DOWN ARROWHEAD
        public const int U_MODIFIER_LETTER_CENTRED_RIGHT_HALF_RING = 0x02D2; // U+02D2	MODIFIER LETTER CENTRED RIGHT HALF RING
        public const int U_MODIFIER_LETTER_CENTRED_LEFT_HALF_RING = 0x02D3; // U+02D3	MODIFIER LETTER CENTRED LEFT HALF RING
        public const int U_MODIFIER_LETTER_UP_TACK = 0x02D4; // U+02D4	MODIFIER LETTER UP TACK
        public const int U_MODIFIER_LETTER_DOWN_TACK = 0x02D5; // U+02D5	MODIFIER LETTER DOWN TACK
        public const int U_MODIFIER_LETTER_PLUS_SIGN = 0x02D6; // U+02D6	MODIFIER LETTER PLUS SIGN
        public const int U_MODIFIER_LETTER_MINUS_SIGN = 0x02D7; // U+02D7	MODIFIER LETTER MINUS SIGN
        public const int U_BREVE = 0x02D8; // U+02D8	BREVE
        public const int U_DOT_ABOVE = 0x02D9; // U+02D9	DOT ABOVE
        public const int U_RING_ABOVE = 0x02DA; // U+02DA	RING ABOVE
        public const int U_OGONEK = 0x02DB; // U+02DB	OGONEK
        public const int U_SMALL_TILDE = 0x02DC; // U+02DC	SMALL TILDE
        public const int U_DOUBLE_ACUTE_ACCENT = 0x02DD; // U+02DD	DOUBLE ACUTE ACCENT
        public const int U_MODIFIER_LETTER_RHOTIC_HOOK = 0x02DE; // U+02DE	MODIFIER LETTER RHOTIC HOOK
        public const int U_MODIFIER_LETTER_CROSS_ACCENT = 0x02DF; // U+02DF	MODIFIER LETTER CROSS ACCENT
        public const int U_MODIFIER_LETTER_EXTRA_HIGH_TONE_BAR = 0x02E5; // U+02E5	MODIFIER LETTER EXTRA-HIGH TONE BAR
        public const int U_MODIFIER_LETTER_HIGH_TONE_BAR = 0x02E6; // U+02E6	MODIFIER LETTER HIGH TONE BAR
        public const int U_MODIFIER_LETTER_MID_TONE_BAR = 0x02E7; // U+02E7	MODIFIER LETTER MID TONE BAR
        public const int U_MODIFIER_LETTER_LOW_TONE_BAR = 0x02E8; // U+02E8	MODIFIER LETTER LOW TONE BAR
        public const int U_MODIFIER_LETTER_EXTRA_LOW_TONE_BAR = 0x02E9; // U+02E9	MODIFIER LETTER EXTRA-LOW TONE BAR
        public const int U_MODIFIER_LETTER_YIN_DEPARTING_TONE_MARK = 0x02EA; // U+02EA	MODIFIER LETTER YIN DEPARTING TONE MARK
        public const int U_MODIFIER_LETTER_YANG_DEPARTING_TONE_MARK = 0x02EB; // U+02EB	MODIFIER LETTER YANG DEPARTING TONE MARK
        public const int U_MODIFIER_LETTER_UNASPIRATED = 0x02ED; // U+02ED	MODIFIER LETTER UNASPIRATED
        public const int U_MODIFIER_LETTER_LOW_DOWN_ARROWHEAD = 0x02EF; // U+02EF	MODIFIER LETTER LOW DOWN ARROWHEAD
        public const int U_MODIFIER_LETTER_LOW_UP_ARROWHEAD = 0x02F0; // U+02F0	MODIFIER LETTER LOW UP ARROWHEAD
        public const int U_MODIFIER_LETTER_LOW_LEFT_ARROWHEAD = 0x02F1; // U+02F1	MODIFIER LETTER LOW LEFT ARROWHEAD
        public const int U_MODIFIER_LETTER_LOW_RIGHT_ARROWHEAD = 0x02F2; // U+02F2	MODIFIER LETTER LOW RIGHT ARROWHEAD
        public const int U_MODIFIER_LETTER_LOW_RING = 0x02F3; // U+02F3	MODIFIER LETTER LOW RING
        public const int U_MODIFIER_LETTER_MIDDLE_GRAVE_ACCENT = 0x02F4; // U+02F4	MODIFIER LETTER MIDDLE GRAVE ACCENT
        public const int U_MODIFIER_LETTER_MIDDLE_DOUBLE_GRAVE_ACCENT = 0x02F5; // U+02F5	MODIFIER LETTER MIDDLE DOUBLE GRAVE ACCENT
        public const int U_MODIFIER_LETTER_MIDDLE_DOUBLE_ACUTE_ACCENT = 0x02F6; // U+02F6	MODIFIER LETTER MIDDLE DOUBLE ACUTE ACCENT
        public const int U_MODIFIER_LETTER_LOW_TILDE = 0x02F7; // U+02F7	MODIFIER LETTER LOW TILDE
        public const int U_MODIFIER_LETTER_RAISED_COLON = 0x02F8; // U+02F8	MODIFIER LETTER RAISED COLON
        public const int U_MODIFIER_LETTER_BEGIN_HIGH_TONE = 0x02F9; // U+02F9	MODIFIER LETTER BEGIN HIGH TONE
        public const int U_MODIFIER_LETTER_END_HIGH_TONE = 0x02FA; // U+02FA	MODIFIER LETTER END HIGH TONE
        public const int U_MODIFIER_LETTER_BEGIN_LOW_TONE = 0x02FB; // U+02FB	MODIFIER LETTER BEGIN LOW TONE
        public const int U_MODIFIER_LETTER_END_LOW_TONE = 0x02FC; // U+02FC	MODIFIER LETTER END LOW TONE
        public const int U_MODIFIER_LETTER_SHELF = 0x02FD; // U+02FD	MODIFIER LETTER SHELF
        public const int U_MODIFIER_LETTER_OPEN_SHELF = 0x02FE; // U+02FE	MODIFIER LETTER OPEN SHELF
        public const int U_MODIFIER_LETTER_LOW_LEFT_ARROW = 0x02FF; // U+02FF	MODIFIER LETTER LOW LEFT ARROW
        public const int U_GREEK_LOWER_NUMERAL_SIGN = 0x0375; // U+0375	GREEK LOWER NUMERAL SIGN
        public const int U_GREEK_TONOS = 0x0384; // U+0384	GREEK TONOS
        public const int U_GREEK_DIALYTIKA_TONOS = 0x0385; // U+0385	GREEK DIALYTIKA TONOS
        public const int U_GREEK_KORONIS = 0x1FBD; // U+1FBD	GREEK KORONIS
        public const int U_GREEK_PSILI = 0x1FBF; // U+1FBF	GREEK PSILI
        public const int U_GREEK_PERISPOMENI = 0x1FC0; // U+1FC0	GREEK PERISPOMENI
        public const int U_GREEK_DIALYTIKA_AND_PERISPOMENI = 0x1FC1; // U+1FC1	GREEK DIALYTIKA AND PERISPOMENI
        public const int U_GREEK_PSILI_AND_VARIA = 0x1FCD; // U+1FCD	GREEK PSILI AND VARIA
        public const int U_GREEK_PSILI_AND_OXIA = 0x1FCE; // U+1FCE	GREEK PSILI AND OXIA
        public const int U_GREEK_PSILI_AND_PERISPOMENI = 0x1FCF; // U+1FCF	GREEK PSILI AND PERISPOMENI
        public const int U_GREEK_DASIA_AND_VARIA = 0x1FDD; // U+1FDD	GREEK DASIA AND VARIA
        public const int U_GREEK_DASIA_AND_OXIA = 0x1FDE; // U+1FDE	GREEK DASIA AND OXIA
        public const int U_GREEK_DASIA_AND_PERISPOMENI = 0x1FDF; // U+1FDF	GREEK DASIA AND PERISPOMENI
        public const int U_GREEK_DIALYTIKA_AND_VARIA = 0x1FED; // U+1FED	GREEK DIALYTIKA AND VARIA
        public const int U_GREEK_DIALYTIKA_AND_OXIA = 0x1FEE; // U+1FEE	GREEK DIALYTIKA AND OXIA
        public const int U_GREEK_VARIA = 0x1FEF; // U+1FEF	GREEK VARIA
        public const int U_GREEK_OXIA = 0x1FFD; // U+1FFD	GREEK OXIA
        public const int U_GREEK_DASIA = 0x1FFE; // U+1FFE	GREEK DASIA


        public const int U_OVERLINE = 0x203E; // Unicode Character "OVERLINE"

        /// <summary>
        /// UTF-8 BOM
        /// Unicode Character "ZERO WIDTH NO-BREAK SPACE" (U+FEFF)
        /// http://www.fileformat.info/info/unicode/char/feff/index.htm
        /// </summary>
        public const int UTF8_BOM = 65279;
    }
}
