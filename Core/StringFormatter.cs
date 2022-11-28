using Lab5.Core.Interfaces;
using Lab5.Core.Services;

namespace Lab5.Core
{
    public class StringFormatter : IStringFormatter
    {
        public static readonly StringFormatter Shared = new StringFormatter();
        private IExpressionCashe _cashe;

        public StringFormatter()
        {
            _cashe = new ExpressionCashe();
        }
        public StringFormatter(IExpressionCashe cashe)
        {
            _cashe = cashe;
        }

        private enum CharType
        {
            ctUnknown,
            ctLetter,   //a..zA..Z
            ctDigit,    //0..9
            ctUnder,    //_
            ctCBO,      //{
            ctCBC,      //}
            ctSBO,      //[
            ctSBC       //]
        }

        private string _letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private string _digits = "0123456789";
        private string _under = "_";
        private string _cbo = "{";
        private string _cbc = "}";
        private string _sbo = "[";
        private string _sbc = "]";

        private int[,] Transitions =
        {
            {0, 0, 0, 0, 0, 0, 0, 0 },  //Error state
            {1, 1, 1, 1, 2, 3, 1, 1 },  //1
            {0, 4, 0, 0, 1, 0, 0, 0 },  //2
            {0, 0, 0, 0, 0, 1, 0, 0 },  //3
            {0, 4, 4, 4, 0, 1, 5, 0 },  //4
            {0, 0, 6, 0, 0, 0, 0, 0 },  //5
            {0, 0, 6, 0, 0, 0, 0, 7 },  //6
            {0, 0, 0, 0, 0, 1, 0, 0 }   //7
        };

        private bool[] IsFinalState =
        {
            false,  //Error state 
            true,   //1
            false,  //2
            false,  //3
            false,  //4
            false,  //5
            false,  //6
            false,  //7
        };

        private CharType GetCharType(char c)
        {
            if (_letters.Contains(c))
                return CharType.ctLetter;
            else if (_digits.Contains(c))
                return CharType.ctDigit;
            else if (_under.Contains(c))
                return CharType.ctUnder;
            else if (_cbo.Contains(c))
                return CharType.ctCBO;
            else if (_cbc.Contains(c))
                return CharType.ctCBC;
            else if (_sbo.Contains(c))
                return CharType.ctSBO;
            else if (_sbc.Contains(c))
                return CharType.ctSBC;
            return CharType.ctUnknown;
        }

        public string Format(string template, object target)
        {
            int state = 1;
            int prevState = state;
            int startPos = 0;
            string result = "";
            for (int i = 0; i < template.Length; i++)
            {
                prevState = state;
                state = Transitions[state, (int)GetCharType(template[i])];

                switch (state)
                {
                    case 0:
                        throw new ArgumentException($"Invalid template, position {i}");
                    case 1:
                        if (prevState == 4 || prevState == 7)
                        {
                            result += _cashe.GetOrAdd(template.AsMemory(startPos, i - startPos).ToString(), target);
                        }
                        else
                        {
                            result += template[i];
                        }
                        break;
                    case 4:
                        if (prevState == 2)
                            startPos = i;
                        break;
                }
            }
            if (IsFinalState[state])
                return result;
            else
                throw new ArgumentException($"Invalid template, position {template.Length - 1}");
        }
    }
}
