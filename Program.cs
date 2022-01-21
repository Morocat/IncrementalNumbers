using System.Diagnostics;

public class TestCases {
    public static void Main(string[] args){ 
        //testAdd();
        testMultiply();
    }

    public static void testMultiply() {
        ExpNum b = new ExpNum(123456);
        ExpNum a = new ExpNum(123);

        a *= b;
        Debug.Assert(a.Equals(new ExpNum().debugSet(new int[]{88, 185, 15, 0, 0, 0}, 0)));

        b = new ExpNum().debugSet(new int[]{999, 999, 999, 999, 999, 999}, 0);
        a = new ExpNum().debugSet(new int[]{123, 456, 789, 777, 888, 999}, 0);
        a *= b;
        Debug.Assert(a.Equals(new ExpNum().debugSet(new int[]{122, 456, 789, 777, 888, 999}, 6)));

        b = new ExpNum().debugSet(new int[]{2, 0, 0, 0, 0, 0}, 0);
        a = new ExpNum().debugSet(new int[]{123, 456, 789, 777, 888, 999}, 3);
        a *= b;

        a = new ExpNum().debugSet(new int[]{1, 0, 0, 0, 0, 0}, 0);
        for (int i = 0; i < 24; i++) {
            if (i == 0)
                Debug.Assert(a.ToString().Equals("1"));
            else if (i == 1)
                Debug.Assert(a.ToString().Equals("10"));
            else if (i == 2)
                Debug.Assert(a.ToString().Equals("100"));
            else if (ExpNum.config.displayType == NumConfig.DisplayType.DISPLAY_TYPE_SCIENTIFIC)
                Debug.Assert(a.ToString().Equals("1.000e"+i));
            Console.WriteLine(a);
            a *= 10;
        }
    }

    public static void testAdd() {
        ExpNum a = new ExpNum(123444);
        a.add(a).assert("246.888k");

        for (uint i = 0; i < 1000; i++) {
            new ExpNum(i).assert(i.ToString());
        }

        new ExpNum(1000).assert("1.000k");
        new ExpNum(57192).assert("57.192k");
        new ExpNum(120543982).assert("120.543M");
        new ExpNum(120543982123).assert("120.543B");
        new ExpNum(120543982123123).assert("120.543T");
        new ExpNum(120543982123123123).assert("120.543Qa");

        int[] vals = new int[] {123, 123, 123, 123, 543, 120};
        new ExpNum().debugSet(vals, 1).assert("120.543Qi");

        new ExpNum(55).add(27).assert("82");
        new ExpNum(27).add(55).assert("82");

        a = new ExpNum().debugSet(vals, 1);
        int[] vals2 = new int[] {123, 456, 789, 0, 0, 0};
        ExpNum b = new ExpNum().debugSet(vals2, 0);
        a.add(b);
        int[] vals3 = new int[] {123 + 456, 123 + 789, 123, 123, 543, 120};
        ExpNum c = new ExpNum().debugSet(vals3, 1);
        Debug.Assert(a.Equals(c));

        a = new ExpNum().debugSet(vals, 1);
        b = new ExpNum().debugSet(vals2, 0);
        b.add(a);
        Debug.Assert(b.Equals(c));

        a = new ExpNum().debugSet(vals, 5);
        b = new ExpNum().debugSet(vals2, 0);
        b.add(a);
        Debug.Assert(b.Equals(a));

        int[] v1 = new int[] {123, 456, 789, 012, 345, 678};
        int[] v2 = new int[] {678, 345, 012, 789, 456, 123};
        a = new ExpNum().debugSet(v1, 5);
        b = new ExpNum().debugSet(v2, 0);
        b.add(a);
        int[] v3 = new int[] {246, 456, 789, 012, 345, 678};
        c = new ExpNum().debugSet(v3, 5);
        Debug.Assert(b.Equals(c));
    }
}