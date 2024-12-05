// <copyright file="BoundaryChecks.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System.Linq;
using ApiAnalysis.UnitTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace ApiAnalysis.UnitTests;

[TestClass]
public class BoundaryChecks
{
    public class SimpleByteClass
    {
        public byte Number { get; set; }
    }

    public class SimpleIntClass
    {
        public int Number { get; set; }
    }

    public class SimpleLongClass
    {
        public long Number { get; set; }
    }

    [TestMethod]
    public void ValidJsonDeserializesAsExpected()
    {
        var json = "{\"Number\":123}";

        var deserialized = JsonConvert.DeserializeObject<SimpleIntClass>(json);

        Assert.IsNotNull(deserialized);
        Assert.AreEqual(123, deserialized.Number);
    }

    [TestMethod]
    public void Byte_NoWarning_ForZero()
    {
        ByteBoundaryChecksWarned(0, false);
    }

    [TestMethod]
    public void Byte_NoWarning_ForOne()
    {
        ByteBoundaryChecksWarned(1, false);
    }

    // boundary = byte.MaxValue * .95 = 242.25
    [TestMethod]
    public void Byte_NoWarning_ForJustBelowBoundary()
    {
        ByteBoundaryChecksWarned(242, false);
    }

    [TestMethod]
    public void Byte_Warning_ForJustOverBoundary()
    {
        ByteBoundaryChecksWarned(243, true);
    }

    [TestMethod]
    public void Byte_Warning_ForMaxValue()
    {
        ByteBoundaryChecksWarned(byte.MaxValue, true);
    }

    // Using this and all the above as DataRow tests are not yet supplorted in class library tests
    public void ByteBoundaryChecksWarned(byte value, bool shouldCreateWarning)
    {
        var json = $"{{\"Number\":{value}}}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleByteClass)).Result;

        Assert.AreEqual(1, resp.Count);

        if (shouldCreateWarning)
        {
            Assert.AreEqual(MessageBuilder.Get.ValueIsCloseToMaxMessage(value.ToString(), PropertyInfoHelper.Get(typeof(SimpleByteClass), nameof(SimpleByteClass.Number)), typeof(byte)), resp.First());
        }
        else
        {
            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
        }
    }

    [TestMethod]
    public void Int_NoWarning_ForZero()
    {
        IntBoundaryChecksWarned(0, false);
    }

    [TestMethod]
    public void Int_NoWarning_ForOne()
    {
        IntBoundaryChecksWarned(1, false);
    }

    // boundary = int.MaxValue * .95 = 2040109464.6499999
    [TestMethod]
    public void Int_NoWarning_ForJustBelowBoundary()
    {
        IntBoundaryChecksWarned(2040109464, false);
    }

    [TestMethod]
    public void Int_Warning_ForJustOverBoundary()
    {
        IntBoundaryChecksWarned(2040109465, true);
    }

    [TestMethod]
    public void Int_Warning_ForMaxValue()
    {
        IntBoundaryChecksWarned(int.MaxValue, true);
    }

    public void IntBoundaryChecksWarned(int value, bool shouldCreateWarning)
    {
        var json = $"{{\"Number\":{value}}}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleIntClass)).Result;

        Assert.AreEqual(1, resp.Count);

        if (shouldCreateWarning)
        {
            Assert.AreEqual(MessageBuilder.Get.ValueIsCloseToMaxMessage(value.ToString(), PropertyInfoHelper.Get(typeof(SimpleIntClass), nameof(SimpleIntClass.Number)), typeof(int)), resp.First());
        }
        else
        {
            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
        }
    }

    [TestMethod]
    public void Long_NoWarning_ForZero()
    {
        LongBoundaryChecksWarned(0, false);
    }

    [TestMethod]
    public void Long_NoWarning_ForOne()
    {
        LongBoundaryChecksWarned(1, false);
    }

    // boundary = long.MaxValue * .95 = 8.7622034350120366E+18 (~ 8762203435012037120)
    [TestMethod]
    public void Long_NoWarning_ForJustBelowBoundary()
    {
        LongBoundaryChecksWarned(8762203435012037119, false);
    }

    [TestMethod]
    public void Long_NoWarning_ForBoundary()
    {
        LongBoundaryChecksWarned(8762203435012037120, false);
    }

    [TestMethod]
    public void Long_Warning_ForJustOverBoundary()
    {
        LongBoundaryChecksWarned(8762203435012037121, true);
    }

    [TestMethod]
    public void Long_Warning_ForMaxValue()
    {
        LongBoundaryChecksWarned(long.MaxValue, true);
    }

    public void LongBoundaryChecksWarned(long value, bool shouldCreateWarning)
    {
        var json = $"{{\"Number\":{value}}}";

        var analyzer = new SimpleJsonAnalyzer();

        var resp = analyzer.AnalyzeJsonAsync(json, typeof(SimpleLongClass)).Result;

        Assert.AreEqual(1, resp.Count);

        if (shouldCreateWarning)
        {
            Assert.AreEqual(MessageBuilder.Get.ValueIsCloseToMaxMessage(value.ToString(), PropertyInfoHelper.Get(typeof(SimpleLongClass), nameof(SimpleLongClass.Number)), typeof(long)), resp.First());
        }
        else
        {
            Assert.AreEqual(MessageBuilder.Get.AllGoodMessage, resp.First());
        }
    }
}