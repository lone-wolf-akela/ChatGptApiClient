#pragma warning disable CS0618 // Type or member is obsolete

#nullable enable

namespace Gemini
{
    /// <summary>
    /// This resource represents a long-running operation where metadata and response fields are strongly typed.
    /// </summary>
    public readonly partial struct BatchGenerateContentOperation : global::System.IEquatable<BatchGenerateContentOperation>
    {
        /// <summary>
        /// This resource represents a long-running operation that is the result of a<br/>
        /// network API call.
        /// </summary>
#if NET6_0_OR_GREATER
        public global::Gemini.BaseOperation? Value1 { get; init; }
#else
        public global::Gemini.BaseOperation? Value1 { get; }
#endif

        /// <summary>
        /// 
        /// </summary>
#if NET6_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.MemberNotNullWhen(true, nameof(Value1))]
#endif
        public bool IsValue1 => Value1 != null;

        /// <summary>
        /// 
        /// </summary>
        public static implicit operator BatchGenerateContentOperation(global::Gemini.BaseOperation value) => new BatchGenerateContentOperation((global::Gemini.BaseOperation?)value);

        /// <summary>
        /// 
        /// </summary>
        public static implicit operator global::Gemini.BaseOperation?(BatchGenerateContentOperation @this) => @this.Value1;

        /// <summary>
        /// 
        /// </summary>
        public BatchGenerateContentOperation(global::Gemini.BaseOperation? value)
        {
            Value1 = value;
        }

        /// <summary>
        /// 
        /// </summary>
#if NET6_0_OR_GREATER
        public global::Gemini.BatchGenerateContentOperationVariant2? Value2 { get; init; }
#else
        public global::Gemini.BatchGenerateContentOperationVariant2? Value2 { get; }
#endif

        /// <summary>
        /// 
        /// </summary>
#if NET6_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.MemberNotNullWhen(true, nameof(Value2))]
#endif
        public bool IsValue2 => Value2 != null;

        /// <summary>
        /// 
        /// </summary>
        public static implicit operator BatchGenerateContentOperation(global::Gemini.BatchGenerateContentOperationVariant2 value) => new BatchGenerateContentOperation((global::Gemini.BatchGenerateContentOperationVariant2?)value);

        /// <summary>
        /// 
        /// </summary>
        public static implicit operator global::Gemini.BatchGenerateContentOperationVariant2?(BatchGenerateContentOperation @this) => @this.Value2;

        /// <summary>
        /// 
        /// </summary>
        public BatchGenerateContentOperation(global::Gemini.BatchGenerateContentOperationVariant2? value)
        {
            Value2 = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public BatchGenerateContentOperation(
            global::Gemini.BaseOperation? value1,
            global::Gemini.BatchGenerateContentOperationVariant2? value2
            )
        {
            Value1 = value1;
            Value2 = value2;
        }

        /// <summary>
        /// 
        /// </summary>
        public object? Object =>
            Value2 as object ??
            Value1 as object 
            ;

        /// <summary>
        /// 
        /// </summary>
        public override string? ToString() =>
            Value1?.ToString() ??
            Value2?.ToString() 
            ;

        /// <summary>
        /// 
        /// </summary>
        public bool Validate()
        {
            return IsValue1 && IsValue2;
        }

        /// <summary>
        /// 
        /// </summary>
        public TResult? Match<TResult>(
            global::System.Func<global::Gemini.BaseOperation?, TResult>? value1 = null,
            global::System.Func<global::Gemini.BatchGenerateContentOperationVariant2?, TResult>? value2 = null,
            bool validate = true)
        {
            if (validate)
            {
                Validate();
            }

            if (IsValue1 && value1 != null)
            {
                return value1(Value1!);
            }
            else if (IsValue2 && value2 != null)
            {
                return value2(Value2!);
            }

            return default(TResult);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Match(
            global::System.Action<global::Gemini.BaseOperation?>? value1 = null,
            global::System.Action<global::Gemini.BatchGenerateContentOperationVariant2?>? value2 = null,
            bool validate = true)
        {
            if (validate)
            {
                Validate();
            }

            if (IsValue1)
            {
                value1?.Invoke(Value1!);
            }
            else if (IsValue2)
            {
                value2?.Invoke(Value2!);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override int GetHashCode()
        {
            var fields = new object?[]
            {
                Value1,
                typeof(global::Gemini.BaseOperation),
                Value2,
                typeof(global::Gemini.BatchGenerateContentOperationVariant2),
            };
            const int offset = unchecked((int)2166136261);
            const int prime = 16777619;
            static int HashCodeAggregator(int hashCode, object? value) => value == null
                ? (hashCode ^ 0) * prime
                : (hashCode ^ value.GetHashCode()) * prime;

            return global::System.Linq.Enumerable.Aggregate(fields, offset, HashCodeAggregator);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Equals(BatchGenerateContentOperation other)
        {
            return
                global::System.Collections.Generic.EqualityComparer<global::Gemini.BaseOperation?>.Default.Equals(Value1, other.Value1) &&
                global::System.Collections.Generic.EqualityComparer<global::Gemini.BatchGenerateContentOperationVariant2?>.Default.Equals(Value2, other.Value2) 
                ;
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool operator ==(BatchGenerateContentOperation obj1, BatchGenerateContentOperation obj2)
        {
            return global::System.Collections.Generic.EqualityComparer<BatchGenerateContentOperation>.Default.Equals(obj1, obj2);
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool operator !=(BatchGenerateContentOperation obj1, BatchGenerateContentOperation obj2)
        {
            return !(obj1 == obj2);
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool Equals(object? obj)
        {
            return obj is BatchGenerateContentOperation o && Equals(o);
        }
    }
}
