using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace HisRoyalRedness.com
{
    #region DateTokenAssertions
    public class DateTokenAssertions : LiteralTokenAssertions<DateToken, DateTime>
    {
        public DateTokenAssertions(DateToken value)
            : base(value)
        { }

        protected override bool TokenEquals(DateToken expected)
            => Subject.Equals(expected);

        protected override bool TypedValueEqual(DateTime expected)
            => Subject.TypedValue.Equals(expected);
    }
    #endregion DateTokenAssertions
    #region FloatTokenAssertions
    public class FloatTokenAssertions : LiteralTokenAssertions<FloatToken, double>
    {
        public FloatTokenAssertions(FloatToken value)
            : base(value)
        { }

        protected override bool TokenEquals(FloatToken expected)
            => Subject.Equals(expected);

        protected override bool TypedValueEqual(double expected)
            => Subject.TypedValue.Equals(expected);
    }
    #endregion FloatTokenAssertions
    #region LimitedIntegerTokenAssertions
    public class LimitedIntegerTokenAssertions : LiteralTokenAssertions<LimitedIntegerToken, BigInteger>
    {
        public LimitedIntegerTokenAssertions(LimitedIntegerToken value)
            : base(value)
        { }

        protected override bool TokenEquals(LimitedIntegerToken expected)
            => Subject.Equals(expected);

        protected override bool TypedValueEqual(BigInteger expected)
            => Subject.TypedValue.Equals(expected);
    }
    #endregion LimitedIntegerTokenAssertions
    #region TimespanTokenAssertions
    public class TimespanTokenAssertions : LiteralTokenAssertions<TimespanToken, TimeSpan>
    {
        public TimespanTokenAssertions(TimespanToken value)
            : base(value)
        { }

        protected override bool TokenEquals(TimespanToken expected)
            => Subject.Equals(expected);

        protected override bool TypedValueEqual(TimeSpan expected)
            => Subject.TypedValue.Equals(expected);
    }
    #endregion TimespanTokenAssertions
    #region TimeTokenAssertions
    public class TimeTokenAssertions : LiteralTokenAssertions<TimeToken, TimeSpan>
    {
        public TimeTokenAssertions(TimeToken value)
            : base(value)
        { }

        protected override bool TokenEquals(TimeToken expected)
            => Subject.Equals(expected);

        protected override bool TypedValueEqual(TimeSpan expected)
            => Subject.TypedValue.Equals(expected);
    }
    #endregion TimeTokenAssertions
    #region UnlimitedIntegerTokenAssertions
    public class UnlimitedIntegerTokenAssertions : LiteralTokenAssertions<UnlimitedIntegerToken, BigInteger>
    {
        public UnlimitedIntegerTokenAssertions(UnlimitedIntegerToken value)
            : base(value)
        { }

        protected override bool TokenEquals(UnlimitedIntegerToken expected)
            => Subject.Equals(expected);

        protected override bool TypedValueEqual(BigInteger expected)
            => Subject.TypedValue.Equals(expected);
    }
    #endregion UnlimitedIntegerTokenAssertions
    #region LiteralTokenAssertions
    public abstract class LiteralTokenAssertions<TToken, TTypedValue> : ReferenceTypeAssertions<TToken, LiteralTokenAssertions<TToken, TTypedValue>>
        where TToken : class, ILiteralToken
    {
        public LiteralTokenAssertions(TToken token)
        {
            Subject = token;
        }

        protected override string Identifier => typeof(TToken).Name;

        public AndConstraint<LiteralTokenAssertions<TToken, TTypedValue>> Be(TToken expected, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .TestToken(expected, Identifier, Subject, TokenEquals);
            return new AndConstraint<LiteralTokenAssertions<TToken, TTypedValue>>(this);
        }

        public AndConstraint<LiteralTokenAssertions<TToken, TTypedValue>> BeTypedValue(TTypedValue expected, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .TestTokenValue(expected, Identifier, Subject, TypedValueEqual);
            return new AndConstraint<LiteralTokenAssertions<TToken, TTypedValue>>(this);
        }


        protected abstract bool TokenEquals(TToken expected);
        protected abstract bool TypedValueEqual(TTypedValue expected);
    }
    #endregion LiteralTokenAssertions

    public static class FluentAssertionsExtensions
    {
        public static DateTokenAssertions Should(this DateToken token) => new DateTokenAssertions(token);
        public static FloatTokenAssertions Should(this FloatToken token) => new FloatTokenAssertions(token);
        public static LimitedIntegerTokenAssertions Should(this LimitedIntegerToken token) => new LimitedIntegerTokenAssertions(token);
        public static TimespanTokenAssertions Should(this TimespanToken token) => new TimespanTokenAssertions(token);
        public static TimeTokenAssertions Should(this TimeToken token) => new TimeTokenAssertions(token);
        public static UnlimitedIntegerTokenAssertions Should(this UnlimitedIntegerToken token) => new UnlimitedIntegerTokenAssertions(token);


        public static Continuation TestToken<TToken>(this AssertionScope scope, TToken expected, string expectedName, TToken actual, Predicate<TToken> tokenEquals)
            where TToken : class, ILiteralToken
            => scope
                // if expected is null, then actual should also be null
                .ForCondition(!(expected is null) || (actual is null)).FailWith($"Expected {expectedName} to be null, but it is wasn't.")

                // if expected is not null, then neither should actual be
                .Then.ForCondition((expected is null) || !(actual is null)).FailWith($"Expected {expectedName} to be '{expected}', but it was null.")

                // if neither is null, then the TypeValues should match
                .Then.ForCondition((expected is null) || (actual is null) || tokenEquals(expected)).FailWith($"Expected {expectedName} to be '{expected}', but is was '{actual}'");

        public static Continuation TestTokenValue<TToken, TTypedValue>(this AssertionScope scope, TTypedValue expected, string expectedName, TToken actual, Predicate<TTypedValue> typedValueEquals)
            where TToken : class, ILiteralToken
            => scope
                // if neither is null, then the TypeValues should match
                .ForCondition(typedValueEquals(expected)).FailWith($"Expected the value of {expectedName} to be '{expected}', but is was '{actual}'");

    }
}
