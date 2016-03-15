using System;
using System.Text;

namespace Sharp
{
 
    public class InvalidrBase31ValueException : System.Exception
    {
        public InvalidrBase31ValueException(object o)
        {

        }
    }
    public class InvalidrBase31DigitException : System.Exception
    {
        public InvalidrBase31DigitException(object o)
        {

        }
    }
    public class InvalidrBase31NumberException : System.Exception
    {
        public InvalidrBase31NumberException(object o)
        {

        }
    }
    public class InvalidrBase31DigitValueException : System.Exception
    {
        public InvalidrBase31DigitValueException(object o)
        {

        }
    }

    /// <summary>
    /// Class representing a rBase31 number
    /// </summary>
    public struct rBase31
    {
        #region Constants (and pseudo-constants)

        /// <summary>
        /// rBase31 containing the maximum supported value for this type
        /// </summary>
        public static readonly rBase31 MaxValue = new rBase31(long.MaxValue);
        /// <summary>
        /// rBase31 containing the minimum supported value for this type
        /// </summary>
        public static readonly rBase31 MinValue = new rBase31(long.MinValue + 1);

        public static readonly String Alphabet = "2NOL6BQZRX4CEGSKFPYTW73VDUHJM9A";
        private static string alphabet;

        public static string Sanitize(string Value) {
            if (Value == "")
            {
                throw new InvalidrBase31NumberException(Value);
            }

            return Value.ToUpper()
                .Replace("0", "O")
                .Replace("I", "L")
                .Replace("1", "L")
                .Replace("8", "B")
                .Replace("5", "S");

        }


        #endregion

        #region Fields

        private long numericValue;

        #endregion

        #region Constructor

        /// <summary>
        /// Instantiate a rBase31 number from a long value
        /// </summary>
        /// <param name="NumericValue">The long value to give to the rBase31 number</param>
        public rBase31(long NumericValue)
        {
            numericValue = 0; //required by the struct.
            this.NumericValue = NumericValue;
        }


        /// <summary>
        /// Instantiate a rBase31 number from a rBase31 string
        /// </summary>
        /// <param name="Value">The value to give to the rBase31 number</param>
        public rBase31(string Value)
        {
            numericValue = 0; //required by the struct.
            this.Value = Sanitize(Value);
        }


        #endregion

        #region Properties

        /// <summary>
        /// Get or set the value of the type using a base-10 long integer
        /// </summary>
        public long NumericValue
        {
            get
            {
                return numericValue;
            }
            set
            {
                //Make sure value is between allowed ranges
                if (value <= long.MinValue || value > long.MaxValue)
                {
                    throw new InvalidrBase31ValueException(value);
                }

                numericValue = value;
            }
        }


        /// <summary>
        /// Get or set the value of the type using a rBase31 string
        /// </summary>
        public string Value
        {
            get
            {
                return rBase31.NumberTorBase31(numericValue);
            }
            set
            {
                try
                {
                    numericValue = rBase31.rBase31ToNumber(Sanitize(value));
                }
                catch
                {
                    //Catch potential errors
                    throw new InvalidrBase31NumberException(value);
                }
            }
        }


        #endregion

        #region Public Static Methods

        /// <summary>
        /// Static method to convert a rBase31 string to a long integer (base-10)
        /// </summary>
        /// <param name="rBase31Value">The number to convert from</param>
        /// <returns>The long integer</returns>
        public static long rBase31ToNumber(string rBase31Value)
        {
            //Make sure we have passed something
            rBase31Value = Sanitize(rBase31Value);

            //Account for negative values:
            bool isNegative = false;

            if (rBase31Value[0] == '-')
            {
                rBase31Value = rBase31Value.Substring(1);
                isNegative = true;
            }

            //Loop through our string and calculate its value
            try
            {
                //Keep a running total of the value 
                long returnValue = 0;
                alphabet = Alphabet;
                long salt = rBase31DigitToNumber(rBase31Value[rBase31Value.Length - 1]);
                SaltAlphabet(salt);

                //Loop through the character in the string (right to left) and add
                //up increasing powers as we go.
                for (int i = 1; i < rBase31Value.Length; i++)
                {
                    returnValue += ((long)Math.Pow(31, i - 1) * rBase31DigitToNumber(rBase31Value[rBase31Value.Length - (i + 1)]));
                }

                //Do negative correction if required:
                if (isNegative)
                {
                    return returnValue * -1;
                }
                else
                {
                    return returnValue;
                }
            }
            catch
            {
                //If something goes wrong, this is not a valid number
                throw new InvalidrBase31NumberException(rBase31Value);
            }
        }  

        /// <summary>
        /// Public static method to convert a long integer (base-10) to a rBase31 number
        /// </summary>
        /// <param name="NumericValue">The base-10 long integer</param>
        /// <returns>A rBase31 representation</returns>
        public static string NumberTorBase31(long NumericValue)
        {
            alphabet = Alphabet;
            string salt = PositiveNumberTorBase31(NumericValue % 31);
            SaltAlphabet(NumericValue % 31);

            return DoNumberTorBase31(NumericValue) + salt;
        }

        private static string DoNumberTorBase31(long NumericValue)
        {
            try
            {
                //Handle negative values:
                if (NumericValue < 0)
                {
                    return string.Concat("-", PositiveNumberTorBase31(Math.Abs(NumericValue)));
                }
                else
                {
                    return PositiveNumberTorBase31(NumericValue);
                }
            }
            catch
            {
                throw new InvalidrBase31ValueException(NumericValue);
            }
        }


        #endregion

        #region Private Static Methods

        private static void SaltAlphabet(long salt)
        {
            if (salt > 0 && salt < 31)
                alphabet = Alphabet.Substring((int)salt) + Alphabet.Substring(0, (int)salt);
            else
                alphabet = Alphabet;
        }

        private static string PositiveNumberTorBase31(long NumericValue)
        {
            //This is a clever recursively called function that builds
            //the base-31 string representation of the long base-10 value
            if (NumericValue < 31)
            {
                //The get out clause; fires when we reach a number less than 
                //31 - this means we can add the last digit.
                return NumberTorBase31Digit((byte)NumericValue).ToString();
            }
            else
            {
                //Add digits from left to right in powers of 31 
                //(recursive)
                return string.Concat(PositiveNumberTorBase31(NumericValue / 31), NumberTorBase31Digit((byte)(NumericValue % 31)).ToString());
            }
        }


        private static byte rBase31DigitToNumber(char rBase31Digit)
        {
            return (byte)alphabet.IndexOf(rBase31Digit.ToString());
        }

        private static char NumberTorBase31Digit(byte NumericValue)
        {
            return alphabet[NumericValue];
        }


        #endregion

        #region Operator Overloads

        /// <summary>
        /// Operator overload
        /// </summary>
        /// <param name="LHS"></param>
        /// <param name="RHS"></param>
        /// <returns></returns>
        public static bool operator >(rBase31 LHS, rBase31 RHS)
        {
            return LHS.numericValue > RHS.numericValue;
        }


        /// <summary>
        /// Operator overload
        /// </summary>
        /// <param name="LHS"></param>
        /// <param name="RHS"></param>
        /// <returns></returns>
        public static bool operator <(rBase31 LHS, rBase31 RHS)
        {
            return LHS.numericValue < RHS.numericValue;
        }


        /// <summary>
        /// Operator overload
        /// </summary>
        /// <param name="LHS"></param>
        /// <param name="RHS"></param>
        /// <returns></returns>
        public static bool operator >=(rBase31 LHS, rBase31 RHS)
        {
            return LHS.numericValue >= RHS.numericValue;
        }


        /// <summary>
        /// Operator overload
        /// </summary>
        /// <param name="LHS"></param>
        /// <param name="RHS"></param>
        /// <returns></returns>
        public static bool operator <=(rBase31 LHS, rBase31 RHS)
        {
            return LHS.numericValue <= RHS.numericValue;
        }


        /// <summary>
        /// Operator overload
        /// </summary>
        /// <param name="LHS"></param>
        /// <param name="RHS"></param>
        /// <returns></returns>
        public static bool operator ==(rBase31 LHS, rBase31 RHS)
        {
            return LHS.numericValue == RHS.numericValue;
        }


        /// <summary>
        /// Operator overload
        /// </summary>
        /// <param name="LHS"></param>
        /// <param name="RHS"></param>
        /// <returns></returns>
        public static bool operator !=(rBase31 LHS, rBase31 RHS)
        {
            return !(LHS == RHS);
        }


        /// <summary>
        /// Operator overload
        /// </summary>
        /// <param name="LHS"></param>
        /// <param name="RHS"></param>
        /// <returns></returns>
        public static rBase31 operator +(rBase31 LHS, rBase31 RHS)
        {
            return new rBase31(LHS.numericValue + RHS.numericValue);
        }


        /// <summary>
        /// Operator overload
        /// </summary>
        /// <param name="LHS"></param>
        /// <param name="RHS"></param>
        /// <returns></returns>
        public static rBase31 operator -(rBase31 LHS, rBase31 RHS)
        {
            return new rBase31(LHS.numericValue - RHS.numericValue);
        }


        /// <summary>
        /// Operator overload
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static rBase31 operator ++(rBase31 Value)
        {
            return new rBase31(Value.numericValue++);
        }


        /// <summary>
        /// Operator overload
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static rBase31 operator --(rBase31 Value)
        {
            return new rBase31(Value.numericValue--);
        }


        /// <summary>
        /// Operator overload
        /// </summary>
        /// <param name="LHS"></param>
        /// <param name="RHS"></param>
        /// <returns></returns>
        public static rBase31 operator *(rBase31 LHS, rBase31 RHS)
        {
            return new rBase31(LHS.numericValue * RHS.numericValue);
        }


        /// <summary>
        /// Operator overload
        /// </summary>
        /// <param name="LHS"></param>
        /// <param name="RHS"></param>
        /// <returns></returns>
        public static rBase31 operator /(rBase31 LHS, rBase31 RHS)
        {
            return new rBase31(LHS.numericValue / RHS.numericValue);
        }


        /// <summary>
        /// Operator overload
        /// </summary>
        /// <param name="LHS"></param>
        /// <param name="RHS"></param>
        /// <returns></returns>
        public static rBase31 operator %(rBase31 LHS, rBase31 RHS)
        {
            return new rBase31(LHS.numericValue % RHS.numericValue);
        }


        /// <summary>
        /// Converts type rBase31 to a base-10 long
        /// </summary>
        /// <param name="Value">The rBase31 object</param>
        /// <returns>The base-10 long integer</returns>
        public static implicit operator long(rBase31 Value)
        {
            return Value.numericValue;
        }


        /// <summary>
        /// Converts type rBase31 to a base-10 integer
        /// </summary>
        /// <param name="Value">The rBase31 object</param>
        /// <returns>The base-10 integer</returns>
        public static implicit operator int(rBase31 Value)
        {
            try
            {
                return (int)Value.numericValue;
            }
            catch
            {
                throw new OverflowException("Overflow: Value too large to return as an integer");
            }
        }


        /// <summary>
        /// Converts type rBase31 to a base-10 short
        /// </summary>
        /// <param name="Value">The rBase31 object</param>
        /// <returns>The base-10 short</returns>
        public static implicit operator short(rBase31 Value)
        {
            try
            {
                return (short)Value.numericValue;
            }
            catch
            {
                throw new OverflowException("Overflow: Value too large to return as a short");
            }
        }


        /// <summary>
        /// Converts a long (base-10) to a rBase31 type
        /// </summary>
        /// <param name="Value">The long to convert</param>
        /// <returns>The rBase31 object</returns>
        public static implicit operator rBase31(long Value)
        {
            return new rBase31(Value);
        }


        /// <summary>
        /// Converts type rBase31 to a string; must be explicit, since
        /// rBase31 > string is dangerous!
        /// </summary>
        /// <param name="Value">The rBase31 type</param>
        /// <returns>The string representation</returns>
        public static explicit operator string(rBase31 Value)
        {
            return Value.Value;
        }


        /// <summary>
        /// Converts a string to a rBase31
        /// </summary>
        /// <param name="Value">The string (must be a rBase31 string)</param>
        /// <returns>A rBase31 type</returns>
        public static implicit operator rBase31(string Value)
        {
            return new rBase31(Value);
        }


        #endregion

        #region Public Override Methods

        /// <summary>
        /// Returns a string representation of the rBase31 number
        /// </summary>
        /// <returns>A string representation</returns>
        public override string ToString()
        {
            return rBase31.NumberTorBase31(numericValue);
        }


        /// <summary>
        /// A unique value representing the value of the number
        /// </summary>
        /// <returns>The unique number</returns>
        public override int GetHashCode()
        {
            return numericValue.GetHashCode();
        }


        /// <summary>
        /// Determines if an object has the same value as the instance
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>True if the values are the same</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is rBase31))
            {
                return false;
            }
            else
            {
                return this == (rBase31)obj;
            }
        }


        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string representation padding the leading edge with
        /// zeros if necessary to make up the number of characters
        /// </summary>
        /// <param name="MinimumDigits">The minimum number of digits that the string must contain</param>
        /// <returns>The padded string representation</returns>
        public string ToString(int MinimumDigits)
        {
            string rBase31Value = rBase31.NumberTorBase31(numericValue);

            if (rBase31Value.Length >= MinimumDigits)
                return rBase31Value;
            else
                return rBase31Value.PadLeft(MinimumDigits, '0');
        }


        #endregion

    }
}