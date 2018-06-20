// -----------------------------------------------------------------------
// <copyright file="ExtensionMethodTests.cs" company="GeomaticTechnologies">
// Copyright (c) GeomaticTechnologies. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.PList
{
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using Xunit;

    /// <summary>
    /// Tests for <see cref="ExtensionMethods"/>.
    /// </summary>
    public class ExtensionMethodTests
    {
        [Fact]
        private void TestGetNullableInt32WithValidData() => TestGetWithValidData(ExtensionMethods.GetNullableInt32, 1234L);

        [Fact]
        private void TestGetNullableInt32WithNoKey() => TestGetWithNoKey(ExtensionMethods.GetNullableInt32);

        [Fact]
        private void TestGetNullableInt32WithInvalidData() => TestGetWithInvalidData(ExtensionMethods.GetNullableInt32);

        [Fact]
        private void TestGetNullableInt64WithValidData() => TestGetWithValidData(ExtensionMethods.GetNullableInt64, 1234L);

        [Fact]
        private void TestGetNullableInt64WithNoKey() => TestGetWithNoKey(ExtensionMethods.GetNullableInt64);

        [Fact]
        private void TestGetNullableInt64WithInvalidData() => TestGetWithInvalidData(ExtensionMethods.GetNullableInt64);

        [Fact]
        private void TestGetNullableBooleanWithValidData() => TestGetWithValidData(ExtensionMethods.GetNullableBoolean, true);

        [Fact]
        private void TestGetNullableBooleanWithNoKey() => TestGetWithNoKey(ExtensionMethods.GetNullableBoolean);

        [Fact]
        private void TestGetNullableBooleanWithInvalidData() => TestGetWithInvalidData(ExtensionMethods.GetNullableBoolean);

        [Fact]
        private void TestGetNullableDateTimeWithValidData() => TestGetWithValidData(ExtensionMethods.GetNullableDateTime, DateTime.Now);

        [Fact]
        private void TestGetNullableDateTimeWithNoKey() => TestGetWithNoKey(ExtensionMethods.GetNullableDateTime);

        [Fact]
        private void TestGetNullableDateTimeWithInvalidData() => TestGetWithInvalidData(ExtensionMethods.GetNullableDateTime);

        [Fact]
        private void TestGetStringWithValidData() => ExtensionMethods.GetString((new Dictionary<string, object> { { "value", "value" } }), "value").Should().Be("value");

        [Fact]
        private void TestGetStringWithNoKey() => ExtensionMethods.GetString((new Dictionary<string, object> { { "value", "value" } }), "value_bad").Should().Be(default(string));

        [Fact]
        private void TestGetStringWithInvalidData() => (new Dictionary<string, object> { { "value", 123456M } }).Invoking(_ => ExtensionMethods.GetString(_, "value")).Should().Throw<InvalidCastException>();

        private static void TestGetWithValidData<T1, T2>(Func<IDictionary<string, object>, string, T1?> function, T2 value)
            where T1 : struct => function((new Dictionary<string, object> { { "value", value } }), "value").Should().Be(value);

        private static void TestGetWithNoKey<T>(Func<IDictionary<string, object>, string, T?> function)
            where T : struct => function((new Dictionary<string, object> { { "value", "value" } }), "value_bad").Should().Be(default(T?));

        private static void TestGetWithInvalidData<T>(Func<IDictionary<string, object>, string, T?> function)
            where T : struct => (new Dictionary<string, object> { { "value", 123456M } }).Invoking(_ => function(_, "value")).Should().Throw<InvalidCastException>();
    }
}
