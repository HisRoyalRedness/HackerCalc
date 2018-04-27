using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    #region Scanner
    public partial class Scanner : IDisposable
    {
        public Buffer buffer; // scanner buffer

        Token t;          // current token
        int ch;           // current input character
        int pos;          // byte position of current character
        int charPos;      // position by unicode characters starting with 0
        int col;          // column number of current character
        int line;         // line number of current character
        int oldEols;      // EOLs that appeared in a comment;
        static readonly Dictionary<int, int> start; // maps first token character to start state

        Token tokenChain; // list of tokens already peeked (first token is a dummy)
        Token pt;         // current peek token

        char[] tval = new char[128]; // text of current token
        int tlen;         // length of current token

        public Scanner(string fileName)
        {
            try
            {
                Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                buffer = new Buffer(stream, false);
                Init();
            }
            catch (IOException)
            {
                throw new FatalError("Cannot open file " + fileName);
            }
        }

        public Scanner(Stream s)
        {
            buffer = new Buffer(s, true);
            Init();
        }

        void Init()
        {
            pos = -1; line = 1; col = 0; charPos = -1;
            oldEols = 0;
            NextCh();
            if (ch == 0xEF)
            { // check optional byte order mark for UTF-8
                NextCh(); int ch1 = ch;
                NextCh(); int ch2 = ch;
                if (ch1 != 0xBB || ch2 != 0xBF)
                {
                    throw new FatalError(String.Format("illegal byte order mark: EF {0,2:X} {1,2:X}", ch1, ch2));
                }
                buffer = new UTF8Buffer(buffer); col = 0; charPos = -1;
                NextCh();
            }
            pt = tokenChain = new Token();  // first token is a dummy
        }

        private void SetScannerBehindT()
        {
            buffer.Pos = t.pos;
            NextCh();
            line = t.line; col = t.col; charPos = t.charPos;
            for (int i = 0; i < tlen; i++) NextCh();
        }

        // get the next token (possibly a token already seen during peeking)
        public Token Scan()
        {
            tokenChain = ScanNextToken(tokenChain);
            ResetPeek();
            tokenChain.prev.next = null;
            tokenChain.prev = null;
            return tokenChain;
        }

        // peek for the next token, ignore pragmas
        public Token Peek()
        {
            do
            {
                pt = ScanNextToken(pt);
            } while (pt.kind > maxT); // skip pragmas

            return pt;
        }

        Token ScanNextToken(Token token)
        {
            if (token.next == null)
            {
                token.next = NextToken();

                if (token.next.kind == Parser._negToken)
                {
                    // If the '-' is the first token, or immediately follows an open bracket or operator,
                    // then we can assume that it must negate the expression that follows
                    switch (token.kind)
                    {
                        case Parser._EOF:
                        case Parser._openBracket:
                        case Parser._notToken:
                        case Parser._multToken:
                        case Parser._addToken:
                        case Parser._shiftToken:
                        case Parser._bitToken:
                            token.next.kind = Parser._notToken; // a negation
                            break;
                        default:
                            token.next.kind = Parser._addToken; // a subtraction
                            break;
                    }
                }

                token.next.prev = token;
            }
            return token.next;
        }

        // make sure that peeking starts at the current scan position
        public void ResetPeek() { pt = tokenChain; }

        #region IDisposable
        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (buffer != null)
                    {
                        buffer.Dispose();
                        buffer = null;
                    }
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion IDisposable

    }
    #endregion Scanner
}
