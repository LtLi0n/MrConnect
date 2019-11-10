using LionLibrary.Framework;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LionLibrary.Commands
{
    public class ModuleBase
    {
        public CommandContext Context { get; internal set; }
        public IReadOnlyDictionary<string, string> Args { get; internal set; }
        public ILogService Logger { get; internal set; }
        public IServiceProvider Services { get; internal set; }

        public ModuleBase() { }

        public bool HasArg(string arg) => Args.ContainsKey(arg);

        private void ParseGeneric<T>(string arg, Type type, Action<T> setOutput)
        {
            try
            {
                if (type == typeof(string)) setOutput((T)Convert.ChangeType(Args[arg], type));
                else if (type == typeof(bool)) setOutput((T)Convert.ChangeType(bool.Parse(Args[arg]), type));
                else if (type == typeof(sbyte)) setOutput((T)Convert.ChangeType(sbyte.Parse(Args[arg]), type));
                else if (type == typeof(byte)) setOutput((T)Convert.ChangeType(byte.Parse(Args[arg]), type));
                else if (type == typeof(short)) setOutput((T)Convert.ChangeType(short.Parse(Args[arg]), type));
                else if (type == typeof(ushort)) setOutput((T)Convert.ChangeType(ushort.Parse(Args[arg]), type));
                else if (type == typeof(int)) setOutput((T)Convert.ChangeType(int.Parse(Args[arg]), type));
                else if (type == typeof(uint)) setOutput((T)Convert.ChangeType(uint.Parse(Args[arg]), type));
                else if (type == typeof(long)) setOutput((T)Convert.ChangeType(long.Parse(Args[arg]), type));
                else if (type == typeof(ulong)) setOutput((T)Convert.ChangeType(ulong.Parse(Args[arg]), type));
                else if (type == typeof(decimal)) setOutput((T)Convert.ChangeType(decimal.Parse(Args[arg]), type));
                else if (type == typeof(double)) setOutput((T)Convert.ChangeType(double.Parse(Args[arg]), type));
                else if (type == typeof(float)) setOutput((T)Convert.ChangeType(float.Parse(Args[arg]), type));
                else if (type == typeof(DateTime)) setOutput((T)Convert.ChangeType(GetArgDateTime(arg, ArgumentConversion.Default), type));
                else throw new NotSupportedException($"{type.Name} conversion is not supported.");
            }
            catch (Exception ex)
            {
                if (ex is OverflowException) throw new ManagedCommandException("Integer overflow was detected while parsing a supplied value.");
                else if (ex is FormatException) throw new ManagedCommandException("Supplied value format was not recognized by the type parser.");
                else if (ex is ArgumentNullException) throw new ManagedCommandException("Supplied value was not initialized.");
                else throw ex;
            }
        }

        public bool NullTryFill<T>(string arg, Action<T?> setOutput) where T : struct
        {
            bool has_arg = HasArg(arg);
            Type type = typeof(T);
            if(has_arg)
            {
                string arg_str = Args[arg];
                if (arg_str == "null") setOutput(null);
                else
                {
                    ParseGeneric(arg, type, setOutput);
                }
            }

            return has_arg;
        }

        public bool TryFill<T>(string arg, Action<T> setOutput)
        {
            bool has_arg = HasArg(arg);
            Type type = typeof(T);

            if (has_arg)
            {
                ParseGeneric(arg, type, setOutput);
            }
            return has_arg;
        }

        public string GetArgString(string arg, ArgumentConversion conversion = ArgumentConversion.Default) => Parse<string>(arg, conversion);
        public bool TryFillString(string arg, ref string val) { bool has_arg = HasArg(arg); if (has_arg) val = Args[arg]; return has_arg; }
        public bool TryFillString(string arg, Action<string> setOutput) { bool has_arg = HasArg(arg); if (has_arg) setOutput(Args[arg]); return has_arg; }

        public bool GetArgBoolean(string arg, ArgumentConversion conversion = ArgumentConversion.Default) => Parse<bool>(arg, conversion);
        public bool TryFillBoolean(string arg, ref bool val) { bool has_arg = HasArg(arg); if (has_arg) val = bool.Parse(Args[arg]); return has_arg; }
        public bool TryFillBoolean(string arg, Action<bool> setOutput) { bool has_arg = HasArg(arg); if (has_arg) setOutput(bool.Parse(Args[arg])); return has_arg; }

        //8bit
        public sbyte GetArgInt8(string arg, ArgumentConversion conversion = ArgumentConversion.Default) => Parse<sbyte>(arg, conversion);
        public bool TryFillInt8(string arg, ref sbyte val) { bool has_arg = HasArg(arg); if (has_arg) val = sbyte.Parse(Args[arg]); return has_arg; }
        public bool TryFillInt8(string arg, Action<sbyte> setOutput) { bool has_arg = HasArg(arg); if (has_arg) setOutput(sbyte.Parse(Args[arg])); return has_arg; }

        public byte GetArgUInt8(string arg, ArgumentConversion conversion = ArgumentConversion.Default) => Parse<byte>(arg, conversion);
        public bool TryFillUInt8(string arg, ref byte val) { bool has_arg = HasArg(arg); if (has_arg) val = byte.Parse(Args[arg]); return has_arg; }
        public bool TryFillUInt8(string arg, Action<byte> setOutput) { bool has_arg = HasArg(arg); if (has_arg) setOutput(byte.Parse(Args[arg])); return has_arg; }

        //16bit
        public short GetArgInt16(string arg, ArgumentConversion conversion = ArgumentConversion.Default) => Parse<short>(arg, conversion);
        public bool TryFillInt16(string arg, ref short val) { bool has_arg = HasArg(arg); if (has_arg) val = short.Parse(Args[arg]); return has_arg; }
        public bool TryFillInt16(string arg, Action<short> setOutput) { bool has_arg = HasArg(arg); if (has_arg) setOutput(short.Parse(Args[arg])); return has_arg; }

        public ushort GetArgUInt16(string arg, ArgumentConversion conversion = ArgumentConversion.Default) => Parse<ushort>(arg, conversion);
        public bool TryFillUInt16(string arg, ref ushort val) { bool has_arg = HasArg(arg); if (has_arg) val = ushort.Parse(Args[arg]); return has_arg; }
        public bool TryFillUInt16(string arg, Action<ushort> setOutput) { bool has_arg = HasArg(arg); if (has_arg) setOutput(ushort.Parse(Args[arg])); return has_arg; }

        //32bit
        public int GetArgInt32(string arg, ArgumentConversion conversion = ArgumentConversion.Default) => Parse<int>(arg, conversion);
        public bool TryFillInt32(string arg, ref int val) { bool has_arg = HasArg(arg); if (has_arg) val = int.Parse(Args[arg]); return has_arg; }
        public bool TryFillInt32(string arg, Action<int> setOutput) { bool has_arg = HasArg(arg); if (has_arg) setOutput(int.Parse(Args[arg])); return has_arg; }

        public uint GetArgUInt32(string arg, ArgumentConversion conversion = ArgumentConversion.Default) => Parse<uint>(arg, conversion);
        public bool TryFillUInt32(string arg, ref uint val) { bool has_arg = HasArg(arg); if (has_arg) val = uint.Parse(Args[arg]); return has_arg; }
        public bool TryFillUInt32(string arg, Action<uint> setOutput) { bool has_arg = HasArg(arg); if (has_arg) setOutput(uint.Parse(Args[arg])); return has_arg; }

        //64bit
        public long GetArgInt64(string arg, ArgumentConversion conversion = ArgumentConversion.Default) => Parse<long>(arg, conversion);
        public bool TryFillInt64(string arg, ref long val) { bool has_arg = HasArg(arg); if (has_arg) val = long.Parse(Args[arg]); return has_arg; }
        public bool TryFillInt64(string arg, Action<long> setOutput) { bool has_arg = HasArg(arg); if (has_arg) setOutput(long.Parse(Args[arg])); return has_arg; }

        public ulong GetArgUInt64(string arg, ArgumentConversion conversion = ArgumentConversion.Default) => Parse<ulong>(arg, conversion);
        public bool TryFillUInt64(string arg, ref ulong val) { bool has_arg = HasArg(arg); if (has_arg) val = ulong.Parse(Args[arg]); return has_arg; }
        public bool TryFillUInt64(string arg, Action<ulong> setOutput) { bool has_arg = HasArg(arg); if (has_arg) setOutput(ulong.Parse(Args[arg])); return has_arg; }

        //decimal
        public decimal GetArgDecimal(string arg, ArgumentConversion conversion = ArgumentConversion.Default) => Parse<decimal>(arg, conversion);
        public bool TryFillDecimal(string arg, ref decimal val) { bool has_arg = HasArg(arg); if (has_arg) val = decimal.Parse(Args[arg]); return has_arg; }
        public bool TryFillDecimal(string arg, Action<decimal> setOutput) { bool has_arg = HasArg(arg); if (has_arg) setOutput(decimal.Parse(Args[arg])); return has_arg; }

        public double GetArgDouble(string arg, ArgumentConversion conversion = ArgumentConversion.Default) => Parse<double>(arg, conversion);
        public bool TryFillDouble(string arg, ref double val) { bool has_arg = HasArg(arg); if (has_arg) val = double.Parse(Args[arg]); return has_arg; }
        public bool TryFillDouble(string arg, Action<double> setOutput) { bool has_arg = HasArg(arg); if (has_arg) setOutput(double.Parse(Args[arg])); return has_arg; }

        public float GetArgFloat(string arg, ArgumentConversion conversion = ArgumentConversion.Default) => Parse<float>(arg, conversion);
        public bool TryFillFloat(string arg, ref float val) { bool has_arg = HasArg(arg); if (has_arg) val = float.Parse(Args[arg]); return has_arg; }
        public bool TryFillFloat(string arg, Action<float> setOutput) { bool has_arg = HasArg(arg); if (has_arg) setOutput(float.Parse(Args[arg])); return has_arg; }

        //datetime
        public DateTime GetArgDateTime(string arg, ArgumentConversion conversion = ArgumentConversion.Default) => Parse(arg, conversion, (x) =>
        {
            long unix_time = GetArgInt64(arg, conversion);
            if (unix_time == default) return default;
            return DateTimeOffset.FromUnixTimeSeconds(unix_time).DateTime;
        });
        public bool TryFillDateTime(string arg, ref DateTime val) { bool has_arg = HasArg(arg); if (has_arg) val = GetArgDateTime(arg, ArgumentConversion.Mandatory); return has_arg; }
        public bool TryFillDateTime(string arg, Action<DateTime> setOutput) { bool has_arg = HasArg(arg); if (has_arg) setOutput(GetArgDateTime(arg, ArgumentConversion.Mandatory)); return has_arg; }

        private T Parse<T>(string arg, ArgumentConversion conversion, Func<string, T> manualConversionFunc = null)
        {
            try
            {
                string val = Args[arg];
                if (string.IsNullOrEmpty(val)) throw new ArgumentNullException($"Arg: {arg} is empty. Conversion is impossible.");
                return manualConversionFunc == null ? (T)Convert.ChangeType(val, typeof(T)) : manualConversionFunc(val);
            }
            catch(Exception ex)
            {
                if (conversion == ArgumentConversion.Mandatory)
                {
                    throw new ArgumentConversionException<T>(ex is ArgumentNullException ? ex.Message : $"Error parsing arg: {arg}.");
                }
                else return default;
            }
        }

        public sbyte[] ParseIdsInt8(string input) => ParseIdsFromStringGeneric(input, (x) => sbyte.Parse(x));
        public byte[] ParseIdsUInt8(string input) => ParseIdsFromStringGeneric(input, (x) => byte.Parse(x));
        public short[] ParseIdsInt16(string input) => ParseIdsFromStringGeneric(input, (x) => short.Parse(x));
        public ushort[] ParseIdsUInt16(string input) => ParseIdsFromStringGeneric(input, (x) => ushort.Parse(x));
        public int[] ParseIdsInt32(string input) => ParseIdsFromStringGeneric(input, (x) => int.Parse(x));
        public uint[] ParseIdsUInt32(string input) => ParseIdsFromStringGeneric(input, (x) => uint.Parse(x));
        public long[] ParseIdsInt64(string input) => ParseIdsFromStringGeneric(input, (x) => long.Parse(x));
        public ulong[] ParseIdsUInt64(string input) => ParseIdsFromStringGeneric(input, (x) => ulong.Parse(x));

        private T[] ParseIdsFromStringGeneric<T>(string input, Func<string, T> parse_func)
            where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
        {
            string[] str_ids = ParseIdsFromString(input);
            T[] ids = new T[str_ids.Length];

            for (int i = 0; i < str_ids.Length; i++)
                ids[i] = parse_func(str_ids[i]);

            return ids;
        }

        private string[] ParseIdsFromString(string input) => Regex.Split(input, @"\D+");
    }

    public class ModuleBase<T> : ModuleBase where T : CommandContext
    {
        public new T Context => (T)base.Context;
        public ModuleBase() { }
    }
}
