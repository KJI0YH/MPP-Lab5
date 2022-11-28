namespace Lab5
{
    public class StringFormatter
    {
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

        public bool Parse(string str)
        {
            int state = 1;
            for (int i = 0; i < str.Length; i++)
            {
                state = Transitions[state, (int)GetCharType(str[i])];
            }
            return IsFinalState[state];
        }
    }
}
