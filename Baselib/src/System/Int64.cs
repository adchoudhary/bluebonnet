
namespace system
{

    public class Int64 : system.ValueType, system.ValueMethod, java.lang.Cloneable
    {

        [java.attr.RetainType] protected long v;



        public static Int64 Box(long v) => new Int64() { v = v };
        public static Int64 Box(long[] a, int i) => new Int64.InArray(a, i);

        public virtual long Get() => v;
        public virtual long VolatileGet() =>
                Util.JavaUnsafe.getLongVolatile(this, ValueOffset);

        public virtual void Set(long v) => this.v = v;
        public virtual void VolatileSet(long v) =>
                Util.JavaUnsafe.putLongVolatile(this, ValueOffset, v);

        public static void Set(long v, Int64 o) => o.Set(v);
        public static void VolatileSet(long v, Int64 o) => o.VolatileSet(v);



        public override bool Equals(object obj)
        {
            var objInt64 = obj as Int64;
            return (objInt64 != null && objInt64.Get() == Get());
        }

        public override int GetHashCode()
        {
            long v = Get();
            return ((int) v) ^ (int) (v >> 32);
        }

        public override string ToString()
        {
            return java.lang.Long.toString(Get());
        }

        public static long OverflowAdd(long a, long b)
        {
            long c = a + b;
            if (((a ^ c) & (b ^ c)) < 0)
                system.Math.OverflowException();
            return c;
        }

        public static long OverflowSubtract(long a, long b)
        {
            long c = a - b;
            if (((a ^ b) & (a ^ c)) < 0)
                system.Math.OverflowException();
            return c;
        }

        public static long OverflowMultiply(long a, long b)
        {
            long c = a * b;
            ulong aAbs = (ulong) java.lang.Math.abs(a);
            ulong bAbs = (ulong) java.lang.Math.abs(b);
            if (((aAbs | bAbs) >> 31) != 0)
            {
                if (    ((b != 0) && (c / b != a))
                     || (a == System.Int64.MinValue && b == -1))
                {
                    system.Math.OverflowException();
                }
            }
            return c;
        }



        void ValueMethod.Clear() => Set(0);
        void ValueMethod.CopyFrom(ValueType from) => Set(((Int64) from).Get());
        void ValueMethod.CopyInto(ValueType into) => ((Int64) into).Set(Get());
        ValueType ValueMethod.Clone() => Box(Get());



        static long ValueOffset
        {
            get
            {
                if (_ValueOffset == -1)
                {
                    var cls = (java.lang.Class) typeof(Int64);
                    _ValueOffset = Util.JavaUnsafe.objectFieldOffset(cls.getDeclaredFields()[0]);
                }
                return _ValueOffset;
            }
        }
        [java.attr.RetainType] static long _ValueOffset = -1;



        //
        // CodeNumber.Indirection methods
        //



        public int Get_U8() => (byte) Get();
        public int Get_I8() => (sbyte) Get();
        public void Set_I8(int v) => Set((int) ((byte) v | ((ulong) Get() & (ulong) 0xFFFFFFFFFFFFFF00)));

        public int Get_U16() => (ushort) Get();
        public int Get_I16() => (short) Get();
        public void Set_I16(int v) => Set((int) ((ushort) v | ((ulong) Get() & (ulong) 0xFFFFFFFFFFFF0000)));

        public int Get_I32() => (int) Get();
        public void Set_I32(int v) => Set((int) ((uint) v | ((ulong) Get() & (ulong) 0xFFFFFFFF00000000)));

        public float Get_F32() => java.lang.Float.intBitsToFloat(Get_I32());
        public void Set_F32(float v) => Set_I32(java.lang.Float.floatToRawIntBits(v));

        public double Get_F64() => java.lang.Double.longBitsToDouble(Get());
        public void Set_F64(double v) => Set(java.lang.Double.doubleToRawLongBits(v));



        //
        // InArray
        //

        private sealed class InArray : Int64
        {
            [java.attr.RetainType] private long[] a;
            [java.attr.RetainType] private int i;

            public InArray(long[] array, int index)
            {
                if (index < 0 || index >= array.Length)
                    throw new System.IndexOutOfRangeException();
                a = array;
                i = index;
            }

            public override long Get() => a[i];
            public override long VolatileGet() =>
                Util.JavaUnsafe.getLongVolatile(a, Util.ElementOffset64(i));

            public override void Set(long v) => a[i] = v;
            public override void VolatileSet(long v) =>
                Util.JavaUnsafe.putLongVolatile(a, Util.ElementOffset64(i), v);
        }

    }



    public class IntPtr : Int64
    {

        public IntPtr(int v) => this.v = v;
        public IntPtr(long v) => this.v = v;

        public static int Size => 8;

        public static readonly IntPtr Zero = new IntPtr(0);
    }



    #pragma warning disable 0659
    public class UInt64 : Int64
    {

        new public static UInt64 Box(long v) => new UInt64() { v = v };
        public static UInt64 Box(ulong[] a, int i) => new UInt64.InArray(a, i);

        public static void Set(long v, UInt64 o) => o.Set(v);
        public static void VolatileSet(long v, UInt64 o) => o.VolatileSet(v);

        public override bool Equals(object obj)
        {
            var objUInt64 = obj as UInt64;
            return (objUInt64 != null && objUInt64.Get() == Get());
        }

        //public int CompareTo(ulong v) => java.lang.Long.compareUnsigned(Get(), (long) v);

        public int CompareTo(ulong v) => CompareTo(Get(), (long) v);

        public static int CompareTo(long a, long b)
            => a == b ? 0 : a + System.Int64.MinValue < b + System.Int64.MinValue ? -1 : 1;

        public static long UnsignedDivision(long dividend, long divisor)
        {
            if (divisor < 0)
            {
                // divisor has highest bit set, so is too large and can fit
                // at most once in dividend, so division becomes a comparison
                return (CompareTo(dividend, divisor)) < 0 ? 0 : 1;
            }
            else if (dividend >= 0)
            {
                // if dividend and divisor are both positive,
                // then the built-in signed division will work fine
                return dividend / divisor;
            }
            else
            {
                return ToUnsignedBigInteger(dividend).divide(
                            ToUnsignedBigInteger(divisor)).longValue();
            }
        }

        public static long UnsignedRemainder(long dividend, long divisor)
        {
            if (dividend > 0 && divisor > 0)
            {
                // if dividend and divisor are both positive,
                // then the built-in signed remainder will work fine
                return dividend % divisor;
            }
            else if (CompareTo(dividend, divisor) < 0)
            {
                // dividend is smaller than remainder
                return dividend;
            }
            else
            {
                return ToUnsignedBigInteger(dividend).remainder(
                            ToUnsignedBigInteger(divisor)).longValue();
            }
        }

        private static java.math.BigInteger ToUnsignedBigInteger(long i)
        {
            if (i >= 0)
                return java.math.BigInteger.valueOf(i);
            long upper = (long) (((ulong) i) >> 32);
            long lower = (long) (ulong) (uint) i;
            // return (upper << 32) + lower
            return (java.math.BigInteger.valueOf(upper).shiftLeft(32)
                                        .add(java.math.BigInteger.valueOf(lower)));
        }

        new public static ulong OverflowAdd(long a, long b)
        {
            ulong c = (ulong) (a + b);
            if (c < (ulong) a || c < (ulong) b)
                system.Math.OverflowException();
            return c;
        }

        new public static ulong OverflowSubtract(long a, long b)
        {
            ulong c = (ulong) (a - b);
            if (c > (ulong) a || c > (ulong) b)
                system.Math.OverflowException();
            return c;
        }

        new public static ulong OverflowMultiply(long a, long b)
        {
            ulong c = (ulong) (a * b);
            if (c < (ulong) a || c < (ulong) b)
                system.Math.OverflowException();
            return c;
        }



        //
        // InArray
        //

        private sealed class InArray : UInt64
        {
            [java.attr.RetainType] private ulong[] a;
            [java.attr.RetainType] private int i;

            public InArray(ulong[] array, int index)
            {
                if (index < 0 || index >= array.Length)
                    throw new System.IndexOutOfRangeException();
                a = array;
                i = index;
            }

            public override long Get() => (long) a[i];
            public override long VolatileGet() =>
                Util.JavaUnsafe.getLongVolatile(a, Util.ElementOffset64(i));

            public override void Set(long v) => a[i] = (ulong) v;
            public override void VolatileSet(long v) =>
                Util.JavaUnsafe.putLongVolatile(a, Util.ElementOffset64(i), v);
        }

    }

}
