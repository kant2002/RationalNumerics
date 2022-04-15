﻿using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Numerics.Rational;

namespace Test
{
  /// <summary>
  /// Usual  implementation of a rational number class based on <see cref="System.Numerics.BigInteger"/>.<br/>
  /// </summary>
  /// <remarks>
  /// <i>This class is only intended for speed comparisons and benchmark tests for the new <see cref="Rational"/> class.</i>
  /// </remarks>
  [DebuggerDisplay("{ToString(\"\"),nq}")]
  public struct UsualRational : IEquatable<UsualRational>, IComparable<UsualRational>, IFormattable
  {
    BigInteger num, den;
    public int Sign => num.Sign;
    /// <summary>
    /// Simply mapped to <see cref="Rational.ToString"/> as it is not part of the speed comparisons.
    /// </summary>
    public override string ToString()
    {
      return ToString(null, null);
    }
    /// <summary>
    /// Simply mapped to <see cref="Rational.ToString(string?, IFormatProvider?)"/> as it is not part of the speed comparisons.
    /// </summary>
    public string ToString(string? format, IFormatProvider? provider = default)
    {
      if (den.IsZero) return NumberFormatInfo.GetInstance(provider).NaNSymbol;
      return ((Rational)this).ToString(format, provider);
    }
    public override int GetHashCode()
    {
      return num.GetHashCode() + den.GetHashCode() * 13;
    }
    public override bool Equals(object? b)
    {
      return b is UsualRational n ? Equals(n) : false;
    }
    public int CompareTo(UsualRational b)
    {
      var s1 = num.Sign;
      if (s1 != b.num.Sign) return s1 > b.num.Sign ? +1 : -1;
      if (s1 == 0) return 0;
      var den = this.den; if (den.Sign < 0) den = -den;
      if (b.den.Sign < 0) b.den = -b.den;
      var s2 = num.CompareTo(b.num) * s1;
      var s3 = den.CompareTo(b.den);
      if (s3 == 0) return +s2 * s1;
      if (s2 == 0) return -s3 * s1;
      if (s2 > 0 && s3 < 0) return +s1;
      if (s3 < 0 && s2 > 0) return -s1;
      return (num * b.den).CompareTo(b.num * den);
    }
    public bool Equals(UsualRational b)
    {
      return num.Equals(b.num) && den.Equals(b.den);
    }
    public static implicit operator UsualRational(int v)
    {
      return new UsualRational { num = v, den = 1 };
    }
    public static implicit operator UsualRational(long v)
    {
      return new UsualRational { num = v, den = 1 };
    }
    public static implicit operator UsualRational(ulong v)
    {
      return new UsualRational { num = v, den = 1 };
    }
    public static implicit operator Rational(UsualRational v)
    {
      return (Rational)v.num / (Rational)v.den;
    }
    public static explicit operator UsualRational(Rational v)
    {
      var cpu = Rational.task_cpu; cpu.push(v); cpu.mod(8);
      if (cpu.sign() < 0) { cpu.neg(0); cpu.neg(1); }
      cpu.swp();
      return new UsualRational { num = (BigInteger)cpu.pop_rat(), den = (BigInteger)cpu.pop_rat() };
    }
    public static UsualRational operator +(UsualRational a)
    {
      return a;
    }
    public static UsualRational operator -(UsualRational a)
    {
      a.num = -a.num; return a;
    }
    public static UsualRational operator +(UsualRational a, UsualRational b)
    {
      a.num = a.num * b.den + a.den * b.num;
      a.den = a.den * b.den;
      a.normalize(); return a;
    }
    public static UsualRational operator -(UsualRational a, UsualRational b)
    {
      a.num = a.num * b.den - a.den * b.num;
      a.den = a.den * b.den;
      a.normalize(); return a;
    }
    public static UsualRational operator *(UsualRational a, UsualRational b)
    {
      a.num *= b.num;
      a.den *= b.den;
      a.normalize(); return a;
    }
    public static UsualRational operator /(UsualRational a, UsualRational b)
    {
      if (b.num.IsZero) throw new DivideByZeroException();
      a.num *= b.den;
      a.den *= b.num;
      a.normalize(); return a;
    }
    public static bool operator ==(UsualRational a, UsualRational b)
    {
      return a.Equals(b);
    }
    public static bool operator !=(UsualRational a, UsualRational b)
    {
      return !a.Equals(b);
    }
    public static bool operator <=(UsualRational a, UsualRational b)
    {
      return a.CompareTo(b) <= 0;
    }
    public static bool operator >=(UsualRational a, UsualRational b)
    {
      return a.CompareTo(b) >= 0;
    }
    public static bool operator <(UsualRational a, UsualRational b)
    {
      return a.CompareTo(b) < 0;
    }
    public static bool operator >(UsualRational a, UsualRational b)
    {
      return a.CompareTo(b) > 0;
    }
    public static UsualRational Abs(UsualRational a)
    {
      return a.Sign < 0 ? -a : a;
    }
    public static UsualRational Min(UsualRational a, UsualRational b)
    {
      return a < b ? a : b;
    }
    public static UsualRational Max(UsualRational a, UsualRational b)
    {
      return a > b ? a : b;
    }
    public static UsualRational Pow(UsualRational a, int b)
    {
      UsualRational result = 1;
      for (var e = unchecked((uint)(b < 0 ? -b : b)); ; e >>= 1)
      {
        if ((e & 1) != 0) result *= a;
        if (e <= 1) break; a *= a;
      }
      if (b < 0) result = 1 / result;
      return result;
    }
    public static UsualRational Round(UsualRational a, int digits)
    {
      var e = Pow(10, digits); var b = a * e;
      var div = BigInteger.DivRem(BigInteger.Abs(b.num), b.den, out var rem);
      if (rem > (b.den >> 1)) div += 1;
      var result = new UsualRational { num = b.Sign >= 0 ? div : -div, den = 1 } / e;
      return result;
    }
    void normalize()
    {
      if (den < 0) { num = -num; den = -den; }
      var gcd = BigInteger.GreatestCommonDivisor(BigInteger.Abs(num), den);
      if (gcd != 1) { num /= gcd; den /= gcd; }
    }
  }
}
