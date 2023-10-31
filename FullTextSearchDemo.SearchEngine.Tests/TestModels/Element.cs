using FullTextSearchDemo.SearchEngine.Models;

namespace FullTextSearchDemo.SearchEngine.Tests.TestModels;

public class Element : IDocument
{
    public string UniqueKey { get; set; }
    
    public bool BooleanProperty { get; set; }
    public byte ByteProperty { get; set; }
    public sbyte SByteProperty { get; set; }
    public char CharProperty { get; set; }
    public decimal DecimalProperty { get; set; }
    public double DoubleProperty { get; set; }
    public float SingleProperty { get; set; }
    public int Int32Property { get; set; }
    public uint UInt32Property { get; set; }
    public nint IntPtrProperty { get; set; }
    public nuint UIntPtrProperty { get; set; }
    public long Int64Property { get; set; }
    public ulong UInt64Property { get; set; }
    public short Int16Property { get; set; }
    public ushort UInt16Property { get; set; }
}