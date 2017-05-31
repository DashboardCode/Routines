using System;
using System.Collections.Generic;

namespace Vse.Routines.Json
{
    public class TestClass
    {
        public string TextField1 { get; set; }
        public string TextField2 { get; set; }
        public TestClass TestClass1 { get; set; }
        public TestClass TestClass2 { get; set; }
        public TestStruct TestStruct { get; set; }
        public TestStruct? NTestStruct1 { get; set; }
        public TestStruct? NTestStruct2 { get; set; }
        public bool BoolField { get; set; }
        public bool? NBoolField1 { get; set; }
        public bool? NBoolField2 { get; set; }
        public List<ListItem> ListItems { get; set; }
        public int Number { get; set; }
        public int? NNumber1 { get; set; }
        public int? NNumber2 { get; set; }
        public float Float0 { get; set; }
        public float Float1 { get; set; }
        public byte[] RowData { get; set; }
        public List<int> Ints { get; set; }
        public List<int?> NInts { get; set; }
        public List<string> Strings { get; set; }
    }

    public struct TestStruct
    {
        public string TextField1 { get; set; }
        public byte Byte1 { get; set; }
        public byte? Byte2 { get; set; }
    }

    public class ListItem
    {
        public byte[] RowData { get; set; }
        public DateTime DateTime { get; set; }
    }
}
