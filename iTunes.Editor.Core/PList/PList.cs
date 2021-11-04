// -----------------------------------------------------------------------
// <copyright file="PList.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.PList;

using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

/// <summary>
/// Represents a PList.
/// </summary>
[XmlRoot(PListElementName)]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This is the correct name")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "MA0049:Type name should not match containing namespace", Justification = "This is by design")]
public class PList : IDictionary<string, object>, IXmlSerializable
{
    private const string ArrayElementName = "array";

    private const string PListElementName = "plist";

    private const string IntegerElementName = "integer";

    private const string RealElementName = "real";

    private const string StringElementName = "string";

    private const string DictionaryElementName = "dict";

    private const string DataElementName = "data";

    private const string DateElementName = "date";

    private const string KeyElementName = "key";

    private const string TrueElementName = "true";

    private const string FalseElementName = "false";

    /// <summary>
    /// Initializes a new instance of the <see cref="PList"/> class.
    /// </summary>
    public PList()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PList"/> class.
    /// </summary>
    /// <param name="dictionary">The dictionary.</param>
    internal PList(IDictionary<string, object> dictionary)
    {
        this.Version = new System.Version(1, 0);
        this.DictionaryImplementation = dictionary;
    }

    /// <inheritdoc />
    public int Count => this.DictionaryImplementation.Count;

    /// <inheritdoc />
    public bool IsReadOnly => this.DictionaryImplementation.IsReadOnly;

    /// <summary>
    /// Gets the version.
    /// </summary>
    public System.Version? Version { get; private set; }

    /// <inheritdoc />
    public ICollection<string> Keys => this.DictionaryImplementation.Keys;

    /// <inheritdoc />
    public ICollection<object> Values => this.DictionaryImplementation.Values;

    /// <summary>
    /// Gets the implementation.
    /// </summary>
    protected IDictionary<string, object> DictionaryImplementation { get; private set; } = new Dictionary<string, object>(System.StringComparer.Ordinal);

    /// <inheritdoc />
    public object this[string key]
    {
        get => this.DictionaryImplementation[key];
        set => this.DictionaryImplementation[key] = value;
    }

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => this.DictionaryImplementation.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)this.DictionaryImplementation).GetEnumerator();

    /// <inheritdoc />
    public void Add(KeyValuePair<string, object> item) => this.DictionaryImplementation.Add(item);

    /// <inheritdoc />
    public void Clear() => this.DictionaryImplementation.Clear();

    /// <inheritdoc />
    public bool Contains(KeyValuePair<string, object> item) => this.DictionaryImplementation.Contains(item);

    /// <inheritdoc />
    public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) => this.DictionaryImplementation.CopyTo(array, arrayIndex);

    /// <inheritdoc />
    public bool Remove(KeyValuePair<string, object> item) => this.DictionaryImplementation.Remove(item);

    /// <inheritdoc />
    public void Add(string key, object value) => this.DictionaryImplementation.Add(key, value);

    /// <inheritdoc />
    public bool ContainsKey(string key) => this.DictionaryImplementation.ContainsKey(key);

    /// <inheritdoc />
    public bool Remove(string key) => this.DictionaryImplementation.Remove(key);

    /// <inheritdoc />
    public bool TryGetValue(string key, out object value) => this.DictionaryImplementation.TryGetValue(key, out value);

    /// <inheritdoc />
    public XmlSchema? GetSchema() => null;

    /// <inheritdoc />
    public void ReadXml(XmlReader reader)
    {
        if (reader is null || !string.Equals(reader.Name, PListElementName, System.StringComparison.Ordinal) || reader.NodeType != XmlNodeType.Element)
        {
            return;
        }

        this.Version = System.Version.Parse(reader.GetAttribute("version"));

        // read through the reader
        if (!ReadWhileWhiteSpace(reader))
        {
            return;
        }

        // read through the dictionary
        if (!string.Equals(reader.Name, DictionaryElementName, System.StringComparison.Ordinal))
        {
            return;
        }

        this.DictionaryImplementation = ReadDictionary(reader);

        if (ReadWhileWhiteSpace(reader) && string.Equals(reader.Name, PListElementName, System.StringComparison.Ordinal) && reader.NodeType == XmlNodeType.EndElement)
        {
            return;
        }

        throw new System.ArgumentException(Properties.Resources.InvalidXml, nameof(reader));
    }

    /// <inheritdoc />
    public void WriteXml(XmlWriter writer)
    {
        if (writer is null)
        {
            throw new System.ArgumentNullException(nameof(writer));
        }

        WriteDictionary(writer, 0, this);
    }

    /// <summary>
    /// Reads the dictionary.
    /// </summary>
    /// <param name="reader">The XML reader.</param>
    /// <returns>The dictionary.</returns>
    private static IDictionary<string, object> ReadDictionary(XmlReader reader)
    {
        IDictionary<string, object> dictionary = new Dictionary<string, object>(System.StringComparer.Ordinal);
        string? key = null;
        object? value = null;

        while (ReadWhileWhiteSpace(reader))
        {
            if (string.Equals(reader.Name, DictionaryElementName, System.StringComparison.Ordinal) && reader.NodeType == XmlNodeType.EndElement)
            {
                return dictionary;
            }

            if (string.Equals(reader.Name, KeyElementName, System.StringComparison.Ordinal))
            {
                _ = ReadWhileWhiteSpace(reader);
                if (key is not null)
                {
                    AddToDictionary();
                }

                key = reader.Value;
                _ = ReadWhileWhiteSpace(reader);
                continue;
            }

            value = ReadValue(reader);
            AddToDictionary();

            void AddToDictionary()
            {
                dictionary.Add(key!, value!);
                key = null;
                value = null;
            }
        }

        return dictionary;
    }

    private static object? ReadValue(XmlReader reader)
    {
        return reader.Name switch
        {
            TrueElementName => true,
            FalseElementName => false,
            IntegerElementName => ReadInteger(reader),
            RealElementName => ReadDouble(reader),
            StringElementName => ReadString(reader),
            DateElementName => ReadDate(reader),
            DictionaryElementName => ReadDictionary(reader),
            ArrayElementName => ReadArray(reader),
            DataElementName => ReadData(reader),
            _ => throw new System.ArgumentException(Properties.Resources.InvalidPListValueType, nameof(reader)),
        };

        static long ReadInteger(XmlReader reader)
        {
            _ = ReadWhileWhiteSpace(reader);
            var longValue = long.Parse(reader.Value, CultureInfo.InvariantCulture);
            _ = ReadWhileWhiteSpace(reader);
            return longValue;
        }

        static double ReadDouble(XmlReader reader)
        {
            _ = ReadWhileWhiteSpace(reader);
            var doubleValue = double.Parse(reader.Value, CultureInfo.InvariantCulture);
            _ = ReadWhileWhiteSpace(reader);
            return doubleValue;
        }

        static string? ReadString(XmlReader reader)
        {
            // read until the end
            var builder = new System.Text.StringBuilder();
            var first = true;
            while (ReadWhileWhiteSpace(reader))
            {
                if (reader.NodeType == XmlNodeType.EndElement)
                {
                    return builder.ToString();
                }

                if (!first)
                {
                    _ = builder.AppendLine();
                }

                first = false;
                _ = builder.Append(reader.Value);
            }

            return first ? null : builder.ToString();
        }

        static System.DateTime ReadDate(XmlReader reader)
        {
            _ = ReadWhileWhiteSpace(reader);
            var dateValue = System.DateTime.Parse(reader.Value, CultureInfo.InvariantCulture);
            _ = ReadWhileWhiteSpace(reader);
            return dateValue;
        }

        static object?[] ReadArray(XmlReader reader)
        {
            var list = new List<object?>();

            while (ReadWhileWhiteSpace(reader))
            {
                if (string.Equals(reader.Name, ArrayElementName, System.StringComparison.Ordinal) && reader.NodeType == XmlNodeType.EndElement)
                {
                    break;
                }

                list.Add(ReadValue(reader));
            }

            return list.ToArray();
        }

        static byte[] ReadData(XmlReader reader)
        {
            _ = ReadWhileWhiteSpace(reader);
            var dataValue = System.Convert.FromBase64String(reader.Value);
            _ = ReadWhileWhiteSpace(reader);
            return dataValue;
        }
    }

    /// <summary>
    /// Writes the dictionary.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="indentLevel">The indent level.</param>
    /// <param name="dictionary">The dictionary to write.</param>
    private static void WriteDictionary(XmlWriter writer, int indentLevel, IDictionary<string, object> dictionary)
    {
        writer.WriteWhitespace(new string('\t', indentLevel));
        writer.WriteStartElement(DictionaryElementName);
        writer.WriteWhitespace(System.Environment.NewLine);

        var indent = new string('\t', indentLevel + 1);
        foreach (var kvp in dictionary)
        {
            writer.WriteWhitespace(indent);
            writer.WriteElementString(KeyElementName, kvp.Key);
            WriteValue(writer, indentLevel + 1, kvp.Value);
            writer.WriteWhitespace(System.Environment.NewLine);
        }

        writer.WriteWhitespace(new string('\t', indentLevel));
        writer.WriteEndElement();
    }

    /// <summary>
    /// Writes the value.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="indentLevel">The indent level.</param>
    /// <param name="value">The value to write.</param>
    private static void WriteValue(XmlWriter writer, int indentLevel, object value)
    {
        var type = value.GetType();
        if (type == typeof(bool))
        {
            var boolValue = (bool)value;
            writer.WriteStartElement(boolValue ? TrueElementName : FalseElementName);
            writer.WriteEndElement();
        }
        else if (type == typeof(int) || type == typeof(long))
        {
            var longValue = (long)value;
            writer.WriteElementString(IntegerElementName, longValue.ToString(CultureInfo.InvariantCulture));
        }
        else if (type == typeof(float) || type == typeof(double))
        {
            var doubleValue = (double)value;
            writer.WriteElementString(RealElementName, doubleValue.ToString(CultureInfo.InvariantCulture));
        }
        else if (type == typeof(string))
        {
            var stringValue = (string)value;
            writer.WriteElementString(StringElementName, stringValue);
        }
        else if (type == typeof(System.DateTime))
        {
            var dateValue = (System.DateTime)value;
            writer.WriteElementString(DateElementName, dateValue.ToUniversalTime().ToString("s", CultureInfo.InvariantCulture) + "Z");
        }
        else if (typeof(IDictionary<string, object>).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
        {
            // put the dictionary on a new line.
            writer.WriteWhitespace(System.Environment.NewLine);
            WriteDictionary(writer, indentLevel, (IDictionary<string, object>)value);
        }
        else if (type == typeof(object[]))
        {
            var arrayValue = (object[])value;
            writer.WriteStartElement(ArrayElementName);
            foreach (var item in arrayValue)
            {
                WriteValue(writer, indentLevel, item);
            }

            writer.WriteEndElement();
        }
        else if (type == typeof(byte[]))
        {
            var byteValue = (byte[])value;
            writer.WriteElementString(DataElementName, System.Convert.ToBase64String(byteValue));
        }
    }

    private static bool ReadWhileWhiteSpace(XmlReader reader)
    {
        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Whitespace)
            {
                continue;
            }

            return true;
        }

        return false;
    }
}
