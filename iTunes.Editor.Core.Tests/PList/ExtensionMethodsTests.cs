// -----------------------------------------------------------------------
// <copyright file="ExtensionMethodsTests.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.PList;

using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

/// <summary>
/// Tests for <see cref="ExtensionMethods"/>.
/// </summary>
public class ExtensionMethodsTests
{
    [Fact]
    internal void TestGetNullableInt32WithValidData() => TestGetWithValidData(ExtensionMethods.GetNullableInt32, 1234L);

    [Fact]
    internal void TestGetNullableInt32WithNullData() => TestGetWithNullData(ExtensionMethods.GetNullableInt32, default(int?));

    [Fact]
    internal void TestGetNullableInt32WithNoKey() => TestGetWithNoKey(ExtensionMethods.GetNullableInt32);

    [Fact]
    internal void TestGetNullableInt32WithInvalidData() => TestGetWithInvalidData(ExtensionMethods.GetNullableInt32);

    [Fact]
    internal void TestGetNullableInt64WithValidData() => TestGetWithValidData(ExtensionMethods.GetNullableInt64, 1234L);

    [Fact]
    internal void TestGetNullableInt64WithNullData() => TestGetWithNullData(ExtensionMethods.GetNullableInt64, default(long?));

    [Fact]
    internal void TestGetNullableInt64WithNoKey() => TestGetWithNoKey(ExtensionMethods.GetNullableInt64);

    [Fact]
    internal void TestGetNullableInt64WithInvalidData() => TestGetWithInvalidData(ExtensionMethods.GetNullableInt64);

    [Fact]
    internal void TestGetNullableBooleanWithValidData() => TestGetWithValidData(ExtensionMethods.GetNullableBoolean, value: true);

    [Fact]
    internal void TestGetNullableBooleanWithNullData() => TestGetWithNullData(ExtensionMethods.GetNullableBoolean, default(bool?));

    [Fact]
    internal void TestGetNullableBooleanWithNoKey() => TestGetWithNoKey(ExtensionMethods.GetNullableBoolean);

    [Fact]
    internal void TestGetNullableBooleanWithInvalidData() => TestGetWithInvalidData(ExtensionMethods.GetNullableBoolean);

    [Fact]
    internal void TestGetNullableDateTimeWithValidData() => TestGetWithValidData(ExtensionMethods.GetNullableDateTime, DateTime.Now);

    [Fact]
    internal void TestGetNullableDateTimeWithNullData() => TestGetWithNullData(ExtensionMethods.GetNullableDateTime, default(DateTime?));

    [Fact]
    internal void TestGetNullableDateTimeWithNoKey() => TestGetWithNoKey(ExtensionMethods.GetNullableDateTime);

    [Fact]
    internal void TestGetNullableDateTimeWithInvalidData() => TestGetWithInvalidData(ExtensionMethods.GetNullableDateTime);

    [Fact]
    internal void TestGetStringWithValidData() => ExtensionMethods.GetNullableString(new Dictionary<string, object?>(StringComparer.Ordinal) { { "value", "value" } }, "value").Should().Be("value");

    [Fact]
    internal void TestGetStringWithNoKey() => ExtensionMethods.GetNullableString(new Dictionary<string, object?>(StringComparer.Ordinal) { { "value", "value" } }, "value_bad").Should().Be(default);

    [Fact]
    internal void TestGetStringWithInvalidData() => new Dictionary<string, object?>(StringComparer.Ordinal) { { "value", 123456M } }.Invoking(values => values.GetNullableString("value")).Should().Throw<InvalidCastException>();

    private static void TestGetWithValidData<T1, T2>(Func<IDictionary<string, object?>, string, T1?> function, T2 value)
        where T1 : struct => function(new Dictionary<string, object?>(StringComparer.Ordinal) { { "value", value } }, "value").Should().Be(value);

    private static void TestGetWithNullData<T1, T2>(Func<IDictionary<string, object?>, string, T1?> function, T2 value)
        where T1 : struct => function(new Dictionary<string, object?>(StringComparer.Ordinal) { { "value", null } }, "value").Should().Be(value);

    private static void TestGetWithNoKey<T>(Func<IDictionary<string, object?>, string, T?> function)
        where T : struct => function(new Dictionary<string, object?>(StringComparer.Ordinal) { { "value", "value" } }, "value_bad").Should().Be(default(T?));

    private static void TestGetWithInvalidData<T>(Func<IDictionary<string, object?>, string, T?> function)
        where T : struct => new Dictionary<string, object?>(StringComparer.Ordinal) { { "value", 123456M } }.Invoking(_ => function(_, "value")).Should().Throw<InvalidCastException>();
}
