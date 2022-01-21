using System.Diagnostics;

public class ExpNum {
    //public static bool DEBUG = true;
    public const int NUM_EXP = 6;
    public static NumConfig config = new NumConfig();
    private int[] vals = new int[NUM_EXP]; // groups of 3 zeroes

    /*
     * an exponent of 0 would mean vals[0] would refer to 0-999
     * an exponent of 1 would mean vals[0] would refer to the thousands.
    */
    private ulong exponent;

    // https://en.wikipedia.org/wiki/Names_of_large_numbers
    private String[] units = new String[]{
        "",
        "k",    // thousand
        "M",    // million
        "B",    // billion
        "T",    // trillion
        "Qa",   // quadrillion
        "Qi",   // quintillion
        "Sx",   // sextillion
        "Sp",   // septillion
        "Oc",   // octillion
        "N",    // novemillion
        "D",    // decillion
        "uD",   // undecillion
        "DD",   // duodecillion
        "TD",   // tredecillion
        "QD",   // quadecillion
        "QiD",  // quindecillion
        "SxD",  // sexdecillion
        "SpD",  // septdecillion
        "OcD",  // octdecillion
        "NoD",  // novendecillion
        "Vi",   // vigintillion
        "UV",   // unvigintillion
        "DV",   // duovigintillion
        "TV",   // tresvigintillion
        "QaV",  // quavigintillion
        "QiV",  // quinvigintillion
        "SxV",  // sexvigintillion
        "SpV",  // septvigintillion
        "OcV",  // octvigintillion
        "NoV",  // novigintillion
        "Tri",  // trigintillion
        "UTr",  // untrigintillion

        // quadragintillion
        // quinquagintillion
        // sexagintillion
        // septuagintillion
        // octogintillion
        // nonagintillion
        // centillion
    };

    public ExpNum() {

    }

    public ExpNum(UInt64 val) {
        if (val != 0) {
            int exp =((int) Math.Log10(val) / 3) - NUM_EXP;
            if (exp < 0)
                exp = 0;
            this.exponent = (ulong)exp;
        }
        
        for (int i = 0; i < NUM_EXP; i++) {
            double v = (double) val;
            double exp = Math.Pow(10, (NUM_EXP - i - 1) * 3);
            this.vals[NUM_EXP - i - 1] = ((int) (val / exp)) % 1000;
        }
    }

    public ExpNum(ExpNum a) {
        this.from(a);
    }

    public override String ToString() {
        return this.DisplayValue;
    }

    public override bool Equals(object? obj) {
        if (!(obj is ExpNum) || obj == null)
            return false;

        if (this.GetHashCode() == obj.GetHashCode())
            return true;
        
        ExpNum a = (ExpNum)obj;
        return Enumerable.SequenceEqual(this.vals, a.vals) 
            && this.exponent == a.exponent;
    }

    private String DisplayValue {
        get {
            String str;
            double dec = 0;
            ulong i = NUM_EXP - 1;
            
            if (exponent == 0) {
                // find highest nonzero value
                for (i = NUM_EXP - 1; i >= 1; i--) {
                    if (this.vals[i] != 0) {
                        if (i != 0) {
                            break;
                        }
                    }
                }
            }

            if (config.displayType == NumConfig.DisplayType.DISPLAY_TYPE_SCIENTIFIC) {
                int val;
                ulong exp;

                if (i == 0)
                    return this.vals[i].ToString();

                if (this.vals[i] < 10) {
                    val = this.vals[i];
                    exp = 0;
                    dec = ((double)this.vals[i - 1]) / 1000.0;
                } else if (this.vals[i] < 100) {
                    val = this.vals[i] / 10;
                    exp = 1;
                    dec = (((double) (this.vals[i] % 10)) + ((double)this.vals[i - 1] / 1000)) / 10;
                } else {
                    val = this.vals[i] / 100;
                    exp = 2;
                    dec = (((double) (this.vals[i] % 100)) + ((double)this.vals[i - 1] / 1000)) / 100;
                }
                str = val.ToString() + dec.ToString("F" + config.decimalPlaces).Substring(1);
                return str + "e" + ((i + exponent) * 3 + exp).ToString();
            }

            str = this.vals[i].ToString();
            if (i != 0) {
                dec = ((double)this.vals[i - 1]) / 1000.0;
                str += dec.ToString("F" + config.decimalPlaces).Substring(1);
            }
            if (config.displayType == NumConfig.DisplayType.DISPLAY_TYPE_SHORT)
                str += units[i + exponent];
            else if (config.displayType == NumConfig.DisplayType.DISPLAY_TYPE_ENGINEERING)
                str += "e" + ((i + exponent) * (ulong)3).ToString();
            
            return str;
        }
    }

#region Adding
    /****************************************
     * ADDING                               *
     ****************************************/
    public ExpNum add(ExpNum a) {
        // first line up the two vals arrays based on the exponent

        // other number is smaller or equal
        if (this.exponent >= a.exponent) {
            ulong expDiff = this.exponent - a.exponent;

            // if the exponent difference is greater than NUM_EXP, don't even bother adding anything
            if (expDiff >= NUM_EXP)
                return this;
            
            // add the values together
            for (ulong i = 0; i < NUM_EXP - expDiff; i++)
                this.vals[i] += a.vals[i + expDiff];
        } 

        // other number is bigger
        else {
            ulong expDiff = a.exponent - this.exponent;
            
            if (expDiff >= NUM_EXP) {
                this.from(a);
                return this;
            }
            this.exponent = a.exponent;

            int diff = (int) expDiff;
            // shift this array's vals to match the other number's exponent powers
            for (int i = 0; i < NUM_EXP - diff; i++)
                this.vals[i] = this.vals[i + diff];
            Array.Fill(this.vals, 0, NUM_EXP - diff, diff);

            // add the two arrays together
            for (int i = 0; i < NUM_EXP; i++)
                this.vals[i] += a.vals[i];
        }

        // propegate all carry values
        for (int i = 0; i < NUM_EXP - 1; i++) {
            if (this.vals[i] > 1000) {
                this.vals[i] -= 1000;
                this.vals[i + 1]++;
            }
        }

        // check for incrementing the exponent value
        if (this.vals[NUM_EXP - 1] > 1000) {
            this.exponent++;
            for (int i = 0; i < NUM_EXP - 1; i++)
                this.vals[i] = this.vals[i + 1];
            
            this.vals[NUM_EXP - 1] = 0;
        }

        return this;
    }

    public ExpNum add(UInt64 a) {
        return this.add(new ExpNum(a));
    }

    public static ExpNum operator+ (ExpNum a, ExpNum b) {
        return new ExpNum(a).add(b);
    }

    public static ExpNum operator+ (ExpNum a, UInt64 b) {
        return new ExpNum(a).add(b);
    }
#endregion
#region Subtracting
    /****************************************
     * SUBTRACTING                          *
     ****************************************/
    public ExpNum subtract(ExpNum a) {
        return this;
    }

    public ExpNum subtract(UInt64 a) {
        return this.subtract(new ExpNum(a));
    }

    public static ExpNum operator- (ExpNum a, ExpNum b) {
        return new ExpNum(a).subtract(b);
    }
#endregion
#region Multiplying
public ExpNum multiply(ExpNum a) {
    ExpNum top = this > a ? this : a;
    ExpNum bot = this < a ? this : a;
    int[] sum = new int[NUM_EXP * 2 + 1];

    for (int i = 0; i < NUM_EXP; i++) { // i represents the bot number's index
        for (int j = 0; j < NUM_EXP; j++) { // j is the top number's index
            sum[j+i] += bot.vals[i] * top.vals[j];
        }
    }

    // propegate all carries
    for (int i = 0; i < NUM_EXP * 2; i++) {
        int mod = sum[i] % 1000;
        sum[i+1] += (sum[i] - mod) / 1000;
        sum[i] = mod;
    }
    // shift array/exponent if needed
    int copyFrom = 0;
    for (int i = NUM_EXP * 2; i >= NUM_EXP; i--) {
        if (sum[i] != 0) {
            copyFrom = i - NUM_EXP + 1;
            break;
        }
    }

    for (int i = 0; i < NUM_EXP; i++)
        this.vals[i] = sum[copyFrom + i];

    this.exponent += (ulong)copyFrom;

    return this;
}

public ExpNum multiply(UInt64 a) {
    return this.multiply(new ExpNum(a));
}

public static ExpNum operator* (ExpNum a, ExpNum b) {
    return new ExpNum(a).multiply(b);
}
public static ExpNum operator* (ExpNum a, UInt64 b) {
    return new ExpNum(a).multiply(b);
}
#endregion

public static bool operator> (ExpNum a, ExpNum b) {
    if (a.exponent != b.exponent)
        return a.exponent > b.exponent;
    for (int i = NUM_EXP - 1; i >= 0; i--) {
        if (a.vals[i] > b.vals[i])
            return true;
        if (a.vals[i] < b.vals[i])
            return false;
    }
    return false;
}

public static bool operator>= (ExpNum a, ExpNum b) {
    if (a.exponent != b.exponent)
        return a.exponent > b.exponent;
    for (int i = NUM_EXP - 1; i >= 0; i--) {
        if (a.vals[i] > b.vals[i])
            return true;
        if (a.vals[i] < b.vals[i])
            return false;
    }
    return true;
}

public static bool operator< (ExpNum a, ExpNum b) {
    if (a.exponent != b.exponent)
        return a.exponent < b.exponent;
    for (int i = NUM_EXP - 1; i >= 0; i--) {
        if (a.vals[i] < b.vals[i])
            return true;
        if (a.vals[i] > b.vals[i])
            return false;
    }
    return false;
}

public static bool operator<= (ExpNum a, ExpNum b) {
    if (a.exponent != b.exponent)
        return a.exponent < b.exponent;
    for (int i = NUM_EXP - 1; i >= 0; i--) {
        if (a.vals[i] < b.vals[i])
            return true;
        if (a.vals[i] > b.vals[i])
            return false;
    }
    return true;
}

#region PrivateHelpers
    /****************************************
     * PRIVATE HELPERS                      *
     ****************************************/
    private void from(ExpNum a) {
        Array.Copy(a.vals, this.vals, NUM_EXP);
        this.exponent = a.exponent;
    }

    public ExpNum debugSet(int[] v, ulong exp) {
        this.exponent = exp;
        Array.Copy(v, vals, NUM_EXP);
        return this;
    }

    public void assert(String str) {
        Console.WriteLine(this.ToString());
        Debug.Assert(this.ToString().Equals(str));
    }

#endregion
}