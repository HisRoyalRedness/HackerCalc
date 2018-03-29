using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace HisRoyalRedness.com
{
    public class DateTokenAssertions : ReferenceTypeAssertions<DateToken, DateTokenAssertions>
    {
        public DateTokenAssertions(DateToken value)
        {
            Subject = value;
        }

        protected override string Identifier => nameof(DateToken);

        public AndConstraint<DateTokenAssertions> Be(DateToken expected, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .TestElement((ILiteralToken<DateTime, DateToken>)expected, nameof(DateToken), (ILiteralToken<DateTime, DateToken>)Subject);
            return new AndConstraint<DateTokenAssertions>(this);
        }
    }

    public static class FluentAssertionsExtensions
    {
        public static DateTokenAssertions Should(this DateToken dateToken) => new DateTokenAssertions(dateToken);

        public static Continuation TestElement<TBaseType, TToken>(this AssertionScope scope, ILiteralToken<TBaseType, TToken> expected, string expectedName, ILiteralToken<TBaseType, TToken> actual)
            where TToken : class, ILiteralToken, ILiteralToken<TBaseType, TToken>
            => scope
                // if expected is null, then actual should also be null
                .ForCondition(!(expected is null) || (actual is null)).FailWith($"Expected {expectedName} to be null, but it is wasn't.")

                // if expected is not null, then neither should actual be
                .Then.ForCondition((expected is null) || !(actual is null)).FailWith($"Expected {expectedName} to be '{expected}', but it was null.")

                // if neither is null, then the TypeValues should match
                .Then.ForCondition((expected is null) || (actual is null) || expected.TypedValue.Equals(actual.TypedValue)).FailWith($"Expected {expectedName} to be '{expected}', but is was '{actual}'");
    }
}
